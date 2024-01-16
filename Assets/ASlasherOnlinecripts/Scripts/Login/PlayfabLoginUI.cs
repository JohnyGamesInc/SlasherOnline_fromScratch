using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class PlayfabLoginUI : MonoBehaviour
    {

        [field:SerializeField] public Button LogInButton { get; private set; }
        [field: SerializeField] public TMP_Text StatusLabel { get; private set; }
        [field: SerializeField] public TMP_InputField InputField { get; private set; }

        [SerializeField] private Color successLabelColor;
        [SerializeField] private Color failureLabelColor;

        
        public event Action<string> SubmitFormCallback = delegate(string s) {  };

        
        private void Awake()
        {
            LogInButton.onClick.AddListener(OnLoginClick);
            InputField.onSubmit.AddListener(OnSubmitPressed);
        }

        
        private void OnLoginClick()
        {
            var loginId = InputField.text;
            SubmitFormCallback(loginId);
        }


        private void OnSubmitPressed(string loginId)
        {
            if (InputField.wasCanceled) return;
            
            SubmitFormCallback(loginId);
        }
        
        
        public void UpdateLabel(bool success)
        {
            if (success)
            {
                StatusLabel.text = "Login Success";
                StatusLabel.color = Color.cyan;
            }
            else
            {
                StatusLabel.text = "Login FAILED";
                StatusLabel.color = Color.red;
            }
        }
        
        
        private void OnDestroy()
        {
            LogInButton.onClick.RemoveAllListeners();
            InputField.onSubmit.RemoveAllListeners();
        }

        
        
    }
}