using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverHighlight;
    public bool isInteractable;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isInteractable) hoverHighlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverHighlight.SetActive(false);
    }

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        hoverHighlight.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
