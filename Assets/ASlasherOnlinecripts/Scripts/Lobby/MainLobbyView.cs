using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{

    public class MainLobbyView : MonoBehaviour
    {

        [field: SerializeField] public TMP_Text LobbyName { get; private set; }
        [field: SerializeField] public GameObject RoomsListContent { get; private set; }
        [field: SerializeField] public GameObject RoomsEntryPrefab { get; private set; }

        [field: SerializeField] public Button ApplyFiltersButton { get; private set; }

        [field: SerializeField] public Button QuickJoinButton { get; private set; }
        [field: SerializeField] public TMP_InputField RoomCodeInput { get; private set; }
        [field: SerializeField] public TMP_InputField RoomPassInput { get; private set; }
        [field: SerializeField] public Button JoinRoomButton { get; private set; }


    }
}