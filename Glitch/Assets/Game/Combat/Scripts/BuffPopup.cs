using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffPopup : MonoBehaviour
{
    public TextMeshProUGUI buffText;
    private float disappearTime;
    private const float DISAPPEAR_TIME_MAX = 1f;
    private Vector3 moveVector;
    private Color textColor;


    private void Awake()
    {
    }

    public void SetUp(bool isBuff, int s)
    {
        disappearTime = DISAPPEAR_TIME_MAX;
        buffText.color = new Color32(255, 255, 255, 255);
        moveVector = new Vector3(0, 5f) * 3f;
        buffText.fontSize = 50;
        if (isBuff || s==28)
        {
            buffText.text = "+" + "<sprite=" + s + ">";
        } else
        {
            buffText.text = "-" + "<sprite=" + s + ">";
        }

        
        
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTime > DISAPPEAR_TIME_MAX * 0.5f)
        {
            //First half of anim
            transform.localScale += Vector3.one * 1f * Time.deltaTime;
        }
        else
        {
            //Second half of anim
            transform.localScale -= Vector3.one * 1f * Time.deltaTime;
        }
        disappearTime -= Time.deltaTime;
        if (disappearTime < 0)
        {
            float disappearSpeed = 5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            buffText.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
