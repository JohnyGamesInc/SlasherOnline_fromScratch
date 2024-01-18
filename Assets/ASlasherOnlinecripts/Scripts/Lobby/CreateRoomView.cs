using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class CreateRoomView : MonoBehaviour
    {
        
        [field: SerializeField] public Button CreateRoomButton { get; private set; }

        [field: SerializeField] public TMP_InputField RoomName { get; private set; }
        [field: SerializeField] public TMP_InputField RoomPassword { get; private set; }

        [field: SerializeField] public Toggle PrivateToggler { get; private set; }



    }
}