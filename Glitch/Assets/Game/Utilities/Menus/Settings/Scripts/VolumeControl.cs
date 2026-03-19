using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class VolumeControl : MonoBehaviour
{

    [SerializeField]
    protected AudioMixer audioMixer;

    [SerializeField]
    protected string _volumeParameter = "MasterVolume";

    [SerializeField]
    protected Slider slider;

    [SerializeField]
    protected Toggle toggle;


    public static float _multiplier = 30f;

    protected bool _disableToggleEvent;


    private void Awake()
    {
        slider.onValueChanged.AddListener(SliderValueChanged);
        toggle.onValueChanged.AddListener(ToggleValueChanged);
    }


   

    // Start is called before the first frame update
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(_volumeParameter, slider.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ToggleValueChanged(bool enableSound)
    {
        if (_disableToggleEvent) return;

        if(enableSound)
        {
            slider.value = slider.maxValue * .7f;
        }
        else
        {
            slider.value = slider.minValue;  
        }
    }
    
    private void SliderValueChanged(float volume)
    {
        audioMixer.SetFloat(_volumeParameter, Mathf.Log10(volume) * _multiplier);
        _disableToggleEvent = true;
        toggle.isOn = slider.value > slider.minValue;
        _disableToggleEvent = false;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(_volumeParameter, slider.value);

    }
}
