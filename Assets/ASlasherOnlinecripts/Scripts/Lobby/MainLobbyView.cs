using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{

    public class MainLobbyView : MonoBehaviourPunCallbacks
    {

        [field: SerializeField] public TMP_Text LobbyName { get; private set; }
        [field: SerializeField] public GameObject RoomsListContent { get; private set; }
        [field: SerializeField] public LobbyEntryView RoomsEntryPrefab { get; private set; }

        [field: SerializeField] public Button ApplyFiltersButton { get; private set; }

        [field: SerializeField] public Button QuickJoinButton { get; private set; }
        [field: SerializeField] public TMP_InputField RoomCodeInput { get; private set; }
        [field: SerializeField] public TMP_InputField RoomPassInput { get; private set; }
        [field: SerializeField] public Button JoinRoomButton { get; private set; }

        [SerializeField] private GameObject entryLobbyTemplate;

        [SerializeField] private CanvasGroup raycastBlocker;
        [SerializeField] private CanvasGroup spinnerUI;

        [SerializeField] private ConnectionUIView connectionView;

        private Dictionary<string, LobbyEntryView> roomsEntries = new();


        private void Awake()
        {
            entryLobbyTemplate.SetActive(false);
            QuickJoinButton.onClick.AddListener(QuickJoinButtonSubscribe);
            JoinRoomButton.onClick.AddListener(() =>
            {
                OnJoinRoomButtonSubscribe(RoomCodeInput.text);
            });
        }


        private void Update()
        {
            ToggleHiding(!PhotonNetwork.InLobby);
            
            if(PhotonNetwork.CurrentLobby != null) 
                LobbyName.text = $"{PhotonNetwork.CurrentLobby.Name} - {PhotonNetwork.CurrentLobby.Type}";
        }
        
        
        private void ToggleHiding(bool isVisible)
        {
            if (isVisible)
            {
                raycastBlocker.alpha = 1;
                raycastBlocker.interactable = true;
                raycastBlocker.blocksRaycasts = true;
                
                spinnerUI.alpha = 0;
                spinnerUI.interactable = false;
                spinnerUI.blocksRaycasts = false;
            }
            else
            {
                raycastBlocker.alpha = 0;
                raycastBlocker.interactable = false;
                raycastBlocker.blocksRaycasts = false;
                
                
                spinnerUI.alpha = 1;
                spinnerUI.interactable = true;
                spinnerUI.blocksRaycasts = true;
            }
        }
        
        
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("Joined To LOBBY");

            // var sqlLobbyFilter = $"{MAP_PROP_KEY} = Map3 AND {LVL_PROP_KEY} BETWEEN 2 AND 4";
            // var joinRandomRoomParams = new OpJoinRandomRoomParams
            // {
            //     SqlLobbyFilter = sqlLobbyFilter
            // };
            // PhotonNetwork.JoinRandomRoom(joinRandomRoomParams);
        }

        
        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("Player left lobby");
        }
        
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            Debug.Log($"Rooms List is Updated [{roomList.Count}]");
            UpdateCachedRoomList(roomList);
        }
        
        
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo info = roomList[i];
                if (info.RemovedFromList)
                {
                    UpdateRoomsList(info, true, OnJoinRoomButtonSubscribe);
                }
                else
                {
                    UpdateRoomsList(info, false, OnJoinRoomButtonSubscribe);
                }
            }
        }
        
        
        private void UpdateRoomsList(RoomInfo roomInfo, bool isRemoved, Action<string> JoinRoomCallback)
        {
            if (!isRemoved)
            {
                if (!roomsEntries.ContainsKey(roomInfo.Name))
                {
                    LobbyEntryView entryView = Instantiate(RoomsEntryPrefab, RoomsListContent.transform).GetComponent<LobbyEntryView>();
                    entryView.RoomNameLabel.text = roomInfo.Name;
                    entryView.PlayerCountLabel.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
                    entryView.RoomEntryButton.onClick.AddListener(() => JoinRoomCallback(roomInfo.Name));
                    roomsEntries[roomInfo.Name] = entryView;
                }
            }

            if (isRemoved)
            {
                if (roomsEntries.ContainsKey(roomInfo.Name))
                {
                    var entryView = roomsEntries[roomInfo.Name];
                    entryView.RoomEntryButton.onClick.RemoveAllListeners();
                    Destroy(entryView.gameObject);
                    roomsEntries.Remove(roomInfo.Name);
                }
            }
        }
        
        
        private void OnJoinRoomButtonSubscribe(string roomName)
        {
            Debug.Log("Lobby Entry Pressed");
            PhotonNetwork.JoinRoom(roomName);
        }


        private void QuickJoinButtonSubscribe()
        {
            Debug.Log("QuickJoin Button Pressed");
        }


        private void OnDestroy()
        {
            ApplyFiltersButton.onClick.RemoveAllListeners();
            QuickJoinButton.onClick.RemoveAllListeners();
            JoinRoomButton.onClick.RemoveAllListeners();
        }
        
        
    }
}