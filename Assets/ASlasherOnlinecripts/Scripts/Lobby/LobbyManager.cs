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


    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        
        [SerializeField] private PhotonLauncherUI ui;
        [SerializeField] private ServerSettings serverSettings;
        [SerializeField] private TMP_Text stateLabel;
        
        
        private readonly string gameVersion = "1";

        private const string GAME_MODE_KEY = "GM";
        private const string AI_MODE_KEY = "AI";

        private const string MAP_PROP_KEY = "MK";
        private const string LVL_PROP_KEY = "LV";

        private TypedLobby sqlLobby = new TypedLobby("SQLDevLobby", LobbyType.SqlLobby);
        private TypedLobby defaultLobby = new TypedLobby("DefaultDevLobby", LobbyType.Default);
        
        
        private Dictionary<string, RoomInfo> cachedRoomList = new();
        
        
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
            // CreateRoom();
            PhotonNetwork.JoinLobby(defaultLobby);
        }

        
        private void CreateRoom()
        {
            var roomOptions = new RoomOptions
            {
                // MaxPlayers = 12,
                PublishUserId = true, // Enabling reserving slots in the room
                // CustomRoomPropertiesForLobby = new []{ MAP_PROP_KEY, LVL_PROP_KEY },
                // CustomRoomProperties = new Hashtable
                // {
                //     { LVL_PROP_KEY, 3 },
                //     { MAP_PROP_KEY, "Map3"}
                // }
            };

            var enterRoomParams = new EnterRoomParams
            {
                RoomName = null,
                RoomOptions = roomOptions,
                // ExpectedUsers = new []{ "" }, //Expect next userIds list in the room
                Lobby = defaultLobby
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
            Debug.Log($"Count of Rooms: {PhotonNetwork.CountOfRooms}");
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
            base.OnRoomListUpdate(roomList);
            Debug.Log($"Rooms is [{roomList.Count}]");
            UpdateCachedRoomList(roomList);
        }

        
        public override void OnConnected()
        {
            Debug.Log("Connected");
        }

        
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            Debug.Log("Master client is switched");
        }

        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.LogError($"Failed joining to the room: {returnCode} {message}");
        }


        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo info = roomList[i];
                if (info.RemovedFromList)
                {
                    cachedRoomList.Remove(info.Name);
                }
                else
                {
                    cachedRoomList[info.Name] = info;
                }
            }
        }

        
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            
            Debug.Log("Joined To LOBBY");

            // var sqlLobbyFilter = $"{MAP_PROP_KEY} = Map3 AND {LVL_PROP_KEY} BETWEEN 2 AND 4";
            // var joinRandomRoomParams = new OpJoinRandomRoomParams
            // {
            //     SqlLobbyFilter = sqlLobbyFilter
            // };
            // PhotonNetwork.JoinRandomRoom(joinRandomRoomParams);
        }

        
        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("Player left lobby");
        }

        
        public override void OnRegionListReceived(RegionHandler regionHandler)
        {
            base.OnRegionListReceived(regionHandler);
            Debug.Log($"Region list received: {regionHandler}");
        }

        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.Log($"New player entered room: {newPlayer.UserId} {newPlayer.NickName}");
        }

        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log($"Player left the room: {otherPlayer.UserId} {otherPlayer.NickName}");
        }

        
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            Debug.Log($"Room properties were updated on {propertiesThatChanged}");
        }

        
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            Debug.Log($"Player Props were updated for {targetPlayer.UserId} on {changedProps}");
        }

        
        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            base.OnFriendListUpdate(friendList);
            Debug.Log($"Friends list were updated {friendList.Count}");
        }

        
        public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            base.OnCustomAuthenticationResponse(data);
            Debug.Log($"Custom authentication response {data}");
        }

        
        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            base.OnCustomAuthenticationFailed(debugMessage);
            Debug.LogError($"Custom authentication Failed {debugMessage}");
        }

        
        public override void OnWebRpcResponse(OperationResponse response)
        {
            base.OnWebRpcResponse(response);
            Debug.Log($"Web RPC Response: {response}");
        }

        
        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            base.OnLobbyStatisticsUpdate(lobbyStatistics);
            Debug.Log($"Lobby statistics were updated: {lobbyStatistics}");
        }

        
        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            base.OnErrorInfo(errorInfo);
            Debug.LogError($"Error Info: {errorInfo}");
        }


        private void OnDestroy()
        {
            if (PhotonNetwork.IsConnected) 
                PhotonNetwork.Disconnect();
        }
        
        
    }
}