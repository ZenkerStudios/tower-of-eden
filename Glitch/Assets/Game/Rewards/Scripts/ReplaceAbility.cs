using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(-46)]
public class ReplaceAbility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerManager player;
    public RewardSystem rewardSystem;
    public Special newAbility;
    public Special abilityToReplace;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;
    public Image itemIcon;
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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        indicator.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Image>().material = GameUtilities.instance.GetColorForRarity(abilityToReplace.GetRarity());
        itemBg.sprite = bgRarity[(int)abilityToReplace.GetRarity() - 2];
        imgFrame.sprite = frameRarity[(int)abilityToReplace.GetRarity() - 2];
        itemIcon.sprite = abilityToReplace.GetSprite();
        itemName.text = abilityToReplace.GetName();
        itemDesc.text = abilityToReplace.GetDesc();
        if (rewardSystem.gold < (30 * (int)newAbility.GetRarity())) rewardSystem.gold = 30 * (int)newAbility.GetRarity();

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

        if (abilityToReplace.GetStatusChance() > 0)
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
        player.RemoveSpecial(abilityToReplace);
        player.AddSpecial(newAbility);
        GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().RemoveSpecialFromCollection(newAbility);
        rewardSystem.replacementWindow.SetActive(false);
        rewardSystem.ReturnToEncounterSelect();
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
