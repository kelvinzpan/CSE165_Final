using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SliderTextValue : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float sliderModifier = this.GetComponentInParent<Slider>().value;
        this.GetComponent<Text>().text = (sliderModifier * 100.0f).ToString();
    }
}
