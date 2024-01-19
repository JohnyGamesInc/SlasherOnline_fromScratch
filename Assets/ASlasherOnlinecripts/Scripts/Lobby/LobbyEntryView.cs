using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class LobbyEntryView : MonoBehaviour
    {

        [field: SerializeField] public TMP_Text RoomNameLabel { get; private set; }
        [field: SerializeField] public TMP_Text PlayerCountLabel { get; private set; }

        [field: SerializeField] public Button RoomEntryButton { get; private set; }


        private void OnDestroy()
        {
            RoomEntryButton.onClick.RemoveAllListeners();
        }
        
        
    }
}