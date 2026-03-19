using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopReplaceItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Special abilityToReplace;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;
    public Image itemIcon;

    public AbilityShopItem shopItem;

    public Image imgFrame;
    public Image itemBg;
    public List<Sprite> bgRarity;
    public List<Sprite> frameRarity;
    public GameObject indicator;

    public TextMeshProUGUI turns;
    public TextMeshProUGUI statusChance;
    public TextMeshProUGUI numTarget;
    public TextMeshProUGUI rank;

    private void Awake()
    {
        indicator.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(abilityToReplace == null)
        {
            gameObject.SetActive(false);
            return;
        }
        itemBg.sprite = bgRarity[(int)abilityToReplace.GetRarity() - 2];
        imgFrame.sprite = frameRarity[(int)abilityToReplace.GetRarity() - 2];
        itemIcon.sprite = abilityToReplace.GetSprite();
        itemName.text = abilityToReplace.GetName();
        itemDesc.text = abilityToReplace.GetDesc();

        statusChance.gameObject.SetActive(false);
        turns.gameObject.SetActive(false);

        int chance = 0;
        if (abilityToReplace.GetAbilityType() == AbilityTypes.Offensive)
        {
            chance = Mathf.Clamp(abilityToReplace.GetStatusChance(), 0, 100);
        }

        GameUtilities.SetTextColorForRarity(itemName, abilityToReplace.GetRarity());
        numTarget.text = "Targets: " + abilityToReplace.GetNumTarget().ToString();
        if (abilityToReplace.GetTargetTypes() == TargetTypes.OnSelf)
        {
            numTarget.text = "Target: Self";
        }

        if (abilityToReplace.GetAbilityType() == AbilityTypes.Offensive && !abilityToReplace.GetDamageType().GetTypeName().Equals("Physical") && !abilityToReplace.GetDamageType().GetTypeName().Equals("Divine") && abilityToReplace.GetStatusChance() > 0)
        {
            statusChance.text = "Base Chance to apply condition: " + chance + "%";
            statusChance.gameObject.SetActive(true);
        }

        if (abilityToReplace.GetDuration() > 0)
        {
            turns.text = "Duration: " + abilityToReplace.GetDuration();
            turns.gameObject.SetActive(true);
        }
        rank.text = "Mastery Rank: " + abilityToReplace.GetMasteryRank();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectThisAbility()
    {
        shopItem.shop.player.RemoveSpecial(abilityToReplace);
        shopItem.shop.player.AddSpecial(shopItem.newAbility);
        shopItem.shop.player.h.playerInventory.ChangeGoldAmount(-shopItem.cost);
        shopItem.shop.offeredAbilities.Remove(shopItem.newAbility);
        Destroy(shopItem.gameObject);
        foreach (Ailment ail in shopItem.shop.player.h.playerInventory.ailments)
        {
            ail.abilityBought = true;
        }
        GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().RemoveSpecialFromCollection(shopItem.newAbility);
        shopItem.shop.EndReplacement();
        gameObject.SetActive(false);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        indicator.SetActive(true);

        if (!pulsating)
        {
            float timer = 0.0025f;
            StartCoroutine(GameUtilities.PulsateUiObject(indicator, timer, 0.02f, 1.1f, 1f, 1f));
            pulsating = true;
            StartCoroutine(PulseTime(0.3f + timer));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        indicator.SetActive(false);
    }

    private bool pulsating = false;

    IEnumerator PulseTime(float timer)
    {
        yield return new WaitForSeconds(timer);
        pulsating = false;

    }
}
