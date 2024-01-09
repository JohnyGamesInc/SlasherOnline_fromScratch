using System;
using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;


namespace SlasherOnline
{
    
    public class PlayfabLogin : MonoBehaviour
    {
        
        private const string AuthGuidKey = "auth_guid_key";
        
        // [SerializeField] private PlayfabLoginUI ui;
        [SerializeField] private EnterGameUI enterUI;
        
        private readonly string titleId = "D4516";
        private readonly string devCustomId = "DevPlayer";


        private void Awake()
        {
            // ui.SubmitFormCallback += TryLogin;
            enterUI.SubscribeOnWithoutAccount(TryLogin);
        }


        private void Start()
        {
            
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                PlayFabSettings.staticSettings.TitleId = titleId;
        }


        private void TryLogin()
        {
            
            var needCreation = PlayerPrefs.HasKey(AuthGuidKey);
            var id = PlayerPrefs.GetString(AuthGuidKey, Guid.NewGuid().ToString());

            var request = new LoginWithCustomIDRequest
            {
                CustomId = id,
                CreateAccount = !needCreation
            };
            
            PlayFabClientAPI.LoginWithCustomID(request,
                result =>
                {
                  PlayerPrefs.SetString(AuthGuidKey, id);
                  OnLoginSuccess(result);
                },
                OnLoginFailure);
        }


        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Playfab Login SUCCESS");
            
            // ui.UpdateLabel(true);
            
            // StartCoroutine(WaitNSecs(2, ChangeSceneToPhoton));
            ChangeSceneToPhoton();
        }


        private void OnLoginFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Playfab Login FAILED: {errorMessage}");
            // ui.UpdateLabel(false);
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
            // ui.SubmitFormCallback -= TryLogin;
        }
        
        
    }
}