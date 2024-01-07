using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;


namespace SlasherOnline
{
    
    public class PlayfabLogin : MonoBehaviour
    {

        private readonly string titleId = "D4516";
        private readonly string devCustomId = "DevPlayer";
        
        
        private void Start()
        {

            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                PlayFabSettings.staticSettings.TitleId = titleId;

            var request = new LoginWithCustomIDRequest
            {
                CustomId = devCustomId,
                CreateAccount = true
            };
            
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }


        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Playfab Login SUCCESS");
        }


        private void OnLoginFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Playfab Login FAILED: {errorMessage}");
        }
        
        
    }
}