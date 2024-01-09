using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class CreateAccountDataWindow : AccountDataWindowBase
    {

        [SerializeField] private InputField mailField;

        [SerializeField] private Button createAccountButton;
        
        [SerializeField] protected Canvas createAccountCanvas;
        [SerializeField] private Canvas enterInGameCanvas;
        
        private string mail;
        

        private void Awake()
        {
            SubscribeElementsUI();
        }


        protected virtual void SubscribeElementsUI()
        {
            base.SubscribeElementsUI();
            
            mailField.onValueChanged.AddListener(inputMail => mail = inputMail);
            createAccountButton.onClick.AddListener(CreateAccount);
            
            backButton.onClick.AddListener(() =>
            {
                createAccountCanvas.enabled = false;
                enterInGameCanvas.enabled = true;
            });
        }


        private void CreateAccount()
        {
            PlayFabClientAPI.RegisterPlayFabUser(
                new RegisterPlayFabUserRequest 
                {
                    Username = username,
                    Email = mail,
                    Password = password
                },
                result =>
                {
                    Debug.Log("Success");
                    SwitchPhotonScene();
                },
                error =>
                {
                    Debug.LogError($"Fail: {error.ErrorMessage}");
                }
            );
            
        }


        private void OnDestroy()
        {
            mailField.onValueChanged.RemoveAllListeners();
            createAccountButton.onClick.RemoveAllListeners();
        }
        
        
    }
}