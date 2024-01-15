using System;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;


namespace SlasherOnline
{


    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {

        [SerializeField] private PhotonLauncherUI ui;
        

        private readonly string gameVersion = "1";
        
        
        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            ui.Init(Connect, Disconnect);
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

            ui.UpdateLabel(false);
        }


        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster was called by PUN");
            
            ui.UpdateLabel(true);
        }
        

    }
}