using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

[DefaultExecutionOrder(-46)]
public class AddAbility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerManager player;
    public RewardSystem rewardSystem;
    public Special newAbility;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;
    public Image itemIcon;
    public GameObject indicator;
    public Image itemBg;
    public Image imgFrame;
    public List<Sprite> bgRarity;
    public List<Sprite> frameRarity;
    public bool owned;

    public TextMeshProUGUI turns;
    public TextMeshProUGUI statusChance;
    public TextMeshProUGUI numTarget;
    public TextMeshProUGUI rank;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        indicator.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Image>().material = GameUtilities.instance.GetColorForRarity(newAbility.GetRarity());
        itemBg.sprite = bgRarity[(int)newAbility.GetRarity() - 2];
        imgFrame.sprite = frameRarity[(int)newAbility.GetRarity() - 2];
        itemIcon.sprite = newAbility.GetSprite();
        itemName.text = newAbility.GetName();
        itemDesc.text = newAbility.GetDesc();


        statusChance.gameObject.SetActive(false);
        turns.gameObject.SetActive(false);


        int chance = 0;
        if (newAbility.GetAbilityType() == AbilityTypes.Offensive)
        {
            chance = Mathf.Clamp(newAbility.GetStatusChance(), 0, 100);
        }

        GameUtilities.SetTextColorForRarity(itemName, newAbility.GetRarity());
        numTarget.text = "Targets: " + newAbility.GetNumTarget().ToString();
        if (newAbility.GetTargetTypes() == TargetTypes.OnSelf)
        {
            numTarget.text = "Target: Self";
        }

        if (newAbility.GetAbilityType() == AbilityTypes.Offensive && !newAbility.GetDamageType().GetTypeName().Equals("Physical") && !newAbility.GetDamageType().GetTypeName().Equals("Divine") && newAbility.GetStatusChance() > 0)
        {
            statusChance.text = "Base Chance to apply condition: " + chance + "%";
            statusChance.gameObject.SetActive(true);
        }

        if (newAbility.GetDuration() > 0)
        {
            turns.text = "Duration: " + newAbility.GetDuration();
            turns.gameObject.SetActive(true);
        }

        rank.text = "Mastery Rank: " + newAbility.GetMasteryRank();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectThisAbility()
    {
        //If player already owns the ability find it and level it up instead
        if (owned)
        {
            foreach (IAbility a in rewardSystem.abilitiesOwned)
            {
                if (a.GetName().Equals(newAbility.GetName()))
                {
                    if ((a is Attack att && att.canLevelup) || (a is Special sp && sp.canLevelup))
                    {
                        a.LevelUp();
                        List<System.Action> popups = new List<System.Action>();
                        popups.Add(() => a.TriggerNoHit(player.popupWindow.transform, a.GetName() + ": +1 Rank!", 12));
                        GameUtilities.ShowLevelUpPopup(player, popups);
                    }
                    if ((int)newAbility.GetRarity() > (int)a.GetRarity())
                    {
                        a.SetRarity(newAbility.GetRarity());
                    }
                    else
                    {
                        player.h.playerInventory.ChangeGoldAmount(30*(int)newAbility.GetRarity());
                    }
                    break;
                }
            }
            rewardSystem.ReturnToEncounterSelect();
        }
        else if(player.h.specials.Count >= player.h.GetMaxAbilityCount())
        {
            rewardSystem.replacementWindow.SetActive(true);
            List<Special> specialsOwned = new List<Special>();
            specialsOwned.AddRange(player.h.specials);
            specialsOwned = specialsOwned.OrderBy(a => Guid.NewGuid()).ToList();
            int toReplaceCount = 0;
            foreach (Special abilityToReplace in specialsOwned)
            {
                if (toReplaceCount >= 3)
                {
                    break;
                }
                var newObj = Instantiate(rewardSystem.pfAbilityToReplace);
                newObj.GetComponent<ReplaceAbility>().newAbility = newAbility;
                newObj.GetComponent<ReplaceAbility>().abilityToReplace = abilityToReplace;
                newObj.GetComponent<ReplaceAbility>().rewardSystem = this.rewardSystem;
                newObj.transform.SetParent(rewardSystem.abilityReplacePanel.transform);
                newObj.transform.localPosition = new Vector2(0, 0);
                newObj.GetComponent<RectTransform>().sizeDelta = rewardSystem.pfAbilityToReplace.GetComponent<RectTransform>().sizeDelta;
                newObj.GetComponent<RectTransform>().localScale = rewardSystem.pfAbilityToReplace.GetComponent<RectTransform>().localScale;
                toReplaceCount++;
            }
        }
        else
        {
            player.AddSpecial(newAbility);
            GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().RemoveSpecialFromCollection(newAbility);
            rewardSystem.ReturnToEncounterSelect();
        }

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
