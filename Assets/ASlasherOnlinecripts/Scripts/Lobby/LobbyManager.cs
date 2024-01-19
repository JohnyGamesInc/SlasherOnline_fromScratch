using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


namespace SlasherOnline
{


    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        
        [FormerlySerializedAs("connetionView")]
        [FormerlySerializedAs("conncetionView")]
        [SerializeField] private ConnectionUIView connectionView;
        [SerializeField] private ServerSettings serverSettings;
        [SerializeField] private TMP_Text stateLabel;
        
        private readonly string gameVersion = "1";

        private const string GAME_MODE_KEY = "GM";
        private const string AI_MODE_KEY = "AI";

        private const string MAP_PROP_KEY = "MK";
        private const string LVL_PROP_KEY = "LV";

        private TypedLobby sqlLobby = new TypedLobby("SQLDevLobby", LobbyType.SqlLobby);
        private TypedLobby defaultLobby = new TypedLobby("DefaultDevLobby", LobbyType.Default);
        
        
        
        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            
            connectionView.Init(Connect, Disconnect);
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
        
        
        private void CreateRoom()
        {
            var roomOptions = new RoomOptions
            {
                MaxPlayers = 12,
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


        private void Disconnect()
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            
            Debug.Log($"Disconnected reason: {cause}");

            connectionView.UpdateLabel(false);
        }


        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster was called by PUN");
            connectionView.UpdateLabel("OnConnectedToMaster was called by PUN", true);
            // CreateRoom();
            PhotonNetwork.JoinLobby(defaultLobby);

            connectionView.PlayerIdLabel.text = PhotonNetwork.LocalPlayer.UserId;
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
        
        
        public override void OnRegionListReceived(RegionHandler regionHandler)
        {
            base.OnRegionListReceived(regionHandler);
            Debug.Log($"Region list received: {regionHandler}");
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