using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private const string daily_reward_time = "daily_reward_time";


        private void Awake()
        {
            // RemoveAccountButton.onClick.AddListener(RemoveAccount);
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
            
            SetUserData();
            GetInventory();
            // MakePurchase();
        }


        private void SetUserData()
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {daily_reward_time, DateTime.UtcNow.ToString()}
                }
            },
                result =>
                {
                    Debug.Log("Successfully updated User Data");
                    GetUserData(playfabId);
                },
                error => OnRequestError(error.GenerateErrorReport())
            );
        }


        private void GetUserData(string playfabId)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
                {
                    PlayFabId = playfabId,
                    Keys = null
                },
                result =>
                {
                    Debug.Log("Got User Data");
                    if (result.Data == null || !result.Data.ContainsKey(daily_reward_time))
                        Debug.Log($"No key in data {daily_reward_time}");
                    else
                        Debug.Log($"Last Daily Reward Time [{result.Data[daily_reward_time].Value}]");
                },
                error => OnRequestError(error.GenerateErrorReport()));
        }


        private void MakePurchase()
        {
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
            {
                CatalogVersion = "DevCatalog",
                ItemId = "health_potion_small",
                Price = 100,
                VirtualCurrency = "GG"
            }, 
                result => Debug.Log("Purchasing Success"),
                error => OnRequestError(error.GenerateErrorReport()));
        }


        private void GetInventory()
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), 
                result => ShowInventory(result.Inventory),
                error => OnRequestError(error.GenerateErrorReport()));
        }


        private void ShowInventory(List<ItemInstance> inventory)
        {
            var firstItem = inventory.First();
            ConsumeItem(firstItem.ItemInstanceId);
        }


        private void ConsumeItem(string itemInstanceId)
        {
            PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
            {
               ConsumeCount = 1,
               ItemInstanceId = itemInstanceId
            }, 
                result => Debug.Log($"Consume item Success: {itemInstanceId}"),
                error => OnRequestError(error.GenerateErrorReport()));
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


        private void OnRequestError(string errorMessage)
        {
            Debug.LogError(errorMessage);
        }
        
        
        private void OnDestroy()
        {
            // RemoveAccountButton.onClick.RemoveAllListeners();
        }
        
        
    }
}