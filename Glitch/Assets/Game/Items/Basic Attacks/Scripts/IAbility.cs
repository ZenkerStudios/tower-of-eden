using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    string GetName();

    string GetDesc();

    string GetLevelUpDesc(int val);

    Rarity GetRarity();

    int GetDuration();

    Sprite GetSprite();

    void SetRarity(Rarity r);

    IEnumerator TriggerAbility(ICharacter owner, Transform ownerTransform, ICharacter target, Transform targetTransform, bool isCrit, CombatSystem cSystem);

    TargetTypes GetTargetTypes();

    AbilityTypes GetAbilityType();

    NumTarget GetNumTarget();

    List<CharacterStats> GetStats();

    DamageTypeEnumValue GetDamageType();

    int GetStatusChance();

    GameObject GetVfx();

    void LevelUp();

    void TriggerNoHit(Transform t, string msg);

    void TriggerNoHit(Transform t, string msg, int fontSize);

    bool CanLevelUp();

    string GetAudio();

    string GetMasteryRank(); 

    string GetLevelupMasteryRank(int val);

    void SetRank(int val);
}
