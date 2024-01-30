using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ASlasherOnline
{
    
    public class CharacterPlayfabManager : MonoBehaviour
    {

        [SerializeField] private GameObject characterSelectPanel;
        [SerializeField] private GameObject characterSlotPrefab;

        [SerializeField] private GameObject characterSlotTemplate;

        private Dictionary<string, CharacterSlotView> characters = new();


        private void Awake()
        {
            characterSlotTemplate.SetActive(false);
        }


        private void Start()
        {
            GetCharacters();
        }


        private void GetCharacters()
        {
            PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
                res =>
                {
                    Debug.Log($"Characters owned: + {res.Characters.Count}");
                }, 
                error => Debug.LogError(error.GenerateErrorReport()));
        }


        private void OnCreateCharacterButtonPressed(CharacterSlotView characterSlot)
        {
            characterSlot.EmptySlotButton.gameObject.SetActive(false);
            characterSlot.CharacterSlotButton.gameObject.SetActive(true);
        }


        private void OnCharacterSelectButtonPressed()
        {
            LoadLobbyScene();
        }


        private void LoadLobbyScene()
        {
            SceneManager.LoadSceneAsync(2);
        }
        
        
        
    }
}