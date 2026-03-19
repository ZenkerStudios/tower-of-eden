using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[DefaultExecutionOrder(-46)]
public class LevelUpAbility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerManager player;
    public RewardSystem rewardSystem;
    public IAbility ability;
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
    private Rarity r;
    private int rankup = 1;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        indicator.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        r = ability.GetRarity();
        if (rewardSystem.isElite) 
        { 
            r = (Rarity)Mathf.Clamp((int)r + 1, 2, 6); 
            rankup = 2; 
        }

        gameObject.transform.GetChild(0).GetComponent<Image>().material = GameUtilities.instance.GetColorForRarity(r);
        itemBg.sprite = bgRarity[(int)r - 2];
        imgFrame.sprite = frameRarity[(int)r - 2];
        itemIcon.sprite = ability.GetSprite();
        itemName.text = ability.GetName();
        itemDesc.text = ability.GetLevelUpDesc(rankup);

        statusChance.gameObject.SetActive(false);
        turns.gameObject.SetActive(false);

        int chance = 0;
        if (ability.GetAbilityType() == AbilityTypes.Offensive)
        {
            chance = Mathf.Clamp(ability.GetStatusChance(), 0, 100);
        }

        GameUtilities.SetTextColorForRarity(itemName, r);

        numTarget.text = "Targets: " + ability.GetNumTarget().ToString();
        if (ability.GetTargetTypes() == TargetTypes.OnSelf)
        {
            numTarget.text = "Target: Self";
        }

        if (ability.GetAbilityType() == AbilityTypes.Offensive && !ability.GetDamageType().GetTypeName().Equals("Physical") && !ability.GetDamageType().GetTypeName().Equals("Divine") && ability.GetStatusChance() > 0) 
        {
            statusChance.text = "Base Chance to apply condition: " + chance + "%";
            statusChance.gameObject.SetActive(true);
        }

        if (ability.GetDuration() > 0)
        {
            turns.text = "Duration: " + ability.GetDuration();
            turns.gameObject.SetActive(true);
        }
        rank.text = "Mastery Rank: " + ability.GetLevelupMasteryRank(rankup);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectThisAbility()
    {
        if (rewardSystem.isElite)
        {
            ability.LevelUp();
        }
        ability.LevelUp();
        ability.SetRarity(r);
        List<System.Action> popups = new List<System.Action>();
        popups.Add(() => ability.TriggerNoHit(player.popupWindow.transform, ability.GetName() + ": +" + rankup +" Rank!", 12));
        GameUtilities.ShowLevelUpPopup(player, popups);
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
