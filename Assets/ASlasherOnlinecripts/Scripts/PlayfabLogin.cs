using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;


namespace SlasherOnline
{
    
    public class PlayfabLogin : MonoBehaviour
    {
        
        [SerializeField] private PlayfabLoginUI ui;

        private readonly string titleId = "D4516";
        private readonly string devCustomId = "DevPlayer";
        

        private void Start()
        {

            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                PlayFabSettings.staticSettings.TitleId = titleId;

            ui.SubmitFormCallback += TryLogin;
        }


        private void TryLogin(string loginId)
        {
            if (string.IsNullOrEmpty(loginId))
                loginId = devCustomId;
            
            var request = new LoginWithCustomIDRequest
            {
                CustomId = loginId,
                CreateAccount = true
            };
            
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }


        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Playfab Login SUCCESS");
            ui.UpdateLabel(true);
            
            StartCoroutine(WaitNSecs(2, ChangeSceneToPhoton));
        }


        private void OnLoginFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Playfab Login FAILED: {errorMessage}");
            ui.UpdateLabel(false);
        }


        private IEnumerator WaitNSecs(int sec, Action actionProcess)
        {
            Debug.Log($"Wait for {sec} seconds");
            yield return new WaitForSeconds(sec);
            Debug.Log($"Finished waiting");
            actionProcess.Invoke();
        }


        private void ChangeSceneToPhoton()
        {
            SceneManager.LoadSceneAsync(1);
        }


        private void OnDestroy()
        {
            ui.SubmitFormCallback -= TryLogin;
        }
        
        
    }
}