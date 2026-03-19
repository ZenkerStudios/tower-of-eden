using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityShopItem : MonoBehaviour
{
    public TextMeshProUGUI costText;
    public int cost;

    [SerializeField]
    protected RewardType boughtWithCurrency;

    private PlayerManager player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

    }

    // Update is called once per frame
    void Update()
    {
        costText.color = new Color32(255, 255, 255, 255);

        switch (boughtWithCurrency)
        {
            case RewardType.Gemstones:
                costText.text = "<sprite=9> " + cost;
                if (player.gemstonesOwned < cost)
                {
                    costText.color = new Color32(255, 0, 0, 255);

                }
                break;
            case RewardType.EtherVial:
                costText.text = "<sprite=7> " + cost;
                if (player.etherVialsOwned < cost)
                {
                    costText.color = new Color32(255, 0, 0, 255);

                }
                break;
            case RewardType.EtherShard:
                costText.text = "<sprite=5> " + cost;
                if (player.etherShardsOwned < cost)
                {
                    costText.color = new Color32(255, 0, 0, 255);

                }
                break;
            case RewardType.GrimoirePage:
                costText.text = "<sprite=13> " + cost;
                if (player.grimoirePagesOwned < cost)
                {
                    costText.color = new Color32(255, 0, 0, 255);

                }
                break;
            case RewardType.MetalScraps:
                costText.text = "<sprite=19> " + cost;
                if (player.metalScrapsOwned < cost)
                {
                    costText.color = new Color32(255, 0, 0, 255);

                }
                break;
        }
    }

    public void BuyVials(int val)
    {
        if (cost <= player.gemstonesOwned)
        {
            player.ChangeGemstonesAmount(-cost);
            player.ChangeVialsAmount(val);
        }
    }

    public void BuyShards(int val)
    {
        if (cost <= player.metalScrapsOwned)
        {
            player.ChangeScrapsAmount(-cost);
            player.ChangeShardAmount(val);
        }
    }

    public void BuyScraps(int val)
    {
        if (cost <= player.etherVialsOwned)
        {
            player.ChangeVialsAmount(-cost);
            player.ChangeScrapsAmount(val);
        }
    }

    public void BuyPage(int val)
    {
        if (cost <= player.etherShardsOwned)
        {
            player.ChangeShardAmount(-cost);
            player.ChangeGrimoirePageAmount(val);
        }
    }

}
