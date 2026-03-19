using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    private bool gamePaused = false;
    public AudioSource pauseMusic;
    public AudioSource pauseAmbience;
    public GameObject escapeMenu;
    public GameController gameController;
    public static PauseController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        pauseMusic.ignoreListenerPause = true;
        pauseAmbience.ignoreListenerPause = true;
        escapeMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        //Scene that can't be paused
        if (gameController.currentScene == Floors.Start
            || gameController.currentScene == Floors.End)
        {
            return;
        }

        //Show menu
        ResetMenuButtons();
        if (!gamePaused)
        {
            Time.timeScale = 0;
            gamePaused = true;
        }
        else
        {
            gamePaused = false;
            Time.timeScale = 1;
        }
        AudioListener.pause = gamePaused;
        escapeMenu.SetActive(gamePaused);
        escapeMenu.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
        escapeMenu.transform.GetChild(1).GetChild(3).gameObject.SetActive(true);
        //Scenes that need to show/hide aditional menu options when paused
        if (gameController.currentScene == Floors.Floor_01
            || gameController.currentScene == Floors.Floor_02
            || gameController.currentScene == Floors.Floor_03
            || gameController.currentScene == Floors.Floor_04
            || gameController.currentScene == Floors.Floor_05)
        {
            escapeMenu.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            escapeMenu.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            escapeMenu.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
            escapeMenu.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
        }

        if(CutsceneManager.instance.inCutscene || DialogueManager.instance.inConversation)
        {
            escapeMenu.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
            escapeMenu.transform.GetChild(1).GetChild(3).gameObject.SetActive(false);

        }

        Destroy(GameObject.FindGameObjectWithTag("Settings"));
    }



    public void ResetMenuButtons()
    {
        escapeMenu.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(false);
        escapeMenu.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(true);
        escapeMenu.transform.GetChild(1).GetChild(1).GetChild(0).gameObject.SetActive(false);
        escapeMenu.transform.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(true);
        escapeMenu.transform.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(false);
        escapeMenu.transform.GetChild(1).GetChild(2).GetChild(1).gameObject.SetActive(true);
        escapeMenu.transform.GetChild(1).GetChild(3).GetChild(0).gameObject.SetActive(false);
        escapeMenu.transform.GetChild(1).GetChild(3).GetChild(1).gameObject.SetActive(true);
        escapeMenu.transform.GetChild(1).GetChild(4).GetChild(0).gameObject.SetActive(false);
        escapeMenu.transform.GetChild(1).GetChild(4).GetChild(1).gameObject.SetActive(true);
    }

    public bool isGamePaused()
    {
        return gamePaused;
    }    
}
