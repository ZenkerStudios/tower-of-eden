using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float speed;
    private float width;
    public bool scrollLeft;

    void Start()
    {
        SetupTexture();
        if (scrollLeft) speed = -speed;
    }

    private void SetupTexture()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        width = sprite.texture.width / sprite.pixelsPerUnit;
    }

    void Scroll()
    {
        float posDelta = speed * Time.deltaTime;
        transform.position += new Vector3(posDelta, 0, 0);
    }


    void CheckReset()
    {
        if((Mathf.Abs(transform.position.x) - width)  > 0)
        {
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);

        }
    }


    void Update()
    {
        Scroll();
        CheckReset();
    }
} 