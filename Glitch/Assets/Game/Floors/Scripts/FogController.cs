using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public GameObject hallwayAFogFront;
    public GameObject hallwayAFogBack;
    public GameObject hallwayBFogFront;
    public GameObject hallwayBFogBack;
    public GameObject bossFogFront;
    public GameObject bossFogBack;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleBossFog(bool val)
    {
        bossFogBack.SetActive(val);
        bossFogFront.SetActive(val);
    }


    public void ToggleHallwayFog(bool val, bool midpoint)
    {
        if(midpoint)
        {
            hallwayAFogFront.SetActive(false);
            hallwayAFogBack.SetActive(false);
            hallwayBFogFront.SetActive(val);
            hallwayBFogBack.SetActive(val);
        }
        else
        {
            hallwayAFogFront.SetActive(val);
            hallwayAFogBack.SetActive(val);
            hallwayBFogFront.SetActive(false);
            hallwayBFogBack.SetActive(false);
        }
    }
}
