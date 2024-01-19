using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{

    public class RoomUserView : MonoBehaviour
    {
        
        [field: SerializeField] public TMP_Text PlayerName { get; private set; }
        [field: SerializeField] public TMP_Text Status { get; private set; }
        [field: SerializeField] public Slider VolumeSlider { get; private set; }
        [field: SerializeField] public Toggle MuteToggle { get; private set; }
        
    }
}