using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityShopItem : MonoBehaviour
{
    public Image itemImg;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI costText;
    public int cost;
    public Special newAbility;
    public Shop shop;
    public bool owned;


    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        itemImg.sprite = newAbility.GetSprite();
        itemName.text = newAbility.GetName();
        GameUtilities.SetTextColorForRarity(itemName, newAbility.GetRarity());
        costText.text = "<sprite=11>" + cost;
    }

    // Update is called once per frame
    void Update()
    {
        costText.color = new Color32(255, 255, 255, 255);
        if(shop.player.h.playerInventory.GetGoldAmount() < cost)
        {
            costText.color = new Color32(255, 0, 0, 255);
        }

    }

    public void BuyItem()
    {
        if (shop.player.h.playerInventory.GetGoldAmount() >= cost)
        {
            //If player already owns the ability find it and level it up instead
            if (owned)
            {
                foreach (IAbility a in shop.GetAbilitiesOwned())
                {
                    if (a.GetName().Equals(newAbility.GetName()))
                    {
                        if((int)newAbility.GetRarity() > (int)a.GetRarity())
                        {
                            a.SetRarity(newAbility.GetRarity());
                        }
                        a.LevelUp();
                        List<System.Action> popups = new List<System.Action>();
                        popups.Add(() => a.TriggerNoHit(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().popupWindow.transform, "+" + a.GetName(), 12));
                        GameUtilities.ShowLevelUpPopup(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>(), popups);
                        break;
                    }
                }
                shop.offeredAbilities.Remove(newAbility);
                shop.player.h.playerInventory.ChangeGoldAmount(-cost);
                foreach (Ailment ail in shop.player.h.playerInventory.ailments)
                {
                    ail.abilityBought = true;
                }
                Destroy(gameObject);
            }
            else if (shop.player.h.specials.Count >= shop.player.h.GetMaxAbilityCount())
            {
                Transform panel = shop.abilityToReplacePanel.transform.GetChild(0).GetChild(0);
                GameUtilities.DeleteAllChildGameObject(panel.gameObject);
                shop.abilityToReplacePanel.transform.parent.gameObject.SetActive(true);
                int toReplaceCount = 0;
                foreach (Special abilityToReplace in shop.GetAbilitiesOwned().OrderBy(a => Guid.NewGuid()).ToList())
                {
                    if (toReplaceCount >= 3)
                    {
                        break;
                    }
                    var newObj = Instantiate(shop.pfAbilityToReplace);
                    newObj.GetComponent<ShopReplaceItem>().abilityToReplace = abilityToReplace;
                    newObj.GetComponent<ShopReplaceItem>().shopItem = this;
                    newObj.transform.SetParent(panel);
                    newObj.transform.localPosition = new UnityEngine.Vector2(0, 0);
                    newObj.GetComponent<RectTransform>().sizeDelta = shop.pfAbilityToReplace.GetComponent<RectTransform>().sizeDelta;
                    newObj.GetComponent<RectTransform>().localScale = shop.pfAbilityToReplace.GetComponent<RectTransform>().localScale;
                    toReplaceCount++;
                }
            }
            else
            {
                shop.player.AddSpecial(newAbility);
                shop.player.h.playerInventory.ChangeGoldAmount(-cost);
                shop.offeredAbilities.Remove(newAbility);
                foreach (Ailment ail in shop.player.h.playerInventory.ailments)
                {
                    ail.abilityBought = true;
                }
                Destroy(gameObject);
            }
        }
    }

    public void ShowDesc()
    {
        GameObject shopdesc = GameObject.Find("ShopDesc");
        shopdesc.GetComponent<TextMeshProUGUI>().text = newAbility.GetDesc();
        GameObject go = shopdesc.transform.GetChild(shopdesc.transform.childCount-1).gameObject;
        go.SetActive(true);
        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newAbility.GetDesc();

        go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
        go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);

        int chance = 0;
        if (newAbility.GetAbilityType() == AbilityTypes.Offensive)
        {
            chance = Mathf.Clamp(newAbility.GetStatusChance(), 0, 100);
        }

        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Targets: " + newAbility.GetNumTarget().ToString();
        if (newAbility.GetTargetTypes() == TargetTypes.OnSelf)
        {
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Target: Self";
        }

        if (newAbility.GetAbilityType() == AbilityTypes.Offensive && !newAbility.GetDamageType().GetTypeName().Equals("Physical") && !newAbility.GetDamageType().GetTypeName().Equals("Divine") && newAbility.GetStatusChance() > 0)
        {
            go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Base Chance to apply condition: " + chance + "%";
            go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
        }

        if (newAbility.GetDuration() > 0)
        {
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Duration: " + newAbility.GetDuration();
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
        }
        go.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Mastery Rank: " + newAbility.GetMasteryRank();
    }


}
