using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//State that starts combat
public class BeginBattle : CombatState
{
    public BeginBattle(CombatSystem cSystem) : base(cSystem)
    {
    }

    public override IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        if (GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && CombatSystem.encounterKey != Floors.Floor_03)
        {
            string s = "";
            switch (CombatSystem.encounterKey)
            {
                case Floors.Floor_01:
                    CombatSystem.eventAlert.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Caught in a Spiderling trap!";
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
                    s = "Is dazed on 1st turn and has reduced accuracy for this encounter.";

                    CombatSystem.hero.CreateOtherCond(Floors.Floor_01.ToString(), s, "68");

                    CombatSystem.hero.isDazed = true;
                    CombatSystem.hero.GetAccuracy().AddModifier(new StatModifier(-5, StatModType.Flat, CombatSystem));
                    break;
                case Floors.Floor_02:
                    CombatSystem.eventAlert.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Within the radius of a deadzone!";
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
                    s = "All elemental affinity reduced to 0 for this encounter.";

                    CombatSystem.hero.CreateOtherCond(Floors.Floor_02.ToString(), s, "69");

                    CombatSystem.hero.SetBattleStatusChance(CombatSystem.specialSystem.allDamageTypes[1], 999999, -999999, GameObject.Find("Floor Manager").GetComponent<FloorManager>());
                    CombatSystem.hero.SetBattleStatusChance(CombatSystem.specialSystem.allDamageTypes[2], 999999, -999999, GameObject.Find("Floor Manager").GetComponent<FloorManager>());
                    CombatSystem.hero.SetBattleStatusChance(CombatSystem.specialSystem.allDamageTypes[3], 999999, -999999, GameObject.Find("Floor Manager").GetComponent<FloorManager>());
                    CombatSystem.hero.SetBattleStatusChance(CombatSystem.specialSystem.allDamageTypes[5], 999999, -999999, GameObject.Find("Floor Manager").GetComponent<FloorManager>());
                    CombatSystem.hero.SetBattleStatusChance(CombatSystem.specialSystem.allDamageTypes[6], 999999, -999999, GameObject.Find("Floor Manager").GetComponent<FloorManager>());
                    break;
                case Floors.Floor_04:
                    CombatSystem.eventAlert.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Their faith has been renewed!";
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    CombatSystem.eventAlert.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
                    break;
            }

            CombatSystem.eventAlert.SetActive(true);
        }
        if (CombatSystem.hero.GetBlock() < CombatSystem.hero.playerInventory.blockPerEnc)
        {
            CombatSystem.hero.SetBlock(CombatSystem.hero.playerInventory.blockPerEnc);
        }
        yield return new WaitForSeconds(1f);
        CheckSpecialCondition();
        yield return new WaitForSeconds(1f);
        CombatSystem.StartCoroutine(GameUtilities.WaitForConversation(() => StartBattle()));
        CheckBonusBarkChance();

        //Artifact initialization needs to happen a little later because enemies need to spawn first
        foreach (Artifact a in CombatSystem.hero.GetArtifacts())
        {
            a.thisArtifact.OnEncounterStart();
        }

        if (CombatSystem.player.numArtifactReroll + CombatSystem.player.numReroll > 0)
        {
            CombatSystem.rerollButton.transform.parent.GetChild(1).gameObject.SetActive(false);
            CombatSystem.CanReroll();

        }

    }

    private void StartBattle()
    {

        if (CombatSystem.player.GetTotalAttempts() == 1 && CombatSystem.player.attempts.Last().numberOfEncounters == 0)
        {
            CombatSystem.textAnimator.Play("Player Text Slide");
        }
        CombatSystem.hero.playerInventory.attacksUsedThisEncounter = 0;
        CombatSystem.hero.playerInventory.specialsUsedThisEncounter = 0;

        if (CombatSystem.encounterKey == Floors.Floor_05 
            && CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Eden, The King")))
        {
            CombatSystem.PrepareEnemyAttacks();
            CombatSystem.SetState(new EnemyTurn(CombatSystem, CombatSystem.GetNextEnemy()));
        }
        else
        {
            CombatSystem.SetState(new HeroTurn(CombatSystem));
        }

    }

    private void CheckBonusBarkChance()
    {
        if(CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Ulea, The Star"))
            && CombatSystem.player.numStarInteraction < 1)
        {
            for (int x = 0; x < CombatSystem.enemyPanel.transform.childCount; x++)
            {
                EnemyToken enemy = CombatSystem.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
                enemy.bonusBarkChance = -999;
            }
        } else if(CombatSystem.player.savedPastConvos.Contains("LYRA_007_06")
            && CombatSystem.encounterKey == Floors.Floor_04)
        {
            for (int x = 0; x < CombatSystem.enemyPanel.transform.childCount; x++)
            {
                EnemyToken enemy = CombatSystem.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
                enemy.bonusBarkChance = 999;
            }
        }
    }

    private void CheckSpecialCondition()
    {
        bool isDorian= CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Sir Dorian Gold, The Fool"));
        bool dorianDecided = 
            CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.DorianDecided);
        bool cutsceneFive = CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Five.ToString());
        if (isDorian && dorianDecided && !cutsceneFive)
        {
            CutsceneManager.instance.Play(Cutscenes.Five);
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Fool, "DORIAN_023_01");

            CombatSystem.StartCoroutine(
                GameUtilities.WaitForCutscene(() => DialogueManager.instance.ShowDialogue(block)));
        }


        bool isEden = CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Eden, The King"));
        bool compendiumSealed = 
            CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed);
        bool cutsceneTen = CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Ten.ToString());
        if (isEden && compendiumSealed && !cutsceneTen)
        {
            CutsceneManager.instance.Play(Cutscenes.Ten);
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.King, "EDEN_017_01");
            CombatSystem.StartCoroutine(
                GameUtilities.WaitForCutscene(() => DialogueManager.instance.ShowDialogue(block)));
        }
    }
}
