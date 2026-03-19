using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class RewardNotif : MonoBehaviour
{
    public TextMeshProUGUI rewardMsg;

    private float disappearTime;
    [SerializeField]
    protected float DISAPPEAR_TIME_MAX = 6f;

    // Start is called before the first frame update
    void Start()
    {
        disappearTime = DISAPPEAR_TIME_MAX;

    }

    // Update is called once per frame
    void Update()
    {
        disappearTime -= Time.deltaTime;
        if (disappearTime < 0)
        {

            Destroy(gameObject);

        }
    }
}
