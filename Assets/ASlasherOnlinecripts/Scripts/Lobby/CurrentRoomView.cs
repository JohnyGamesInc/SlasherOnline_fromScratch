using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class CurrentRoomView : MonoBehaviour
    {

        [field: SerializeField] public TMP_Text RoomName { get; private set; }
        [field: SerializeField] public TMP_InputField LobbyCode { get; private set; }
        [field: SerializeField] public GameObject PlayersListContent { get; private set; }
        [field: SerializeField] public GameObject PlayerEntryPrefab { get; private set; }
        
        [field: SerializeField] public Button ReadyButton { get; private set; }
        [field: SerializeField] public Button NotReadyButton { get; private set; }
        [field: SerializeField] public Button LeaveRoomButton { get; private set; }

    }
}