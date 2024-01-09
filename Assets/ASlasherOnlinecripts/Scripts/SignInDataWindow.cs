using System;
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
            });
        }


        private void SignIn()
        {
            PlayFabClientAPI.LoginWithPlayFab(
                new LoginWithPlayFabRequest
                {
                    Username = username,
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
            signInButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }
        
        
    }
}