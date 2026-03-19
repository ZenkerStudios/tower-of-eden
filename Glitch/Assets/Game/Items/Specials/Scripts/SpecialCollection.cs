using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class SpecialCollection : MonoBehaviour
{
    public List<Special> allSpecials;
    public List<Attack> allAttacks;
    public List<Special> allSpecialsToUse;

    public Attack fighterAttacks;
    public List<Special> fighterSpecials;
    public Attack engineerAttacks;
    public List<Special> engineerSpecials;
    public Attack mageAttacks;
    public List<Special> mageSpecials;
    public Attack brawlerAttacks;
    public List<Special> brawlerSpecials;


    // Start is called before the first frame update
    void Start()
    {
        ResetCollection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetSpecialsByWeaponType(WeaponTypes weaponTypes)
    {
        ResetCollection();
        allAttacks = new List<Attack>();
        switch (weaponTypes)
        {
            case WeaponTypes.Melancolia:
                allSpecials = new List<Special>(fighterSpecials);
                allAttacks.Add(fighterAttacks); 
                break;
            case WeaponTypes.Delta:
                allSpecials = new List<Special>(engineerSpecials);
                allAttacks.Add(engineerAttacks);
                break;
            case WeaponTypes.Firestorm:
                allSpecials = new List<Special>(mageSpecials);
                allAttacks.Add(mageAttacks);
                break;
            case WeaponTypes.Renegade:
                allSpecials = new List<Special>(brawlerSpecials);
                allAttacks.Add(brawlerAttacks);
                break;

        }
    }
    public void ResetCollection()
    {
        allSpecialsToUse = new List<Special>(allSpecials);
    }

    public bool RemoveSpecialFromCollection(Special ability)
    {
        Special toRemove = null;
        bool res = false;
        foreach (Special a in allSpecialsToUse)
        {
            if (a.GetName().Equals(ability.GetName()))
            {
                toRemove = a;
                res = true;
            }
        }
        allSpecialsToUse.Remove(toRemove);
        return res;
    }

    public bool AddSpecialToCollection(Special ability)
    {
        foreach (Special a in allSpecials)
        {
            if (a.GetName().Equals(ability.GetName()))
            {
                allSpecialsToUse.Add(a);
                return true;
            }
        }
        return false;
    }
}
