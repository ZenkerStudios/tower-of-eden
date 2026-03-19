using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoHitPopup : MonoBehaviour
{
    public TextMeshProUGUI missText;
    private float disappearTime;
    private const float DISAPPEAR_TIME_MAX = 1f;
    private Vector3 moveVector;
    private Color textColor;


    private void Awake()
    {
    }

    public void SetUp(string message, int font)
    {
        disappearTime = DISAPPEAR_TIME_MAX;
        moveVector = new Vector3(0, Random.Range(0, 1f)) * 3f;
        missText.text = message;
        missText.color = new Color32(255, 255, 255, 255);
        missText.fontSize = font;
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
            missText.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
