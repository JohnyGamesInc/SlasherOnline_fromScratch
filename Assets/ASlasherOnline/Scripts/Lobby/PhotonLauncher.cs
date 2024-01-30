using System;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;
using UnityEngine.Serialization;


namespace SlasherOnline
{


    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {

        [FormerlySerializedAs("ui")]
        [SerializeField] private ConnectionUIView uiView;
        

        private readonly string gameVersion = "1";
        
        
        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            uiView.Init(Connect, Disconnect);
        }
        

        private void Connect()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.JoinRandomRoom();
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }


        private void Disconnect()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            
            Debug.Log($"Disconnected reason: {cause}");

            uiView.UpdateLabel(false);
        }


        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster was called by PUN");
            
            uiView.UpdateLabel(true);
        }
        

    }
}