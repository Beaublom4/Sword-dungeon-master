using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DungeonManager : MonoBehaviourPunCallbacks
{
    public static DungeonManager Instance;

    PhotonView pv;

    [SerializeField] GameObject playerManager;
    public GameObject starterDoor;

    [HideInInspector] public bool dungeonReady, playerSpawned, dungeonStarted;

    [Header("Master dungeon info")]
    [SerializeField] int playersReady;

    [Header("Solo dungeon info")]
    [SerializeField] bool isReady;

    [Header("Grade")]
    bool upGrade;
    [SerializeField] int score;
    public int totalSecrets;
    [SerializeField] int secretsCollected;
    [SerializeField] int deaths;
    public int totalRoomsToClear;
    [SerializeField] int roomsCleared;
    [SerializeField] int puzzlesFinished;
    bool adTime = true;
    [SerializeField] float timePlayed;

    [Header("Chests")]
    [SerializeField] GameObject magicalChest;
    [SerializeField] GameObject darknessChest, diamondChest, goldenChest, reinforcedChest;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        Instance = this;
    }
    private void Start()
    {
        HudManager.Instance.currentLobbyText.text = DungeonCreator.dungeonPreset.name;
        HudManager.Instance.currentObjective.text = "Find and kill the boss";
        HudManager.Instance.SetUpPercentage();

        SpawnPlayerInfo();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            upGrade = true;
        }

        if (adTime)
        {
            timePlayed += Time.deltaTime;
            System.TimeSpan time = System.TimeSpan.FromSeconds(timePlayed);
            if(timePlayed < 3600)
                HudManager.Instance.timeExpired.text = "Time expired: " + string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            else
                HudManager.Instance.timeExpired.text = "Time expired: " + string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }

        if (!playerSpawned && dungeonReady)
        {
            playerSpawned = true;
            DungeonCreated();
        }
    }
    public void DungeonCreated()
    {
        if(HubManager.Instance != null)
            HubManager.Instance.LoadLevels();
        PhotonNetwork.Instantiate(playerManager.name, Vector3.zero, Quaternion.identity);
    }

    public void SpawnPlayerInfo()
    {
        pv.RPC("RPC_SpawnPlayerInfo", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }
    [PunRPC]
    void RPC_SpawnPlayerInfo(Player _player)
    {
        PlayerInDungeon player = Instantiate(HudManager.Instance.playerName, HudManager.Instance.playerHolder).GetComponent<PlayerInDungeon>();
        player.SetUp(_player);
    }

    public void ChangePlayerInfo(Player _player, float _currentHealth, float _maxHealth)
    {
        pv.RPC("RPC_ChangePlayerInfo", RpcTarget.AllBuffered, _player, _currentHealth, _maxHealth);
    }
    [PunRPC]
    void RPC_ChangePlayerInfo(Player _player, float _currentHealth, float _maxHealth)
    {
        foreach(Transform t in HudManager.Instance.playerHolder)
        {
            PlayerInDungeon pin = t.GetComponent<PlayerInDungeon>();
            if(pin.player == _player)
            {
                pin.ShowHealth(_currentHealth, _maxHealth);
            }
        }
    }

    public void ReadyUp()
    {
        if (dungeonStarted)
            return;

        isReady = !isReady;
        pv.RPC("RPC_ReadyUp", RpcTarget.All, PhotonNetwork.LocalPlayer, isReady);
    }
    [PunRPC]
    void RPC_ReadyUp(Player player, bool ready)
    {
        foreach(Transform t in HudManager.Instance.playerHolder)
        {
            PlayerInDungeon pin = t.GetComponent<PlayerInDungeon>();
            if(pin.player == player)
            {
                pin.ToggleReady(ready);
            }
        }

        if (!PhotonNetwork.IsMasterClient)
            return;

        if (ready)
        {
            playersReady++;

            if (playersReady >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                pv.RPC("RPC_StartDungeon", RpcTarget.All);
            }
        }
        else
        {
            playersReady--;
        }
    }
    [PunRPC]
    void RPC_StartDungeon()
    {
        dungeonStarted = true;
        starterDoor.SetActive(false);

        PlayerHealth[] healths = FindObjectsOfType<PlayerHealth>();
        foreach(PlayerHealth health in healths)
        {
            if (health.GetComponent<PhotonView>().IsMine)
            {
                pv.RPC("RPC_ShowHealth", RpcTarget.All, PhotonNetwork.LocalPlayer, health.currentHealth, Stats.health);
            }
        }
    }
    [PunRPC]
    void RPC_ShowHealth(Player player, float currentHealth, float maxHealth)
    {
        foreach (Transform t in HudManager.Instance.playerHolder)
        {
            PlayerInDungeon pin = t.GetComponent<PlayerInDungeon>();
            if (pin.player == player)
            {
                pin.ShowHealth(currentHealth, maxHealth);
            }
        }
    }
    public void PickUpSecret(Secret secret)
    {
        secretsCollected++;
        DungeonRoom dungeonRoom = secret.GetComponentInParent<DungeonRoom>();
        //add buffs
        Destroy(secret.gameObject);
        StartCoroutine(UpdateCurrentRoom(dungeonRoom));
    }
    IEnumerator UpdateCurrentRoom(DungeonRoom room)
    {
        yield return new WaitForSeconds(.1f);
        room.ShowCollectedSecrets();
    }
    public void RoomCleared()
    {
        roomsCleared++;
        HudManager.Instance.PercentageCleared(totalRoomsToClear, roomsCleared);
    }
    public void PuzzleFinished()
    {
        puzzlesFinished++;
    }
    public void SpawnChests(bool dead)
    {
        adTime = false;
        score = 100;

        float timeExpiredMinutes = timePlayed / 60;
        if (timeExpiredMinutes >= 10 && timeExpiredMinutes < 15)
        {
            Debug.Log("Time between 10 min / 15 min");
            score -= 10;
        }
        else if (timeExpiredMinutes >= 15)
        {
            Debug.Log("Time longer 15 min");
            score -= 25;
        }

        score -= 5 * deaths;
        score -= 5 * (totalSecrets - secretsCollected);

        score -= 15 * (DungeonCreator.dungeonPreset.puzzleRooms - puzzlesFinished);

        if (totalRoomsToClear > 0 && roomsCleared > 0)
        {
            float clearedPercentage = (roomsCleared / totalRoomsToClear) * 100;
            if (clearedPercentage < 100 && clearedPercentage >= 75f)
            {
                Debug.Log("clear percentage: 99 - 75");
                score -= 10;
            }
            else if (clearedPercentage < 75f && clearedPercentage >= 50f)
            {
                Debug.Log("clear percentage: 74 - 50");
                score -= 25;
            }
            else if (clearedPercentage < 50f)
            {
                Debug.Log("clear percentage: < 50");
                score -= 50;
            }
        }

        if (score < 0 || dead)
            score = 0;

        if (upGrade)
            score = 100;

        if (score == 100)
            magicalChest.SetActive(true);
        if (score > 90)
            darknessChest.SetActive(true);
        if(score > 75)
            diamondChest.SetActive(true);
        if (score > 50)
            goldenChest.SetActive(true);
        if (score > 25)
            reinforcedChest.SetActive(true);
    }
}
