using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[Serializable]
public class PlayerInventory
{

    public int blockPerTurn, artifactBlockPerTurn, ailmentArtifactRedux, healPerTurn, attacksUsedThisEncounter, specialsUsedThisEncounter;
    public bool alwaysShowUniqueRoom, healInUniqueRoom, rerollCrit, enemyDazedStart, bonusSlotXp;
    public Floors noGoldFloor = Floors.Start;


    protected int goldOwned;

    public List<Artifact> artifacts = new List<Artifact>();
    public List<Consumable> combatShopConsumables = new List<Consumable>();

    
    private int ailmentBonus, hazardBonus, rarityBonus;
    
    public int iceStatus;
    public int fireStatus;
    public int poisonStatus;
    public int psychicStatus;
    public int lightningStatus;

    public WeaponMod weaponMod;

    //Pro mod
    public bool halfHealthEnc, halfGoldEnc, halfHpDmg, dmgPerAilment, critOnFrozen,
        critOnPoison, hasResistance, attackTarget, attackPoisonType, attackPsychicType;
    public int iceModResist, fireModResist, poisonModResist, psychicModResist, lightningModResist,
        blockPerEnc, extraDmgPerGold, attackDazeChance, burnSpread, shockSpread,
        poisonSpread, vulnerableConfusion, healOnCrit, resistanceIndex;


    //Con mod
    public bool ailmentDiffMod, lowerVulnerableMult, lowerAttackTarget, lowerDivineStack;
    public int ailmentChanceMod;
    public List<string> takeExtraDamageTypes = new List<string>();
    public List<string> inflictLessDamageTypes = new List<string>();
    public List<Ailment> ailments = new List<Ailment>();
    public int iceModStatus, fireModStatus, poisonModStatus, psychicModStatus, lightningModStatus, attackSelfDmg;

    public PlayerInventory()
    {
        
    }

    public void AddArtifact(Artifact a)
    {
        a.EquipArtifact();
        artifacts.Add(a);
    }

    public void RemoveArtifact(Artifact a)
    {
        if(artifacts.Remove(a))
        {
            a.thisArtifact.DisablePassiveStat();
        }
    }

    int maxAilmentCount = 3;

    public bool AddAilment(Ailment a)
    {
        if(ailments.Count < maxAilmentCount)
        {
            ailments.Add(a);
            return true;
        }
        return false;
    }

    public void RemoveAilment()
    {
        if(ailments.Count > 0)
        {
            Ailment thisAilment = ailments[0];
            if (thisAilment.conIndex == 6 && thisAilment.artifactToDisable != null)
            {
                thisAilment.artifactToDisable.thisArtifact.EnableArtifact();
               
            }

            ailments.RemoveAt(0);
        }
    }

    public Artifact GetRandomArtifact()
    {
        if(artifacts.Count > 0)
        {
            return artifacts[UnityEngine.Random.Range(0, artifacts.Count)];
        } else
        {
            return null;
        }
    }

    public void SetGoldAmount(int gold)
    {
        goldOwned = Mathf.Clamp(gold, 0, 999999);
    }

    public void ChangeGoldAmount(int gold)
    {
        goldOwned = Mathf.Clamp(goldOwned + gold, 0, 9999999);
    }

    public int GetGoldAmount()
    {
        return goldOwned;
    }

    public int GetAilmentBonus() { return -ailmentBonus; }
    public int GetHazardBonus() { return -hazardBonus; }
    public int GetRarityBonus() { return rarityBonus; }

    public void SetAilmentBonus(int val) { ailmentBonus = val; }
    public void SetHazardBonus(int val) { hazardBonus = val; }
    public void SetRarityBonus(int val) { rarityBonus = val; }

    public void ActivateLateWeaponMod()
    {
        if (hasResistance)
        {
            weaponMod.player.h.permanentResistances.Add(weaponMod.player.h.baseAttack.specialSystem.allDamageTypes[resistanceIndex]);
        }

        if (lowerAttackTarget)
        {
            weaponMod.player.h.baseAttack.numTarget = NumTarget.One;
        }
        if (attackTarget)
        {
            weaponMod.player.h.baseAttack.numTarget = NumTarget.Two;
        }

        if (attackPoisonType)
        {
            weaponMod.player.h.baseAttack.ChangeDamageType(weaponMod.player.h.baseAttack.specialSystem.allDamageTypes[5], 15);
            weaponMod.player.h.baseAttack.vfx = weaponMod.player.h.baseAttack.specialSystem.allDamageTypes[5].vfx;
        }

        if (attackPsychicType)
        {
            weaponMod.player.h.baseAttack.ChangeDamageType(weaponMod.player.h.baseAttack.specialSystem.allDamageTypes[6], 15);
            weaponMod.player.h.baseAttack.vfx = weaponMod.player.h.baseAttack.specialSystem.allDamageTypes[6].vfx;
        }

        if (lowerVulnerableMult)
        {
            float[] vInfo = weaponMod.player.h.GetVulnerableInfo();
            weaponMod.player.h.SetVulnerableInfo(new float[] { vInfo[0], 1.25f });
        }

        if (lowerDivineStack)
        {
            int[] dInfo = weaponMod.player.h.GetDamnationInfo();
            weaponMod.player.h.SetDamnationInfo(new int[] { dInfo[0], dInfo[1], dInfo[2], 5});
        }



    }

}
