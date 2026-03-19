using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using UnityEngine;


[CreateAssetMenu(fileName = "newArtifact", menuName = "Item/Artifact", order = 3)]
public class Artifact : ScriptableObject
{
    public string artifactName;
    [TextArea(5, 10)]
    public string artifactDesc;
    public Rarity rarity;

    public int artifactIndex;

    public DamageTypeEnumValue damageType;

    public Sprite artifactSprite;

    public ArtifactItem thisArtifact;

    public Sprite GetSprite()
    {
        return artifactSprite;
    }
    public int GetSpriteIndex()
    {
        return artifactIndex-1;
    }

    public string GetDesc()
    {
        return artifactDesc;
    }

    public string GetName()
    {
        if(artifactName == null || artifactName.Equals(""))
        {
            return name.Replace("(Clone)", "");
        }
        return artifactName;
    }

    public void EquipArtifact()
    {
        thisArtifact = new ArtifactItem(artifactIndex, this);
    }
}


[Serializable]
public class ArtifactItem
{
    private Artifact artifact;
    private int artifactIndex;
    private PlayerManager player;
    private CombatSystem cs;
    public bool isDisabled;
    private int amount;
    private int fakeTurnAmount = 99999;

    public int encounterCount, takedownCount, abilityCount;
    private Floors noGoldFloor;

    public ArtifactItem(int index, Artifact art)
    {
        artifact = art;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
       
        artifactIndex = index;
        switch (artifactIndex)
        {

            /* Common

            1. Increase max HP <sprite=45> by 50
            2. Upon pick up, Attacks deal Fire <sprite=37> Damage
            3. Upon pick up, Attacks deal Lightning <sprite=41> Damage
            4. Upon pick up, Attacks Deal Poison <sprite=38> Damage
            5. Increase Strength <sprite=51> by 3
            6. Increase Critical Chance <sprite=47> by 15
            7. Gain 5 Fire <sprite=37> Status Chance
            8. Gain 5 Ice <sprite=42> Status Chance
            9. Gain 5 Lightning <sprite=41> Status Chance
            10. Gain 5 Psychic <sprite=40> Status Chance
            11. Gain 5 Poison <sprite=38> Status Chance 
            12. Heal an additional 10 HP <sprite=45> per room
            13. Increase Fire <sprite=37> Resist by 20
            14. Increase Lightning <sprite=41> Resist by 20
            15. Increase Poison <sprite=38> Resist by 20
            16. Increase Psychic <sprite=40> Resist by 20
            17. Upon pick up, increase mastery of a random Ability
            18. Upon pick up, gain 3 random Consumable
            */
            case 1:
                amount = 50;
                player.h.GetHealthStats().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                player.h.ChangeHealth(amount);
                break;
            case 2:
                player.h.baseAttack.ChangeDamageType(artifact.damageType, 15);
                player.h.baseAttack.vfx = artifact.damageType.vfx;
                break;
            case 3:
                player.h.baseAttack.ChangeDamageType(artifact.damageType, 15);
                player.h.baseAttack.vfx = artifact.damageType.vfx;
                break;
            case 4:
                player.h.baseAttack.ChangeDamageType(artifact.damageType, 15);
                player.h.baseAttack.vfx = artifact.damageType.vfx;
                break;
            case 5:
                amount = 3;
                player.h.GetStrength().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 6:
                amount = 15;
                player.h.GetCritChance().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 7:
                amount = player.h.fireStatus;
                player.h.fireStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 8:
                amount = player.h.iceStatus;
                player.h.iceStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 9:
                amount = player.h.lightningStatus;
                player.h.lightningStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 10:
                amount = player.h.psychicStatus;
                player.h.psychicStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 11:
                amount = player.h.poisonStatus;
                player.h.poisonStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 12:
                amount = player.healthOnExit;
                player.healthOnExit = amount + 10;
                break;
            case 13:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.fireResist;
                player.h.fireResist = Mathf.Clamp(amount + 20, 0, 100);
                break;
            case 14:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.lightningResist;
                player.h.lightningResist = Mathf.Clamp(amount + 20, 0, 100);
                break;
            case 15:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.poisonResist;
                player.h.poisonResist = Mathf.Clamp(amount + 20, 0, 100);
                break;
            case 16:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.psychicResist;
                player.h.psychicResist = Mathf.Clamp(amount + 20, 0, 100);
                break;
            case 17:
                List<IAbility> abilitiesOwned = new List<IAbility>();
                abilitiesOwned.Add(player.h.baseAttack);
                abilitiesOwned.AddRange(player.h.specials);
                abilitiesOwned = abilitiesOwned.OrderBy(a => System.Guid.NewGuid()).ToList();
                if (abilitiesOwned.Count > 0)
                {
                    IAbility a = abilitiesOwned[0];
                    a.LevelUp();
                    List<System.Action> popups = new List<System.Action>();
                    popups.Add(() => a.TriggerNoHit(player.popupWindow.transform, a.GetName() + ": +1 Rank!", 12));
                    GameUtilities.ShowLevelUpPopup(player, popups);
                }
                else
                {
                    player.h.playerInventory.ChangeGoldAmount(30);
                }
                break;
            case 18:
                Consumable.GiveRandomCombatConsumable(3, player);
                break;

            //recurring(need to check if disabled before trigger)
            //per battle/on enemy(trigger every beginning of battle)
            //on pick up(just one trigger and thats it)
            //update(amount needs to be updated and triggered again)

            /* Uncommon
             
            19. Gain Physical <sprite=43> Resist
            20. Gain Divine <sprite=39> Resist
            21. Increase Ice <sprite=42> Resist by 10
            22. Increase Dominance <sprite=54> by 3
            23. Upon pick up, gain 200 Gold <sprite=11> 
            24. Gain 10 Dominance <sprite=54> during next encounter if no Attack is used this encounter
            25. Attack has increased target
            26. Increase Accuracy <sprite=46> by 15
            27. Gain 5 max HP <sprite=45> every 5 Skill used during an encounter
            28. Gain an additional 15 Gold <sprite=11> per room
            29. Increase Lifesteal <sprite=55> by 15
            30. Increase Lifestrike <sprite=50> by 20
            31. A Unique room will show whenever possible
            32. Heal 10 HP <sprite=45> whenever you enter an Unique room
            33. Upon pick up, regain all HP <sprite=45> 
            34. Reduce all enemy max HP <sprite=45> by 10
            35. Gain a random Consumable every 5 encounter
            36. Increase the maximum number for equipped Skills by 2
            37. Reduce all enemy Lifesteal <sprite=55> by 5
            38. Reduce all enemy Critical Chance <sprite=47> by 5
            39. Reduce all enemy Toughness <sprite=49> by 3
            40. Reduce all enemy Accuracy <sprite=46> by 5
            41. Gain 1 Critical Chance <sprite=47> every 3 encounter 
            42. Gain 1 Lifesteal <sprite=55> every 3 encounter 
            */

            case 19:
                player.h.permanentResistances.Add(artifact.damageType);
                break;
            case 20:
                player.h.permanentResistances.Add(artifact.damageType);
                break;
            case 21:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.iceResist;
                player.h.iceResist = Mathf.Clamp(amount + 10, 0, 100);
                break;
            case 22:
                amount = 3;
                player.h.GetDominance().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 23:
                player.h.playerInventory.ChangeGoldAmount(200);
                break;
            case 24: //Check to trigger every beginning of encounter
                amount = 10;
                break;
            case 25:
                amount = (int)player.h.baseAttack.GetNumTarget();
                player.h.baseAttack.numTarget = (NumTarget)Mathf.Clamp(amount + 1, 1, 4);
                break;
            case 26:
                amount = 15;
                player.h.GetAccuracy().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 27: //Check to trigger every end of turn
                amount = 0;
                break;
            case 28:
                amount = player.goldOnExit;
                player.goldOnExit = amount + 15;
                break;
            case 29:
                amount = 15;
                player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 30:
                amount = 20;
                player.h.GetAttackLifestrike().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 31:
                player.h.playerInventory.alwaysShowUniqueRoom = true;
                break;
            case 32:
                player.h.playerInventory.healInUniqueRoom = true;
                break;
            case 33:
                player.h.ChangeHealth(999999);
                break;
            case 34: //Check to trigger every beginning of encounter
                amount = 10;
                break;
            case 35: //Check to trigger every end of encounter
                break;
            case 36:
                amount = 10;
                player.h.SetMaxAbilityCount(10);
                break;
            case 37: //Check to trigger every beginning of encounter
                amount = 5;
                break;
            case 38: //Check to trigger every beginning of encounter
                amount = 5;
                break;
            case 39: //Check to trigger every beginning of encounter
                amount = 3;
                break;
            case 40: //Check to trigger every beginning of encounter
                amount = 5;
                break;
            case 41: //Check to trigger every end of encounter
                amount = 0;
                break;
            case 42: //Check to trigger every end of encounter
                amount = 0;
                break;



            /* Rare
             
            43. Increase mastery rank of a random Ability after every 5 encounters
            44. Gain 1 Block <sprite=44> every turn
            45. Gain 1 Dominance <sprite=54> for every 5 takedowns
            46. Gain 1 Strength <sprite=51> every 5 takedowns
            47. Gain 20 Fire <sprite=37> Resist for the first 3 turns of encounter
            48. Gain 20 Ice <sprite=42> Resist for the first 3 turns of encounter
            49. Gain 20 Lightning <sprite=41> Resist for the first 3 turns of encounter
            50. Gain 20 Poison <sprite=38> Resist for the first 3 turns of encounter
            51. Gain 20 Psychic <sprite=40> Resist for the first 3 turns of encounter
            52. Gain 20 Fire <sprite=37> Status Chance for the first 3 turns of encounter
            53. Gain 20 Ice <sprite=42> Status Chance for the first 3 turns of encounter
            54. Gain 20 Lightning <sprite=41> Status Chance for the first 3 turns of encounter
            55. Gain 20 Poison <sprite=39> Status Chance for the first 3 turns of encounter
            56. Gain 20 Psychic <sprite=40> Status Chance for the first 3 turns of encounter
            57. Gain 1 Reroll <sprite=24> per critical hit on your next turn
            58. Reduce chance for getting Ailment <sprite=65> 
            59. Heal 10 HP <sprite=45> on first Attack of encounter
            60. Gain 1 Accuracy <sprite=46> every 5 Attack during an encounter
            61. Gain 1 Lifestrike <sprite=50> every 5 Attack during an encounter
            62. Gain 1 Toughness <sprite=49> every 8 encounter 
            */
            case 43: //Check to trigger every end of encounter
                break;
            case 44:
                amount = 1;
                if (player.h.playerInventory.artifactBlockPerTurn < amount)
                {
                    player.h.playerInventory.artifactBlockPerTurn = amount;
                }
                break;
            case 45: //Check to trigger every beginning of turn 
                break;
            case 46: //Check to trigger every beginning of turn 
                break;
            case 47: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 48: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 49: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 50: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 51: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 52: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 53: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 54: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 55: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 56: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 57:
                player.h.playerInventory.rerollCrit = true;
                break;
            case 58:
                amount = 5;
                player.h.playerInventory.ailmentArtifactRedux = amount;
                break;
            case 59: //Check to trigger every end of turn
                break;
            case 60: //Check to trigger every end of turn
                amount = 0;
                break;
            case 61: //Check to trigger every end of turn
                amount = 0;
                break;
            case 62: //Check to trigger every end of encounter
                break;



            /* Legendary

            63. Upon pick up, remove Ailment
            64. Gain 2 Block <sprite=44> at the beginning of every encounter
            65. Gain 1 Action <sprite=48> 
            66. Gain 10 Strength <sprite=51> on First Attack of encounter
            67. Upon pick up, gain 1 Revive <sprite=32> 
            68. Gain 1 Reroll <sprite=24> 
            69. Heal 5 HP <sprite=45> every turn 
            70. The first time you lose HP <sprite=45> during an encounter, gain 10 Lifesteal <sprite=55> and 10 Lifestrike <sprite=50> for remainder of encounter
            71. Gain 15 Accuracy <sprite=46> for the first 3 turns of encounter
            72. Gain 25 Critical Chance <sprite=47> for the first 3 turns of encounter
            73. Gain 20 Lifestrike <sprite=50> for the first 3 turns of encounter
            74. Gain 30 Lifesteal <sprite=55> for the first 3 turns of encounter
            75. Gain 8 Strength <sprite=51> for the first 3 turns of encounter
            76. Gain 5 Dominance <sprite=54> for the first 5 turns of encounter
            77. Gain 1 Action <sprite=48> for the first 3 turns of encounter
            */
            case 63:
                player.h.playerInventory.RemoveAilment();
                player.RenderAilment();
                break;
            case 64:  //Check to trigger every beginning of encounter
                break;
            case 65:
                amount = 1;
                player.h.GetActions().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 66://Check to trigger every beginning of encounter end of turn
                break;
            case 67:
                player.numArtifactRevive = 1;
                player.RenderRevives();
                break;
            case 68:
                player.numArtifactReroll = 1;
                break;
            case 69:
                amount = 5;
                player.h.playerInventory.healPerTurn = amount;
                break;
            case 70: //Check to trigger every beginning of turn
                break;
            case 71: //Check to trigger every beginning of encounter
                amount = 15;
                break;
            case 72: //Check to trigger every beginning of encounter
                amount = 25;
                break;
            case 73: //Check to trigger every beginning of encounter
                amount = 20;
                break;
            case 74: //Check to trigger every beginning of encounter
                amount = 30;
                break;
            case 75: //Check to trigger every beginning of encounter
                amount = 8;
                break;
            case 76: //Check to trigger every beginning of encounter
                amount = 5;
                break;
            case 77: //Check to trigger every beginning of encounter
                amount = 1;
                break;


            /* Exalted
            (artifacts are corrupted. Only 1 per run)

            78. Upon pick up, Attack and Skills are of Legendary rarity. On the 5th turn of combat, all remaining enemies heal to full HP <sprite=45> and gain bonus Dominance <sprite=54> 
            79. All enemies start combat Dazed <sprite=28>. Upon pick up, all Abilities reset to mastery rank 1.
            80. Start every encounter with max HP <sprite=45> and 2 Revive <sprite=32>. Upon pick up, lose all Revive <sprite=32> 
            81. Upon pick up, gain 5 legendary Artifacts. Upon pick up, lose all other artifacts
            82. Increase Toughness <sprite=49> by 15. Cannot gain Gold <sprite=11> reward for remainder of floor
             */
            case 78: //Check to trigger every beginning of turn
                player.h.baseAttack.SetRarity(Rarity.Legendary);
                foreach (Special s in player.h.specials)
                {
                    s.SetRarity(Rarity.Legendary);
                }
                break;
            case 79:
                player.h.playerInventory.enemyDazedStart = true;
                player.h.baseAttack.SetRank(1);
                foreach (Special s in player.h.specials)
                {
                    s.SetRank(1);
                }
                break;
            case 80:
                player.numRevives = 2;
                player.numArtifactRevive = 0;
                player.RenderRevives();
                break;
            case 81:
                List<Artifact> tempArtifactList = new List<Artifact>(player.h.playerInventory.artifacts);
                foreach (Artifact a in tempArtifactList)
                {
                    a.thisArtifact.DisablePassiveStat();
                }
                player.h.playerInventory.artifacts = new List<Artifact>();
                GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().ResetArtifacts();
                List<Artifact> artifactToSelect =
                    GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().tier4ArtifactsReadonly.OrderBy(a => System.Guid.NewGuid()).ToList();
                for (int x = 0; x < 5; x++)
                {
                    player.AddArtifact(artifactToSelect[x]);
                    GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().tier4Artifacts.Remove(artifactToSelect[x]);
                }
                break;
            case 82:
                amount = 10;
                player.h.GetToughness().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                noGoldFloor = GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode;
                player.h.playerInventory.noGoldFloor = noGoldFloor;
                break;
        }
    }

    public void EnableArtifact()
    {
        isDisabled = false;
        switch (artifactIndex)
        {

            /* Common */
            case 1:
                amount = 50;
                player.h.GetHealthStats().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                cs.UpdateHeroBattleStats();
                break;
            case 5:
                amount = 3;
                player.h.GetStrength().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 6:
                amount = 15;
                player.h.GetCritChance().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 7:
                amount = player.h.fireStatus;
                player.h.fireStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 8:
                amount = player.h.iceStatus;
                player.h.iceStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 9:
                amount = player.h.lightningStatus;
                player.h.lightningStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 10:
                amount = player.h.psychicStatus;
                player.h.psychicStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 11:
                amount = player.h.poisonStatus;
                player.h.poisonStatus = Mathf.Clamp(amount + 5, 0, 100);
                break;
            case 12:
                amount = player.healthOnExit;
                player.healthOnExit = amount + 10;
                break;
            case 13:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.fireResist;
                player.h.fireResist = Mathf.Clamp(amount + 20, 0, 100);
                break;
            case 14:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.lightningResist;
                player.h.lightningResist = Mathf.Clamp(amount + 20, 0, 100);
                break;
            case 15:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.poisonResist;
                player.h.poisonResist = Mathf.Clamp(amount + 20, 0, 100);
                break;
            case 16:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.psychicResist;
                player.h.psychicResist = Mathf.Clamp(amount + 20, 0, 100);
                break;

            /* Uncommon */
            case 19:
                player.h.permanentResistances.Add(artifact.damageType);
                break;
            case 20:
                player.h.permanentResistances.Add(artifact.damageType);
                break;
            case 21:
                player.h.permanentResistances.Add(artifact.damageType);
                amount = player.h.iceResist;
                player.h.iceResist = Mathf.Clamp(amount + 10, 0, 100);
                break;
            case 22:
                amount = 3;
                player.h.GetDominance().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 23:
                amount = 1;
                if (player.h.playerInventory.artifactBlockPerTurn < amount)
                {
                    player.h.playerInventory.artifactBlockPerTurn = amount;
                }
                break;
            case 25:
                amount = (int)player.h.baseAttack.GetNumTarget();
                player.h.baseAttack.numTarget = (NumTarget)Mathf.Clamp(amount + 1, 1, 4);
                break;
            case 26:
                amount = 15;
                player.h.GetAccuracy().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 28:
                amount = player.goldOnExit;
                player.goldOnExit = amount + 10;
                break;
            case 29:
                amount = 15;
                player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 30:
                amount = 20;
                player.h.GetAttackLifestrike().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 31:
                player.h.playerInventory.alwaysShowUniqueRoom = true;
                break;
            case 32:
                player.h.playerInventory.healInUniqueRoom = true;
                break;
            case 36:
                amount = 10;
                player.h.SetMaxAbilityCount(10);
                break;
         
            /* Rare */
            case 57:
                player.h.playerInventory.rerollCrit = true;
                break;
            case 58:
                amount = 5;
                player.h.playerInventory.ailmentArtifactRedux = amount;
                break;

            /* Legendary */
            case 65:
                amount = 1;
                player.h.GetActions().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 68:
                player.numArtifactReroll = 1;
                break;
            case 69:
                amount = 5;
                player.h.playerInventory.healPerTurn = amount;
                break;

            /* Exalted */
            case 79:
                player.h.playerInventory.enemyDazedStart = true;
                break;
            case 82:
                amount = 3;
                player.h.GetToughness().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                player.h.playerInventory.noGoldFloor = noGoldFloor;
                break;
        }
    }

    public void OnEncounterStart()
    {
        if(isDisabled)
        {
            return;
        }

        switch (artifactIndex)
        {

            /* Uncommon */
            case 24: //Check to trigger every beginning of encounter
                if (player.h.playerInventory.attacksUsedThisEncounter <= 0)
                {
                    player.h.GetDominance().AddModifier(new StatModifier(amount, StatModType.Flat, fakeTurnAmount, this));
                    _ = player.h.GetDominance().Value;
                }
                break;
            case 34: //Check to trigger every beginning of encounter
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                //Apply to enemies
                for (int x = 0; x < cs.enemyPanel.transform.childCount; x++)
                {
                    EnemyToken eHUD = cs.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
                    eHUD.GetHealthStats().AddModifier(new StatModifier(-amount, StatModType.Flat, fakeTurnAmount, this));
                    eHUD.ResetMaxHealth();
                }
                break;
            case 37: //Check to trigger every beginning of encounter
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                //Apply to enemies
                for (int x = 0; x < cs.enemyPanel.transform.childCount; x++)
                {
                    EnemyToken eHUD = cs.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
                    eHUD.GetSpecialLifeSteal().AddModifier(new StatModifier(-amount, StatModType.Flat, fakeTurnAmount, this));
                    _ = eHUD.GetSpecialLifeSteal().Value;
                }
                break;
            case 38: //Check to trigger every beginning of encounter
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                //Apply to enemies
                for (int x = 0; x < cs.enemyPanel.transform.childCount; x++)
                {
                    EnemyToken eHUD = cs.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
                    eHUD.GetCritChance().AddModifier(new StatModifier(-amount, StatModType.Flat, fakeTurnAmount, this));
                    _ = eHUD.GetCritChance().Value;
                }
                break;
            case 39: //Check to trigger every beginning of encounter
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                //Apply to enemies
                for (int x = 0; x < cs.enemyPanel.transform.childCount; x++)
                {
                    EnemyToken eHUD = cs.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
                    eHUD.GetToughness().AddModifier(new StatModifier(-amount, StatModType.Flat, fakeTurnAmount, this));
                    _ = eHUD.GetToughness().Value;
                }
                break;
            case 40: //Check to trigger every beginning of encounter
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                //Apply to enemies
                for (int x = 0; x < cs.enemyPanel.transform.childCount; x++)
                {
                    EnemyToken eHUD = cs.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
                    eHUD.GetAccuracy().AddModifier(new StatModifier(-amount, StatModType.Flat, fakeTurnAmount, this));
                    _ = eHUD.GetAccuracy().Value;
                }
                break;

            /* Rare */
            case 47: //Check to trigger every beginning of encounter
                player.h.AddBattleResistance(artifact.damageType, 3, amount, this);
                break;
            case 48: //Check to trigger every beginning of encounter
                player.h.AddBattleResistance(artifact.damageType, 3, amount, this);
                break;
            case 49: //Check to trigger every beginning of encounter
                player.h.AddBattleResistance(artifact.damageType, 3, amount, this);
                break;
            case 50: //Check to trigger every beginning of encounter
                player.h.AddBattleResistance(artifact.damageType, 3, amount, this);
                break;
            case 51: //Check to trigger every beginning of encounter
                player.h.AddBattleResistance(artifact.damageType, 3, amount, this);
                break;
            case 52: //Check to trigger every beginning of encounter
                player.h.SetBattleStatusChance(artifact.damageType, 3, amount, this);
                break;
            case 53: //Check to trigger every beginning of encounter
                player.h.SetBattleStatusChance(artifact.damageType, 3, amount, this);
                break;
            case 54: //Check to trigger every beginning of encounter
                player.h.SetBattleStatusChance(artifact.damageType, 3, amount, this);
                break;
            case 55: //Check to trigger every beginning of encounter
                player.h.SetBattleStatusChance(artifact.damageType, 3, amount, this);
                break;
            case 56: //Check to trigger every beginning of encounter
                player.h.SetBattleStatusChance(artifact.damageType, 3, amount, this);
                break;



            /* Legendary */
            case 64:  //Check to trigger every beginning of encounter
                if (player.h.GetBlock() < 2)
                {
                    player.h.SetBlock(2);
                }
                break;
            case 66: //Check to trigger every beginning of encounter, end of attack
                amount = 10;
                player.h.GetStrength().AddModifier(new StatModifier(amount, StatModType.Flat, fakeTurnAmount, this));
                break;
            case 70:
                amount = 0;
                break;
            case 71: //Check to trigger every beginning of encounter
                player.h.GetAccuracy().AddModifier(new StatModifier(amount, StatModType.Flat, 3, this));
                break;
            case 72: //Check to trigger every beginning of encounter
                player.h.GetCritChance().AddModifier(new StatModifier(amount, StatModType.Flat, 3, this));
                break;
            case 73: //Check to trigger every beginning of encounter
                player.h.GetAttackLifestrike().AddModifier(new StatModifier(amount, StatModType.Flat, 3, this));
                break;
            case 74: //Check to trigger every beginning of encounter
                player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(amount, StatModType.Flat, 3, this));
                break;
            case 75: //Check to trigger every beginning of encounter
                player.h.GetStrength().AddModifier(new StatModifier(amount, StatModType.Flat, 3, this));
                break;
            case 76: //Check to trigger every beginning of encounter
                player.h.GetDominance().AddModifier(new StatModifier(amount, StatModType.Flat, 5, this));
                break;
            case 77: //Check to trigger every beginning of encounter
                player.h.GetActions().AddModifier(new StatModifier(amount, StatModType.Flat, 3, this));
                break;


            /* Exalted */
            case 80: //Check to trigger every beginning of encounter
                player.h.ChangeHealth(9999999);
                player.numRevives = 2;
                player.numArtifactRevive = 0;
                player.RenderRevives();
                break;
        }
    }

    public void OnEncounterEnd()
    {
        if (isDisabled)
        {
            return;
        }

        switch (artifactIndex)
        {
            /* Uncommon */
            case 35: //Check to trigger every end of encounter
                //1 consumable every 5 encounter
                encounterCount++;
                if (encounterCount > 4)
                {
                    encounterCount = 0;
                    Consumable.GiveRandomCombatConsumable(1, player);

                }
                break;
            case 41: //Check to trigger every end of encounter
                //+1 crit chance every 3
                encounterCount++;
                if (encounterCount > 2)
                {
                    encounterCount = 0;
                    amount += 1;
                    player.h.GetCritChance().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                }
                break;
            case 42: //Check to trigger every end of encounter
                //+1 lifesteal every 3
                encounterCount++;
                if (encounterCount > 2)
                {
                    encounterCount = 0;
                    amount += 1;
                    player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                }
                break;



            /* Rare */
            case 43://Check to trigger every end of encounter
                //Level up random ability
                encounterCount++;
                if (encounterCount > 4)
                {
                    encounterCount = 0;
                    List<IAbility> abilitiesOwned = new List<IAbility>();
                    abilitiesOwned.Add(player.h.baseAttack);
                    abilitiesOwned.AddRange(player.h.specials);
                    abilitiesOwned = abilitiesOwned.OrderBy(a => System.Guid.NewGuid()).ToList();

                    foreach(IAbility a in abilitiesOwned)
                    {
                        if ((a is Attack att && att.canLevelup) || (a is Special sp && sp.canLevelup))
                        {
                            a.LevelUp();
                            List<System.Action> popups = new List<System.Action>();
                            popups.Add(() => a.TriggerNoHit(player.popupWindow.transform, a.GetName() + ": +1 Rank!", 12));
                            GameUtilities.ShowLevelUpPopup(player, popups);
                            break;
                        }
                    }
                }
                break;
            case 62: //Check to trigger every end of encounter
                //+1 toughness every 8
                encounterCount++;
                if (encounterCount > 7)
                {
                    encounterCount = 0;
                    amount += 1;
                    player.h.GetToughness().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                }
                break;

        }
    }

    public void OnTurnStart()
    {
        if (isDisabled)
        {
            return;
        }

        switch (artifactIndex)
        {
            /* Rare */
            case 45: //Check to trigger every beginning of turn
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                int domTemp = cs.playerKillCount - cs.oldDomKillCount;
                takedownCount += domTemp;
                cs.oldDomKillCount = cs.playerKillCount;
                amount = takedownCount / 5;
                player.h.GetDominance().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 46: //Check to trigger every beginning of turn 
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                int strTemp = cs.playerKillCount - cs.oldStrKillCount;
                takedownCount += strTemp;
                cs.oldStrKillCount = cs.playerKillCount;
                amount = takedownCount / 5;
                player.h.GetStrength().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;

            /* Legendary */
            case 70: //Check to trigger every beginning of turn
                // +10 lstrike and lsteal after first health loss only
                if(amount > player.h.hp)
                {
                    player.h.GetAttackLifestrike().AddModifier(new StatModifier(10, StatModType.Flat, fakeTurnAmount, this));
                    player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(10, StatModType.Flat, fakeTurnAmount, this));
                }
                amount = player.h.hp;
                break;

            /* Exalted */
            case 78: //Check to trigger every beginning of turn
                // Check if 5th turn to heal enemies
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                if(cs.turnUsed == 4)
                {
                    for (int x = 0; x < cs.enemyPanel.transform.childCount; x++)
                    {
                        EnemyToken eHUD = cs.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
                        eHUD.ChangeHealth(99999);
                        cs.GetCombatState().CreateFx(cs.specialSystem.allVfx[0], eHUD.transform);
                        AudioManager.instance.PlaySfxSound("Buff");
                        eHUD.GetDominance().AddModifier(new StatModifier(3, StatModType.Flat, fakeTurnAmount, this));
                    }
                }
                break;
        
        }
    }

    public void OnTurnEnd()
    {
        if (isDisabled)
        {
            return;
        }

        switch (artifactIndex)
        {
            /* Uncommon */
            case 27: //Check to trigger every end of turn
                //+5 Max HP every 5 special
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                int hpTemp = player.h.playerInventory.specialsUsedThisEncounter - cs.oldSpecialCount;
                abilityCount += hpTemp;
                cs.oldSpecialCount = player.h.playerInventory.specialsUsedThisEncounter;
                amount = abilityCount / 5;
                player.h.GetHealthStats().AddModifier(new StatModifier(amount * 5, StatModType.Flat, artifact));
                cs.UpdateHeroBattleStats();
                break;

            /* Rare */
            case 45: //Check to trigger every beginning of turn
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                int domTemp = cs.playerKillCount - cs.oldDomKillCount;
                takedownCount += domTemp;
                cs.oldDomKillCount = cs.playerKillCount;
                amount = takedownCount / 5;
                player.h.GetDominance().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 46: //Check to trigger every beginning of turn 
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                int strTemp = cs.playerKillCount - cs.oldStrKillCount;
                takedownCount += strTemp;
                cs.oldStrKillCount = cs.playerKillCount;
                amount = takedownCount / 5;
                player.h.GetStrength().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                break;
            case 59: //Check to trigger every end of turn
                // +10 HP after first attack only
                if (player.h.playerInventory.attacksUsedThisEncounter == 1)
                {
                    cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                    player.h.ChangeHealth(10);
                    cs.GetCombatState().CreateFx(cs.specialSystem.allVfx[5], cs.heroIcon.transform.parent.parent.transform);
                    AudioManager.instance.PlaySfxSound("Heal");
                    cs.specialSystem.TriggerDamagePopUp(cs.heroTransform, 10, 10, true, false);
                }
                break;
            case 60: //Check to trigger every end of turn
                     //+1 accuracy every 5
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                int accTemp = player.h.playerInventory.attacksUsedThisEncounter - cs.oldAttackCount;
                abilityCount += accTemp;
                cs.oldAttackCount = player.h.playerInventory.attacksUsedThisEncounter; 
                amount = abilityCount / 5;
                player.h.GetAccuracy().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                _ = player.h.GetAccuracy().Value;
                break;
            case 61: //Check to trigger every end of turn
                     //+1 lifestrike every 5
                cs = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
                int lsTemp = player.h.playerInventory.attacksUsedThisEncounter - cs.oldAttackCount;
                abilityCount += lsTemp;
                cs.oldAttackCount = player.h.playerInventory.attacksUsedThisEncounter;
                abilityCount = lsTemp;
                amount = abilityCount / 5;
                player.h.GetAttackLifestrike().AddModifier(new StatModifier(amount, StatModType.Flat, artifact));
                _ = player.h.GetAttackLifestrike().Value;
                break;
        }
    }

    public void EndOfAttack()
    {
        switch (artifactIndex)
        {
            /* Legendary */
            case 66://Check to trigger every beginning of encounter end of attack
                // +10 str until first attack only
                if (player.h.playerInventory.attacksUsedThisEncounter > 0)
                {
                    player.h.GetStrength().RemoveAllModFromSource(this);
                }
                break;
        }
    }

    public void DisablePassiveStat()
    {
        isDisabled = true;
        switch (artifactIndex)
        {

            /* Common */
            case 1:
                player.h.GetHealthStats().RemoveAllModFromSource(artifact);
                break;
            case 5:
                player.h.GetStrength().RemoveAllModFromSource(artifact);
                break;
            case 6:
                player.h.GetCritChance().RemoveAllModFromSource(artifact);
                break;
            case 7:
                player.h.fireStatus = Mathf.Clamp(player.h.fireStatus - 5, 0, player.h.fireStatus);
                break;
            case 8:
                player.h.iceStatus = Mathf.Clamp(player.h.iceStatus - 5, 0, player.h.iceStatus);
                break;
            case 9:
                player.h.lightningStatus = Mathf.Clamp(player.h.lightningStatus - 5, 0, player.h.lightningStatus);
                break;
            case 10:
                player.h.psychicStatus = Mathf.Clamp(player.h.psychicStatus - 5, 0, player.h.psychicStatus);
                break;
            case 11:
                player.h.poisonStatus = Mathf.Clamp(player.h.poisonStatus - 5, 0, player.h.poisonStatus);
                break;
            case 12:
                player.healthOnExit = Mathf.Clamp(player.healthOnExit - 10, 0, player.healthOnExit);
                break;
            case 13:
                player.h.permanentResistances.Remove(artifact.damageType);
                player.h.fireResist = Mathf.Clamp(amount - 20, 0, amount);
                break;
            case 14:
                player.h.permanentResistances.Remove(artifact.damageType);
                player.h.lightningResist = Mathf.Clamp(amount - 20, 0, amount);
                break;
            case 15:
                player.h.permanentResistances.Remove(artifact.damageType);
                player.h.poisonResist = Mathf.Clamp(amount - 20, 0, amount);
                break;
            case 16:
                player.h.permanentResistances.Remove(artifact.damageType);
                player.h.psychicResist = Mathf.Clamp(amount - 20, 0, amount);
                break;

            /* Uncommon */
            case 19:
                player.h.permanentResistances.Remove(artifact.damageType);
                break;
            case 20:
                player.h.permanentResistances.Remove(artifact.damageType);
                break;
            case 21:
                player.h.permanentResistances.Remove(artifact.damageType);
                player.h.iceResist = Mathf.Clamp(amount - 10, 0, amount);
                break;
            case 22:
                player.h.GetDominance().RemoveAllModFromSource(artifact);
                break;
            case 23:
                player.h.playerInventory.artifactBlockPerTurn = 0;
                break;
            case 25:
                player.h.baseAttack.ResetNumTarget();
                break;
            case 26:
                player.h.GetAccuracy().RemoveAllModFromSource(artifact);
                break;
            case 28:
                player.goldOnExit = Mathf.Clamp(amount - 10, 0, amount);
                break;
            case 29:
                player.h.GetSpecialLifeSteal().RemoveAllModFromSource(artifact);
                break;
            case 30:
                player.h.GetAttackLifestrike().RemoveAllModFromSource(artifact);
                break;
            case 31:
                player.h.playerInventory.alwaysShowUniqueRoom = false;
                break;
            case 32:
                player.h.playerInventory.healInUniqueRoom = false;
                break;
            case 36:
                int currentMax = player.h.specials.Count;
                if (currentMax > 8)
                {
                    player.h.SetMaxAbilityCount(currentMax);
                }
                else
                {
                    player.h.SetMaxAbilityCount(8);
                }
                break;

            /* Rare */
            case 57:
                player.h.playerInventory.rerollCrit = false;
                break;
            case 58:
                player.h.playerInventory.ailmentArtifactRedux = 0;
                break;



            /* Legendary */
            case 65:
                player.h.GetActions().RemoveAllModFromSource(artifact);
                break;
            case 68:
                player.numArtifactReroll = 0;
                break;
            case 69:
                player.h.playerInventory.healPerTurn = 0;
                break;


            /* Exalted */
            case 79:
                player.h.playerInventory.enemyDazedStart = false;
                break;
            case 80:
                player.numRevives = 0;
                player.numArtifactRevive = 0;
                player.RenderRevives();
                break;
            case 82:
                player.h.GetToughness().RemoveAllModFromSource(artifact);
                player.h.playerInventory.noGoldFloor = Floors.Start;
                break;
        }
    }
}