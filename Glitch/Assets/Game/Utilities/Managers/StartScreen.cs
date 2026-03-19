using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartScreen : MonoBehaviour, IPointerClickHandler
{
    private GameController gc;
    

    private void Awake()
    {

    }

    public void OpenSetting()
    {
        gc.OpenSetting();
    }
        
    public void Quit()
    {
        gc.QuitGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        gc = GameController.instance;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            ButtonPress();
        }
    }

    IEnumerator StartAnim()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(this.GetComponent<Animator>());
    }

    bool buttonPressed = false;
    private void ButtonPress()
    {
        if (buttonPressed) return;
        AudioManager.instance.PlaySfxSound("Button_Click");
        GetComponent<Animator>().Play("Start Screen Transition");
        buttonPressed = true;
        StartCoroutine(StartAnim());
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ButtonPress();
    }
}
