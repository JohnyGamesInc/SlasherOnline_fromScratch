using UnityEngine;
using UnityEngine.UI;


namespace SlasherOnline
{

    public class CircleProgressBar : MonoBehaviour
    {
        
        [SerializeField] private Image TargetImage;

        
        private void Update()
        {
            TargetImage.fillAmount += Time.deltaTime;
            if (TargetImage.fillAmount > 0.99f) TargetImage.fillAmount = 0.0f;
        }
        
        
    }
}