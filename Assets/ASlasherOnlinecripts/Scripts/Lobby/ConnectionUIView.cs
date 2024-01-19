using System;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class ConnectionUIView : MonoBehaviour
    {

        [field:SerializeField] public Button ConnectButton { get; private set; }
        
        [field:SerializeField] public Button DisconnectButton { get; private set; }

        [field: SerializeField] public TMP_Text StatusLabel { get; private set; }

        [field: SerializeField] public TMP_Text PlayerIdLabel { get; private set; }



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
        
        
        public void UpdateLabel(string text, bool success)
        {
            StatusLabel.text = text;
                
            if (success) 
                StatusLabel.color = Color.cyan;
            else
                StatusLabel.color = Color.red;
        }
        
        
        private void OnDestroy()
        {
            ConnectButton.onClick.RemoveAllListeners();
            DisconnectButton.onClick.RemoveAllListeners();
        }

        
        
    }
}