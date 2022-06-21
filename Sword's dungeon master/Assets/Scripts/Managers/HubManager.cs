using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HubManager : MonoBehaviourPunCallbacks
{
    PhotonView pv;

    public static HubManager Instance;
    bool active, loading;
    [SerializeField] GameObject panel;

    public GameObject PlayerPrefab;
    public Vector3 spawnPoint;

    public static bool openParty;

    [Header("Player name")]
    [SerializeField] GameObject nameInput;

    [Header("Party")]
    [SerializeField] Toggle openRoomToggle;
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerListHolder;

    [Header("Party list")]
    [SerializeField] GameObject partyInfoPrefab;
    [SerializeField] Transform partyListHolder;
    string newRoomName;

    [Header("Floors")]
    public GameObject floorSelectButton;
    public static DungeonPreset dungeonPreset;

    [SerializeField] DungeonPreset[] floors;
    [SerializeField] GameObject floorInfoPrefab;
    [SerializeField] Transform floorListHolder;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        Instance = this;
        SpawnDungeonButtons();
    }
    private void Update()
    {
        if (loading)
        {
            HudManager.Instance.loadingSlider.value = PhotonNetwork.LevelLoadingProgress;
        }

        if (!active)
            return;

        if (Input.GetButtonDown("Cancel"))
        {
            if (PlayerInteract.inMenu)
                return;
            active = false;

            panel.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
        {
            JoinedRoom();
        }
    }
    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.NickName != "")
            NameCheck();
        else
            nameInput.SetActive(true);
    }
    public void ChangeName(string newName)
    {
        PhotonNetwork.NickName = newName;
        if(PhotonNetwork.InRoom)
            pv.RPC("RPC_SyncName", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, newName);
        nameInput.SetActive(false);
        NameCheck();
    }
    void NameCheck()
    {
        PhotonNetwork.JoinLobby();
    }
    [PunRPC]
    void RPC_SyncName(Player player, string playerName)
    {
        foreach(Transform t in playerListHolder)
        {
            PlayerInfo playerInfo = playerListHolder.GetComponent<PlayerInfo>();
            if (playerInfo.player == player)
            {
                playerInfo.playerName.text = playerName;
            }
        }
    }
    public override void OnJoinedLobby()
    {
        JoinedLobby();
    }
    void JoinedLobby()
    {
        foreach (Transform t in playerListHolder)
        {
            Destroy(t.gameObject);
        }

        if (newRoomName == null)
        {
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 4;
            ro.IsOpen = openParty;
            PhotonNetwork.CreateRoom(PhotonNetwork.NickName, ro);
        }
        else
        {
            PhotonNetwork.JoinRoom(newRoomName);
            newRoomName = "";
        }
    }
    public override void OnJoinedRoom()
    {
        JoinedRoom();
    }
    void JoinedRoom()
    {
        HudManager.Instance.currentLobbyText.text = "Hub";
        HudManager.Instance.currentObjective.text = "Start new floor";

        PhotonNetwork.Instantiate(PlayerPrefab.name, spawnPoint, Quaternion.identity);

        pv.RPC("RPC_SpawnPlayerInfo", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);

        openRoomToggle.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        floorSelectButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    [PunRPC]
    void RPC_SpawnPlayerInfo(Player player)
    {
        GameObject newInfo = Instantiate(playerInfoPrefab, playerListHolder);
        PlayerInfo pi = newInfo.GetComponent<PlayerInfo>();
        pi.player = player;
        pi.playerName.text = player.NickName;
        pi.level.text = 1.ToString();
    }
    public void OpenDungeonManager()
    {
        active = true;
        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ChangePartyOpen(bool toggle)
    {
        if (toggle)
        {
            //Open party
            openParty = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
        else
        {
            //Close party
            openParty = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }
    public void JoinParty(string _name)
    {
        if (_name == "")
            return;
        newRoomName = _name;
        PhotonNetwork.LeaveRoom();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;
        ro.IsOpen = openParty;
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, ro);
    }
    public void SpawnDungeonButtons()
    {
        foreach(DungeonPreset preset in floors)
        {
            FloorInfo info = Instantiate(floorInfoPrefab, floorListHolder).GetComponent<FloorInfo>();
            info.floor = preset;
            info.floorName.text = preset.name;
        }
    }
    public void StartDungeon(FloorInfo floor)
    {
        DungeonCreator.dungeonPreset = floor.floor;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        loading = true;
        HudManager.Instance.loadingPanel.SetActive(true);
        PhotonNetwork.LoadLevel(1);
    }
    public void LoadLevels()
    {
        if (PhotonNetwork.IsMasterClient)
            return;
        pv.RPC("RPC_DungeonLoad", RpcTarget.Others);
    }
    [PunRPC]
    void RPC_DungeonLoad()
    {
        PhotonNetwork.LoadLevel(1);
    }
}