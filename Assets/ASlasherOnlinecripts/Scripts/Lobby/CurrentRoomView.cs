using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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

        [SerializeField] private ConnectionUIView connectionView;
        
        [SerializeField] private GameObject templateUserPanel;


        private CanvasGroup mainCanvas;

        private Dictionary<string, RoomUserView> roomUsers = new();


        private void Awake()
        {
            mainCanvas = GetComponent<CanvasGroup>();
            ReadyButton.onClick.AddListener(ReadyButtonSubscribe);
            NotReadyButton.onClick.AddListener(NotReadyButtonSubscribe);
            LeaveRoomButton.onClick.AddListener(LeaveRoomButtonSubscribe);
            
            templateUserPanel.SetActive(false);
        }


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
        }
        
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.LogError($"Failed joining to the room: {returnCode} {message}");
        }


        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed joining to the room {returnCode}: {message}");
            connectionView.UpdateLabel($"Failed joining to the room {returnCode}: {message}", false);
        }
        
        
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log($"Left from the room");
            connectionView.UpdateLabel($"Left from the room", true);
            
            var userView = roomUsers[PhotonNetwork.LocalPlayer.UserId];
            roomUsers.Remove(PhotonNetwork.LocalPlayer.UserId);
            Destroy(userView.gameObject);
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
            
        }


        private void NotReadyButtonSubscribe()
        {
            
        }


        private void LeaveRoomButtonSubscribe()
        {
            Debug.Log("Leave Room button Pressed");
            PhotonNetwork.LeaveRoom();
        }


        private void OnDestroy()
        {
            ReadyButton.onClick.RemoveAllListeners();
            NotReadyButton.onClick.RemoveAllListeners();
            LeaveRoomButton.onClick.RemoveAllListeners();
        }
        
        
    }
}