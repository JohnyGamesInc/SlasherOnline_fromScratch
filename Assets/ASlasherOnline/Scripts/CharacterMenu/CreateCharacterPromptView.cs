using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ASlasherOnline
{
    
    public class CreateCharacterPromptView : MonoBehaviour
    {

        public Button BackButton;
        public Button CreateButton;

        public InputField InputField;

        
        

        private void OnDestroy()
        {
            BackButton.onClick.RemoveAllListeners();
            CreateButton.onClick.RemoveAllListeners();
        }
        
        
    }
}