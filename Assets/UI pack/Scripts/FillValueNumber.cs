using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillValueNumber : MonoBehaviour
{
    public Image TargetImage;
    // Update is called once per frame
    void Update()
    {
        // float amount = TargetImage.fillAmount + Time.deltaTime;
        TargetImage.fillAmount += Time.deltaTime;
        if (TargetImage.fillAmount > 0.99f) TargetImage.fillAmount = 0.0f;
        // gameObject.GetComponent<Text>().text = amount.ToString("F0");
    }
}
