using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemySytem", menuName = "Enemies/EnemySystem", order = 1)]
public class EnemySystem : ScriptableObject
{
    public List<EnemyDetails> details;
    public static List<DamageTypeEnumValue> allResistance;

    public void InitializeEnemy(EnemyNpc en)
    {
        if (en == null) return;

        SetAllAbilities(en);

        SetResistance(en);
    }

    public void SetResistance(EnemyNpc en)
    {
        var enType = details.Find(e => e.enemyType == en._myType);
        if (enType == null) return;

        foreach (DamageTypeEnumValue d in enType.resistanceAgainst)
        {
            if (!en.resistances.Contains(d))
            {
                en.resistances.Add(d);
            }
        }
    }

    public void SetAllAbilities(EnemyNpc en)
    {
        en.allAbilites = new List<Special>();
        en.allAbilites.AddRange(en.bossSpecials);
        en.allAbilites.AddRange(en.attacks);
        en.allAbilites.Add(en.specialAbility);
        en.allAbilites.Add(en.deathAbility);
        en.allAbilites.RemoveAll(item => item == null);
        en.allAbilites = en.allAbilites.Distinct().ToList();
        en.allAbilites = en.allAbilites.OrderBy(s => s.GetName()).ToList();
    }

}


[Serializable]
public class EnemyDetails
{
    public EnemyEnumValue enemyType;
    public List<DamageTypeEnumValue> resistanceAgainst;


  

}