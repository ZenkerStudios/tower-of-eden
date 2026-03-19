using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "newSpecialSystem", menuName = "Item/SpecialSystem", order = 5)]
public class SpecialSystem : ScriptableObject
{
    public List<DamageTypeEnumValue> allDamageTypes;
    public List<int> statsIconIndex;
    public List<GameObject> allVfx;
    public string positiveEffectHexVal;
    public string negativeEffectHexVal;
    public GameObject pfDamagePopup;
    public GameObject pfBuffPopup;
    public GameObject pfNoHitPopup;

    public bool ApplyEffects(ICharacter owner, ICharacter target, IAbility ability, Transform transform, bool triggerNotif)
    {
        if(ability.GetDamageType() == null)
        {
            return false;
        }
        MethodInfo methodInfo = typeof(EffectsMethods).GetMethod("Apply" + ability.GetDamageType().name);
        // writes all the property names
        bool applied = (bool)methodInfo.Invoke(new EffectsMethods(), new System.Object[] { owner, target, ability });
        if(applied && triggerNotif)
        {
            transform.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(Condition(ability.GetDamageType(), transform));
        }
        return true;
    }

    public void TriggerDamagePopUp(Transform t, int initialAmount, int amount, bool isHeal, bool isCrit)
    {
        var newDmgPopup = GameObject.Instantiate(pfDamagePopup);
        newDmgPopup.transform.SetParent(t);
        newDmgPopup.transform.localPosition = new Vector2(0, 0);
        newDmgPopup.GetComponent<RectTransform>().sizeDelta = pfDamagePopup.GetComponent<RectTransform>().sizeDelta;
        newDmgPopup.GetComponent<RectTransform>().localScale = pfDamagePopup.GetComponent<RectTransform>().localScale;
        DamagePopup popUp = newDmgPopup.GetComponent<DamagePopup>();
        popUp.SetUp(initialAmount, amount, isHeal, isCrit);
    }

    public void TriggerIconPopUpForStat(Transform t, bool isBuff, CharacterStats stat)
    {
        var newDmgPopup = GameObject.Instantiate(pfBuffPopup);
        newDmgPopup.transform.SetParent(t);
        newDmgPopup.transform.localPosition = new Vector2(0, 0);
        newDmgPopup.GetComponent<RectTransform>().sizeDelta = pfBuffPopup.GetComponent<RectTransform>().sizeDelta;
        newDmgPopup.GetComponent<RectTransform>().localScale = pfBuffPopup.GetComponent<RectTransform>().localScale;
        BuffPopup popUp = newDmgPopup.GetComponent<BuffPopup>();
        AudioManager.instance.PlaySfxSound("Buff_Popup");
        popUp.SetUp(isBuff, GetIconIndex(stat));
    }

    public void TriggerIconPopUpWithIndex(Transform t, bool isBuff, int index)
    {
        var newDmgPopup = GameObject.Instantiate(pfBuffPopup);
        newDmgPopup.transform.SetParent(t);
        newDmgPopup.transform.localPosition = new Vector2(0, 0);
        newDmgPopup.GetComponent<RectTransform>().sizeDelta = pfBuffPopup.GetComponent<RectTransform>().sizeDelta;
        newDmgPopup.GetComponent<RectTransform>().localScale = pfBuffPopup.GetComponent<RectTransform>().localScale;
        BuffPopup popUp = newDmgPopup.GetComponent<BuffPopup>();
        AudioManager.instance.PlaySfxSound("Buff_Popup");
        popUp.SetUp(isBuff, index);
    }

    public int GetIconIndex(CharacterStats stat)
    {
        int index = 0;
        switch(stat)
        {
            case CharacterStats.Health:
                index = 45;
                break;
            case CharacterStats.Accuracy:
                index = 46;
                break;
            case CharacterStats.CritChance:
                index = 47;
                break;
            case CharacterStats.Actions:
                index = 48;
                break;
            case CharacterStats.Block:
                index = 44;
                break;
            case CharacterStats.Resistance:
                index = 49;
                break;
            case CharacterStats.Toughness:
                index = 49;
                break;
            case CharacterStats.Lifestrike:
                index = 50;
                break;
            case CharacterStats.Strength:
                index = 51;
                break;
            case CharacterStats.Lifesteal:
                index = 55;
                break;
            case CharacterStats.Dominance:
                index = 54;
                break;
            case CharacterStats.Dazed:
                index = 28;
                break;
            case CharacterStats.Gold:
                index = 11;
                break;
            case CharacterStats.Cleanse:
                index = 52;
                break;
            case CharacterStats.MultiHit:
                index = 45;
                break;
            case CharacterStats.NewSkill:
                index = 36;
                break;
            case CharacterStats.HealthRegen:
                index = 53;
                break;
        }
        return index;

    }

    public void TriggerConditionPopup(Transform t, DamageTypeEnumValue dt)
    {
        t.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(Condition(dt, t));
    }

    public void TriggerNoHit(Transform t, string msg)
    {
        Color c = new Color(255, 255, 255, 255);
        TriggerNoHit(t, msg, c, 40);
    }

    public void TriggerNoHit(Transform t, string msg, int font)
    {
        Color c = new Color(255, 255, 255, 255);
        TriggerNoHit(t, msg, c, font);
    }

    public void TriggerNoHit(Transform t, string msg, Color c, int font)
    {
        var newObj = GameObject.Instantiate(pfNoHitPopup);
        newObj.transform.SetParent(t);
        newObj.transform.localPosition = new Vector2(0, 0);
        newObj.GetComponent<RectTransform>().sizeDelta = pfNoHitPopup.GetComponent<RectTransform>().sizeDelta;
        newObj.GetComponent<RectTransform>().localScale = pfNoHitPopup.GetComponent<RectTransform>().localScale;
        NoHitPopup popUp = newObj.GetComponent<NoHitPopup>();
        popUp.SetUp(msg, font);
        popUp.missText.color = c;
    }

    public IEnumerator Condition(DamageTypeEnumValue dt, Transform transform)
    {
        Color c;
        ColorUtility.TryParseHtmlString("#" + dt.textColor, out c);
        switch (dt.name)
        {
            case "Fire":
                TriggerNoHit(transform, "Burning", c, 45);
                break;
            case "Ice":
                TriggerNoHit(transform, "Frozen", c, 45);
                break;
            case "Poison":
                TriggerNoHit(transform, "Poisoned", c, 45);
                break;
            case "Psychic":
                TriggerNoHit(transform, "Vulnerable", c, 45);
                break;
            case "Lightning":
                TriggerNoHit(transform, "Shocked", c, 45);
                break;
            case "Divine":
                TriggerNoHit(transform, "Condemned", c, 45);
                break;
        }

        yield break;
    }
}



[Serializable]
public class EffectsMethods
{
    public static void AddCondition(ICharacter target, DamageTypeEnumValue dt)
    {
        if (target is EnemyToken npc)
        {
            npc.cSystem.TrackBountyCondition(dt.spriteIndex);

        }

        if (!target.GetConditions().Contains(dt.GetConditionName()))
        {
            target.CreateCondToken(dt);
        }
    }
    //Physical, Ice, Fire, Psychic, Poison, Radiant, Lightning
    //Most effects don't stack
    public bool ApplyPhysical(ICharacter owner, ICharacter target, IAbility ability)
    {
        //Default
        return false;
    }

    public bool ApplyIce(ICharacter owner, ICharacter target, IAbility ability)
    {
        //Chance to apply frozen condition. Frozen enemies skip a turn
        int chance = ability.GetStatusChance() + owner.GetStatusChance(ability.GetDamageType()); 
        if (target.HasResistance("Ice")) chance -= target.GetFrozenResistance();
        if (owner is PlayerHero && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode == Floors.Floor_02)
        {
            chance = 0;
        }
        if (UnityEngine.Random.Range(1, 101) <= chance)
        {       
            int frozenTurn = Math.Max(target.GetIsFrozen(), owner.GetFrozenTurns());
            target.SetIsFrozen(frozenTurn);
            AddCondition(target, ability.GetDamageType());
            return true;
        }
        return false;
    }

    public bool ApplyFire(ICharacter owner, ICharacter target, IAbility ability)
    {
        //Chance to apply burning condition. Burning enemies take damage every turn for 2 turns.
        int chance = ability.GetStatusChance() + owner.GetStatusChance(ability.GetDamageType()); 
        if (target.HasResistance("Fire")) chance -= target.GetBurningResistance();
        if (owner is PlayerHero && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode == Floors.Floor_02)
        {
            chance = 0;
        }
        if (UnityEngine.Random.Range(1, 101) <= chance)
        {
            int burnTurn = Math.Max(target.GetIsBurning()[0], owner.GetBurningInfo()[0]);
            int burnStat = Math.Max(target.GetIsBurning()[1], owner.GetBurningInfo()[1]);
            int[] burn = new int[] { burnTurn, burnStat };
            target.SetIsBurning(burn);
            AddCondition(target, ability.GetDamageType());
            return true;
        }
        return false;

    }

    public bool ApplyPsychic(ICharacter owner, ICharacter target, IAbility ability)
    {
        //Chance to apply vulnerable condition. Vulnerable enemies take 1.5x damage for 2 turn.
        int chance = ability.GetStatusChance() + owner.GetStatusChance(ability.GetDamageType()); 
        if (target.HasResistance("Psychic")) chance -= target.GetVulnerableResistance();
        if (owner is PlayerHero && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode == Floors.Floor_02)
        {
            chance = 0;
        }
        if (UnityEngine.Random.Range(1, 101) <= chance)
        {
            float vulnerableTurn = Math.Max(target.GetIsVulnerable()[0], owner.GetVulnerableInfo()[0]);
            float vulnerableStat = Math.Max(target.GetIsVulnerable()[1], owner.GetVulnerableInfo()[1]);
            float[] vulnerable = new float[] { vulnerableTurn, vulnerableStat };
            target.SetIsVulnerable(vulnerable);
            AddCondition(target, ability.GetDamageType());
            return true;
        }
        return false;

    }

    public bool ApplyPoison(ICharacter owner, ICharacter target, IAbility ability)
    {
        //Chance to apply poisoned condition. Poisoned enemies can't heal for 2 turns. 
        int chance = ability.GetStatusChance() + owner.GetStatusChance(ability.GetDamageType()); 
        if (target.HasResistance("Poison")) chance -= target.GetPoisonResistance();
        if (owner is PlayerHero && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode == Floors.Floor_02)
        {
            chance = 0;
        }
        if (UnityEngine.Random.Range(1, 101) <= chance)
        {
            int poisonTurn = Math.Max(target.GetIsPoisoned(), owner.GetPoisonedTurns());
            target.SetIsPoisoned(poisonTurn);
            AddCondition(target, ability.GetDamageType());
            return true;
        }
        return false;

    }

    public bool ApplyDivine(ICharacter owner, ICharacter target, IAbility ability)
    {
        //Always apply Damnation stack. Stacks expire after 2 turns and deals damage based on the number of stacks. Adding a new stack resets expiration.  
        if (target.GetDamnation()[1] > 0)
        {
            int[] temp = new int[4] { owner.GetDamnationInfo()[0], Mathf.Clamp(target.GetDamnation()[1] += owner.GetDamnationInfo()[1], 0, owner.GetDamnationInfo()[3]), owner.GetDamnationInfo()[2], owner.GetDamnationInfo()[3] };

            //turn, stacks, dmg, max
            int damnTurn = Math.Max(target.GetDamnation()[0], temp[0]);
            int damnStack = Math.Max(target.GetDamnation()[1], temp[1]);
            int damnStat = Math.Max(target.GetDamnation()[2], temp[2]);
            int damnMax = Math.Min(target.GetDamnation()[3], temp[3]);
            int[] damn = new int[] { damnTurn, damnStack, damnStat, damnMax };

            target.SetDamnation(damn);
            AddCondition(target, ability.GetDamageType());
        }
        else
        {
            target.SetDamnation(owner.GetDamnationInfo());
            AddCondition(target, ability.GetDamageType());
        }
        return true;
    }

    public bool ApplyLightning(ICharacter owner, ICharacter target, IAbility ability)
    {
        //Chance to apply shocked condition. Shocked enemies have a lower chance to hit
        int chance = ability.GetStatusChance() + owner.GetStatusChance(ability.GetDamageType()); 
        if (target.HasResistance("Lightning")) chance -= target.GetShockedesistance();
        if (owner is PlayerHero && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode == Floors.Floor_02)
        {
            chance = 0;
        }
        if (UnityEngine.Random.Range(1, 101) <= chance)
        {
            int shockedTurn = Math.Max(target.GetIsShocked()[0], owner.GetShockedInfo()[0]);
            int shockedStat = Math.Max(target.GetIsShocked()[1], owner.GetShockedInfo()[1]);
            int[] shocked = new int[] { shockedTurn, shockedStat };
            target.SetIsShocked(shocked);
            AddCondition(target, ability.GetDamageType());
            return true;
        }
        return false;

    }


}