using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactShopItem : MonoBehaviour
{
    public int cost;
    public TextMeshProUGUI costText;

    public Shop shop;
    public Artifact artifact;
    public TextMeshProUGUI itemName;
    public Image itemIcon;



    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        itemName.text = artifact.GetName();
        itemIcon.sprite = artifact.GetSprite();
        GameUtilities.SetTextColorForRarity(itemName, artifact.rarity);
        costText.text = "<sprite=11>" + cost;
    }


    // Update is called once per frame
    void Update()
    {
        costText.color = new Color32(255, 255, 255, 255);
        if (shop.player.h.playerInventory.GetGoldAmount() < cost)
        {
            costText.color = new Color32(255, 0, 0, 255);

        }
    }

    public void BuyArtifact()
    {
        if (shop.player.h.playerInventory.GetGoldAmount() >= cost)
        {
            foreach (Ailment ail in shop.player.h.playerInventory.ailments)
            {
                ail.artifactBought = true;
            }
            shop.player.AddArtifact(artifact);
            shop.player.h.playerInventory.ChangeGoldAmount(-cost);
            GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().selectedArtifacts.Remove(artifact);
            Destroy(gameObject);

        }
    }


    public void ShowDesc()
    {
        GameObject shopdesc = GameObject.Find("ShopDesc");
        shopdesc.GetComponent<TextMeshProUGUI>().text = artifact.GetDesc();
        GameObject go = shopdesc.transform.GetChild(shopdesc.transform.childCount-1).gameObject;
        go.SetActive(false);
    }

}
