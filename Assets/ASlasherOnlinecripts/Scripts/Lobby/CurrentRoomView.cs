using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace SlasherOnline
{
    
    public class CurrentRoomView : MonoBehaviourPunCallbacks
    {

        [field: SerializeField] public TMP_Text RoomName { get; private set; }
        [field: SerializeField] public TMP_InputField LobbyCode { get; private set; }
        [field: SerializeField] public GameObject PlayersListContent { get; private set; }
        [field: SerializeField] public RoomUserView PlayerEntryPrefab { get; private set; }
        
        [field: SerializeField] public Button ReadyButton { get; private set; }
        [field: SerializeField] public Button NotReadyButton { get; private set; }
        [field: SerializeField] public Button LeaveRoomButton { get; private set; }
        
        [SerializeField] private CanvasGroup readyCanvas;
        [SerializeField] private CanvasGroup notReadyCanvas;

        [SerializeField] private Toggle closeRoomToggle;
        [SerializeField] private Toggle visibleRoomToggle;

        [SerializeField] private ConnectionUIView connectionView;
        
        [SerializeField] private GameObject templateUserPanel;
        
        [SerializeField] private TMP_Text expectedFriends;


        private CanvasGroup mainCanvas;

        private Dictionary<string, RoomUserView> roomUsers = new();
        
        private TypedLobby defaultLobby = new TypedLobby("DefaultDevLobby", LobbyType.Default);

        private Coroutine playerStatusChecker;


        private void Awake()
        {
            mainCanvas = GetComponent<CanvasGroup>();
            ReadyButton.onClick.AddListener(ReadyButtonSubscribe);
            NotReadyButton.onClick.AddListener(NotReadyButtonSubscribe);
            LeaveRoomButton.onClick.AddListener(LeaveRoomButtonSubscribe);
            closeRoomToggle.onValueChanged.AddListener(OnCloseRoomToggle);
            visibleRoomToggle.onValueChanged.AddListener(OnVisibleRoomToggle);
            
            templateUserPanel.SetActive(false);
            
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        // private void Start()
        // {
        //     playerStatusChecker = StartCoroutine(CheckAllIsReady());
        // }


        private void Update()
        {
            ToggleHiding(PhotonNetwork.InRoom);
        }


        private void ToggleHiding(bool isVisible)
        {
            if (isVisible)
            {
                mainCanvas.alpha = 1;
                mainCanvas.interactable = true;
                mainCanvas.blocksRaycasts = true;
            }
            else
            {
                mainCanvas.alpha = 0;
                mainCanvas.interactable = false;
                mainCanvas.blocksRaycasts = false;
            }
        }

        
        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined to the room: {PhotonNetwork.CurrentRoom.Name}");
            RoomName.text = PhotonNetwork.CurrentRoom.Name;
            LobbyCode.text = PhotonNetwork.CurrentLobby.Name;
            
            connectionView.UpdateLabel($"Joined to the room: {PhotonNetwork.CurrentRoom.Name}", true);

            PhotonNetwork.PlayerList.ToList().ForEach(p =>
            {
                RoomUserView roomUser = Instantiate(PlayerEntryPrefab, PlayersListContent.transform);
                roomUsers[p.UserId] = roomUser;
                roomUser.PlayerName.text = p.UserId;
            });

            UpdateCurrentRoomPlayersEntries();

            closeRoomToggle.isOn = !PhotonNetwork.CurrentRoom.IsOpen;
            visibleRoomToggle.isOn = !PhotonNetwork.CurrentRoom.IsVisible;
            expectedFriends.text = PhotonNetwork.CurrentRoom.ExpectedUsers != null ? ExpectedUsersToString(PhotonNetwork.CurrentRoom.ExpectedUsers) : "";
        }

        
        private void UpdateCurrentRoomPlayersEntries()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                var playerEntry = roomUsers[player.UserId];
                player.CustomProperties.TryGetValue("IsReady", out object isReady);
                if (isReady != null) 
                    playerEntry.Status.text = (bool) isReady ? "<color=#0C9F00>Ready" : "<color=#B70000>Not Ready";
            }
        }


        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.LogError($"Failed joining to the room: {returnCode} {message}");
            connectionView.UpdateLabel($"Failed joining to the room {returnCode}: {message}", false);
            PhotonNetwork.JoinLobby(defaultLobby);
        }


        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed joining to the room {returnCode}: {message}");
            connectionView.UpdateLabel($"Failed joining to the room {returnCode}: {message}", false);
            PhotonNetwork.JoinLobby(defaultLobby);
        }
        
        
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log($"Left from the room");
            connectionView.UpdateLabel($"Left from the room", true);
            
            foreach (var userEntry in roomUsers)
            {
                var userView = roomUsers[userEntry.Key];
                Destroy(userView.gameObject);
            }
            
            roomUsers.Clear();
        }
        
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.Log($"New player entered room: {newPlayer.UserId} {newPlayer.NickName}");
            
            RoomUserView roomUser = Instantiate(PlayerEntryPrefab, PlayersListContent.transform);
            roomUsers[newPlayer.UserId] = roomUser;
            roomUser.PlayerName.text = newPlayer.UserId;
        }

        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log($"Player left the room: {otherPlayer.UserId} {otherPlayer.NickName}");
            
            var userView = roomUsers[otherPlayer.UserId];
            roomUsers.Remove(otherPlayer.UserId);
            Destroy(userView.gameObject);
        }

        
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            Debug.Log($"Room properties were updated on {propertiesThatChanged}");
        }


        private void ReadyButtonSubscribe()
        {
            readyCanvas.alpha = 0;
            readyCanvas.interactable = false;
            readyCanvas.blocksRaycasts = false;
            
            notReadyCanvas.alpha = 1;
            notReadyCanvas.interactable = true;
            notReadyCanvas.blocksRaycasts = true;
            
            if (!PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                {
                    {"IsReady", true},
                }))
            {
                Debug.LogError("Failed to Set IsReady custom property");
            }
            
            photonView.RPC(nameof(UpdateRoomPlayerStatusRPC), RpcTarget.All, true, PhotonNetwork.LocalPlayer.UserId);
            // PhotonNetwork.LoadLevel("PunBasicBigRoom");
            photonView.RPC(nameof(LevelLoadRPC), RpcTarget.All);
            // SceneManager.LoadScene("PunBasicBigRoom");
        }


        private void NotReadyButtonSubscribe()
        {
            readyCanvas.alpha = 1;
            readyCanvas.interactable = true;
            readyCanvas.blocksRaycasts = true;
            
            notReadyCanvas.alpha = 0;
            notReadyCanvas.interactable = false;
            notReadyCanvas.blocksRaycasts = false;
            
            if (!PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                {
                    {"IsReady", false},
                }))
            {
                Debug.LogError("Failed to Set IsReady custom property");
            }
            
            photonView.RPC(nameof(UpdateRoomPlayerStatusRPC), RpcTarget.All, false, PhotonNetwork.LocalPlayer.UserId);
        }


        [PunRPC]
        private void UpdateRoomPlayerStatusRPC(bool isReady, string userId)
        {
            var roomUser = roomUsers[userId];
            roomUser.Status.text = isReady ? "<color=#0C9F00>Ready" : "<color=#B70000>Not Ready";
            PhotonNetwork.LoadLevel("PunBasicBigRoom");
        }


        [PunRPC]
        private void LevelLoadRPC()
        {
            // StartCoroutine(LevelLoader());
            PhotonNetwork.LoadLevel("PunBasicBigRoom");
        }


        private IEnumerator LevelLoader()
        {
            PhotonNetwork.LoadLevel("PunBasicBigRoom");

            while (PhotonNetwork.LevelLoadingProgress < 1)
            {
                yield return new WaitForEndOfFrame();
            }
        }


        private IEnumerator CheckAllIsReady()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                
                if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        player.CustomProperties.TryGetValue("IsReady", out object IsReady);
                        if(IsReady == null || !(bool)IsReady) 
                            yield break;
                    }
                    PhotonNetwork.LoadLevel("PunBasicBigRoom");
                }
            }
        }


        private void LeaveRoomButtonSubscribe()
        {
            Debug.Log("Leave Room button Pressed");
            
            NotReadyButtonSubscribe();
            
            if (!PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                {
                    {"IsReady", null},
                }))
            {
                Debug.LogError("Failed to Set IsReady custom property");
            }

            PhotonNetwork.LeaveRoom();
        }


        private void OnCloseRoomToggle(bool isToggled)
        {
            Debug.Log("Room Close toggled");
            PhotonNetwork.CurrentRoom.IsOpen = !isToggled;
            PhotonNetwork.CurrentRoom.IsVisible = !isToggled;
            visibleRoomToggle.isOn = !isToggled;
        }

        
        private void OnVisibleRoomToggle(bool isToggled)
        {
            Debug.Log("Room Visibility toggled");
            PhotonNetwork.CurrentRoom.IsVisible = isToggled;
        }
        
        
        private string ExpectedUsersToString(string[] usersIds)
        {
            var text = "";
            foreach (var user in usersIds)
            {
                text += user;
            }

            return text;
        }
        

        private void OnDestroy()
        {
            ReadyButton.onClick.RemoveAllListeners();
            NotReadyButton.onClick.RemoveAllListeners();
            LeaveRoomButton.onClick.RemoveAllListeners();
            closeRoomToggle.onValueChanged.RemoveAllListeners();
            
            StopAllCoroutines();
        }
        
        
    }
}