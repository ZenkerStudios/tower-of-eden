using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public Button returnButton;
    public Sprite On;
    public Sprite Off;
    public Image fullscreenToggle;

    private void Start()
    {

        List<string> options = new List<string>();

        if (Screen.fullScreen)
        {
            fullscreenToggle.transform.parent.GetComponent<Toggle>().isOn = true;
            fullscreenToggle.sprite = On;
        }
        else
        {
            fullscreenToggle.transform.parent.GetComponent<Toggle>().isOn = false;
            fullscreenToggle.sprite = Off;
        }

    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            fullscreenToggle.sprite = On;
            PlayerPrefs.SetInt("fullscreen", 1);
        }
        else
        {
            fullscreenToggle.sprite = Off;
            PlayerPrefs.SetInt("fullscreen", 0);
        }

        Screen.SetResolution(
           PlayerPrefs.GetInt("ResWidth", 1920),
           PlayerPrefs.GetInt("ResHeight", 1080), PlayerPrefs.GetInt("fullscreen", 1) == 1);
    }

    public void ReturnToMainMenu()
    {
        GameUtilities.CloseMessageWindow();

        Destroy(this.gameObject);
    }

}
