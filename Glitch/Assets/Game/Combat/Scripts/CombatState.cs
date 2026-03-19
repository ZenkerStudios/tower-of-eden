using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Predefined battle states
public abstract class CombatState
{
    protected CombatSystem CombatSystem;

    public List<EnemyToken> damagedEnemies;

    public CombatState(CombatSystem cSystem)
    {
        this.CombatSystem = cSystem;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Upkeep()
    {
        yield break;
    }

    public virtual IEnumerator Attack()
    {
        yield break;
    }

    public virtual IEnumerator Resolve()
    {
        yield break;
    }

    public void CreateFx(GameObject fx, Transform t)
    {
        var newVfx = GameObject.Instantiate(fx);
        newVfx.transform.SetParent(t);
        newVfx.transform.localPosition = new Vector2(0, 0);
        newVfx.GetComponent<RectTransform>().sizeDelta = fx.GetComponent<RectTransform>().sizeDelta;
        newVfx.GetComponent<RectTransform>().localScale = fx.GetComponent<RectTransform>().localScale;
    }

    public IEnumerator IceBreak(ICharacter t)
    {
        yield return new WaitForSeconds(.75f);
        t.CheckConditions();
    }

    public void IsEnemySquadFullHealth()
    {
        damagedEnemies = new List<EnemyToken>();
        for (int x = 0; x < CombatSystem.enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = CombatSystem.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            if (eHUD.hp < eHUD.GetMaxHealth())
            {
                damagedEnemies.Add(eHUD);
            }
        }
    }

    public void BlockAttack(ICharacter target, Transform t)
    {
        AudioManager.instance.PlaySfxSound("Block");
        CombatSystem.specialSystem.TriggerNoHit(t, "Blocked");
        target.SetBlock(target.GetBlock() - 1);
        if (target is EnemyToken npc)
        {
            npc.PulsateShield();
        }
        else
        {
            CombatSystem.dodges++;
            CombatSystem.hudAnimator.Play("Hero Shield Pulse", 1);
        }
    }

    public bool AreCurrentActionsValid(EnemyToken thisToken)
    {
        bool res = true;
        if (thisToken.GetName().Equals("Eden")) return true;
        IsEnemySquadFullHealth();
        if(thisToken.GetActions().Value != thisToken.preparedMoves.Count)
        {
            damagedEnemies.Remove(thisToken);
            thisToken.SetMoves(CombatSystem.maxEnemyCount - CombatSystem.enemyCount, damagedEnemies.Count <= 0);
            thisToken.RenderThinking();
        }

        List<int> index = new List<int>();
        foreach (Special s in thisToken.preparedMoves)
        {
            if(s.GetAbilityType() == AbilityTypes.Recovery)
            {
                if (s.GetTargetTypes() == TargetTypes.OnSelf)
                {
                    if ((s.numTarget == NumTarget.One && thisToken.hp >= thisToken.GetMaxHealth())
                        || (s.numTarget != NumTarget.One && damagedEnemies.Count <= 0 && thisToken.hp >= thisToken.GetMaxHealth()))
                    {
                        thisToken.RenderThinking();
                        index.Add(thisToken.preparedMoves.IndexOf(s));
                        res = false;
                    }
                }
                else if ((s.GetTargetTypes() == TargetTypes.OnTarget) && damagedEnemies.Count <= 0)
                {
                    res = false;
                    thisToken.RenderThinking();
                    index.Add(thisToken.preparedMoves.IndexOf(s));
                }
            }

            if(!s.GetName().Equals("_Gift of Resistance") && s.GetAbilityType() == AbilityTypes.Enchantment && s.GetTargetTypes() == TargetTypes.OnTarget && CombatSystem.enemyPanel.transform.childCount <= 1)
            {
                thisToken.RenderThinking();
                index.Add(thisToken.preparedMoves.IndexOf(s));
                res = false;
            }

            if(s.GetName().Equals("_Genesis") && CombatSystem.deadEnemyNpcs.Count <= 0)
            {
                thisToken.RenderThinking();
                index.Add(thisToken.preparedMoves.IndexOf(s));
                res = false;
            }
        }

        foreach(int i in index)
        {
            thisToken.preparedMoves[i] = thisToken.enemyNpc.attacks[0];
        }
        return res;
    }

    public EnemyToken isCocoonAvailable()
    {
        for (int x = 0; x < CombatSystem.enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = CombatSystem.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            if (eHUD.GetName().Equals("White Cocoon")
                || eHUD.GetName().Equals("Red Cocoon")
                || eHUD.GetName().Equals("Purple Cocoon"))
            {
                return eHUD;
            }
        }
        return null;
    }

    public List<EnemyToken> areSpiderlingsAvailable()
    {
        List<EnemyToken> tokens = new List<EnemyToken>();

        for (int x = 0; x < CombatSystem.enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = CombatSystem.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            if (eHUD.GetName().Equals("Spiderling"))
            {
                tokens.Add(eHUD);
            }
        }
        return tokens;
    }

    public bool CheckRagnaSummons(EnemyToken eHUD, IAbility s)
    {
        if (eHUD.GetName().Equals("Ragna, The Empress")
            && s.GetName().Equals("_Summon Cocoon")
            && !isCocoonAvailable())
        {
            CombatSystem.SpawnEnemy(GetCocoon(), true, true);
            return true;
        }
        return false;
    }

    private bool offeredRed = false;
    private EnemyNpc GetCocoon()
    {
        int chosenIndex = UnityEngine.Random.Range(0, (int)CombatSystem.player.chosenDifficulty - 1);
        if(offeredRed)
        {
            chosenIndex = 1;
        }
        else
        {
            offeredRed = chosenIndex == 2;
        }
        EnemyNpc temp = CombatSystem.ragnaSpecialSummons[chosenIndex];
        return temp;
    }

    public bool CocoonFeast(EnemyToken ragna)
    {
        EnemyToken token = isCocoonAvailable();
        if (token == null) { return false; }
        if (ragna.GetName().Equals("Ragna, The Empress") && token.selfDestructTurn <= 1)
        {
            Special sp = token.enemyNpc.specialAbility;
            sp.TriggerNoHit(token.transform, "Consumed by The Empress");
            if (sp.GetName().Equals("_Cocoon Heal"))
            {
                int heal = token.hp;
                if (ragna.GetIsPoisoned() > 0) heal = 0;
                ragna.ChangeHealth(heal);
                sp.specialSystem.TriggerDamagePopUp(ragna.transform, heal, heal, true, false);
            }
            else if (sp.GetName().Equals("_Cocoon Ability"))
            {
                ragna.enemyNpc.attacks.Add(sp.skillToGain);
            }
            else if (sp.GetName().Equals("_Cocoon Empower"))
            {
                sp.powerAmount[0] = token.hp;
                CombatSystem.StartCoroutine(sp.TriggerAbility(ragna, ragna.transform,
                       ragna, ragna.transform,
                       false, CombatSystem));
            }
            token.ChangeHealth(-9999);

            return true;            
        }
        return false;
    }

    public bool Recall(Special sp, EnemyToken ragna)
    {
        if (!sp.GetName().Equals("_Recall")) return false;
        if (!ragna.GetName().Equals("Ragna, The Empress")) return false;
        List<EnemyToken> spiderlings = areSpiderlingsAvailable();

        if(spiderlings.Count > 0)
        {
            CreateFx(sp.GetVfx(), ragna.transform);
            ragna.GetActions().AddModifier(new StatModifier(spiderlings.Count, StatModType.Flat, 1, sp));
            sp.specialSystem.TriggerIconPopUpForStat(ragna.transform, true, CharacterStats.Actions);
        }
        
        foreach (EnemyToken token in spiderlings)
        {
            sp.TriggerNoHit(token.transform, "Recalled to The Empress");
            token.ChangeHealth(-9999);
        }
            
        return true;
    }



    public bool GeneratorLeft(EnemyToken generator)
    {
        if (!generator.GetName().Equals("Ether Generator")) return false;
        EnemyToken acrid = GetEnemyToken(InteractableNpcs.Magician);
        if(acrid == null)
        {
            generator.ChangeHealth(-9999);
            return true;
        }
        return false;
    }

    public void CheckAcridTotalResistance(EnemyToken acrid)
    {
        if (!acrid.GetName().Equals("Acrid, The Magician")) return;
        int count = acrid.GetResistances().Count + CombatSystem.hero.GetResistances().Count;
        acrid.GetDominance().AddModifier(new StatModifier(count, StatModType.Flat, acrid.traits[0]));
    }

    public bool AcridEnchant(Special sp)
    {
        if (!sp.GetName().Equals("_Gift of Resistance")) return false;
        ICharacter target = CombatSystem.hero;
        Transform tTransform = CombatSystem.heroTransform;
        int index = UnityEngine.Random.Range(0, CombatSystem.specialSystem.allDamageTypes.Count);
        int resistAmount = UnityEngine.Random.Range(1, 20 - (int)CombatSystem.player.chosenDifficulty);
        target.AddBattleResistance(CombatSystem.specialSystem.allDamageTypes[index], sp.GetDuration(), resistAmount, sp);
        CombatSystem.specialSystem.TriggerIconPopUpWithIndex(tTransform, true, Int16.Parse(CombatSystem.specialSystem.allDamageTypes[index].spriteIndex));
        CreateFx(sp.GetVfx(), CombatSystem.heroIcon.transform.parent.parent.transform);
        return true;
    }

    public bool NexusMode(Special sp, EnemyToken dorian)
    {
        if (!dorian.GetName().Equals("Sir Dorian Gold, The Fool")) return false;
        //1: sword, gauntlet  | 2: sword, gauntlet, gun | 3: sword, gauntlet, gun, spear | 4: sword, gauntlet, gun, spear, guard
        int bossSpecialCount = Mathf.Clamp(CombatSystem.player.chosenDifficulty, 1, 4);
        bool result = false;
        switch (sp.GetName())
        {
            case "_Gauntlet Mode":
                dorian.bossSpriteImg.sprite = CombatSystem.dorianModes[0].enemySprite;
                dorian.enemyNpc.enemySprite = CombatSystem.dorianModes[0].enemySprite;
                dorian.enemyNpc.attacks = CombatSystem.dorianModes[0].attacks;
                dorian.enemyNpc.bossSpecials = CombatSystem.dorianModes[0].bossSpecials.Take(bossSpecialCount).ToList();
                dorian.maxMeterGain = CombatSystem.dorianModes[0].maxMeterGain;
                dorian.enemyNpc.enemySystem.SetAllAbilities(dorian.enemyNpc);
                dorian.RemoveTrait(Traits.Indomitable);
                dorian.RemoveTrait(Traits.Safeguard);
                dorian.RemoveTrait(Traits.Relentless);
                dorian.RemoveTrait(Traits.Unruly);
                dorian.AddTrait(Traits.Vigilant);
                result = true;
                CombatSystem.dorianModesUsed.Add(CombatSystem.dorianModes[0].GetName());
                break;
            case "_Guard Mode":
                dorian.bossSpriteImg.sprite = CombatSystem.dorianModes[1].enemySprite;
                dorian.enemyNpc.enemySprite = CombatSystem.dorianModes[1].enemySprite;
                dorian.enemyNpc.attacks = CombatSystem.dorianModes[1].attacks;
                dorian.enemyNpc.bossSpecials = CombatSystem.dorianModes[1].bossSpecials.Take(bossSpecialCount).ToList();
                dorian.maxMeterGain = CombatSystem.dorianModes[1].maxMeterGain;
                dorian.enemyNpc.enemySystem.SetAllAbilities(dorian.enemyNpc);
                dorian.RemoveTrait(Traits.Indomitable);
                dorian.RemoveTrait(Traits.Vigilant);
                dorian.RemoveTrait(Traits.Relentless);
                dorian.RemoveTrait(Traits.Unruly);
                dorian.AddTrait(Traits.Safeguard);
                result = true;
                CombatSystem.dorianModesUsed.Add(CombatSystem.dorianModes[1].GetName());
                break;
            case "_Gun Mode":
                dorian.bossSpriteImg.sprite = CombatSystem.dorianModes[2].enemySprite;
                dorian.enemyNpc.enemySprite = CombatSystem.dorianModes[2].enemySprite;
                dorian.enemyNpc.attacks = CombatSystem.dorianModes[2].attacks;
                dorian.enemyNpc.bossSpecials = CombatSystem.dorianModes[2].bossSpecials.Take(bossSpecialCount).ToList();
                dorian.maxMeterGain = CombatSystem.dorianModes[2].maxMeterGain;
                dorian.enemyNpc.enemySystem.SetAllAbilities(dorian.enemyNpc);
                dorian.RemoveTrait(Traits.Indomitable);
                dorian.RemoveTrait(Traits.Safeguard);
                dorian.RemoveTrait(Traits.Relentless);
                dorian.RemoveTrait(Traits.Vigilant);
                dorian.AddTrait(Traits.Unruly);
                result = true;
                CombatSystem.dorianModesUsed.Add(CombatSystem.dorianModes[2].GetName());
                break;
            case "_Spear Mode":
                dorian.bossSpriteImg.sprite = CombatSystem.dorianModes[3].enemySprite;
                dorian.enemyNpc.enemySprite = CombatSystem.dorianModes[3].enemySprite;
                dorian.enemyNpc.attacks = CombatSystem.dorianModes[3].attacks;
                dorian.enemyNpc.bossSpecials = CombatSystem.dorianModes[3].bossSpecials.Take(bossSpecialCount).ToList();
                dorian.maxMeterGain = CombatSystem.dorianModes[3].maxMeterGain;
                dorian.enemyNpc.enemySystem.SetAllAbilities(dorian.enemyNpc);
                dorian.RemoveTrait(Traits.Indomitable);
                dorian.RemoveTrait(Traits.Safeguard);
                dorian.RemoveTrait(Traits.Vigilant);
                dorian.RemoveTrait(Traits.Unruly);
                dorian.AddTrait(Traits.Relentless);
                result = true;
                CombatSystem.dorianModesUsed.Add(CombatSystem.dorianModes[3].GetName());
                break;
            case "_Sword Mode":
                dorian.bossSpriteImg.sprite = CombatSystem.dorianModes[4].enemySprite;
                dorian.enemyNpc.enemySprite = CombatSystem.dorianModes[4].enemySprite;
                dorian.enemyNpc.attacks = CombatSystem.dorianModes[4].attacks;
                dorian.enemyNpc.bossSpecials = CombatSystem.dorianModes[4].bossSpecials.Take(bossSpecialCount).ToList();
                dorian.maxMeterGain = CombatSystem.dorianModes[4].maxMeterGain;
                dorian.enemyNpc.enemySystem.SetAllAbilities(dorian.enemyNpc);
                dorian.RemoveTrait(Traits.Vigilant);
                dorian.RemoveTrait(Traits.Safeguard);
                dorian.RemoveTrait(Traits.Relentless);
                dorian.RemoveTrait(Traits.Unruly);
                dorian.AddTrait(Traits.Indomitable);
                result = true;
                CombatSystem.dorianModesUsed.Add(CombatSystem.dorianModes[4].GetName());
                break;
        }

        CreateFx(sp.GetVfx(), dorian.transform);

        int heal = 50;
        if (dorian.GetIsPoisoned() > 0) heal = 0;
        dorian.ChangeHealth(heal);
        sp.specialSystem.TriggerDamagePopUp(dorian.transform, heal, heal, true, false);

        return result;
    }

    public bool AngelRevive(IAbility sp)
    {
        List<EnemyNpc> chooseFrom = CombatSystem.deadEnemyNpcs.OrderBy(a => System.Guid.NewGuid()).ToList();
        chooseFrom.RemoveAll(x => x.GetName().Equals("Lyra, The High Priestess"));
        chooseFrom.RemoveAll(x => x.GetName().Equals("Ulea, The Star"));
        chooseFrom.RemoveAll(x => x.GetName().Equals("Acrid, The Magician"));
        chooseFrom.RemoveAll(x => x.GetName().Equals("Sir Dorian Gold, The Fool"));
        chooseFrom.RemoveAll(x => x.GetName().Equals("Ragna, The Empress"));
        chooseFrom.RemoveAll(x => x.GetName().Equals("Angel of The Star"));
        if (sp.GetName().Equals("_Genesis") && chooseFrom.Count > 0)
        {
            Transform revived = CombatSystem.SpawnEnemy(chooseFrom[0], true, true).transform;
            CombatSystem.specialSystem.TriggerNoHit(revived, "Revived");
            CreateFx(sp.GetVfx(), revived);
            CombatSystem.deadEnemyNpcs.Remove(chooseFrom[0]);
            CombatSystem.Destroy(chooseFrom[0]);
            return true;
        }
        return false;
    }

    public void ApotheosisTurn(EnemyToken lyra)
    {
        if (!lyra.GetName().Equals("Lyra, The High Priestess")) return;

        //Apotheosis over
        if (CombatSystem.totalApotheosisTurn < 1)
        {
            //Change sprite
            lyra.SetVulnerableInfo(new float[] { 2f, 1.5f });
            lyra.bossSpriteImg.sprite = lyra.enemyNpc.enemySprite;
            for (int x = 0; x < CombatSystem.enemyPanel.transform.childCount; x++)
            {
                EnemyToken eHUD = CombatSystem.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();

                if (eHUD.GetName().Equals("Ulea, The Star"))
                {
                    //change sprite
                    eHUD.SetDamnationInfo(new int[] { 2, 1, 5, 10 });
                    eHUD.bossSpriteImg.sprite = eHUD.enemyNpc.enemySprite;

                }
            }
        }

        //Is apotheosis active
        CombatSystem.isApotheosis = CombatSystem.totalApotheosisTurn > 0;


    }

    public void LyraVerse(Special sp, EnemyToken lyra)
    {
        if (!sp.GetName().Equals("_Perfect Pitch") || !lyra.GetName().Equals("Lyra, The High Priestess") || CombatSystem.isApotheosis) return;
        CombatSystem.verse = Mathf.Clamp(CombatSystem.verse+1, 0, 3);

    }

    public bool CheckLyraApotheosis(Special sp)
    {
        if (!sp.GetName().Equals("_Final Verse") || CombatSystem.verse < 3) return false;

        CombatSystem.totalApotheosisTurn = Mathf.Clamp(3 + ((int)CombatSystem.player.chosenDifficulty - 2), 3, 6);
        CombatSystem.isApotheosis = true;
        CombatSystem.verse = 0;

        for (int x = 0; x < CombatSystem.enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = CombatSystem.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            eHUD.CreateOtherCond("Apotheosis", "Empowered by Lyra's Final Verse for /trn turns.", "62");

            List<Action> popups = new List<Action>();
            popups.Add(() => CombatSystem.specialSystem.TriggerIconPopUpForStat(eHUD.transform, true, CharacterStats.Dominance));
            popups.Add(() => CombatSystem.specialSystem.TriggerIconPopUpForStat(eHUD.transform, true, CharacterStats.Lifesteal));
            popups.Add(() => CombatSystem.specialSystem.TriggerIconPopUpForStat(eHUD.transform, true, CharacterStats.Actions));
            popups.Add(() => CombatSystem.specialSystem.TriggerIconPopUpForStat(eHUD.transform, true, CharacterStats.Block));

            if (eHUD.GetName().Equals("Lyra, The High Priestess"))
            {
                //change sprite
                eHUD.SetVulnerableInfo(new float[] { 3f, 1.75f});
                popups.Add(() => CombatSystem.specialSystem.TriggerIconPopUpWithIndex(eHUD.transform, true, 40));
                eHUD.bossSpriteImg.sprite = CombatSystem.lyraUleaTransformation[0];
            }
            else if (eHUD.GetName().Equals("Ulea, The Star"))
            {
                //change sprite
                eHUD.SetDamnationInfo(new int[] { 2, 2, 8, 10});
                popups.Add(() => CombatSystem.specialSystem.TriggerIconPopUpWithIndex(eHUD.transform, true, 39));
                eHUD.bossSpriteImg.sprite = CombatSystem.lyraUleaTransformation[1];
            }
            eHUD.GetDominance().AddModifier(new StatModifier(5, StatModType.Flat, CombatSystem.totalApotheosisTurn, sp));
            eHUD.GetSpecialLifeSteal().AddModifier(new StatModifier(50, StatModType.Flat, CombatSystem.totalApotheosisTurn, sp));
            eHUD.GetActions().AddModifier(new StatModifier(1, StatModType.Flat, CombatSystem.totalApotheosisTurn, sp));
            if (eHUD.GetBlock() < 1)
            {
                eHUD.SetBlock(1);
            }
            eHUD.StartCoroutine(TriggerApotheosisPopups(popups));

            GameUtilities.ShowLevelUpPopup(eHUD, popups);
        }

        return true;


    }

    private IEnumerator TriggerApotheosisPopups(List<Action> popups)
    {
        foreach (Action act in popups)
        {
            act();
            yield return new WaitForSeconds(0.65f);
        }
        yield break;
    }

    public bool CheckEdenPhaseOneCloneSummons(EnemyToken eHUD, IAbility s)
    {
        if (eHUD.GetName().Equals("Eden, The King")
            && s.GetName().Equals("_Clone Spell"))
        {
            //Audio should be default BG music handled by Floor Manager
            cloneFight = true;
            foreach (EnemyNpc npcToSummon in CombatSystem.fm.floorEnemies)
            {
                CombatSystem.SpawnEnemy(npcToSummon, false, true);
            }
            CombatSystem.Destroy(eHUD.gameObject);
            return true;
        }
        return false;
    }

    private static bool cloneFight = false;
    private static bool edenPhaseTwo = false;
    private static bool edenPhaseFour = false;
    public bool CheckEdenPhaseTwo()
    {
        if (!cloneFight) return false;

        AudioManager.instance.PlaySceneMusic(CombatSystem.fm.floorBossAudioSource[0]);

        cloneFight = false;
        CombatSystem.SpawnEnemy(CombatSystem.fm.finalBosses[1].boss[0], false, true);
        edenPhaseTwo = true;
        return true;
    }
    public bool CheckEdenPhaseThree()
    {
        if (!edenPhaseTwo) return false;

        AudioManager.instance.PlaySceneMusic(CombatSystem.fm.floorBossAudioSource[1]);

        edenPhaseTwo = false;
        CombatSystem.SpawnEnemy(CombatSystem.fm.finalBosses[2].boss[0], false, true);
        return true;
    }

    public void CheckEdenPhaseFour()
    {
        edenPhaseFour = true;
        AudioManager.instance.PlayFloorMusic(CombatSystem.fm.floorBg2AudioSource);
        CombatSystem.SpawnEnemy(CombatSystem.fm.floorSpecialEncounter[0].boss[0], true, true);
    }

    public bool EdenSeal(Special sp, EnemyToken eden)
    {
        if (!sp.GetName().Equals("_Eden's Seal")) return false;
        CombatSystem.seal = Mathf.Clamp(CombatSystem.seal + 1, 0, 10);
        CreateFx(sp.GetVfx(), eden.transform);
        List<System.Action> popups = new List<System.Action>();
        popups.Add(() => CombatSystem.specialSystem.TriggerIconPopUpWithIndex(eden.transform, true, 63));
        GameUtilities.ShowPopup(eden, popups);
        return true;
    }

    public bool EdenGratitude(Special sp, EnemyToken eden)
    {
        if (!sp.GetName().Equals("_Eden's Gratitude")) return false;
        CreateFx(sp.GetVfx(), CombatSystem.heroTransform);
        sp.specialSystem.TriggerDamagePopUp(CombatSystem.heroTransform, 100, 100, true, false);
        CombatSystem.hero.ChangeHealth(100);

        CombatSystem.StartCoroutine(sp.TriggerAbility(eden, eden.transform, CombatSystem.hero, CombatSystem.heroTransform, false, CombatSystem)); 
        return true;
    }


    public void AddUleaCondition()
    {
        if (CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Ulea, The Star"))
            && CombatSystem.player.numStarInteraction < 1
            && !CombatSystem.player.savedPastConvos.Contains("LYRA_010_01"))
        {
            CombatSystem.hero.CreateOtherCond(CombatSystem.uleaCondName, CombatSystem.uleaCondDesc, "60");
        }

        if (barkIndex > 9 && CombatSystem.turnUsed > 9)
        {
            CombatSystem.RemoveHeroCondition(CombatSystem.uleaCondName);
            CombatSystem.hero.RemoveCondition(CombatSystem.uleaCondName);
        }
}


    private static int barkIndex = 0;
    public IEnumerator TriggerEncounterBarks(ICharacter character)
    {
        if(CombatSystem.hero.GetConditions().Contains(CombatSystem.uleaCondName)
            || (CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumStabilized) && !CombatSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed)))
        {
            if (barkIndex == 0)
            {
                yield return new WaitForSeconds(1f);
            }
            CombatSystem.StartCoroutine(GameUtilities.WaitForConversation(() => CombatSystem.StartCoroutine(ValidateBarks(character))));
        }
    }


    private static bool izaakBarking = false;
    private IEnumerator ValidateBarks(ICharacter character)
    {
        if (CombatSystem.enemyList.Any(npc => npc.GetName().Equals("Ulea, The Star"))
         && CombatSystem.player.numStarInteraction < 1
         && barkIndex < CombatSystem.player.h.GetNpc().barks.Count)
        {
            DialogueBlock bark = CombatSystem.player.h.GetNpc().barks[barkIndex];
            InteractableNpc owner = bark.interactingWithNpc;
            bool validSpeaker = character.GetNpc() != null && character.GetNpc().thisNpc == owner.thisNpc;
            MonoBehaviour ownerTransform = GetBarkParent(owner, true);
            if(ownerTransform == null && owner.thisNpc == InteractableNpcs.Priestess)
            {
                barkIndex++;
            }
            else
            {
                if (validSpeaker && !character.GetNpc().isBarking)
                {
                    barkIndex++;
                    DialogueManager.instance.ShowBarkWithOwner(
                        owner, ownerTransform, bark);
                }
            
                if(barkIndex < CombatSystem.player.h.GetNpc().barks.Count)
                {
                    DialogueBlock nextBark = CombatSystem.player.h.GetNpc().barks[barkIndex];
                    InteractableNpc nextOwner = nextBark.interactingWithNpc;
                    if (nextOwner.thisNpc == InteractableNpcs.Eden)
                    {
                        yield return new WaitForSeconds(1.5f);
                        barkIndex++;
                        DialogueManager.instance.ShowBarkWithOwner(
                            nextOwner, GetBarkParent(nextOwner, true), nextBark);
                    }
                }
            }
           


        }
        else if (edenPhaseFour && barkIndex < CombatSystem.player.h.GetNpc().deathBarks.Count)
        {
            DialogueBlock bark = CombatSystem.player.h.GetNpc().deathBarks[barkIndex];
            InteractableNpc owner = bark.interactingWithNpc;
            bool validSpeaker = character.GetNpc() != null && character.GetNpc().thisNpc == owner.thisNpc;
            bool izaakNotBarking = owner.thisNpc == InteractableNpcs.Izaak && !izaakBarking;
            if (izaakNotBarking || validSpeaker && !character.GetNpc().isBarking)
            {
                DialogueManager.instance.ShowBarkWithOwner(
                    owner, GetBarkParent(owner, false), bark);
                barkIndex++;
                if (izaakNotBarking) izaakBarking = false;
                if (barkIndex < CombatSystem.player.h.GetNpc().deathBarks.Count)
                {
                    DialogueBlock nextBark = CombatSystem.player.h.GetNpc().deathBarks[barkIndex];
                    InteractableNpc nextOwner = nextBark.interactingWithNpc;
                    validSpeaker = owner.thisNpc == InteractableNpcs.Eden && nextOwner.thisNpc == InteractableNpcs.Izaak;
                    if (validSpeaker && !CombatSystem.player.h.GetNpc().isBarking)
                    {
                        izaakBarking = true;
                        barkIndex++;
                        yield return new WaitForSeconds(3f);
                        DialogueManager.instance.ShowBarkWithOwner(
                            nextOwner, GetBarkParent(nextOwner, false), nextBark);
                        
                    }
                }
            }
        }
    }

    private MonoBehaviour GetBarkParent(InteractableNpc owner, bool isUleaEnc)
    {
        MonoBehaviour parent = null;
        if(isUleaEnc)
        {
            switch(owner.GetCharacterName())
            {
                case "Izaak":
                    parent = CombatSystem.heroIcon.transform.parent.parent.GetComponent<MonoBehaviour>();
                    break;
                case "Eden":
                    parent = CombatSystem.heroIcon.transform.parent.parent.GetComponent<MonoBehaviour>();
                    break;
                case "Lyra, The High Priestess":
                case "Ulea, The Star":
                    var target = GetEnemyToken(owner.thisNpc);
                    if(target != null) parent = target.activeSprite;
                    break;
            }
        }
        else
        {
            switch (owner.GetCharacterName())
            {
                case "Izaak":
                    parent = CombatSystem.heroIcon.transform.parent.parent.GetComponent<MonoBehaviour>();
                    break;
                case "Eden":
                    parent = GetEnemyToken(owner.thisNpc).activeSprite;
                    break;
            }
        }

        return parent;
    }

    private EnemyToken GetEnemyToken(InteractableNpcs owner)
    {
        for (int x = 0; x < CombatSystem.enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = CombatSystem.enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            if (eHUD.enemyNpc.interactableNpc != null && eHUD.enemyNpc.interactableNpc.thisNpc == owner)
            {
                return eHUD;
            }
        }
        return null;
    }


}

