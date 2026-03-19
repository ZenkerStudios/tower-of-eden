using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BountyHud : MonoBehaviour
{
    public Bounty thisBounty;
    public TextMeshProUGUI bountyName;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI reward;
    public GameObject completedIcon;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        completedIcon.SetActive(thisBounty.completed);
    }

    public void InitBounty(Bounty bounty)
    {
        completedIcon.SetActive(false);
        thisBounty = bounty;
        bountyName.text = bounty.bountyName;
        desc.text = bounty.bountyInfo;
        switch (bounty.bountyRewards)
        {
            case RewardType.Gemstones:
                reward.text = "<sprite=" + 9 + "> " + bounty.reward;
                break;
            case RewardType.MetalScraps:
                reward.text = "<sprite=" + 19 + "> " + bounty.reward;
                break;
            case RewardType.EtherVial:
                reward.text = "<sprite=" + 7 + "> " + bounty.reward;
                break;
            case RewardType.EtherShard:
                reward.text = "<sprite=" + 5 + "> " + bounty.reward;
                break;
            case RewardType.GrimoirePage:
                reward.text = "<sprite=" + 13 + "> " + bounty.reward;
                break;
        }
    }

}
