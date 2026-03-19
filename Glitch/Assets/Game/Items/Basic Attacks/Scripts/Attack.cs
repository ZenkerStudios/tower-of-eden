using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;


[CreateAssetMenu(fileName = "newAttack", menuName = "Item/Attack", order = 2)]
public class Attack : ScriptableObject, IAbility
{
    public SpecialSystem specialSystem;
    public bool canLevelup;
    public Rarity rarity;
    public string attackDesc;
    private string attackDescReadOnly;
    public AbilityTypes abilityType;
    public TargetTypes targetType;
    public NumTarget numTarget;
    private NumTarget originalNumTarget;

    //How long ability lasts
    public int abilityDuration;
    public int statusChance;
    private int statusChanceGainOnTypeChange;

    public List<CharacterStats> stats;
    public List<StatModType> modType;
    public List<int> powerAmount;
    private List<int> powerAmountReadonly;

    //Index for damage types and status chance should be 1-1 pairing
    //The damage type that the effect deals
    public DamageTypeEnumValue damageType;
    private DamageTypeEnumValue originalDamageType;

    public Sprite abilitySprite;
    public int abilitySpriteIndex;

    public GameObject vfx;
    private int masteryRank = 1;
    private bool positiveEffect = true;

    public IEnumerator TriggerAbility(ICharacter owner, Transform ownerTransform, ICharacter target, Transform targetTransform, bool isCrit, CombatSystem cSystem)
    {

        int initialAmount;
        int finalAmount;
        int reduxAmount = 0;
        int attackDmg = 0;
        bool hit = true;
        for (int i = 0; i < stats.Count; i++)
        {
            finalAmount = powerAmount[i];
            if (abilityType == AbilityTypes.Offensive || abilityType == AbilityTypes.Affliction)
            {
                positiveEffect = false;
                reduxAmount = (int)target.GetToughness().Value;
                attackDmg = -(int)owner.GetStrength().Value;
            }

            if (!positiveEffect)
            {
                finalAmount *= -1;
            }
            finalAmount += attackDmg;
            
            switch (stats[i])
            {
                case CharacterStats.Health:
                case CharacterStats.MultiHit:
                    if (owner is PlayerHero h)
                    {
                        
                        finalAmount = finalAmount + DealModDmg(h, finalAmount);
                        finalAmount = finalAmount + AilmentDealModDmg(h, finalAmount);
                        finalAmount = finalAmount + GoldDealModDmg(h, finalAmount);
                        if (h.playerInventory.ailments.Exists(a => a.conIndex == 5) && h.hp < (h.GetMaxHealth() / 3))
                        {
                            finalAmount /= 2;
                        }
                        finalAmount = finalAmount - InflictModDmg(h, finalAmount);
                    }

                    bool physicalResist = target.HasResistance("Physical");
                    if (target is PlayerHero)
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

                    if (owner.GetAttackLifestrike().Value > 0 && !positiveEffect)
                    {
                        int healFor = (int)(owner.GetAttackLifestrike().Value * -finalAmount / 100);
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
            }

            yield return new WaitForSeconds(0.1f);

        }

        if ((abilityType == AbilityTypes.Affliction || abilityType == AbilityTypes.Offensive) && hit)
        {
            specialSystem.ApplyEffects(owner, target, this, targetTransform, true);
        }

        yield break;
    }

    private int GetCritDamage(int baseDmg)
    {
        return (int)(baseDmg * 1.5f);
    }

    public DamageTypeEnumValue GetDamageType()
    {
        return damageType;
    }

    public int GetStatusChance()
    {
        return statusChance + statusChanceGainOnTypeChange;
    }

    public GameObject GetVfx()
    {
        return vfx;
    }

    public string GetName()
    {
        return name.Replace("(Clone)", "");
    }

    private int healthIncrease = 1;
    public void LevelUp()
    {
        for (int i = 0; i < stats.Count; i++)
        {
            switch (stats[i])
            {
                case CharacterStats.Health:
                case CharacterStats.MultiHit:
                    powerAmount[i] += healthIncrease;
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
        healthIncrease = Mathf.Clamp((int)rarity, 3, 5);
    }

    public Rarity GetRarity()
    {
        return rarity;
    }

    public TargetTypes GetTargetTypes()
    {
        return targetType;
    }

    public NumTarget GetNumTarget()
    {
        return numTarget;
    }

    public AbilityTypes GetAbilityType()
    {
        return abilityType;
    }

    public void ChangeDamageType(DamageTypeEnumValue dt, int status)
    {
        statusChanceGainOnTypeChange = status;
        damageType = dt;
        SetDesc();
    }

    public void ResetDamageType()
    {
        statusChanceGainOnTypeChange = 0;
        damageType = originalDamageType;
        SetDesc();
    }

    public void ResetNumTarget()
    {
        numTarget = originalNumTarget;

    }

    public Sprite GetSprite()
    {
        return abilitySprite;
    }

    private void Awake()
    {
        originalNumTarget = numTarget;
        powerAmountReadonly = new List<int>(powerAmount);
        originalDamageType = damageType;
        SetRarity(rarity);
        SetDesc();
    }

    public void SetDesc()
    {
        string desc = "";
        string[] lst = attackDesc.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        for (int x = 0; x < lst.Length; x++)
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
            else if (lst[x].Equals("/turn"))
            {
                desc += " " + abilityDuration;
            }
            else if (lst[x].Equals("/multi"))
            {
                desc += " " + powerAmount.Count;
            }
            else
            {
                desc += " " + lst[x];
            }
        }
      
        attackDescReadOnly = desc + ".";

    }


    public string GetLevelUpDesc(int val)
    {
        string desc = "";
        string[] lst = attackDesc.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        for (int x = 0; x < lst.Length; x++)
        {

           if (lst[x].Equals("/dmg"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                lst[x + 1] = "<color=#" + damageType.textColor + ">" + powerAmount[index] + "</color> <sprite=" + damageType.spriteIndex + ">";

                int power = powerAmount[index] + healthIncrease;
                if (power < powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.negativeEffectHexVal + ">(+" + (healthIncrease * val) + ")</color>";
                } else if (power > powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.positiveEffectHexVal + ">(+" + (healthIncrease * val) + ")</color>";
                }
            }
            else if (lst[x].Equals("/heal"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                lst[x + 1] = "<color=#00FF00>" + powerAmount[index] + "</color>";

                int power = powerAmount[index] + healthIncrease;
                if (power < powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.negativeEffectHexVal + ">(+" + (healthIncrease * val) + ")</color>";
                }
                else if (power > powerAmount[index])
                {
                    lst[x + 1] += "<color=#" + specialSystem.positiveEffectHexVal + ">(+" + (healthIncrease * val) + ")</color>";
                }
            }
            else if (lst[x].Equals("/status"))
            {
                int index = int.Parse(lst[x + 1].Substring(lst[x + 1].Length - 1));
                int power = powerAmount[index];
                lst[x + 1] = power + " <sprite=" + (int)stats[index] + ">";
            }
            else if (lst[x].Equals("/turn"))
            {
                desc += " " + abilityDuration;
            }
            else if (lst[x].Equals("/multi"))
            {
                desc += "" + powerAmount.Count;
            }
            else
            {
                desc += " " + lst[x];
            }
        }

        return desc;
    }

    public string GetDesc()
    {
        return attackDescReadOnly;
    }

    public int GetDuration()
    {
        return abilityDuration;
    }

    public bool CanLevelUp()
    {
        if(masteryRank >= ((int)rarity * 2)) return false;
        return canLevelup;
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
            return (int) (dmg * (float)(hero.playerInventory.weaponMod.GetConAmount()/100f));
        }
        return 0;
    }

    private int TakeModDmg(PlayerHero hero, int dmg)
    {
        if (hero.playerInventory.takeExtraDamageTypes.Contains(damageType.GetTypeName()))
        {
            return (int) (dmg * (float)(hero.playerInventory.weaponMod.GetConAmount()/100f));
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
            return (int)(dmg * (float)(mod/ 100f));
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
        switch(abilityType)
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
        if(!CanLevelUp())
        {
            return "Max";
        } else
        {
            return masteryRank + "";
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
