using System;
using System.Collections;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class PlayfabAccountManager : MonoBehaviour
    {

        [SerializeField] private TMP_Text titleLabel;
        
        [field: SerializeField] public Button RemoveAccountButton { get; private set; }

        private string playfabId;


        private void Awake()
        {
            RemoveAccountButton.onClick.AddListener(RemoveAccount);
        }


        private void Start()
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
                account =>
                {
                    playfabId = account.AccountInfo.PlayFabId;
                    titleLabel.text = $"PLayfab Id: {account.AccountInfo.PlayFabId}";
                    titleLabel.text += $"\nNickname: {account.AccountInfo.Username}";
                    titleLabel.text += $"\nCreated: {account.AccountInfo.Created}";
                },
                error =>
                {
                    Debug.LogError($"Error: {error.GenerateErrorReport()}");
                });
        }
        
        
        private void RemoveAccount()
        {
            StartCoroutine(RemoveAccountPostRequest());
        }

        
        IEnumerator RemoveAccountPostRequest()
        {
            UnityWebRequest request = new UnityWebRequest("https://titleId.playfabapi.com/Admin/DeletePlayer", "POST");
            
            byte[] bodyRaw = Encoding.UTF8.GetBytes($"{{ \"PlayFabId\": \"{playfabId}\" }}");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            
            request.SetRequestHeader("X-SecretKey", "U4PRIYCQJIGKHAPZ4MFYQPN4PHZTFZMAW49QBZMSU19RS11YHI");
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                SceneManager.LoadSceneAsync(0);
            }
            
            
            // using (UnityWebRequest www = UnityWebRequest.Post("https://titleId.playfabapi.com/Admin/DeletePlayer", $"{{ \"PlayFabId\": \"{playfabId}\" }}", "application/json"))
            // {
            //     www.SetRequestHeader("X-SecretKey", "U4PRIYCQJIGKHAPZ4MFYQPN4PHZTFZMAW49QBZMSU19RS11YHI");
            //     www.SetRequestHeader("Content-Type", "application/json");
            //     
            //     yield return www.SendWebRequest();
            //
            //     if (www.result != UnityWebRequest.Result.Success)
            //     {
            //         Debug.Log(www.error);
            //     }
            //     else
            //     {
            //         Debug.Log("Form upload complete!");
            //         SceneManager.LoadSceneAsync(0);
            //     }
            // }
        }
        
        
        private void OnDestroy()
        {
            RemoveAccountButton.onClick.RemoveAllListeners();
        }
        
        
    }
}