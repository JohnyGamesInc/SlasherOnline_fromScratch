using System;
using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class PlayfabLogin : MonoBehaviour
    {
        
        private const string AuthGuidKey = "auth_guid_key";
        
        [SerializeField] private EnterGameUI enterUI;
        [SerializeField] private GameObject progressBar;
        [SerializeField] private GameObject errorLabel;
        
        private readonly string titleId = "D4516";
        private readonly string devCustomId = "DevPlayer";


        private void Awake()
        {
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
            
            progressBar.gameObject.SetActive(true);
            
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
            
            progressBar.gameObject.SetActive(false);
            ChangeSceneToCharacterSelection();
        }


        private void OnLoginFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Playfab Login FAILED: {errorMessage}");
            progressBar.gameObject.SetActive(false);
            errorLabel.gameObject.SetActive(true);
            errorLabel.GetComponent<Text>().text = errorMessage;
        }


        private IEnumerator WaitNSecs(int sec, Action actionProcess)
        {
            Debug.Log($"Wait for {sec} seconds");
            yield return new WaitForSeconds(sec);
            Debug.Log($"Finished waiting");
            actionProcess.Invoke();
        }


        private void ChangeSceneToCharacterSelection()
        {
            SceneManager.LoadSceneAsync(1);
        }


        private void OnDestroy()
        {
            // ui.SubmitFormCallback -= TryLogin;
        }
        
        
    }
}