//Interface for a playable character and npcs
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter 
{
    string GetName();

    int GetStatusChance(DamageTypeEnumValue dt);

    void SetStatusChance(DamageTypeEnumValue dt, int val);

    //Fozen
    int GetFrozenResistance();

    void SetIsFrozen(int turn);

    int GetIsFrozen();

    void SetFrozenTurns(int turn);

    int GetFrozenTurns();

    //Burning
    int GetBurningResistance();

    void SetIsBurning(int[] burning);

    int[] GetIsBurning();

    void SetBurningInfo(int[] burning);

    int[] GetBurningInfo();

    //Vulnerable
    int GetVulnerableResistance();

    void SetIsVulnerable(float[] vulnerable);

    float[] GetIsVulnerable();

    void SetVulnerableInfo(float[] vulnerable);

    float[] GetVulnerableInfo();

    //Poisoned
    int GetPoisonResistance();

    void SetIsPoisoned(int turn);

    int GetIsPoisoned();

    void SetPoisonedTurns(int turn);

    int GetPoisonedTurns();

    //Shocked
    int GetShockedesistance();

    void SetIsShocked(int[] shocked);

    int[] GetIsShocked();

    void SetShockedInfo(int[] shocked);

    int[] GetShockedInfo();

    //Smite
    void SetDamnation(int[] smite);

    int[] GetDamnation();

    void SetDamnationInfo(int[] smite);

    int[] GetDamnationInfo();


    //Resistances
    List<DamageTypeEnumValue> GetResistances();

    bool HasResistance(string dt);

    void AddBattleResistance(DamageTypeEnumValue dt, int dur, int amount, object src);

    //Conditions
    bool CheckConditions();

    void AddCondition(string condName);

    bool RemoveCondition(string st);

    List<string> GetConditions();

    void CreateCondToken(DamageTypeEnumValue dt);

    //Stats
    StatSystem GetCritChance();

    StatSystem GetActions();

    StatSystem GetAttackLifestrike();

    StatSystem GetSpecialLifeSteal();

    StatSystem GetToughness();

    StatSystem GetAccuracy();

    StatSystem GetDominance();

    StatSystem GetStrength();

    //Health
    StatSystem GetHealthStats();

    void ChangeHealth(int hp);

    void IncreaseMaxHealth(int val);

    //Health Regen
    int[] GetHealthRegen();

    void SetHealthRegen(int[] healthRegen);

    //Block
    void SetBlock(int val);

    int GetBlock();

    void ResetAffinities();

    void Cleanse();

    void CreateOtherCond(string condName, string desc, string sp);

    void PlayIceFX();

    void ResetConditions();

    InteractableNpc GetNpc();

}





    
