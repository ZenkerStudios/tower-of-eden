using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("End", 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void End()
    {
        GameController.instance.LoadGame(Floors.Hub);
    }
}
