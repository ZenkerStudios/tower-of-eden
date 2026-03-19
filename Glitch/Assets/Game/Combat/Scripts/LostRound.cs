using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LostRound : CombatState
{
    public LostRound(CombatSystem cSystem) : base(cSystem)
    {
    }

    private void TriggerDeath()
    {
        CombatSystem.player.inBattle = false;
        CombatSystem.player.playerManagerAnimator.Play("Death Anim");
        AudioManager.instance.PlaySceneMusic("Game_Over");
        CombatSystem.player.TriggerDeathBark();
        CombatSystem.hero.ChangeAbilitySlotXp(CombatSystem.xpGainedThisEncounter);
    }

    public override IEnumerator Start()
    {
        CheckSpecialCondition();
        CombatSystem.player.attempts.Last().FinalUpdate(CombatSystem, false, false);
        DialogueManager dialogueManager = DialogueManager.instance;

        //Play death animation
        TriggerDeath();

        for (int x = CombatSystem.deadEnemyNpcs.Count - 1; x > 0; x--)
        {
            CombatSystem.Destroy(CombatSystem.deadEnemyNpcs[x]);
            CombatSystem.deadEnemyNpcs.RemoveAt(x);
        }
        yield return new WaitForSeconds(5f);

        CombatSystem.StartCoroutine(Resolve());
    }

    public override IEnumerator Resolve()
    {
        //Reset game
        GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().DestroyOnRunFinish();

        CombatSystem.player.PlayerReset(true);
        CombatSystem.Destroy(CombatSystem.gameObject);
        GameController.instance.LoadGame(Floors.Hub);
        return base.Resolve();
    }

    private void CheckSpecialCondition()
    {
        bool isRagna = CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Ragna, The Empress"));
        bool ragnaChallengeIssued = CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.RagnaChallenge);
        if (isRagna && ragnaChallengeIssued)
        {
            CombatSystem.player.savedDialogueConditionsMet.Add(DialogueConditions.RagnaChallengeLost);
        }
    }
}
