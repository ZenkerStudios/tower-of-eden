using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI initialDmgText;
    public TextMeshProUGUI finalDmgText;
    private float disappearTime;
    private const float DISAPPEAR_TIME_MAX = 1f;
    private Vector3 moveVector;
    private Color textColor;


    private void Awake()
    {
    }

    public void SetUp(int initialAmount, int amount, bool isHeal, bool isCrit)
    {
        initialDmgText.text = "";
        if(initialAmount < amount)
        {
            initialDmgText.text = Mathf.Abs(initialAmount) + "";

        }

        disappearTime = DISAPPEAR_TIME_MAX;
        moveVector = new Vector3(Random.Range(-1f, 1f), Random.Range(2.5f, 5f)) * 3f;
        finalDmgText.text = Mathf.Abs(amount) + "";
        finalDmgText.fontSize = 45;
        initialDmgText.fontSize = 40;

        if (isHeal)
        {
            if (isCrit)
            {
                finalDmgText.fontSize = 55;

            }
            finalDmgText.color = new Color32(0, 255, 0, 255);
        }
        else
        {
            if (isCrit)
            {
                finalDmgText.color = new Color32(255, 0, 0, 255);
                finalDmgText.fontSize = 55;

            }
            else
            {
                finalDmgText.color = new Color32(255, 255, 255, 255);
            }

        }
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if(disappearTime > DISAPPEAR_TIME_MAX * 0.5f)
        {
            //First half of anim
            transform.localScale += Vector3.one * 1f * Time.deltaTime;
        } else {
            //Second half of anim
            transform.localScale -= Vector3.one * 1f * Time.deltaTime;
        }
        disappearTime -= Time.deltaTime;
        if(disappearTime < 0)
        {
            float disappearSpeed = 5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            finalDmgText.color = textColor;
            if(textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
