using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

//Enemy turn state
public class EnemyTurn : CombatState
{
    //turn, heal
    int[] healthRegen;
    //turn
    int frozen;
    private bool isFrozen;
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
    EnemyToken thisToken;
    private string tokenName = "";

    public EnemyTurn(CombatSystem cSystem, EnemyToken token) : base(cSystem)
    {
        thisToken = token;
        tokenName = thisToken.GetName();
        CombatSystem.playerTurn = false;
        thisToken.myTurn = true;
        CheckAcridTotalResistance(thisToken);
    }

    public override IEnumerator Start()
    {
        if(thisToken.toBreakIce)
        {
            thisToken.toBreakIce = false;
            thisToken.BreakIce();
        }
        thisToken.VerifyBark(CombatSystem.hero.hp, thisToken.preparedMoves);
        thisToken.GetSpritePos();

        healthRegen = thisToken.GetHealthRegen();
        frozen = thisToken.GetIsFrozen();
        poisoned = thisToken.GetIsPoisoned();
        burning = thisToken.GetIsBurning();
        vulnerable = thisToken.GetIsVulnerable();
        shocked = thisToken.GetIsShocked();
        damnation = thisToken.GetDamnation();
        isFrozen = frozen > 0;
        
        thisToken.PulsateConditions();
        
        yield return new WaitForSeconds(1.75f);
        CombatSystem.StartCoroutine(Upkeep());

    }

    

    public override IEnumerator Upkeep()
    {
        thisToken.ResetTokenInfo();
        thisToken.OnSelfStatsUpkeep();
        CombatSystem.StartCoroutine(TriggerEncounterBarks(thisToken));

        //Check for condition spread
        if (poisoned > 0 && CombatSystem.PercentileChanceToHappen(CombatSystem.hero.playerInventory.poisonSpread))
        {
            //spread poisoned
            List<EnemyToken> spreadTo = GetAllyTargets(1, thisToken, false);
            if (spreadTo.Count > 0)
            {
                spreadTo[0].SetIsPoisoned(1);
                EffectsMethods.AddCondition(spreadTo[0], CombatSystem.specialSystem.allDamageTypes[5]);
                CombatSystem.specialSystem.TriggerConditionPopup(spreadTo[0].transform, CombatSystem.specialSystem.allDamageTypes[5]);
                CreateFx(CombatSystem.specialSystem.allVfx[8], spreadTo[0].transform);
                AudioManager.instance.PlaySfxSound("Poison");
                yield return new WaitForSeconds(.5f);
            }
        }

        if (shocked[0] > 0 && CombatSystem.PercentileChanceToHappen(CombatSystem.hero.playerInventory.shockSpread))
        {
            //spread shocked
            List<EnemyToken> spreadTo = GetAllyTargets(1, thisToken, false);
            if (spreadTo.Count > 0 && spreadTo[0].GetIsShocked()[0] < 2)
            {
                spreadTo[0].SetIsShocked(new int[] { 1, shocked[1] });
                EffectsMethods.AddCondition(spreadTo[0], CombatSystem.specialSystem.allDamageTypes[3]);
                CombatSystem.specialSystem.TriggerConditionPopup(spreadTo[0].transform, CombatSystem.specialSystem.allDamageTypes[3]);
                CreateFx(CombatSystem.specialSystem.allVfx[6], spreadTo[0].transform);
                AudioManager.instance.PlaySfxSound("Lightning");
                yield return new WaitForSeconds(.5f);
            }
        }

        if (burning[0] > 0 && CombatSystem.PercentileChanceToHappen(CombatSystem.hero.playerInventory.burnSpread))
        {
            //spread burning
            List<EnemyToken> spreadTo = GetAllyTargets(1, thisToken, false);
            if (spreadTo.Count > 0 && spreadTo[0].GetIsBurning()[0] < 2)
            {
                spreadTo[0].SetIsBurning(new int[] { 1, burning[1] });
                EffectsMethods.AddCondition(spreadTo[0], CombatSystem.specialSystem.allDamageTypes[1]);
                CombatSystem.specialSystem.TriggerConditionPopup(spreadTo[0].transform, CombatSystem.specialSystem.allDamageTypes[1]);
                CreateFx(CombatSystem.specialSystem.allVfx[4], spreadTo[0].transform);
                AudioManager.instance.PlaySfxSound("Fire");
                yield return new WaitForSeconds(.5f);
            }
        }

        //Health Regen
        if (healthRegen[0] > 0)
        {
            healthRegen[0] -= 1;
            int finalAmount = healthRegen[1];
            if (thisToken.GetIsPoisoned() > 0) finalAmount = 0;
            CreateFx(CombatSystem.specialSystem.allVfx[5], thisToken.transform);
            AudioManager.instance.PlaySfxSound("Heal");
            CombatSystem.specialSystem.TriggerDamagePopUp(thisToken.transform, healthRegen[1], finalAmount, true, false);
            thisToken.ChangeHealth(finalAmount);
            yield return new WaitForSeconds(.5f);
        }

        //Burning
        if (burning[0] > 0)
        {
            burning[0] -= 1;
            int finalAmount = burning[1];
            if(thisToken.GetBlock() > 0)
            {
                BlockAttack(thisToken, thisToken.transform);
            } else
            {
                if (CombatSystem.hero.GetIsVulnerable()[0] > 0) finalAmount = (int)(finalAmount * CombatSystem.hero.GetIsVulnerable()[1]);
                CreateFx(CombatSystem.specialSystem.allVfx[4], thisToken.transform);
                AudioManager.instance.PlaySfxSound("Fire");
                CombatSystem.specialSystem.TriggerNoHit(thisToken.transform, "" + finalAmount);
                thisToken.ChangeHealth(-finalAmount);
                CombatSystem.TrackBountyDamage(thisToken, CombatSystem.specialSystem.allDamageTypes[1], -finalAmount);

            }
            yield return new WaitForSeconds(.5f);
        }

        //Smite
        if (damnation[0] > 0 && ((damnation[0] == 1 && damnation[1] > 0) || damnation[1] >= damnation[3]))
        {
            damnation[0] = 0;
            int dmg = damnation[2] * damnation[1];
            if (thisToken.GetBlock() > 0)
            {
                BlockAttack(thisToken, thisToken.transform);
            }
            else
            {
                if (CombatSystem.hero.GetIsVulnerable()[0] > 0) dmg = (int)(dmg * CombatSystem.hero.GetIsVulnerable()[1]);
                if (thisToken.HasResistance("Divine"))
                {
                    dmg -= dmg / 3;
                }
                CreateFx(CombatSystem.specialSystem.allVfx[3], thisToken.transform);
                AudioManager.instance.PlaySfxSound("Divine_Smite");
                CombatSystem.specialSystem.TriggerNoHit(thisToken.transform, "" + dmg);
                thisToken.ChangeHealth(-dmg);
                CombatSystem.TrackBountyDamage(thisToken, CombatSystem.specialSystem.allDamageTypes[0], -dmg);
            }

            yield return new WaitForSeconds(.5f);
        }
        else
        {
            damnation[0] -= 1;
        }

        if (thisToken.GetConditions().Count > 0)
        {
            yield return new WaitForSeconds(.5f);
        }
        GeneratorLeft(thisToken);

        if (thisToken.IsDead())
        {
            CombatSystem.enemyTurnIndex -= 1;
            CombatSystem.StartCoroutine(Resolve());
        }
        else if (frozen > 0
            || thisToken.isDazed
            || thisToken.GetActions().Value <= 0
            || thisToken.canSelfDestruct && thisToken.selfDestructTurn <= 1)
        {
            if (frozen > 0)
            {
                DamageTypeEnumValue dt = CombatSystem.specialSystem.allDamageTypes[2];
                thisToken.StartCoroutine(CombatSystem.specialSystem.Condition(dt, thisToken.transform));
                yield return new WaitForSeconds(.5f);
            }
            thisToken.isDazed = false;
            thisToken.dazedIcon.SetActive(false);
            CombatSystem.StartCoroutine(Resolve());
        } 
        else
        {
            CombatSystem.StartCoroutine(Attack());
        }
    }

    public List<EnemyToken> GetAllyTargets(int numToSelect, EnemyToken self, bool selfIncluded)
    {
        List<int> unusedIndex = Enumerable.Range(0, CombatSystem.enemyPanel.transform.childCount).ToList();
        List<EnemyToken> targets = new List<EnemyToken>();
        unusedIndex.Remove(self.transform.GetSiblingIndex());
        int startingCount = 0;
        if(selfIncluded)
        {
            targets.Add(self);
            startingCount = 1;
        }
        List<int> tlist = unusedIndex.OrderBy(a => System.Guid.NewGuid()).ToList();

        for (int x = startingCount, y = 0; x < numToSelect && y < unusedIndex.Count; x++, y++)
        {
            targets.Add(CombatSystem.enemyPanel.transform.GetChild(unusedIndex[y]).GetComponent<EnemyToken>());
        }

        return targets;
    }

    public List<EnemyToken> GetHealingTargets(int numToSelect, EnemyToken self, bool selfIncluded)
    {
        IsEnemySquadFullHealth();
        List<EnemyToken> targets = new List<EnemyToken>();
        damagedEnemies.Remove(self);
        int startingCount = 0;
        if (selfIncluded)
        {
            targets.Add(self);
            startingCount = 1;
        }
        List<EnemyToken> tlist = damagedEnemies.OrderBy(a => System.Guid.NewGuid()).ToList();

        for (int x = startingCount, y = 0; x < numToSelect && y < damagedEnemies.Count; x++, y++)
        {
            targets.Add(damagedEnemies[y]);
        }

        return targets;
    }


    public override IEnumerator Attack()
    {
        if (!AreCurrentActionsValid(thisToken))
        {
            yield return new WaitForSeconds(1f);
        }
        float numActions = thisToken.actions.Value;
        for (int z = 0; z < numActions; z++)
        {
            thisToken.ResetTokenInfo();
            thisToken.tokenAnimator.Play("Attack_Anim");
            float hit = thisToken.accuracy.Value - shocked[1];
            //Get attack at first index since they are removed after use
            IAbility sp = thisToken.GetMove(0);
            if (sp.GetAbilityType() == AbilityTypes.Summon)
            {
                if (CheckRagnaSummons(thisToken, sp))
                {
                    yield return new WaitForSeconds(.75f);
                }
                else if (AngelRevive(sp))
                {
                    yield return new WaitForSeconds(.75f);
                } else if (CheckEdenPhaseOneCloneSummons(thisToken, sp))
                {
                    yield return new WaitForSeconds(.75f);
                    CombatSystem.enemyTurnIndex = 0;
                    CombatSystem.SetState(new HeroTurn(CombatSystem));
                    CombatSystem.textAnimator.Play("Player Text Slide");
                    CombatSystem.UpdateNpcBattleStats();
                    yield break;
                }
                else
                {
                    //Did regular enemy summon
                    for (int n = thisToken.enemyNpc.maxNumAllies; n > 0 && CombatSystem.enemyCount < CombatSystem.maxEnemyCount; n--)
                    {
                        EnemyNpc npcToSummon = thisToken.GetAllyToken();
                        CombatSystem.SpawnEnemy(npcToSummon, true, true);
                    }
                }
            }
            else if (sp.GetAbilityType() == AbilityTypes.Enchantment)
            {
                AudioManager.instance.PlaySfxSound(sp.GetAudio());
                
                if (Recall((Special)sp, thisToken))
                {
                    yield return new WaitForSeconds(.75f);

                } 
                else if (AcridEnchant((Special)sp))
                {
                    yield return new WaitForSeconds(.75f);

                }
                else if (NexusMode((Special)sp, thisToken))
                {
                    yield return new WaitForSeconds(.75f);
                }
                else if (CheckLyraApotheosis((Special)sp))
                {
                    yield return new WaitForSeconds(.75f);
                }
                else if (EdenSeal((Special)sp, thisToken))
                {
                    yield return new WaitForSeconds(.75f);
                    if (CombatSystem.player.savedPastConvos.Contains("EDEN_BARK_010") && CombatSystem.seal >= 10)
                    {
                        CombatSystem.defeatedEnemies.Add(thisToken.GetName());
                        CombatSystem.deadEnemyNpcs.Add(thisToken.enemyNpc);
                        z = 999;
                        thisToken.preparedMoves = new List<Special>();
                        thisToken.RenderIntent();
                        thisToken.ChangeHealth(-999999999);
                    }
                }
                else if (EdenGratitude((Special)sp, thisToken))
                {
                    yield return new WaitForSeconds(.75f);
                }
                else
                {
                    List<EnemyToken> targets = GetAllyTargets((int)sp.GetNumTarget(), thisToken, sp.GetTargetTypes() is TargetTypes.OnSelf);
                    foreach (EnemyToken npc in targets)
                    {
                        CombatSystem.StartCoroutine(sp.TriggerAbility(thisToken, thisToken.transform,
                            npc, npc.transform,
                            CombatSystem.PercentileChanceToHappen(thisToken.critChance.Value), CombatSystem));
                        CreateFx(sp.GetVfx(), npc.transform);
                    }
                }

            }
            else if (sp.GetAbilityType() == AbilityTypes.Recovery)
            {
                AudioManager.instance.PlaySfxSound(sp.GetAudio());

                List<EnemyToken> targets = GetHealingTargets((int)sp.GetNumTarget(), thisToken, sp.GetTargetTypes() is TargetTypes.OnSelf);
                foreach (EnemyToken npc in targets)
                {
                    CombatSystem.StartCoroutine(sp.TriggerAbility(thisToken, thisToken.transform,
                        npc, npc.transform,
                        CombatSystem.PercentileChanceToHappen(thisToken.critChance.Value), CombatSystem));
                    CreateFx(sp.GetVfx(), npc.transform);
                }
            }
            else
            {

                if (CocoonFeast(thisToken))
                {
                    yield return new WaitForSeconds(.75f);
                }
                else if (CombatSystem.PercentileChanceToHappen(hit))
                {
                    ICharacter target = CombatSystem.hero;
                    Transform tTransform = CombatSystem.heroTransform;

                    if (vulnerable[0] > 0 && (Random.Range(1, 101) <= CombatSystem.hero.playerInventory.vulnerableConfusion))
                    {
                        List<EnemyToken> allyTarget = GetAllyTargets(1, thisToken, false);
                        target = allyTarget[0];
                        tTransform = allyTarget[0].transform;
                    }

                    if (!sp.GetStats().Contains(CharacterStats.MultiHit) && target.GetBlock() > 0 &&
                      (sp.GetAbilityType() == AbilityTypes.Offensive ||
                      sp.GetAbilityType() == AbilityTypes.Affliction))
                    {
                        //If the attack gets blocked
                        BlockAttack(target, tTransform);
                    }
                    else
                    {
                        CombatSystem.lastKilledBy = tokenName;
                        AudioManager.instance.PlaySfxSound(sp.GetAudio());
                        CombatSystem.StartCoroutine(sp.TriggerAbility(thisToken, thisToken.transform,
                        target, tTransform,
                        CombatSystem.PercentileChanceToHappen(thisToken.critChance.Value), CombatSystem));
                        LyraVerse((Special)sp, thisToken);
                        if (sp.GetVfx() != null)
                        {
                            if (sp.GetVfx().name.Equals("Ice FX"))
                            {
                                target.PlayIceFX();
                                CombatSystem.hero.StartCoroutine(IceBreak(CombatSystem.hero));
                            }
                            else
                            {
                                if(target is EnemyToken)
                                {
                                    CreateFx(sp.GetVfx(), tTransform);

                                } else
                                {
                                    CreateFx(sp.GetVfx(), CombatSystem.heroIcon.transform.parent.parent.transform);
                                }
                            }
                        }

                    }
                    if(thisToken.buffAfterDamage != null)
                    {
                        thisToken.buffDamageNumber = 0;
                        thisToken.GetDominance().AddModifier(new StatModifier(thisToken.buffDamageNumber, StatModType.Flat, thisToken.buffAfterDamage));

                    }
                }
                else
                {
                    CombatSystem.dodges++;
                    AudioManager.instance.PlaySfxSound("Miss");
                    sp.TriggerNoHit(CombatSystem.heroTransform, "Miss");
                }
            }

            CombatSystem.UpdateNpcBattleStats();
            CombatSystem.HeroDead();
            thisToken.RenderIntent();

            yield return new WaitForSeconds(1.5f);
        }
        thisToken.preparedMoves = new List<Special>();

        CombatSystem.StartCoroutine(Resolve());

    }

    //Checks win condition
    public override IEnumerator Resolve()
    {
        if (thisToken != null)
        {
            thisToken.isThinking.SetActive(false);

            frozen = thisToken.GetIsFrozen();
            poisoned = thisToken.GetIsPoisoned();
            burning = thisToken.GetIsBurning();
            vulnerable = thisToken.GetIsVulnerable();
            shocked = thisToken.GetIsShocked();
            damnation = thisToken.GetDamnation();
            healthRegen = thisToken.GetHealthRegen();

            frozen -= 1;
            poisoned -= 1;
            shocked[0] -= 1;
            vulnerable[0] -= 1;

            thisToken.SetHealthRegen(healthRegen);
            thisToken.SetIsFrozen(frozen);
            thisToken.SetIsBurning(burning);
            thisToken.SetIsPoisoned(poisoned);
            thisToken.SetIsVulnerable(vulnerable);
            thisToken.SetIsShocked(shocked);
            thisToken.SetDamnation(damnation);

            thisToken.CheckConditions();
            thisToken.myTurn = false;

            if (!isFrozen && thisToken.isTransforming && --thisToken.transformationTurn <= 0)
            {
                EnemyNpc revived = thisToken.reviveTokens;
                int index = thisToken.transform.GetSiblingIndex();
                GameObject newObj = CombatSystem.SpawnEnemy(revived, true, true);
                thisToken.isTransforming = false;
                newObj.SetActive(false);
                CombatSystem.specialSystem.TriggerNoHit(thisToken.activeSprite.transform, "Transform");
                CreateFx(CombatSystem.specialSystem.allVfx[0], thisToken.transform);
                AudioManager.instance.PlaySfxSound("Buff");
                yield return new WaitForSeconds(.75f);
                thisToken.ChangeHealth(-99999);
                CombatSystem.UpdateNpcBattleStats();
                newObj.transform.SetSiblingIndex(index);
                yield return new WaitForSeconds(.5f);
                newObj.SetActive(true);


            }

            if (!isFrozen && thisToken.canSelfDestruct && --thisToken.selfDestructTurn <= 0)
            {
                thisToken.ChangeHealth(-99999);
                CombatSystem.UpdateNpcBattleStats();
                CombatSystem.enemyTurnIndex--;
                yield return new WaitForSeconds(.5f);
            }

        }

        CombatSystem.UpdateNpcBattleStats();

        yield return new WaitForSeconds(.5f);

        //If player loses
        if (CombatSystem.HeroDead())
        {
            CombatSystem.lastKilledBy = tokenName;
            CombatSystem.SetState(new LostRound(CombatSystem));
        }
        else if (CombatSystem.AllNPCDead())
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
        else
        {
            if(CombatSystem.LastEnemyToGo())
            {
                CombatSystem.UnshowInfo();
                yield return new WaitForSeconds(0.75f);

                CombatSystem.enemyTurnIndex = 0;
                CombatSystem.SetState(new HeroTurn(CombatSystem));
                CombatSystem.textAnimator.Play("Player Text Slide");
            } else
            {
                CombatSystem.SetState(new EnemyTurn(CombatSystem, CombatSystem.GetNextEnemy()));
            }
        }
        CombatSystem.UpdateNpcBattleStats();
    }

}
