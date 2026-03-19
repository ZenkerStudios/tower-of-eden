using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiParallax : MonoBehaviour
{
    [Tooltip("Start from furthest to the nearest object.")]
    [SerializeField] private GameObject[] ParalaxObjects;
    [SerializeField] private GameObject anchor;
    [SerializeField] private float MouseSpeedX = 1f;

    //Paralax effect will be applied as an offset to the original positions
    private Vector3[] OriginalPositions;
    private Vector3 anchorOriginalPosition;

    // Start is called before the first frame update
    void Start()
    {
        GetOriginalPos();
    }

    private float elapsedTime;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (anchor.transform.position != anchorOriginalPosition) GetOriginalPos();

        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / 3;
        float x;
        x = (Input.mousePosition.x - (Screen.width / 2)) * MouseSpeedX / Screen.width;

        //For each object in ParalaxObjects calculate and apply an offset based on cursor position
        for (int i = 1; i < ParalaxObjects.Length + 1; i++)
        {
            var endPos = OriginalPositions[i - 1] + (new Vector3(x, 0, 0f) * i * ((i - 1) - (ParalaxObjects.Length / 2)));
            //ParalaxObjects[i - 1].transform.position = OriginalPositions[i - 1] + (new Vector3(x, y, 0f) * i * ((i - 1) - (ParalaxObjects.Length / 2)));
            ParalaxObjects[i - 1].transform.position = Vector3.Lerp(ParalaxObjects[i - 1].transform.position, endPos, percentageComplete);


        }

    }


    private void GetOriginalPos()
    {
        anchorOriginalPosition = anchor.transform.position;

        OriginalPositions = new Vector3[ParalaxObjects.Length];
        for (int i = 0; i < ParalaxObjects.Length; i++)
        {
            OriginalPositions[i] = ParalaxObjects[i].transform.position;
        }
    }
}
