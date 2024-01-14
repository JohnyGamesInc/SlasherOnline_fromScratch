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
                errorLabel.gameObject.SetActive(false);
            });
        }


        private void CreateAccount()
        {
            progressBar.gameObject.SetActive(true);
            
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
                    progressBar.gameObject.SetActive(false);
                    SwitchPhotonScene();
                },
                error =>
                {
                    progressBar.gameObject.SetActive(false);
                    Debug.LogError($"Fail: {error.ErrorMessage}");
                    errorLabel.gameObject.SetActive(true);
                    errorLabel.GetComponent<Text>().text = error.ErrorMessage;
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