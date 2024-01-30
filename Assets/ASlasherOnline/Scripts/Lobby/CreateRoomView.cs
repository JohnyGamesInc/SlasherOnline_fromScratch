using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class CreateRoomView : MonoBehaviourPunCallbacks
    {
        
        [field: SerializeField] public Button CreateRoomButton { get; private set; }
        [field: SerializeField] public TMP_InputField RoomName { get; private set; }
        [field: SerializeField] public TMP_InputField RoomPassword { get; private set; }
        [field: SerializeField] public Toggle PrivateToggler { get; private set; }

        [SerializeField] private ConnectionUIView connectionView;
        
        [SerializeField] private TMP_InputField friendsIdInput;
        [SerializeField] private Button submitFriendIdButton;
        [SerializeField] private TMP_Text friendsIDs;
        
        private CanvasGroup canvas;
        
        private TypedLobby defaultLobby = new TypedLobby("DefaultDevLobby", LobbyType.Default);

        private List<string> expectedUsersList = new();
        

        private void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
            Init(OnCreateRoomButtonSubscribe);
            submitFriendIdButton.onClick.AddListener(OnReserveFriendIdSubscribe);
        }


        private void Update()
        {
            ToggleHiding(!PhotonNetwork.InRoom);
        }


        private void Init(Action<string, string, bool> CreateRoomPressedCallback)
        {
            CreateRoomButton.onClick.AddListener(() =>
            {
                var roomName = string.IsNullOrEmpty(RoomName.text) ? null : RoomName.text;
                var roomPassword = string.IsNullOrEmpty(RoomPassword.text) ? null : RoomPassword.text;
                var isPrivate = PrivateToggler.isOn;
                
                CreateRoomPressedCallback.Invoke(roomName, roomPassword, isPrivate);
            });
        }


        private void ToggleHiding(bool isVisible)
        {
            if (isVisible)
            {
                canvas.alpha = 1;
                canvas.interactable = true;
                canvas.blocksRaycasts = true;
            }
            else
            {
                canvas.alpha = 0;
                canvas.interactable = false;
                canvas.blocksRaycasts = false;
            }
        }
        
        
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("From CreateRoomView: Joined To LOBBY");
            CleanExpectedUsersListCache();
        }
        
        
        public override void OnCreatedRoom()
        {
            Debug.Log($"Created room: {PhotonNetwork.CurrentRoom.Name}");
            connectionView.UpdateLabel($"Created room: {PhotonNetwork.CurrentRoom.Name}", true);
            Debug.Log($"Count of Rooms: {PhotonNetwork.CountOfRooms}");
        }


        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed creating the room {returnCode}: {message}");
            connectionView.UpdateLabel($"Failed creating the room {returnCode}: {message}", false);
        }
        
        
        private void OnCreateRoomButtonSubscribe(string roomName, string roomPassword, bool isPrivate)
        {
            var roomOptions = new RoomOptions
            {
                MaxPlayers = 4,
                PublishUserId = true,
                IsVisible = !isPrivate,
            };
            
            var enterRoomParams = new EnterRoomParams
            {
                RoomName = roomName,
                RoomOptions = roomOptions,
                Lobby = defaultLobby,
                ExpectedUsers = expectedUsersList.ToArray()
            };
            
            if (expectedUsersList.Count == 0)
                PhotonNetwork.CreateRoom(enterRoomParams.RoomName, enterRoomParams.RoomOptions);
            else 
                PhotonNetwork.CreateRoom(enterRoomParams.RoomName, enterRoomParams.RoomOptions, defaultLobby, enterRoomParams.ExpectedUsers);
        }
        
        
        private void OnReserveFriendIdSubscribe()
        {
            var id = friendsIdInput.text;
            friendsIDs.text += $"{id}, ";
            expectedUsersList.Add(id);
        }


        private void CleanExpectedUsersListCache()
        {
            expectedUsersList.Clear();
            friendsIDs.text = "IDs: ";
        }
        

        private void OnDestroy()
        {
            CreateRoomButton.onClick.RemoveAllListeners();
            submitFriendIdButton.onClick.RemoveAllListeners();
        }
        
        
    }
}