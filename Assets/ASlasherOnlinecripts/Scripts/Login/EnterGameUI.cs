using System;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class EnterGameUI : MonoBehaviour
    {

        [SerializeField] private Button signInButton;
        [SerializeField] private Button createAccountButton;
        [SerializeField] private Button withoutAccountButton;

        [SerializeField] private Canvas enterInGameCanvas;
        [SerializeField] private Canvas createAccountCanvas;
        [SerializeField] private Canvas signInCanvas;


        private void Awake()
        {
            signInButton.onClick.AddListener(() =>
            {
                signInCanvas.enabled = true;
                enterInGameCanvas.enabled = false;
            });
            
            createAccountButton.onClick.AddListener(() =>
            {
                createAccountCanvas.enabled = true;
                enterInGameCanvas.enabled = false;
            });
        }


        public void SubscribeOnWithoutAccount(Action withoutAccountAction)
        {
            withoutAccountButton.onClick.AddListener(() => withoutAccountAction());
        }
        

        private void OnDestroy()
        {
            signInButton.onClick.RemoveAllListeners();
            createAccountButton.onClick.RemoveAllListeners();
            withoutAccountButton.onClick.RemoveAllListeners();
        }
        
        
    }
}