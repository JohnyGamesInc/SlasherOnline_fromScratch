using System;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class PhotonLauncherUI : MonoBehaviour
    {

        [field:SerializeField] public Button ConnectButton { get; private set; }
        
        [field:SerializeField] public Button DisconnectButton { get; private set; }

        [field: SerializeField] public TMP_Text StatusLabel { get; private set; }
        
        
        
        private void Awake()
        {
            
        }


        public void Init(Action OnConnectButtonPressed, Action OnDisconnectButtonPressed)
        {
            ConnectButton.onClick.AddListener(() => OnConnectButtonPressed());
            DisconnectButton.onClick.AddListener(() => OnDisconnectButtonPressed());
        }
        
        
        public void UpdateLabel(bool success)
        {
            if (success)
            {
                StatusLabel.text = "Client Connected to Master Server";
                StatusLabel.color = Color.cyan;
            }
            else
            {
                StatusLabel.text = "Client Disconnected from Master Server";
                StatusLabel.color = Color.red;
            }
        }
        
        
        private void OnDestroy()
        {
            ConnectButton.onClick.RemoveAllListeners();
            DisconnectButton.onClick.RemoveAllListeners();
        }

        
        
    }
}