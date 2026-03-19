using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WonRound : CombatState
{
    public WonRound(CombatSystem CombatSystem) : base(CombatSystem)
    {
    }

    private void ResetPlayerTurn()
    {
        CombatSystem.enemyTurnIndex = 0;
        CombatSystem.SetState(new HeroTurn(CombatSystem));
        CombatSystem.textAnimator.Play("Player Text Slide");
        CombatSystem.UpdateNpcBattleStats();
    }

    public override IEnumerator Start()
    {
        if (CheckEdenPhaseTwo())
        {
            yield return new WaitForSeconds(.75f);
            ResetPlayerTurn();
        } else if (CheckEdenPhaseThree())
        {
            yield return new WaitForSeconds(1.25f);
            ResetPlayerTurn();
        }
        else
        {
            CombatSystem.StartCoroutine(Upkeep());
        }
    }
    public override IEnumerator Upkeep()
    {

        if (GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && CombatSystem.encounterKey == Floors.Floor_03)
        {
            CombatSystem.eventAlert.transform.GetComponentInChildren<TextMeshProUGUI>().text = "An Elite knight challenges you to a duel!";
            CombatSystem.eventAlert.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            CombatSystem.eventAlert.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            CombatSystem.eventAlert.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            CombatSystem.eventAlert.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);

            CombatSystem.BeginFloorSpecialEnc();
            CombatSystem.eventAlert.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            ResetPlayerTurn();
            CombatSystem.eventAlert.SetActive(false); 
        } else if (CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Sir Dorian Gold, The Fool"))
            && CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.DorianResolve)
            && !CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.SoldierResolve))
        {
            //Where cutscene 6 used to be referenced.
            DialogueBlock block =
               DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Fool, "DORIAN_045_01");
            CombatSystem.StartCoroutine(StartDialogue(block));

            CombatSystem.StartCoroutine(
                GameUtilities.WaitForConversation(() => CombatSystem.BeginDorianSpecialEnc()));
            ResetPlayerTurn();

        }
        else if (CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Eden, The King"))
            && CombatSystem.player.savedPastConvos.Contains("EDEN_016_05")
            && !CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Nine.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Nine);
            CombatSystem.player.savedDialogueConditionsMet.Add(DialogueConditions.CompendiumStabilized);
            CombatSystem.StartCoroutine(
                GameUtilities.WaitForCutscene(() => CheckEdenPhaseFour()));
            ResetPlayerTurn();
        }
        else
        {
            float time = TriggerFinalCutscene();
            CombatSystem.StartCoroutine(
                 GameUtilities.WaitForConversation(time, () => CombatSystem.StartCoroutine(Resolve())));
        }
    }



    private float TriggerFinalCutscene()
    {
        if (CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Ragna, The Empress"))
            && CombatSystem.player.savedPastConvos.Contains("RAGNA_021_02")
            && !CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Eleven.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Eleven);
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Empress, "RAGNA_022_01");
            CombatSystem.StartCoroutine(GameUtilities.WaitForCutscene(() => CombatSystem.StartCoroutine(StartDialogue(block))));
        }
        else if (CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Acrid, The Magician"))
            && CombatSystem.player.savedPastConvos.Contains("ACRID_019_04")
            && !CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Twelve.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Twelve);
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Magician, "ACRID_020_01");
            CombatSystem.StartCoroutine(GameUtilities.WaitForCutscene(() => CombatSystem.StartCoroutine(StartDialogue(block))));
        }
        else if (CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Sir Dorian Gold, The Fool"))
            && CombatSystem.player.savedPastConvos.Contains("DORIAN_024_05")
            && !CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Thirteen.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Thirteen);
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Fool, "DORIAN_026_01");
            CombatSystem.StartCoroutine(GameUtilities.WaitForCutscene(() => CombatSystem.StartCoroutine(StartDialogue(block))));
        }
        else if (CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Lyra, The High Priestess"))
            && CombatSystem.player.savedPastConvos.Contains("LYRA_017_03")
            && !CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Fourteen.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Fourteen);
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Priestess, "LYRA_018_01");
            CombatSystem.StartCoroutine(GameUtilities.WaitForCutscene(() => CombatSystem.StartCoroutine(StartDialogue(block))));
        }
        else if (CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Eden, The King"))
            && CombatSystem.player.savedPastConvos.Contains("EDEN_001_03")
            && !CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Two.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Two);
        }
        else if (CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Eden, The King"))
            && CombatSystem.player.savedPastConvos.Contains("EDEN_002_05")
            && !CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Four.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Four);
        }
        else if (CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Eden, The King"))
            && CombatSystem.player.savedPastConvos.Contains("EDEN_013_05")
            && !CombatSystem.player.cutscenesUnlocked.Contains(Cutscenes.Eight.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Eight);
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.King, "EDEN_014_01");
            CombatSystem.StartCoroutine(GameUtilities.WaitForCutscene(() => CombatSystem.StartCoroutine(StartDialogue(block))));
        }
        else if (CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Eden"))
            && CombatSystem.player.savedPastConvos.Contains("IZAAK_BARK_011")
            && !CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed))
        {
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.King, "EDEN_019_01");
            CombatSystem.StartCoroutine(StartDialogue(block));
            CombatSystem.player.savedDialogueConditionsMet.Add(DialogueConditions.CompendiumSealed);
        }
        else if(CombatSystem.deadEnemyNpcs.Any(npc => npc.GetName().Equals("Ulea, The Star"))
            && CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.StarRevealed)
            && !CombatSystem.player.HasDefeatedEnemy("Ulea, The Star"))
        {
            CutsceneManager.instance.Play(Cutscenes.Seven);
            CombatSystem.player.savedDialogueConditionsMet.Add(DialogueConditions.FirstUleaDefeat);
          
        }
        return 1f;
    }
    private IEnumerator StartDialogue(DialogueBlock block)
    {
        yield return new WaitForSeconds(.75f);

        if (block != null)
        {
            DialogueManager.instance.ShowDialogue(block);
        }
    }
    public override IEnumerator Resolve()
    {
        //Encounter officially ends here
        CombatSystem.hero.battleIceStatus = new StatSystem();
        CombatSystem.hero.battleFireStatus = new StatSystem();
        CombatSystem.hero.battlePoisonStatus = new StatSystem();
        CombatSystem.hero.battlePsychicStatus = new StatSystem();
        CombatSystem.hero.battleLightningStatus = new StatSystem();

        CombatSystem.hero.battleIceResist = new StatSystem();
        CombatSystem.hero.battleFireResist = new StatSystem();
        CombatSystem.hero.battlePoisonResist = new StatSystem();
        CombatSystem.hero.battlePsychicResist = new StatSystem();
        CombatSystem.hero.battleLightningResist = new StatSystem();
        CombatSystem.hero.battlePhysicalResist = new StatSystem();
        CombatSystem.hero.battleDivineResist = new StatSystem();

        CombatSystem.hero.ResetConditions();

        if (GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered)
        {
            GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered = false;
            switch (CombatSystem.encounterKey)
            {
                case Floors.Floor_01:
                    CombatSystem.hero.GetAccuracy().RemoveAllModFromSource(CombatSystem);
                    break;
            }
        }

        foreach (Artifact a in CombatSystem.hero.GetArtifacts())
        {
            a.thisArtifact.OnEncounterEnd();
        }
        CheckSpecialCondition();

        if (CombatSystem.encounterKey == Floors.Floor_05)
        {
            CombatSystem.player.attempts.Last().FinalUpdate(CombatSystem, true, false);
        }
        else
        {
            CombatSystem.player.attempts.Last().PartialUpdateAttempt(CombatSystem);
        }

        FloorManager fm = GameObject.Find("Floor Manager").GetComponent<FloorManager>();

        foreach (Ailment ail in CombatSystem.hero.playerInventory.ailments)
        {
            ail.enemiesKilled += CombatSystem.playerKillCount;
            ail.wonEncounter = true;
            ail.beatMiniBoss = CombatSystem.isMiniBoss;
        }
        CombatSystem.hero.ChangeAbilitySlotXp(CombatSystem.xpGainedThisEncounter);
        CombatSystem.xpGainedThisEncounter = 0;

        for (int x = CombatSystem.deadEnemyNpcs.Count - 1; x > 0; x--)
        {
            CombatSystem.Destroy(CombatSystem.deadEnemyNpcs[x]);
            CombatSystem.deadEnemyNpcs.RemoveAt(x);
        }

        yield return new WaitForSeconds(0.5f);
        CombatSystem.StartCoroutine(GameUtilities.WaitForConversation(() => GetRewards()));

        yield return new WaitForSeconds(2.5f);


    }

    private void GetRewards()
    {
        foreach(Bounty b in CombatSystem.player.equippedBounties)
        {
            b.CheckBountyConditions(CombatSystem);
        }

        var window = GameObject.Instantiate(CombatSystem.rewardScreen);
        window.transform.SetParent(CombatSystem.transform);
        window.transform.localPosition = new Vector2(0, 0);
        window.GetComponent<RectTransform>().sizeDelta = CombatSystem.rewardScreen.GetComponent<RectTransform>().sizeDelta;
        window.GetComponent<RectTransform>().localScale = CombatSystem.rewardScreen.GetComponent<RectTransform>().localScale;
        window.GetComponent<RewardSystem>().GetRandomReward(CombatSystem.difficulty, CombatSystem.reward, CombatSystem.encounterKey, CombatSystem.elite);
        CombatSystem.hudAnimator.gameObject.SetActive(false);
    }

    private void CheckSpecialCondition()
    {
        bool isRagna = CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Ragna, The Empress"));
        bool ragnaChallengeIssued = CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.RagnaChallenge);
        if(isRagna && ragnaChallengeIssued)
        {
            CombatSystem.player.savedDialogueConditionsMet.Add(DialogueConditions.RagnaChallengeWon);
        }
    }
}
