using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "newSpecial", menuName = "Item/Special", order = 1)]
public class Special : ScriptableObject, IAbility
{
    public SpecialSystem specialSystem;
    public bool canLevelup;
    public Rarity rarity;
    public string specialDesc;
    private string specialDescReadOnly;
    public AbilityTypes abilityType;
    public TargetTypes targetType;
    public NumTarget numTarget;

    //How long ability lasts
    public int abilityDuration;
    public int statusChance;

    public List<CharacterStats> stats;
    public List<StatModType> modType;
    public List<int> powerAmount;
    private List<int> powerAmountReadonly;

    public Special skillToGain;

    //Index for damage types and status chance should be 1-1 pairing
    //The damage type that the effect deals 
    public DamageTypeEnumValue damageType;

    public Sprite abilitySprite;
    public int abilitySpriteIndex;

    public GameObject vfx;
    private int masteryRank = 1;

    private bool positiveEffect = true;

    public IEnumerator TriggerAbility(ICharacter owner, Transform ownerTransform, ICharacter target, Transform targetTransform, bool isCrit, CombatSystem cSystem)
    {
        if (GetName().Equals("Lifeline") || GetName().Equals("Drain")) isCrit = false;
        List<Action> popups = new List<Action>();
        int initialAmount;
        int finalAmount;
        int reduxAmount = 0;
        int specialDmg = 0;
        bool hit = true;
        for (int i = 0; i < stats.Count; i++)
        {
            if (abilityType == AbilityTypes.Offensive || abilityType == AbilityTypes.Affliction)
            {
                positiveEffect = false;
                if (stats[i] == CharacterStats.Health
                    || stats[i] == CharacterStats.MultiHit)
                {
                    reduxAmount = (int)target.GetToughness().Value;
                    specialDmg = -(int)owner.GetDominance().Value;
                }
            }
            finalAmount = powerAmount[i];
            if (!positiveEffect)
            {
                finalAmount *= -1;
            }
            finalAmount += specialDmg;

            switch (stats[i])
            {
                case CharacterStats.Block:
                    if (target.GetBlock() < finalAmount)
                    {
                        target.SetBlock(finalAmount);
                    }
                    finalAmount = 0;
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Block));
                    break;
                case CharacterStats.Health:
                case CharacterStats.MultiHit:

                    if (owner is PlayerHero && targetType != TargetTypes.OnSelf)
                    {
                        finalAmount = finalAmount + DealModDmg((PlayerHero)owner, finalAmount);
                        finalAmount = finalAmount + AilmentDealModDmg((PlayerHero)owner, finalAmount);
                        finalAmount = finalAmount + GoldDealModDmg((PlayerHero)owner, finalAmount);
                        finalAmount = finalAmount - InflictModDmg((PlayerHero)owner, finalAmount);
                    }

                    bool physicalResist = target.HasResistance("Physical");
                    if (target is PlayerHero && targetType != TargetTypes.OnSelf)
                    {
                        PlayerHero hero = (PlayerHero)target;
                        finalAmount = finalAmount + TakeModDmg(hero, finalAmount);
                        physicalResist = hero.GetPhysicalResistance();
                    }

                    if (target.GetIsVulnerable()[0] > 0 && !positiveEffect) finalAmount = (int)(finalAmount * target.GetIsVulnerable()[1]);
                    if (isCrit)
                    {
                        finalAmount = GetCritDamage(finalAmount);
                    }
                    initialAmount = finalAmount;

                    if (target is EnemyToken en && en.HasTrait(Traits.Kingly) && -en.TraitAmount(Traits.Kingly) > finalAmount && !positiveEffect) finalAmount = -en.TraitAmount(Traits.Kingly);

                    if (target.GetIsPoisoned() > 0 && positiveEffect) finalAmount = 0;
                    if (physicalResist && !positiveEffect && damageType.name.Equals("Physical")) finalAmount = (int)(finalAmount * 0.7f);

                    if (!positiveEffect) finalAmount = Mathf.Clamp(finalAmount + reduxAmount, finalAmount, 0);

                    if (target.GetBlock() > 0 && !positiveEffect)
                    {
                        finalAmount = 0;
                        cSystem.GetCombatState().BlockAttack(target, targetTransform);
                        hit = false;
                    }
                    else
                    {
                        hit = true;
                        target.ChangeHealth(finalAmount);
                        specialSystem.TriggerDamagePopUp(targetTransform, initialAmount, finalAmount, positiveEffect, isCrit);
                        cSystem.TrackBountyDamage(target, damageType, finalAmount);
                    }

                    if (owner.GetSpecialLifeSteal().Value > 0 && !positiveEffect)
                    {
                        int healFor = (int)(owner.GetSpecialLifeSteal().Value / 100 * -finalAmount);
                        int initialHeal = healFor;
                        if (owner.GetIsPoisoned() > 0) healFor = 0;
                        owner.ChangeHealth(healFor);
                        specialSystem.TriggerDamagePopUp(ownerTransform, initialHeal, healFor, true, false);
                        cSystem.TrackBountyHeal(owner, healFor);
                    }
                    if (hit && stats[i] == CharacterStats.MultiHit && i < stats.Count-1)
                    {
                        specialSystem.ApplyEffects(owner, target, this, targetTransform, false);
                    }
                    break;
                case CharacterStats.CritChance:
                    target.GetCritChance().AddModifier(new StatModifier(finalAmount,
                        modType[i], abilityDuration, this));
                    finalAmount = 0;
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.CritChance));
                    break; 
                case CharacterStats.Actions:
                    target.GetActions().AddModifier(new StatModifier(finalAmount,
                         modType[i], abilityDuration, this));
                    finalAmount = 0;
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Actions));
                    if (target is EnemyToken n)
                    {
                        if(owner is EnemyToken && (ownerTransform.GetSiblingIndex() >= targetTransform.GetSiblingIndex()))
                        {
                            //Do nothing
                        } else 
                        {
                            cSystem.GetCombatState().AreCurrentActionsValid(n);
                        }
                    }
                    break;
                case CharacterStats.Resistance:
                    target.AddBattleResistance(damageType, abilityDuration, finalAmount, this);
                    finalAmount = 0;
                    popups.Add(() => specialSystem.TriggerIconPopUpWithIndex(targetTransform, positiveEffect, Int16.Parse(damageType.spriteIndex)));
                    break;
                case CharacterStats.Accuracy:
                    target.GetAccuracy().AddModifier(new StatModifier(finalAmount,
                         modType[i], abilityDuration, this));
                    finalAmount = 0;
                    if(GetName().Equals("_Sanguine Sacrifice"))
                    {
                        popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, false, CharacterStats.Accuracy));
                    }
                    else
                    {
                        popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Accuracy));
                    }
                    break;
                case CharacterStats.Toughness:
                    target.GetToughness().AddModifier(new StatModifier(finalAmount, modType[i], abilityDuration, this));
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Toughness));
                    break;
                case CharacterStats.Lifestrike:
                    target.GetAttackLifestrike().AddModifier(new StatModifier(finalAmount, modType[i], abilityDuration, this));
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Lifestrike));
                    break;
                case CharacterStats.Lifesteal:
                    target.GetSpecialLifeSteal().AddModifier(new StatModifier(finalAmount, modType[i], abilityDuration, this));
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Lifesteal));
                    break;
                case CharacterStats.Strength:
                    target.GetStrength().AddModifier(new StatModifier(finalAmount, modType[i], abilityDuration, this));
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Strength));
                    break;
                case CharacterStats.Dominance:
                    target.GetDominance().AddModifier(new StatModifier(finalAmount, modType[i], abilityDuration, this));
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Dominance));
                    break;
                case CharacterStats.Cleanse:
                    target.Cleanse();
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Cleanse));
                    break;
                case CharacterStats.Gold:
                    if(target is PlayerHero pc)
                    {
                        popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Gold));
                        pc.playerInventory.ChangeGoldAmount(finalAmount);
                    }
                    break;
                case CharacterStats.Dazed:
                    if (target is EnemyToken npc)
                    {
                        npc.isDazed = true;
                        npc.RenderDazed();
                        popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.Dazed));
                    }
                    break;
                case CharacterStats.NewSkill:
                    if (target is EnemyToken ene)
                    {
                        ene.enemyNpc.attacks.Add(skillToGain);
                    }
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.NewSkill));
                    break;
                case CharacterStats.HealthRegen:
                    int regenTurn = Math.Max(target.GetHealthRegen()[0], abilityDuration);
                    int regenAmount = Math.Max(target.GetHealthRegen()[1], finalAmount);
                    int[] regen = new int[] { regenTurn, regenAmount };
                    target.CreateOtherCond("Regenerating", "Heal " + finalAmount + " HP <sprite=45> every turn for /trn turns.", "53");
                    target.SetHealthRegen(regen);
                    popups.Add(() => specialSystem.TriggerIconPopUpForStat(targetTransform, positiveEffect, CharacterStats.HealthRegen));
                    break;

            }

            yield return new WaitForSeconds(0.1f);

        }

        if((abilityType == AbilityTypes.Affliction || abilityType == AbilityTypes.Offensive) && hit)
        {
            specialSystem.ApplyEffects(owner, target, this, targetTransform, true);
        }

        foreach(Action act in popups)
        {
            act();
            yield return new WaitForSeconds(0.65f);
        }
        yield break;
    }

    private int GetCritDamage(int baseDmg)
    {
        return (int)(baseDmg * 1.5f);
    }


    public TargetTypes GetTargetTypes()
    {
        return targetType;
    }

    public AbilityTypes GetAbilityType()
    {
        return abilityType;
    }

    public NumTarget GetNumTarget()
    {
        return numTarget;
    }


    public DamageTypeEnumValue GetDamageType()
    {
        return damageType;
    }

    public int GetStatusChance()
    {
        return statusChance;
    }

    public GameObject GetVfx()
    {
        return vfx;
    }

    public string GetName()
    {
        return name.Replace("(Clone)", "");
    }

    private void Awake()
    {
        powerAmountReadonly = new List<int>(powerAmount);
        SetRarity(rarity);
        SetDesc();
    }

    public void SetDesc()
    {
        string desc = "";
        string[] lst = specialDesc.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        for(int x = 0; x < lst.Length; x++)
        {
            if (lst[x].Equals("/dmg"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                lst[x + 1] = "<color=#" + damageType.textColor + ">" + powerAmount[index] + "</color> <sprite=" + damageType.spriteIndex + ">";
            }
            else if (lst[x].Equals("/heal"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                lst[x + 1] = "<color=#00FF00>" + powerAmount[index] + "</color>";
            }
            else if (lst[x].Equals("/status"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                lst[x + 1] = powerAmount[index] + "";
            }
            else if (lst[x].Equals("/multi"))
            {
                desc += " " + powerAmount.Count;
            }
            else if (lst[x].Equals("/turn"))
            {
                desc += " " + abilityDuration;
            }
            else
            {
                desc += " " + lst[x];
            }
        }

        specialDescReadOnly = desc + ".";

    }

    public string GetDesc()
    {
        return specialDescReadOnly;
    }

    public string GetLevelUpDesc(int newRarity)
    {
        string desc = "";
        string[] lst = specialDesc.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        for (int x = 0; x < lst.Length; x++)
        {
           
            if (lst[x].Equals("/dmg"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                lst[x + 1] = "<color=#" + damageType.textColor + ">" + powerAmount[index] + "</color> <sprite=" + damageType.spriteIndex + ">";

                int power = Mathf.Clamp(powerAmount[index] + healthIncrease, 0, 99);
                if (power < powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.negativeEffectHexVal + ">(+" + (healthIncrease + GetRarityBonus(newRarity)[1]) + ")</color>";
                } else if (power > powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.positiveEffectHexVal + ">(+" + (healthIncrease + GetRarityBonus(newRarity)[1]) + ")</color>";
                }
            }
            else if (lst[x].Equals("/heal"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                lst[x + 1] = "<color=#00FF00>" + powerAmount[index] + "</color>";

                int power = Mathf.Clamp(powerAmount[index] + healthIncrease, -10, 99);
                if (power < powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.negativeEffectHexVal + ">(+" + (healthIncrease + GetRarityBonus(newRarity)[1]) + ")</color>";
                }
                else if (power > powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.positiveEffectHexVal + ">(+" + (healthIncrease + GetRarityBonus(newRarity)[1]) + ")</color>";
                }
            }
            else if (lst[x].Equals("/status"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                lst[x + 1] = powerAmount[index] + "";

                int power = powerAmount[index];
                int mod = 0;
                switch (stats[index])
                {
                    case CharacterStats.Block:
                        if (power < blockIncrease)
                        {
                            power += 1;
                            mod = blockIncrease + GetRarityBonus(newRarity)[0];
                        }
                        break;
                    case CharacterStats.CritChance:
                        power += critIncrease;
                        mod = critIncrease + GetRarityBonus(newRarity)[2];
                        break;
                    case CharacterStats.Actions:
                        if (power < attNumIncrease)
                        {
                            mod = attNumIncrease + GetRarityBonus(newRarity)[3];
                            power += 1;
                        }
                        break;
                    case CharacterStats.Accuracy:
                        power += hitIncrease;
                        mod = hitIncrease + GetRarityBonus(newRarity)[4];
                        break;
                    case CharacterStats.Resistance:
                        mod = resistIncrease + GetRarityBonus(newRarity)[5];
                        power += resistIncrease;
                        break;
                }

                if (power < powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.negativeEffectHexVal + ">(+" + mod + ")</color>";
                }
                else if (power > powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.positiveEffectHexVal + ">(+" + mod + ")</color>";
                }
            }
            else if (lst[x].Equals("/multi"))
            {
                desc += " " + powerAmount.Count;
            }
            else if (lst[x].Equals("/turn"))
            {
                desc += " " + abilityDuration;
            }
            else
            {
                desc += " " + lst[x];
            }
        }


        return desc;
    }

    private int blockIncrease = 1;
    private int healthIncrease = 1;
    private int critIncrease = 1;
    private int attNumIncrease = 1;
    private int hitIncrease = 1;
    private int resistIncrease = 1;
    public void LevelUp()
    {
        for (int i = 0; i < stats.Count; i++)
        {
            switch (stats[i])
            {
                case CharacterStats.Block:
                    if (powerAmount[i] < blockIncrease)
                    {
                        powerAmount[i] += blockIncrease;
                    }
                    else canLevelup = false;
                    break;
                case CharacterStats.Health:
                case CharacterStats.MultiHit:
                case CharacterStats.HealthRegen:
                    if (powerAmount[i] < 0)
                    {
                        powerAmount[i] = Mathf.Clamp(powerAmount[i] + healthIncrease, -99, -1);
                    }
                    else
                    {
                        powerAmount[i] = Mathf.Clamp(powerAmount[i] + healthIncrease, 0, 99);
                    }
                    break;
                case CharacterStats.CritChance:
                    powerAmount[i] += critIncrease;
                    break;
                case CharacterStats.Actions:
                    if(powerAmount[i] < attNumIncrease)
                    {
                        powerAmount[i] += 1;
                    }
                    break;
                case CharacterStats.Accuracy:
                    powerAmount[i] += hitIncrease;
                    break;
                case CharacterStats.Resistance:
                    powerAmount[i] += resistIncrease;
                    break;
            }
        }
        masteryRank++;
        statusChance += (int)rarity;
        SetDesc();
    }

    public void SetRarity(Rarity r)
    {
        rarity = r;
        blockIncrease = Mathf.Clamp((int)rarity - 3, 1, 3);
        healthIncrease = Mathf.Clamp((int)rarity, 3, 5);
        critIncrease = Mathf.Clamp((int)rarity, 1, 6);
        attNumIncrease = Mathf.Clamp((int)rarity - 5, 1, 2);
        hitIncrease = Mathf.Clamp((int)rarity, 2, 6);
        resistIncrease = Mathf.Clamp((int)rarity, 3, 5);
    }

    private int[] GetRarityBonus(int rarity)
    {
        if (rarity <= (int)this.rarity) return new int[] { 0, 0, 0, 0, 0, 0 };
        return new int[] { 
            Mathf.Clamp((int)rarity - 3, 1, 3),
            Mathf.Clamp((int)rarity, 3, 5),
            Mathf.Clamp((int)rarity, 1, 6),
            Mathf.Clamp((int)rarity - 5, 1, 2),
            Mathf.Clamp((int)rarity, 2, 6),
            Mathf.Clamp((int)rarity, 3, 5)};
    }


    public Rarity GetRarity()
    {
        return rarity;
    }

    public Sprite GetSprite()
    {
        return abilitySprite;
    }

    public int GetDuration()
    {
        return abilityDuration;
    }

    public bool CanLevelUp()
    {
        if (masteryRank >= ((int)rarity * 2)) return false;
        bool blk = false, hp = false, cc = false, act = false, acc = false, res = false;
        for (int i = 0; i < stats.Count; i++)
        {
            switch (stats[i])
            {
                case CharacterStats.Block:
                    blk = blockIncrease > 0;
                    break;
                case CharacterStats.Health:
                case CharacterStats.MultiHit:
                case CharacterStats.HealthRegen:
                    hp = healthIncrease > 0;
                    break;
                case CharacterStats.CritChance:
                    cc = critIncrease > 0;
                    break;
                case CharacterStats.Actions:
                    act = attNumIncrease > 0;
                    break;
                case CharacterStats.Accuracy:
                    acc = hitIncrease > 0;
                    break;
                case CharacterStats.Resistance:
                    res = resistIncrease > 0;
                    break;
            }
        }
        return canLevelup && (blk || hp || cc || act || acc || res);
    }
    
    public void TriggerNoHit(Transform t, string msg)
    {
        specialSystem.TriggerNoHit(t, msg);
    }

    public void TriggerNoHit(Transform t, string msg, int font)
    {
        specialSystem.TriggerNoHit(t, msg, font);
    }

    public List<CharacterStats> GetStats()
    {
        return stats;
    }

    private int InflictModDmg(PlayerHero hero, int dmg)
    {
        if (hero.playerInventory.inflictLessDamageTypes.Contains(damageType.GetTypeName()))
        {
            return (int)(dmg * (float)(hero.playerInventory.weaponMod.GetConAmount() / 100f));
        }
        return 0;
    }

    private int TakeModDmg(PlayerHero hero, int dmg)
    {
        if (hero.playerInventory.takeExtraDamageTypes.Contains(damageType.GetTypeName()))
        {
            return (int)(dmg * (float)(hero.playerInventory.weaponMod.GetConAmount() / 100f));
        }
        return 0;
    }

    private int DealModDmg(PlayerHero hero, int dmg)
    {
        if (hero.playerInventory.halfHpDmg && hero.hp < (hero.GetMaxHealth() / 2))
        {
            return (int)(dmg * .30f);
        }
        return 0;
    }

    private int AilmentDealModDmg(PlayerHero hero, int dmg)
    {
        if (hero.playerInventory.dmgPerAilment && hero.playerInventory.ailments.Count > 0)
        {
            int mod = 10 * hero.playerInventory.ailments.Count;
            return (int)(dmg * (float)(mod / 100f));
        }
        return 0;
    }

    private int GoldDealModDmg(PlayerHero hero, int dmg)
    {
        int mod = hero.playerInventory.extraDmgPerGold * hero.playerInventory.GetGoldAmount();
        return -(mod / 100);
    }

    public string GetAudio()
    {
        switch (abilityType)
        {
            case AbilityTypes.Offensive:
                return damageType.audio;
            case AbilityTypes.Recovery:
                return "Heal";
            case AbilityTypes.Enchantment:
                return "Buff";
            case AbilityTypes.Affliction:
                return "Debuff";
            case AbilityTypes.Summon:
                return "Summon";
        }
        return "Summon";
    }

    public string GetMasteryRank()
    {
        if (!CanLevelUp())
        {
            return "Max";
        }
        else
        {
            return masteryRank + "";
        }
    }
    public int GetMasteryRankInt()
    {
        if (!CanLevelUp())
        {
            return 0;
        }
        else
        {
            return masteryRank;
        }
    }

    public string GetLevelupMasteryRank(int val)
    {
        if (!CanLevelUp())
        {
            return "Max";
        }
        else
        {
            return masteryRank + val + "";
        }
    }

    public void SetRank(int val)
    {
        masteryRank = val;
        powerAmount = new List<int>(powerAmountReadonly);
        SetDesc();
        for (int x = 1; x < val; x++)
        {
            LevelUp();
        }
    }

}