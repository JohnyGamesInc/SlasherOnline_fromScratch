using System;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;


namespace SlasherOnline
{
    
    public class PlayfabAccountManager : MonoBehaviour
    {

        [SerializeField] private TMP_Text titleLabel;


        private void Start()
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
                account =>
                {
                    titleLabel.text = $"PLayfab Id: {account.AccountInfo.PlayFabId}";
                },
                error =>
                {
                    Debug.LogError($"Error: {error.GenerateErrorReport()}");
                });
        }
        
        
    }
}