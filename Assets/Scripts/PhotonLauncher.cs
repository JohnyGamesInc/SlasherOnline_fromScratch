using System;
using Photon.Pun;
using UnityEngine;


namespace SlasherOnline
{


    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {

        private readonly string gameVersion = "1";
        
        
        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        private void Start()
        {
            Connect();
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


        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster was called by PUN");
        }
        
        
        
    }
}