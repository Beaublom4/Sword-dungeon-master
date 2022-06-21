using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

[ExecuteAlways]
public class DungeonCreator : MonoBehaviour
{
    public static DungeonCreator Instance;

    PhotonView pv;

    [Header("Grid")]
    public static DungeonPreset dungeonPreset;
    [SerializeField] Vector2 gridOffset = new Vector2(20, 20);
    [SerializeField] Vector2 roomSize = new Vector2(20, 20);

    [Header("Mini map")]
    [SerializeField] Camera miniMapCam;

    [Header("Rooms")]
    [SerializeField] GameObject startRoom;
    [SerializeField] GameObject wallRoom;
    [SerializeField] GameObject[] normalRooms;
    [SerializeField] GameObject[] deathRooms;
    [SerializeField] GameObject[] challangeRooms;
    [SerializeField] GameObject[] puzzleRooms;

    [Header("Spawning")]
    bool stopped;
    Vector2 currentGridLoc;
    int lastDirection;
    DungeonRoom currentRoom;
    List<DungeonRoom> roomsToCheck = new List<DungeonRoom>();

    public bool created;

    [Header("Counter")]
    [SerializeField] float cooldownTime = .25f;
    float spawnCooldown;

    [Header("Current dungeon")]
    [SerializeField] NavMeshSurface sur;
    [SerializeField] List<DungeonRoom> rooms = new List<DungeonRoom>();
    [SerializeField] List<Vector2> pathRooms = new List<Vector2>();
    [SerializeField] List<DungeonRoom> walls = new List<DungeonRoom>();
    Vector2 deathRoomLoc;

    [Header("Boss")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject[] bossRooms;

    [Header("Chests Room")]
    [SerializeField] Transform chestSpawnPos;

    private void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();
    }
    private void OnDrawGizmosSelected()
    {
        if (dungeonPreset == null)
        {
            return;
        }

        for (int x = 0; x < dungeonPreset.gridSize.x; x++)
        {
            for (int y = 0; y < dungeonPreset.gridSize.y; y++)
            {
                for (int i = 0; i < pathRooms.Count; i++)
                {
                    if (new Vector2(x, y) == pathRooms[i])
                    {
                        if (new Vector2(x, y) == deathRoomLoc)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(transform.position + new Vector3(x * gridOffset.x, 0, y * gridOffset.y), 1f);
                            break;
                        }

                        Gizmos.color = Color.blue;
                        if (i + 1 < pathRooms.Count)
                            Gizmos.DrawLine(transform.position + new Vector3(pathRooms[i].x * gridOffset.x, 10, pathRooms[i].y * gridOffset.y), transform.position + new Vector3(pathRooms[i + 1].x * gridOffset.x, 10, pathRooms[i + 1].y * gridOffset.y));
                        break;
                    }
                }
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position + new Vector3(x * gridOffset.x, 0, y * gridOffset.y), new Vector3(roomSize.x - .5f, 1, roomSize.y - .5f));
            }
        }
    }
    private void Start()
    {
        if (!Application.isPlaying)
            return;

        Chest[] chests = FindObjectsOfType<Chest>();
        foreach(Chest chest in chests)
        {
            chest.preset = dungeonPreset;
        }

        currentGridLoc = new Vector2(Mathf.RoundToInt(Random.Range(0, dungeonPreset.gridSize.y - 1)), -1);
        SpawnRoom(currentGridLoc, startRoom, true, false, true, false);
        DungeonManager.Instance.starterDoor = currentRoom.GetComponent<DungeonRoom>().door;
        spawnCooldown = cooldownTime;

        miniMapCam.transform.position = new Vector3((dungeonPreset.gridSize.x / 2) * gridOffset.x, miniMapCam.transform.position.y, (dungeonPreset.gridSize.y / 2) * gridOffset.y);
        miniMapCam.orthographicSize = (dungeonPreset.gridSize.x * .75f) * gridOffset.x;
    }
    private void Update()
    {
        if (!Application.isPlaying)
            return;

        if (spawnCooldown < 0 && !stopped)
        {
            NextRoom();
            spawnCooldown = cooldownTime;
        }
        else
        {
            spawnCooldown -= Time.deltaTime;
        }


    }
    void NextRoom()
    {
        //forward 0, back 1, right 2, left 3
        List<int> directions = new List<int>();
        #region Check Directions
        if (currentRoom.forward)
        {
            if (!CheckForRoom(currentGridLoc + new Vector2(0, 1)))
            {
                if (CheckForInGrid(currentGridLoc + new Vector2(0, 1)))
                {
                    directions.Add(0);
                }
            }
        }
        if (currentRoom.back)
        {
            if (!CheckForRoom(currentGridLoc + new Vector2(0, -1)))
            {
                if (CheckForInGrid(currentGridLoc + new Vector2(0, -1)))
                {
                    directions.Add(1);
                }
            }
        }
        if (currentRoom.right)
        {
            if (!CheckForRoom(currentGridLoc + new Vector2(1, 0)))
            {
                if (CheckForInGrid(currentGridLoc + new Vector2(1, 0)))
                {
                    directions.Add(2);
                }
            }
        }
        if (currentRoom.left)
        {
            if (!CheckForRoom(currentGridLoc + new Vector2(-1, 0)))
            {
                if (CheckForInGrid(currentGridLoc + new Vector2(-1, 0)))
                {
                    directions.Add(3);
                }
            }
        }
        #endregion

        if (directions.Count == 0)
        {
            stopped = true;

            Destroy(currentRoom.gameObject);
            SpawnRoom(currentGridLoc, deathRooms[lastDirection], true, false, true, false);
            deathRoomLoc = currentGridLoc;
            Invoke("FillEmptyRooms", cooldownTime);

            return;
        }

        int chosenDirection = directions[Random.Range(0, directions.Count)];
        lastDirection = chosenDirection;

        switch (chosenDirection) 
        {
            case 0:
                currentGridLoc += new Vector2(0, 1);
                break;
            case 1:
                currentGridLoc += new Vector2(0, -1);
                break;
            case 2:
                currentGridLoc += new Vector2(1, 0);
                break;
            case 3:
                currentGridLoc += new Vector2(-1, 0);
                break;
        }

        List<GameObject> roomsWithOptions = new List<GameObject>();
        #region Check Room Options
        foreach(GameObject checkRoom in normalRooms)
        {
            DungeonRoom checkRoomScript = checkRoom.GetComponent<DungeonRoom>();

            if(chosenDirection == 0)
            {
                if (!checkRoomScript.back)
                    continue;
            }
            else if(chosenDirection == 1)
            {
                if (!checkRoomScript.forward)
                    continue;
            }
            else if (chosenDirection == 2)
            {
                if (!checkRoomScript.left)
                    continue;
            }
            else if (chosenDirection == 3)
            {
                if (!checkRoomScript.right)
                    continue;
            }

            bool isPossible = false;

            if (currentRoom.forward)
            {
                if (!CheckForRoom(currentGridLoc + new Vector2(0, 1)))
                {
                    if (CheckForInGrid(currentGridLoc + new Vector2(0, 1)))
                    {
                        isPossible = true;
                    }
                }
            }
            if (currentRoom.back)
            {
                if (!CheckForRoom(currentGridLoc + new Vector2(0, -1)))
                {
                    if (CheckForInGrid(currentGridLoc + new Vector2(0, -1)))
                    {
                        isPossible = true;
                    }
                }
            }
            if (currentRoom.right)
            {
                if (!CheckForRoom(currentGridLoc + new Vector2(1, 0)))
                {
                    if (CheckForInGrid(currentGridLoc + new Vector2(1, 0)))
                    {
                        isPossible = true;
                    }
                }
            }
            if (currentRoom.left)
            {
                if (!CheckForRoom(currentGridLoc + new Vector2(-1, 0)))
                {
                    if (CheckForInGrid(currentGridLoc + new Vector2(-1, 0)))
                    {
                        isPossible = true;
                    }
                }
            }
            if (isPossible)
            {
                roomsWithOptions.Add(checkRoom);
            }
        }
        #endregion

        if (roomsWithOptions.Count == 0)
        {
            stopped = true;

            SpawnRoom(currentGridLoc, deathRooms[chosenDirection], true, false, true, false);
            deathRoomLoc = currentGridLoc;
            Invoke("FillEmptyRooms", cooldownTime);

            return;
        }

        GameObject selectedRoom = roomsWithOptions[Random.Range(0, roomsWithOptions.Count)];
        SpawnRoom(currentGridLoc, selectedRoom, true, false, true, false);
    }
    void SpawnRoom(Vector2 gridLoc, GameObject room, bool isPath, bool addToCheck, bool isRoom, bool isWall)
    {
        currentRoom = Instantiate(room, GetWorldPos(gridLoc), Quaternion.identity, transform).GetComponent<DungeonRoom>();
        if (isRoom)
        {
            rooms.Add(currentRoom);
            currentRoom.gridLoc = gridLoc;
        }
        else if(isWall)
        {
            walls.Add(currentRoom);
            currentRoom.gridLoc = gridLoc;
            return;
        }

        if (isPath)
            pathRooms.Add(gridLoc);

        if (addToCheck)
            roomsToCheck.Add(currentRoom);
        else
            currentRoom.linked = true;
    }
    Vector3 GetWorldPos(Vector2 gridLoc)
    {
        Vector3 worldPos = transform.position + new Vector3(gridLoc.x * gridOffset.x, 0, gridLoc.y * gridOffset.y);
        return worldPos;
    }
    bool CheckForInGrid(Vector2 checkLoc)
    {
        if(checkLoc.x < 0 || checkLoc.x > dungeonPreset.gridSize.x - 1)
        {
            return false;
        }
        if (checkLoc.y < 0 || checkLoc.y > dungeonPreset.gridSize.y - 1)
        {
            return false;
        }
        return true;
    }
    bool CheckForRoom(Vector2 checkLoc)
    {
        foreach(DungeonRoom room in rooms)
        {
            if(room.gridLoc == checkLoc)
            {
                return true;
            }
        }
        return false;
    }
    void FillEmptyRooms()
    {
        for (int x = 0; x < dungeonPreset.gridSize.x; x++)
        {
            for (int y = 0; y < dungeonPreset.gridSize.y; y++)
            {
                if (!CheckForRoom(new Vector2(x, y)))
                {
                    SpawnRoom(new Vector2(x, y), normalRooms[Random.Range(0, normalRooms.Length)], false, true, true, false);
                }
            }
        }

        foreach(DungeonRoom room in roomsToCheck)
        {
            CheckForHasConnection(room);
        }

        Invoke("CheckEmptyRoomsIfConnected", 2);
    }
    void CheckEmptyRoomsIfConnected()
    {
        foreach(DungeonRoom room in roomsToCheck)
        {
            if (!room.linked)
            {
                SpawnRoom(room.gridLoc, wallRoom, false, false, false, true);
                Destroy(room.gameObject);
            }
        }
        Invoke("SpawnWalls", cooldownTime);
    }
    void SpawnWalls()
    {
        for (int x = 0; x < dungeonPreset.gridSize.x; x++)
        {
            Vector2 newLoc = new Vector2(x, -1);
            if (newLoc == rooms[0].gridLoc)
                continue;
            SpawnRoom(newLoc, wallRoom, false, false, false, true);
        }
        for (int x = 0; x < dungeonPreset.gridSize.x; x++)
        {
            SpawnRoom(new Vector2(x, dungeonPreset.gridSize.y), wallRoom, false, false, false, true);
        }
        for (int y = 0; y < dungeonPreset.gridSize.y; y++)
        {
            SpawnRoom(new Vector2(dungeonPreset.gridSize.x, y), wallRoom, false, false, false, true);
        }
        for (int y = 0; y < dungeonPreset.gridSize.y; y++)
        {
            SpawnRoom(new Vector2(-1, y), wallRoom, false, false, false, true);
        }

        Invoke("SpawnSpecialRooms", cooldownTime);
    }
    void CheckForHasConnection(DungeonRoom checkRoom)
    {
        if (checkRoom.forward)
        {
            if (CheckForRoom(checkRoom.gridLoc + new Vector2(0, 1)))
            {
                foreach(DungeonRoom room in rooms)
                {
                    if(room.gridLoc == checkRoom.gridLoc + new Vector2(0, 1))
                    {
                        if(room.back)
                            checkRoom.linkScript.rooms[0] = room;
                    }
                }
            }
        }
        if (checkRoom.back)
        {
            if (CheckForRoom(checkRoom.gridLoc + new Vector2(0, -1)))
            {
                foreach (DungeonRoom room in rooms)
                {
                    if (room.gridLoc == checkRoom.gridLoc + new Vector2(0, -1))
                    {
                        if (room.forward)
                            checkRoom.linkScript.rooms[1] = room;
                    }
                }
            }
        }
        if (checkRoom.right)
        {
            if (CheckForRoom(checkRoom.gridLoc + new Vector2(1, 0)))
            {
                foreach (DungeonRoom room in rooms)
                {
                    if (room.gridLoc == checkRoom.gridLoc + new Vector2(1, 0))
                    {
                        if (room.left)
                            checkRoom.linkScript.rooms[2] = room;
                    }
                }
            }
        }
        if (checkRoom.left)
        {
            if (CheckForRoom(checkRoom.gridLoc + new Vector2(-1, 0)))
            {
                foreach (DungeonRoom room in rooms)
                {
                    if (room.gridLoc == checkRoom.gridLoc + new Vector2(-1, 0))
                    {
                        if (room.right)
                            checkRoom.linkScript.rooms[3] = room;
                    }
                }
            }
        }

        checkRoom.linkScript.enabled = true;
    }
    void SpawnSpecialRooms()
    {
        #region Mini boss rooms
        for (int i = 0; i < dungeonPreset.miniBossCount; i++)
        {
            List<PossibleSpecialRoomLocs> possibleWalls = new List<PossibleSpecialRoomLocs>();
            #region CheckPossibleWalls
            foreach (DungeonRoom wall in walls)
            {
                PossibleSpecialRoomLocs newWall = new PossibleSpecialRoomLocs();
                bool possible = false;
                //check forward
                foreach (DungeonRoom checkRoom in rooms)
                {
                    if(wall.gridLoc + new Vector2(0, 1) == checkRoom.gridLoc)
                    {
                        if (!checkRoom.back)
                        {
                            continue;
                        }
                        if (checkRoom.linked)
                        {
                            possible = true;
                            newWall.possibleDirections.Add(0);
                            break;
                        }
                    }
                }
                //check back
                foreach (DungeonRoom checkRoom in rooms)
                {
                    if (wall.gridLoc + new Vector2(0, -1) == checkRoom.gridLoc)
                    {
                        if (!checkRoom.forward)
                            continue;
                        if (checkRoom.linked)
                        {
                            possible = true;
                            newWall.possibleDirections.Add(1);
                            break;
                        }
                    }
                }
                //check right
                foreach (DungeonRoom checkRoom in rooms)
                {
                    if (wall.gridLoc + new Vector2(1, 0) == checkRoom.gridLoc)
                    {
                        if (!checkRoom.left)
                            continue;
                        if (checkRoom.linked)
                        {
                            possible = true;
                            newWall.possibleDirections.Add(2);
                            break;
                        }
                    }
                }
                //check left
                foreach (DungeonRoom checkRoom in rooms)
                {
                    if (wall.gridLoc + new Vector2(-1, 0) == checkRoom.gridLoc)
                    {
                        if (!checkRoom.right)
                            continue;
                        if (checkRoom.linked)
                        {
                            possible = true;
                            newWall.possibleDirections.Add(3);
                            break;
                        }
                    }
                }
                if (possible)
                {
                    newWall.wall = wall;
                    possibleWalls.Add(newWall);
                }
            }
            #endregion

            if (possibleWalls.Count == 0)
                break;

            int randomWall = Random.Range(0, possibleWalls.Count);
            int randomDirIndex = Random.Range(0, possibleWalls[randomWall].possibleDirections.Count);
            walls.Remove(possibleWalls[randomWall].wall.GetComponent<DungeonRoom>());
            Destroy(possibleWalls[randomWall].wall.gameObject);
            SpawnRoom(possibleWalls[randomWall].wall.gridLoc, challangeRooms[possibleWalls[randomWall].possibleDirections[randomDirIndex]], false, false, false, false);

            float randomMiniBoss = Random.Range(0, 100);
            float latestRandom = 0;
            foreach(DungeonPreset.MiniBoss miniBoss in dungeonPreset.miniBosses)
            {
                latestRandom += miniBoss.spawnChance;
                if(miniBoss.spawnChance < latestRandom)
                {
                    Instantiate(miniBoss.boss, currentRoom.miniBossSpawnLoc.position, currentRoom.miniBossSpawnLoc.rotation, currentRoom.transform);
                    break;
                }
            }
        }
        #endregion
        #region Puzzle rooms
        for (int i = 0; i < dungeonPreset.puzzleRooms; i++)
        {
            List<PossibleSpecialRoomLocs> possibleWalls = new List<PossibleSpecialRoomLocs>();
            #region CheckPossibleWalls
            foreach (DungeonRoom wall in walls)
            {
                PossibleSpecialRoomLocs newWall = new PossibleSpecialRoomLocs();
                bool possible = false;
                //check forward
                foreach (DungeonRoom checkRoom in rooms)
                {
                    if (wall.gridLoc + new Vector2(0, 1) == checkRoom.gridLoc)
                    {
                        if (!checkRoom.back)
                        {
                            continue;
                        }
                        if (checkRoom.linked)
                        {
                            possible = true;
                            newWall.possibleDirections.Add(0);
                            break;
                        }
                    }
                }
                //check back
                foreach (DungeonRoom checkRoom in rooms)
                {
                    if (wall.gridLoc + new Vector2(0, -1) == checkRoom.gridLoc)
                    {
                        if (!checkRoom.forward)
                            continue;
                        if (checkRoom.linked)
                        {
                            possible = true;
                            newWall.possibleDirections.Add(1);
                            break;
                        }
                    }
                }
                //check right
                foreach (DungeonRoom checkRoom in rooms)
                {
                    if (wall.gridLoc + new Vector2(1, 0) == checkRoom.gridLoc)
                    {
                        if (!checkRoom.left)
                            continue;
                        if (checkRoom.linked)
                        {
                            possible = true;
                            newWall.possibleDirections.Add(2);
                            break;
                        }
                    }
                }
                //check left
                foreach (DungeonRoom checkRoom in rooms)
                {
                    if (wall.gridLoc + new Vector2(-1, 0) == checkRoom.gridLoc)
                    {
                        if (!checkRoom.right)
                            continue;
                        if (checkRoom.linked)
                        {
                            possible = true;
                            newWall.possibleDirections.Add(3);
                            break;
                        }
                    }
                }
                if (possible)
                {
                    newWall.wall = wall;
                    possibleWalls.Add(newWall);
                }
            }

            if (possibleWalls.Count == 0)
                break;

            #endregion
            int randomWall = Random.Range(0, possibleWalls.Count);
            int randomDirIndex = Random.Range(0, possibleWalls[randomWall].possibleDirections.Count);
            Destroy(possibleWalls[randomWall].wall.gameObject);
            SpawnRoom(possibleWalls[randomWall].wall.gridLoc, puzzleRooms[possibleWalls[randomWall].possibleDirections[randomDirIndex]], false, false, false, false);
        }
        #endregion

        Secret[] secrets = FindObjectsOfType<Secret>();

        DungeonManager.Instance.totalSecrets = secrets.Length;

        Invoke("NavMeshBake", cooldownTime);
    }
    void NavMeshBake()
    {
        //foreach(NavMeshSurface surface in navmeshes)
        //{
        //    surface.BuildNavMesh();
        //}

        sur.BuildNavMesh();

        DungeonManager.Instance.dungeonReady = true;
    }
    public Vector3 GetSpawnPoint()
    {
        return rooms[0].transform.position + new Vector3(0, 3,0);
    }
    public void GoToBossRoom(Transform player)
    {
        player.position = spawnPoint.position;
        player.rotation = spawnPoint.rotation;
    }
    public void GoToChestRoom(Transform player)
    {
        player.position = chestSpawnPos.position;
        player.rotation = chestSpawnPos.rotation;
    }
}
[System.Serializable]
public class PossibleSpecialRoomLocs
{
    public DungeonRoom wall;
    public List<int> possibleDirections = new List<int>();
}
