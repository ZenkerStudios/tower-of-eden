using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleOnHoverDesc : MonoBehaviour, IPointerEnterHandler
{
    public CombatUiComponent combatUiComponent;
    public TextMeshProUGUI abilityname;
    public TextMeshProUGUI description;
    public TextMeshProUGUI turns;
    public TextMeshProUGUI statusChance;
    public TextMeshProUGUI numTarget;
    private PlayerHero hero;
    


    private void Awake()
    {

        hero = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().h;
        if (!(combatUiComponent is CombatUiComponent.Character))
        {
            statusChance.gameObject.SetActive(false);
            turns.gameObject.SetActive(false);
            numTarget.gameObject.SetActive(false);
        }

    }

    public void OnHover()
    {
        if(combatUiComponent is CombatUiComponent.Character)
        {
            GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>().SetCharacterDesc(hero);
            return;
        }

        statusChance.gameObject.SetActive(false);
        turns.gameObject.SetActive(false);
        description.text = "";
        abilityname.text = "";
        turns.text = "";
        statusChance.text = "";
        numTarget.text = "";

        IAbility ability = null;
        switch (combatUiComponent)
        {
            case CombatUiComponent.Attack:
                ability = hero.baseAttack;
                break;
            case CombatUiComponent.Special1:
                ability = hero.generatedSpecials[0];
                break;
            case CombatUiComponent.Special2:
                ability = hero.generatedSpecials[1];
                break;
            case CombatUiComponent.Special3:
                ability = hero.generatedSpecials[2];
                break;
            case CombatUiComponent.Special4:
                ability = hero.generatedSpecials[3];
                break;
            case CombatUiComponent.Special5:
                ability = hero.generatedSpecials[4];
                break;
            case CombatUiComponent.Special6:
                ability = hero.generatedSpecials[5];
                break;
            case CombatUiComponent.Special7:
                ability = hero.generatedSpecials[6];
                break;
            case CombatUiComponent.Special8:
                ability = hero.generatedSpecials[7];
                break;
            case CombatUiComponent.Special9:
                ability = hero.generatedSpecials[8];
                break;
            case CombatUiComponent.Special10:
                ability = hero.generatedSpecials[9];
                break;
        }

        int chance = 0;
        if(ability.GetAbilityType() == AbilityTypes.Offensive)
        {
            chance = Mathf.Clamp(ability.GetStatusChance() + hero.GetStatusChance(ability.GetDamageType()), 0, 100);
        }
        abilityname.text = ability.GetName();
        GameUtilities.SetTextColorForRarity(abilityname, ability.GetRarity());
        description.text = ability.GetDesc();
        numTarget.text = "Targets: " + ability.GetNumTarget().ToString();
        if (ability.GetTargetTypes() == TargetTypes.OnSelf)
        {
            numTarget.text = "Target: Self";
        }

        if (ability.GetAbilityType() == AbilityTypes.Offensive && !ability.GetDamageType().GetTypeName().Equals("Physical") && !ability.GetDamageType().GetTypeName().Equals("Divine") && ability.GetStatusChance() > 0)
        {
            statusChance.text = "Chance to apply condition: " + chance + "%";
            statusChance.gameObject.SetActive(true);
        }

        if (ability.GetDuration() > 0)
        {
            turns.text = "Duration: " + ability.GetDuration();
            turns.gameObject.SetActive(true);
        }

        numTarget.gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>().SetAbilityDesc();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            OnHover();

        } catch(System.ArgumentOutOfRangeException) {}
    }
}
