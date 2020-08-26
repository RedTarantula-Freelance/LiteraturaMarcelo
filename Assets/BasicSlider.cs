using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicSlider : MonoBehaviour
{
    public string parameter;
     public AudioManager am;
    public Slider slider;



    void Start()
    {
        am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        slider = GetComponent<Slider>();
        slider.value = am.GetVolumeFloat(parameter);
    }


}
