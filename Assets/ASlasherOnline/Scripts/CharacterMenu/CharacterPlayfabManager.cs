using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ASlasherOnline
{
    public class CharacterPlayfabManager : MonoBehaviour
    {
        [SerializeField] private CreateCharacterPromptView characterCreationPrompt;

        [SerializeField] private GameObject characterSelectPanel;
        [SerializeField] private GameObject characterSlotPrefab;

        [SerializeField] private GameObject characterSlotTemplate;
        [SerializeField] private Transform slotsParent;

        private Dictionary<string, CharacterSlotView> characterSlots = new();


        private void Awake()
        {
            characterSlotTemplate.SetActive(false);
            characterCreationPrompt.BackButton.onClick.AddListener(OnBackInPromptButtonPressed);
            characterCreationPrompt.CreateButton.onClick.AddListener(OnCreateCharacterInPromptButtonPressed);
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
                    OnGetCharactersSuccess(res.Characters);
                },
                error => OnRequestError(error));
        }


        private void OnEmptySlotButtonPressed(CharacterSlotView characterSlot)
        {
            Debug.Log("Character Select Pressed");

            characterSelectPanel.gameObject.SetActive(false);
            characterCreationPrompt.gameObject.SetActive(true);
            
        }


        private void OnCharacterSelectButtonPressed()
        {
            Debug.Log("Character Select Pressed");
            LoadLobbyScene();
        }


        private void LoadLobbyScene()
        {
            SceneManager.LoadSceneAsync(2);
        }


        private void OnGetCharactersSuccess(List<CharacterResult> characters)
        {
            foreach (var charMap in characterSlots)
            {
                Destroy(charMap.Value.gameObject);
            }
            
            characterSlots.Clear();
            
            foreach (var character in characters)
            {
                var charSlot = Instantiate(characterSlotPrefab, slotsParent)
                    .GetComponent<CharacterSlotView>();
                charSlot.EmptySlotButton.gameObject.SetActive(false);
                charSlot.CharacterSlotButton.gameObject.SetActive(true);
                
                charSlot.Name.text = character.CharacterName;
                GetCharacterStatistic(character, charSlot);
                characterSlots[character.CharacterName] = charSlot;
            }

            var emptySlot = Instantiate(characterSlotPrefab, slotsParent)
                .GetComponent<CharacterSlotView>();
            characterSlots["empty"] = emptySlot;
            MakeSlotSubscribes(emptySlot);
        }


        private void GetCharacterStatistic(CharacterResult character, CharacterSlotView charSlot)
        {
            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest()
            {
                CharacterId = character.CharacterId
            },
                result =>
                {
                    charSlot.Level.text = result.CharacterStatistics["Level"].ToString();
                    charSlot.Gold.text = result.CharacterStatistics["Gold"].ToString();
                    charSlot.XP.text = result.CharacterStatistics["XP"].ToString();
                },
                error => OnRequestError(error));
        }


        private void MakeSlotSubscribes(CharacterSlotView slotView)
        {
            slotView.EmptySlotButton.onClick.AddListener(() => OnEmptySlotButtonPressed(slotView));
            slotView.CharacterSlotButton.onClick.AddListener(() => OnCharacterSelectButtonPressed());
        }


        private void OnBackInPromptButtonPressed()
        {
            characterSelectPanel.gameObject.SetActive(true);
            characterCreationPrompt.gameObject.SetActive(false);
        }


        private void OnCreateCharacterInPromptButtonPressed()
        {
            Debug.Log("Create Character Button Pressed");
            if (string.IsNullOrEmpty(characterCreationPrompt.InputField.text)) return;

            var newCharName = characterCreationPrompt.InputField.text;
            Debug.Log($"New Character Name: [{newCharName}]");
            
            CreateCharacterWithItemId(newCharName, "character_token");
        }


        private void CreateCharacterWithItemId(string charName, string itemId)
        {
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
                {
                    CatalogVersion = "DevCatalog",
                    ItemId = "character_token",
                    Price = 0,
                    VirtualCurrency = "GG"
                }, 
                result => Debug.Log("Purchasing character_token Success"),
                error => OnRequestError(error));
            
            
            PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
                {
                    CharacterName = charName,
                    ItemId = itemId
                }, 
                result => { UpdateCharacterStatistics(result.CharacterId); },
                error => OnRequestError(error));
        }


        private void UpdateCharacterStatistics(string characterId)
        {
            PlayFabClientAPI.UpdateCharacterStatistics(
                new UpdateCharacterStatisticsRequest
                {
                    CharacterId = characterId,
                    CharacterStatistics = new Dictionary<string, int>
                    {
                        {"Level", 1},
                        {"XP", 0}, 
                        {"Gold", 0},
                        {"HP", 100},
                        {"Damage", 10}
                    }
                }, 
                result =>
                {
                    Debug.Log($"Initial stats set, telling client to update character list");
                    GetCharacters();
                    characterCreationPrompt.gameObject.SetActive(false);
                    characterSelectPanel.gameObject.SetActive(true);
                }, 
                error => OnRequestError(error));
        }


        private void OnRequestError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }


        private void OnDestroy()
        {
            foreach (var slotViewKV in characterSlots)
            {
                slotViewKV.Value.EmptySlotButton.onClick.RemoveAllListeners();
                slotViewKV.Value.CharacterSlotButton.onClick.RemoveAllListeners();
            }
        }
        
        //TODO Correct clearing prefabs of characters
    }
}