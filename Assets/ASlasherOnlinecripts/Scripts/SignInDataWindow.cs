using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class SignInDataWindow : AccountDataWindowBase
    {

        [SerializeField] private Button signInButton;

        [SerializeField] protected Canvas enterInGameCanvas;
        [SerializeField] protected Canvas signInCanvas;

        
        protected override void SubscribeElementsUI()
        {
            base.SubscribeElementsUI();
            
            signInButton.onClick.AddListener(SignIn);
            
            backButton.onClick.AddListener(() =>
            {
                signInCanvas.enabled = false;
                enterInGameCanvas.enabled = true;
                errorLabel.gameObject.SetActive(false);
            });
        }


        private void SignIn()
        {
            progressBar.gameObject.SetActive(true);
            
            PlayFabClientAPI.LoginWithPlayFab(
                new LoginWithPlayFabRequest
                {
                    Username = username,
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
            signInButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }
        
        
    }
}