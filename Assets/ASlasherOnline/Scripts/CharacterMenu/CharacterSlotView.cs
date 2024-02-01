using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ASlasherOnline
{
    
    public class CharacterSlotView : MonoBehaviour
    {
        
        public Button EmptySlotButton;
        public Button CharacterSlotButton;
        
        public TMP_Text Name;
        public TMP_Text Level;
        public TMP_Text XP;
        public TMP_Text Gold;

        

        private void OnDestroy()
        {
            
        }
        
        
    }
}