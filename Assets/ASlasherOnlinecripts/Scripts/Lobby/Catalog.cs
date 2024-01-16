using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


namespace SlasherOnline
{
    
    public class Catalog : MonoBehaviour
    {

        private readonly Dictionary<string, CatalogItem> devCatalog = new();


        private void Start()
        {
            PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnFailure);
        }

        
        private void OnGetCatalogSuccess(GetCatalogItemsResult result)
        {
            HandleCatalog(result.Catalog);
            Debug.Log("Catalog was loaded successfully");
        }


        private void HandleCatalog(List<CatalogItem> catalog)
        {
            foreach (var item in catalog)
            {
                devCatalog.Add(item.ItemId, item);
                Debug.Log($"Catalog Item Loaded: [{item.ItemId}]");
            }
        }
        
        
        private void OnFailure(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Something went wrong: {errorMessage}");
        }
        
        
        
    }
}