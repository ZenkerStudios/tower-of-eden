using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTurn : CombatState
{
    //turn, heal
    int[] healthRegen;
    //turn
    int frozen;
    //turn
    int poisoned;
    //turn, dmg
    int[] burning;
    //turn, dmgMult
    float[] vulnerable;
    //turn, reductionAmount
    int[] shocked;
    //turn, stacks, dmg, max
    int[] damnation;

    private int actions;
    public HeroTurn(CombatSystem cSystem) : base(cSystem)
    {
    }

    private void TriggerHealPerTurn(int val)
    {
        if (val > 0 && CombatSystem.hero.GetIsPoisoned() < 1)
        {
            CombatSystem.hero.ChangeHealth(val);
            CombatSystem.GetCombatState().CreateFx(CombatSystem.specialSystem.allVfx[5], CombatSystem.heroIcon.transform.parent.parent.transform);
            AudioManager.instance.PlaySfxSound("Heal");
            CombatSystem.specialSystem.TriggerDamagePopUp(CombatSystem.heroTransform, val, val, true, false);
        }
    }

    public override IEnumerator Start()
    {
        //if first ulea encounter give condition where can't attack her 
        AddUleaCondition();

        foreach (Artifact a in CombatSystem.hero.GetArtifacts())
        {
            a.thisArtifact.OnTurnStart();
        }

        TriggerHealPerTurn(CombatSystem.hero.playerInventory.healPerTurn);
    

        if (CombatSystem.hero.toBreakIce)
        {
            CombatSystem.hero.toBreakIce = false;
            CombatSystem.hero.BreakIce();
        }

        if (CombatSystem.hero.GetBlock() < CombatSystem.hero.playerInventory.blockPerTurn)
        {
            CombatSystem.hero.SetBlock(CombatSystem.hero.playerInventory.blockPerTurn);
        }

        if (CombatSystem.hero.GetBlock() < CombatSystem.hero.playerInventory.artifactBlockPerTurn)
        {
            CombatSystem.hero.SetBlock(CombatSystem.hero.playerInventory.artifactBlockPerTurn);
        }

        CombatSystem.hudAnimator.Play("Hero Condition Pulse", 2);         
        
        CombatSystem.rerollThisTurn = CombatSystem.player.numReroll + CombatSystem.player.numArtifactReroll  + CombatSystem.critReroll;
        CombatSystem.critReroll = 0;
        CombatSystem.CanReroll();
        CombatSystem.playerTurn = true;
        if (CombatSystem.turnUsed <= 0 && CombatSystem.player.GetTotalAttempts() > 0)
        {
            //Play start turn Animation
            CombatSystem.hudAnimator.Play("Player HUD Setup");
        }

        CombatSystem.turnUsed++;
        CombatSystem.PerformReroll(0);
        CombatSystem.UpdateAbilitySprites();
        CombatSystem.turnText.gameObject.SetActive(true);

        healthRegen = CombatSystem.hero.GetHealthRegen();
        frozen = CombatSystem.hero.GetIsFrozen();
        poisoned = CombatSystem.hero.GetIsPoisoned();
        burning = CombatSystem.hero.GetIsBurning();
        vulnerable = CombatSystem.hero.GetIsVulnerable();
        shocked = CombatSystem.hero.GetIsShocked();
        damnation = CombatSystem.hero.GetDamnation();

        CombatSystem.PrepareEnemyAttacks();

        if (GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered)
        {
            yield return new WaitForSeconds(1f);
            CombatSystem.eventAlert.SetActive(false);
        }

        CombatSystem.StartCoroutine(Upkeep());
    }

    public override IEnumerator Upkeep()
    {
        CombatSystem.hero.OnSelfStatsUpkeep();
        CombatSystem.StartCoroutine(TriggerEncounterBarks(CombatSystem.hero));

        
        //Health Regen
        if (healthRegen[0] > 0)
        {
            healthRegen[0] -= 1;
            int finalAmount = healthRegen[1];
            if (CombatSystem.hero.GetIsPoisoned() > 0) finalAmount = 0;
            CreateFx(CombatSystem.specialSystem.allVfx[5], CombatSystem.heroIcon.transform.parent.parent.transform);
            AudioManager.instance.PlaySfxSound("Heal");
            CombatSystem.specialSystem.TriggerDamagePopUp(CombatSystem.heroTransform, healthRegen[1], finalAmount, true, false);
            CombatSystem.hero.ChangeHealth(finalAmount);
            yield return new WaitForSeconds(.5f);
        }

        //Burning
        if (burning[0] > 0)
        {
            burning[0] -= 1;
            int finalAmount = burning[1];
            if (CombatSystem.hero.GetBlock() > 0)
            {
                BlockAttack(CombatSystem.hero, CombatSystem.heroTransform);
            }
            else
            {
                if (CombatSystem.hero.GetIsVulnerable()[0] > 0) finalAmount = (int)(finalAmount * CombatSystem.hero.GetIsVulnerable()[1]);
                CombatSystem.hero.ChangeHealth(-finalAmount);
                CreateFx(CombatSystem.specialSystem.allVfx[4], CombatSystem.heroIcon.transform.parent.parent.transform);
                AudioManager.instance.PlaySfxSound("Fire");
                CombatSystem.specialSystem.TriggerNoHit(CombatSystem.heroTransform, "" + finalAmount);
            }
            CombatSystem.lastKilledBy = "Burning";

            yield return new WaitForSeconds(.5f);

        }

        //Smite
        if (damnation[0] > 0 && ((damnation[0] == 1 && damnation[1] > 0) || damnation[1] >= damnation[3]))
        {
            damnation[0] = 0;
            int dmg = damnation[2] * damnation[1];
            if (CombatSystem.hero.GetBlock() > 0)
            {
                BlockAttack(CombatSystem.hero, CombatSystem.heroTransform);
            }
            else
            {
                if (CombatSystem.hero.GetIsVulnerable()[0] > 0) dmg = (int)(dmg * CombatSystem.hero.GetIsVulnerable()[1]);
                if (CombatSystem.hero.GetDivineResistance())
                {
                    dmg -= dmg / 3;
                }
                CreateFx(CombatSystem.specialSystem.allVfx[3], CombatSystem.heroIcon.transform.parent.parent.transform);
                AudioManager.instance.PlaySfxSound("Divine_Smite");
                CombatSystem.hero.ChangeHealth(-dmg);
                CombatSystem.specialSystem.TriggerNoHit(CombatSystem.heroTransform, "" + dmg);
            }
            CombatSystem.lastKilledBy = "Damnation";

            yield return new WaitForSeconds(.5f);
        }
        else
        {
            damnation[0] -= 1;
        }
        actions = (int)CombatSystem.hero.GetActions().Value;

        //Frozen
        if (CombatSystem.hero.isDazed || frozen > 0 || CombatSystem.HeroDead())
        {
            if(frozen > 0)
            {
                DamageTypeEnumValue dt = CombatSystem.specialSystem.allDamageTypes[2];
                CombatSystem.StartCoroutine(CombatSystem.specialSystem.Condition(dt, CombatSystem.heroTransform));
                yield return new WaitForSeconds(.5f);
            }
            CombatSystem.hero.isDazed = false;
            CombatSystem.abilityUsed = null;
            CombatSystem.targetIndex = 0;
            actions = 0;
            CombatSystem.ActionButtonsInteractable(false);
            CombatSystem.UpdateAbilitySprites();
            CombatSystem.StartCoroutine(Attack());
        }
        else
        {
            CombatSystem.ActionButtonsInteractable(true);
            CombatSystem.UpdateAbilitySprites();
            CombatSystem.StartCoroutine(base.Upkeep());
        }
    }

    public override IEnumerator Attack()
    {
        CombatSystem.ActionButtonsInteractable(false);
        return Resolve();
    }

    public bool GetCrit(PlayerHero hero, EnemyToken enemy)
    {
        if(hero.playerInventory.critOnFrozen && enemy.GetIsFrozen() > 0)
        {
            return true;
        } else if (hero.playerInventory.critOnPoison && enemy.GetIsPoisoned() > 0)
        {
            return true;
        }
        return CombatSystem.PercentileChanceToHappen(CombatSystem.hero.critChance.Value);
    }

    private void DamageToSelf(IAbility attack)
    {
        if (CombatSystem.player.selectedWeaponType == WeaponTypes.Delta && attack is Attack)
        {
            CombatSystem.hero.ChangeHealth(CombatSystem.hero.playerInventory.attackSelfDmg);
        }
    }

    private void AttackDaze(EnemyToken target)
    {
        if (UnityEngine.Random.Range(1, 101) <= CombatSystem.hero.playerInventory.attackDazeChance)
        {
            target.isDazed = true;
            target.RenderDazed();
            CombatSystem.specialSystem.TriggerIconPopUpForStat(target.transform, false, CharacterStats.Dazed);
            CreateFx(CombatSystem.specialSystem.allVfx[1], target.transform);
        }
    }

    private void HealOnCrit(bool isCrit)
    {
        if(CombatSystem.hero.playerInventory.healOnCrit > 0 && CombatSystem.player.selectedWeaponType == WeaponTypes.Melancolia && isCrit)
        {
            CombatSystem.specialSystem.TriggerDamagePopUp(CombatSystem.heroTransform, CombatSystem.hero.playerInventory.healOnCrit, CombatSystem.hero.playerInventory.healOnCrit, true, false);
            CombatSystem.hero.ChangeHealth(CombatSystem.hero.playerInventory.healOnCrit);
        }
    }

    private bool WillHit(float chanceToHit)
    {
        bool res = true;
        if(CombatSystem.numberOfMisses < 5)
        {
            res = CombatSystem.PercentileChanceToHappen(chanceToHit);
        }
        if (res) CombatSystem.numberOfMisses = 0;
        else CombatSystem.numberOfMisses++;
        return res;
    }

    public override IEnumerator Resolve()
    {
        CombatSystem.rerollButton.interactable = false;
        CombatSystem.UnshowInfo();
        if(CombatSystem.abilityUsed != null)
        {
            List<EnemyToken> targets = CombatSystem.GetPotentialTargets(CombatSystem.abilityUsed, CombatSystem.targetIndex);
            foreach (EnemyToken t in targets)
            {
                t.GetSpritePos();

                //Chance to hit
                float hit = CombatSystem.hero.accuracy.Value - shocked[1];
                if (WillHit(hit))
                {
                    DamageToSelf(CombatSystem.abilityUsed);
                    if (!CombatSystem.abilityUsed.GetStats().Contains(CharacterStats.MultiHit) && t.GetBlock() > 0 &&
                        (CombatSystem.abilityUsed.GetAbilityType() == AbilityTypes.Offensive ||
                        CombatSystem.abilityUsed.GetAbilityType() == AbilityTypes.Affliction))
                    {
                        BlockAttack(t, t.transform);
                    } else
                    {
                        AudioManager.instance.PlaySfxSound(CombatSystem.abilityUsed.GetAudio());
                        bool isCrit = GetCrit(CombatSystem.hero, t);
                        if(isCrit && CombatSystem.hero.playerInventory.rerollCrit)
                        {
                            CombatSystem.critReroll++;
                        }
                        if (isCrit) CombatSystem.critHit++;
                        HealOnCrit(isCrit);
                        CombatSystem.StartCoroutine(CombatSystem.abilityUsed.TriggerAbility(CombatSystem.hero, CombatSystem.heroTransform, t, t.transform, isCrit, CombatSystem));
                        if (CombatSystem.abilityUsed.GetVfx() != null)
                        {
                            if(CombatSystem.abilityUsed.GetVfx().name.Equals("Ice FX"))
                            {
                                t.PlayIceFX();
                                t.StartCoroutine(IceBreak(t));
                            } else
                            {
                                CreateFx(CombatSystem.abilityUsed.GetVfx(), t.transform);
                                if (CombatSystem.player.selectedWeaponType == WeaponTypes.Firestorm
                                    && CombatSystem.abilityUsed is Attack
                                    && CombatSystem.hero.playerInventory.attackDazeChance > 0)
                                {
                                    yield return new WaitForSeconds(0.65f);
                                    AttackDaze(t);
                                }
                            }
                        }
                    }
                } else
                {
                    CombatSystem.hitEveryAttack = false;
                    AudioManager.instance.PlaySfxSound("Miss");
                    CombatSystem.abilityUsed.TriggerNoHit(t.transform, "Miss");
                }
            }
            if(CombatSystem.abilityUsed is Attack)
            {
                CombatSystem.hero.playerInventory.attacksUsedThisEncounter++;
            }
            else
            {
                CombatSystem.hero.playerInventory.specialsUsedThisEncounter++;
            }
        }
        foreach (Artifact a in CombatSystem.hero.GetArtifacts())
        {
            a.thisArtifact.EndOfAttack();
        }
        yield return new WaitForSeconds(.4f);
        CombatSystem.DeselectAllTargets();
        yield return new WaitForSeconds(.4f);
        CombatSystem.targetSelected = false;

        actions--;
        CombatSystem.GainXp();

        if (actions > 0 && !CombatSystem.AllNPCDead())
        {
            //Should give new attack
            CombatSystem.PerformReroll(0);
            CombatSystem.ActionButtonsInteractable(true);
            CombatSystem.UpdateAbilitySprites();

            yield break;
            
        }

        frozen = CombatSystem.hero.GetIsFrozen();
        poisoned = CombatSystem.hero.GetIsPoisoned();
        burning = CombatSystem.hero.GetIsBurning();
        vulnerable = CombatSystem.hero.GetIsVulnerable();
        shocked = CombatSystem.hero.GetIsShocked();
        damnation = CombatSystem.hero.GetDamnation();
        healthRegen = CombatSystem.hero.GetHealthRegen();

        CombatSystem.DisableDesc();
        CombatSystem.abilityUsed = null;

        frozen -= 1;
        poisoned -= 1;
        shocked[0] -= 1;
        vulnerable[0] -= 1;

        CombatSystem.DeselectAllTargets();
        CombatSystem.targetIndex = 0;

        CombatSystem.hero.SetHealthRegen(healthRegen);
        CombatSystem.hero.SetIsFrozen(frozen);
        CombatSystem.hero.SetIsBurning(burning);
        CombatSystem.hero.SetIsPoisoned(poisoned);
        CombatSystem.hero.SetIsVulnerable(vulnerable);
        CombatSystem.hero.SetIsShocked(shocked);
        CombatSystem.hero.SetDamnation(damnation);

        CombatSystem.hero.CheckConditions();

        yield return new WaitForSeconds(1.5f);
        CombatSystem.StartCoroutine(GameUtilities.WaitForConversation(() => Continue()));

    }

    private void Continue()
    {

        foreach (Artifact a in CombatSystem.hero.GetArtifacts())
        {
            a.thisArtifact.OnTurnEnd();
        }

        //If player loses
        if (CombatSystem.AllNPCDead())
        {
            CombatSystem.hero.SetHealthRegen(new int[2]);
            CombatSystem.hero.SetIsFrozen(0);
            CombatSystem.hero.SetIsBurning(new int[2]);
            CombatSystem.hero.SetIsPoisoned(0);
            CombatSystem.hero.SetIsVulnerable(new float[2]);
            CombatSystem.hero.SetIsShocked(new int[2]);
            CombatSystem.hero.SetDamnation(new int[4]);

            CombatSystem.hero.CheckConditions();
            CombatSystem.SetState(new WonRound(CombatSystem));
        }
        else if (CombatSystem.HeroDead())
        {
            CombatSystem.SetState(new LostRound(CombatSystem));
        }
        else
        {
            if (CombatSystem.isApotheosis)
            {
                CombatSystem.totalApotheosisTurn = Mathf.Clamp(CombatSystem.totalApotheosisTurn - 1, 0, CombatSystem.totalApotheosisTurn);
            }

            CombatSystem.SetState(new EnemyTurn(CombatSystem, CombatSystem.GetNextEnemy()));
            CombatSystem.textAnimator.Play("Enemy Text Slide");
        }

    }
}

    
