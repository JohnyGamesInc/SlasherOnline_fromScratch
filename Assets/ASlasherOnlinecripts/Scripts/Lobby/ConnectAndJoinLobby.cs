using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using TMPro;
using UnityEngine;


namespace SlasherOnline
{


    public class ConnectAndJoinLobby : MonoBehaviourPunCallbacks
    {
        
        [SerializeField] private PhotonLauncherUI ui;
        [SerializeField] private ServerSettings serverSettings;
        [SerializeField] private TMP_Text stateLabel;
        
        
        private readonly string gameVersion = "1";

        private const string GAME_MODE_KEY = "GM";
        private const string AI_MODE_KEY = "AI";

        private const string MAP_PROP_KEY = "MK";
        private const string LVL_PROP_KEY = "LV";

        private TypedLobby sqlLobby = new TypedLobby("DevLobby", LobbyType.SqlLobby);
        
        
        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            ui.Init(Connect, Disconnect);
        }


        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings(serverSettings.AppSettings);
                PhotonNetwork.GameVersion = gameVersion;

                Debug.Log("Photon Network is Connecting");
            }
            else
            {
                Debug.Log("Photon Network is Already Connected");
            }
        }


        private void Update()
        {
            // Do we need here LoadBalancingClient.Service() ?
            stateLabel.text = PhotonNetwork.NetworkClientState.ToString();
        }


        private void Connect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("Network is Disconnected... Trying to reconnect");
                PhotonNetwork.ConnectUsingSettings(serverSettings.AppSettings);
                PhotonNetwork.GameVersion = gameVersion;
            }
            
            if (!PhotonNetwork.InRoom)
                CreateRoom();
        }


        private void Disconnect()
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            
            Debug.Log($"Disconnected reason: {cause}");

            ui.UpdateLabel(false);
        }


        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster was called by PUN");
            ui.UpdateLabel("OnConnectedToMaster was called by PUN", true);
        }

        
        private void CreateRoom()
        {
            var roomOptions = new RoomOptions
            {
                MaxPlayers = 12,
                PublishUserId = true, // Enabling reserving slots in the room
                // CustomRoomPropertiesForLobby = new []{ GAME_MODE_KEY }
                CustomRoomPropertiesForLobby = new []{ MAP_PROP_KEY, LVL_PROP_KEY },
                CustomRoomProperties = new Hashtable
                {
                    { LVL_PROP_KEY, 3 },
                    { MAP_PROP_KEY, "Map3"}
                }
            };

            var enterRoomParams = new EnterRoomParams
            {
                RoomName = "DevRoom",
                RoomOptions = roomOptions,
                ExpectedUsers = new []{ "" }, //Expect next userIds list in the room
                Lobby = sqlLobby
            };
            
            PhotonNetwork.CreateRoom(enterRoomParams.RoomName, enterRoomParams.RoomOptions);
        }


        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined to the room: {PhotonNetwork.CurrentRoom.Name}");
            ui.UpdateLabel($"Joined to the room: {PhotonNetwork.CurrentRoom.Name}", true);
        }


        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed joining to the room {returnCode}: {message}");
            ui.UpdateLabel($"Failed joining to the room {returnCode}: {message}", false);
        }


        public override void OnCreatedRoom()
        {
            Debug.Log($"Created room: {PhotonNetwork.CurrentRoom.Name}");
            ui.UpdateLabel($"Created room: {PhotonNetwork.CurrentRoom.Name}", true);
        }


        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed creating the room {returnCode}: {message}");
            ui.UpdateLabel($"Failed creating the room {returnCode}: {message}", false);
        }


        public override void OnLeftRoom()
        {
            Debug.Log($"Left from the room");
            ui.UpdateLabel($"Left from the room", true);
        }


        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log($"Rooms is [{roomList.Count}]");
        }

        public override void OnConnected()
        {
            base.OnConnected();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();

            var sqlLobbyFilter = $"{MAP_PROP_KEY} = Map3 AND {LVL_PROP_KEY} BETWEEN 2 AND 4";
            var joinRandomRoomParams = new OpJoinRandomRoomParams
            {
                SqlLobbyFilter = sqlLobbyFilter
            };
            // PhotonNetwork.JoinRandomRoom(joinRandomRoomParams);
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
        }

        public override void OnRegionListReceived(RegionHandler regionHandler)
        {
            base.OnRegionListReceived(regionHandler);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        }

        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            base.OnFriendListUpdate(friendList);
        }

        public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            base.OnCustomAuthenticationResponse(data);
        }

        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            base.OnCustomAuthenticationFailed(debugMessage);
        }

        public override void OnWebRpcResponse(OperationResponse response)
        {
            base.OnWebRpcResponse(response);
        }

        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            base.OnLobbyStatisticsUpdate(lobbyStatistics);
        }

        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            base.OnErrorInfo(errorInfo);
        }


        private void OnDestroy()
        {
            if (PhotonNetwork.IsConnected) 
                PhotonNetwork.Disconnect();
        }
        
        
    }
}