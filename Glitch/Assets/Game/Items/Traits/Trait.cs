using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Trait
{
    public Traits trait;
    public int powerAmount;

    private StatModType modType = StatModType.Flat;
    private static string[] traitDesc = new string[]
    {
        "Volatile: Can self destruct.",
        "Pacifist: Never attacks.",
        "Undying: Can revive itself",
        "Devourer: Ragna can consume her cocoons to heal.",
        "Amalgamation: Acrid gains Dominance for every resistance Type enemies and allies have.",
        "Nexus Arm: Dorian's Nexus Arm allows him to switch fighting styles at will.",
        "Apotheosis: Lyra sings her Final Verse after 3 Perfect Pitch.",
        "Kingly: Damage against Eden is reduced to a fixed amount every hit.",


        "Relentless: Has an extra Action <sprite=48>.",
        "Unruly: Higher base Critical Chance <sprite=47>.",
        "Vigilant: Higher base Accuracy <sprite=46>.",
        "Tenacious: Has Physical <sprite=43> resistance.",
        "Pious: Has Divine <sprite=39> resistance.",
        "Stoic: Has Lightning <sprite=41> resistance.",
        "Zealous: Has Fire <sprite=37> resistance.",
        "Aberrant: Has Poison <sprite=38> resistance.",
        "Intrepid: Has Psychic <sprite=40> resistance.",
        "Adept: Has Ice <sprite=42> resistance.",
        "Mender: Higher base Lifesteal <sprite=55>.",
        "Indomitable: Gain bonus Dominance <sprite=54>for their next attack after taking damage.",
        "Sheltered: Always has Block <sprite=44> at the start of combat.",
        "Safeguard: Gain bonus Block <sprite=44> after taking damage.",
    };

    public Trait(Traits t)
    {
        trait = t;
        switch (trait)
        {
            case Traits.Unruly:
            case Traits.Vigilant:
                //Crit, Accuracy, 
                powerAmount = 15;
                break;
            case Traits.Mender:
                //Healing
                powerAmount = 25;
                break;
            case Traits.Stoic:
            case Traits.Zealous:
            case Traits.Aberrant:
            case Traits.Intrepid:
            case Traits.Adept:
                //Resistance
                powerAmount = 10;
                break;
            case Traits.Indomitable:
                powerAmount = 3;
                break;
            case Traits.Relentless:
            case Traits.Sheltered:
            case Traits.Safeguard:
                //Extra action, Block at start, Block on dmg
                powerAmount = 1;
                break;
        }
    }

    public Trait(Traits t, int amount)
    {
        trait = t;
        powerAmount = amount;
    }


    public void ApplyPassive(EnemyToken owner, SpecialSystem specialSystem)
    {
        int finalAmount = powerAmount;
        switch (trait)
        {
            case Traits.Volatile:
                break;
            case Traits.Pacifist:
                break;
            case Traits.Undying:
                break;
            case Traits.Apotheosis:
                break;


            case Traits.Relentless:
                owner.GetActions().AddModifier(new StatModifier(finalAmount, modType, this));
                break;
            case Traits.Unruly:
                owner.GetCritChance().AddModifier(new StatModifier(finalAmount, modType, this));
                break;
            case Traits.Vigilant:
                owner.GetAccuracy().AddModifier(new StatModifier(finalAmount, modType, this));
                break;
            case Traits.Tenacious:
                owner.enemyNpc.resistances.Add(specialSystem.allDamageTypes[4]);
                break;
            case Traits.Pious:
                owner.enemyNpc.resistances.Add(specialSystem.allDamageTypes[0]);
                break;
            case Traits.Stoic:
                owner.enemyNpc.resistances.Add(specialSystem.allDamageTypes[3]);
                owner.traitLightningResist = Mathf.Clamp(finalAmount, 0, 100);
                break;
            case Traits.Zealous:
                owner.enemyNpc.resistances.Add(specialSystem.allDamageTypes[1]);
                owner.traitFireResist = Mathf.Clamp(finalAmount, 0, 100);
                break;
            case Traits.Aberrant:
                owner.enemyNpc.resistances.Add(specialSystem.allDamageTypes[5]);
                owner.traitPoisonResist = Mathf.Clamp(finalAmount, 0, 100);
                break;
            case Traits.Intrepid:
                owner.enemyNpc.resistances.Add(specialSystem.allDamageTypes[6]);
                owner.traitPsychicResist = Mathf.Clamp(finalAmount, 0, 100);
                break;
            case Traits.Adept:
                owner.enemyNpc.resistances.Add(specialSystem.allDamageTypes[2]);
                owner.traitIceResist = Mathf.Clamp(finalAmount, 0, 100);
                break;
            case Traits.Mender:
                owner.GetSpecialLifeSteal().AddModifier(new StatModifier(finalAmount, modType, this));
                break;
            case Traits.Indomitable:
                owner.buffAfterDamage = this;
                break;
            case Traits.Sheltered:
                if (owner.GetBlock() < finalAmount)
                {
                    owner.SetBlock(finalAmount);
                }
                break;
            case Traits.Safeguard:
                owner.blockAfterDamage = this;
                break;
        }
    }

    public void RemovePassive(EnemyToken owner, SpecialSystem specialSystem)
    {
        owner.GetAccuracy().RemoveAllModFromSource(this);
        owner.GetCritChance().RemoveAllModFromSource(this);
        owner.GetActions().RemoveAllModFromSource(this);
        owner.GetSpecialLifeSteal().RemoveAllModFromSource(this);
        owner.GetDominance().RemoveAllModFromSource(this);
        int finalAmount = powerAmount;

        switch (trait)
        {
            case Traits.Volatile:
                break;
            case Traits.Pacifist:
                break;
            case Traits.Undying:
                break;
            case Traits.Apotheosis:
                break;

            case Traits.Tenacious:
                owner.enemyNpc.resistances.Remove(specialSystem.allDamageTypes[4]);
                break;
            case Traits.Pious:
                owner.enemyNpc.resistances.Remove(specialSystem.allDamageTypes[0]);
                break;
            case Traits.Stoic:
                owner.enemyNpc.resistances.Remove(specialSystem.allDamageTypes[3]);
                owner.traitLightningResist = 0;
                break;
            case Traits.Zealous:
                owner.enemyNpc.resistances.Remove(specialSystem.allDamageTypes[1]);
                owner.traitFireResist = 0;
                break;
            case Traits.Aberrant:
                owner.enemyNpc.resistances.Remove(specialSystem.allDamageTypes[5]);
                owner.traitPoisonResist = 0;
                break;
            case Traits.Intrepid:
                owner.enemyNpc.resistances.Remove(specialSystem.allDamageTypes[6]);
                owner.traitPsychicResist = 0;
                break;
            case Traits.Adept:
                owner.enemyNpc.resistances.Remove(specialSystem.allDamageTypes[2]);
                owner.traitIceResist = 0;
                break;
            case Traits.Indomitable:
                owner.buffAfterDamage = null;
                owner.buffDamageNumber = 0;
                break;
            case Traits.Sheltered:
                owner.SetBlock(0);
                break;
            case Traits.Safeguard:
                owner.blockAfterDamage = null;
                break;
        }
    }

    public string GetTextSprite()
    {
        int index = 0 + (int)trait;
        return "<sprite=" + index + "> ";
    }

    public string GetName()
    {
        return trait.ToString();
    }

    public string GetTraitDesc()
    {
        return traitDesc[(int)trait];
    }
}

