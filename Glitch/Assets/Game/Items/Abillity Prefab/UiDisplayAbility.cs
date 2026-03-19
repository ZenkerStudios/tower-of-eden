using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiDisplayAbility : MonoBehaviour, IPointerEnterHandler
{
    public Image abilityImg;
    public IAbility ability;

    public Image imgFrame;
    public List<Sprite> frameRarity;

   
    private PlayerManager player;

    public void Initialize(IAbility a, PlayerManager pm)
    {
        ability = a;
        player = pm;
        abilityImg.sprite = ability.GetSprite();
        imgFrame.sprite = frameRarity[(int)ability.GetRarity() - 2];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowDesc();
    }

    public void ShowDesc()
    {
        if (!player.inBattle) return;
        player.abilityRank.gameObject.SetActive(false);
        player.statusChance.gameObject.SetActive(false);
        player.turns.gameObject.SetActive(false);
        player.description.text = "";
        player.turns.text = "";
        player.statusChance.text = "";
        player.numTarget.text = "";

        int chance = 0;
        if (ability.GetAbilityType() == AbilityTypes.Offensive)
        {
            chance = Mathf.Clamp(ability.GetStatusChance() + player.h.GetStatusChance(ability.GetDamageType()), 0, 100);
        }
        player.abilityname.text = ability.GetName();
        player.description.text = ability.GetDesc();
        player.numTarget.text = "Targets: " + ability.GetNumTarget().ToString();
        if (ability.GetTargetTypes() == TargetTypes.OnSelf)
        {
            player.numTarget.text = "Target: Self";
        }

        if (ability.GetAbilityType() == AbilityTypes.Offensive && !ability.GetDamageType().GetTypeName().Equals("Physical") && !ability.GetDamageType().GetTypeName().Equals("Divine") && ability.GetStatusChance() > 0)
        {
            player.statusChance.text = "Chance to apply condition: " + chance + "%";
            player.statusChance.gameObject.SetActive(true);
        }

        if (ability.GetDuration() > 0)
        {
            player.turns.text = "Duration: " + ability.GetDuration();
            player.turns.gameObject.SetActive(true);
        }

        player.abilityRank.text = "Mastery Rank: " + ability.GetMasteryRank();
        player.abilityRank.gameObject.SetActive(true);
        player.numTarget.gameObject.SetActive(true);

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
