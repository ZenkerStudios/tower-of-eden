using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

[DefaultExecutionOrder(1)]
[System.Serializable]
public class PlayerManager : MonoBehaviour
{
    //No save needed
    public bool inBattle = false;
    public GameObject popupWindow;
    public GameObject edenPng;
    public GameObject revivePanel;
    public GameObject pfDisplayArtifact;
    public GameObject artifactDisplayPanel;
    public GameObject pfAilmentItem;
    public GameObject ailmentPanel;
    public GameObject pfConsumableLoadout;
    public GameObject consumablePanel;
    public UiDisplayAbility baseAttackDisplay;
    public GameObject pfDisplayAbility;
    public GameObject pfRevive;
    public GameObject abilityDisplayPanel;
    public GameObject modIcon;
    public GameObject boonIcon;
    public GameObject revengeanceIcon;
    public TextMeshProUGUI hpText;

    public TextMeshProUGUI abilityname;
    public TextMeshProUGUI description;
    public TextMeshProUGUI turns;
    public TextMeshProUGUI statusChance;
    public TextMeshProUGUI numTarget;
    public TextMeshProUGUI abilityRank;

    public int maxBounties = 4;

    public Animator playerManagerAnimator;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI gem;
    public TextMeshProUGUI vials;
    public TextMeshProUGUI shards;
    public TextMeshProUGUI scraps;
    public TextMeshProUGUI grimoire;

    public GameObject bountyPanel;
    public GameObject pfBountyHud;
    
    public int healthPerInfusion = 25;

    #region Player manager info to save
    public WeaponTypes selectedWeaponType;

    public int healthOnExit = 5;
    public int goldOnExit = 20;

    public int chosenDifficulty = 1;
    public int highestDifficultyWon = 1;

    public PlayerHero h;
    public List<Consumable> cityShopConsumables = new List<Consumable>();

    public int numRevives = 0;

    public int numArtifactRevive = 0;
    public bool usedArtifactRevive = false;

    public int numReroll = 0;

    public int numArtifactReroll = 0;


    public List<DialogueConditions> savedDialogueConditionsMet = new List<DialogueConditions>();

    public List<string> savedInvalidConvos;
    public List<string> savedPastConvos;


    public int grimoirePagesOwned = 0;
    public int gemstonesOwned = 0;
    public int metalScrapsOwned = 0;
    public int etherVialsOwned = 0;
    public int etherShardsOwned = 0;
    public Floors highestFloorReached;

    public int F101;
    public int F102;
    public int F103;

    public int F201;
    public int F202;
    public int F203;

    public int F301;
    public int F302;
    public int F303;

    public int F401;
    public int F402;
    #endregion

    #region Chruch info to save
    public int numChurchInteraction = 0;
    public int churchAdmirationLevel = 0;
    public bool wentToChurchThisRun = false;
    public bool giftedToChurchThisRun = false;
    public int churchBoon = 0;
    public string churchBoonDesc = "";
    public int revengeanceTrait = 0;
    #endregion

    #region Academy Info to Save
    public int numAcademyInteraction = 0;
    public int academyAdmirationLevel = 0;
    public bool wentToAcademyThisRun = false;
    public bool giftedToAcademyThisRun = false;

    public int burnTurnIncrease = 0;
    public int shockedTurnIncrease = 0;
    public int vulnerableTurnIncrease = 0;
    public int damnedStackIncrease = 0;
    public int reviveIncrease = 0;
    public int maxHealthIncrease = 0;
    public int accuracyIncrease = 0;
    public int critChanceIncrease = 0;

    public int fireResistIncrease = 0;
    public int iceResistIncrease = 0;
    public int lightningResistIncrease = 0;
    public int psychicResistIncrease = 0;
    public int poisonResistIncrease = 0;
    public int actionIncrease = 0;
    public int entryHealthIncrease = 0;
    public int goldStart = 0;

    public int fireStatusChance = 0;
    public int iceStatusChance = 0;
    public int lightningStatusChance = 0;
    public int poisonStatusChance = 0;
    public int psychicStatusChance = 0;
    public int entryGoldIncrease = 0;
    public int attackLifesteal = 0;
    public int specialLifesteal = 0;

    public List<string> equippedBonuses;
    #endregion

    #region Blacksmith info to save
    public int blacksmithAdmirationLevel = 0;
    public int numBlacksmithInteraction = 0;
    public bool giftedToBlacksmithThisRun = false;
    public bool wentToBlacksmithThisRun = false;

    public int firestormWeaponRank = 0;
    public int melancoliaWeaponRank = 1;
    public int deltaWeaponRank = 0;
    public int renegadeWeaponRank = 0;

    public bool useMod;
    public WeaponMod weaponMod;
    #endregion

    #region shop info to save
    public bool wentShopkeepThisRun = false;
    public int numShopkeepInteraction = 0;
    public int combatShopInteraction = 0;
    #endregion

    #region bounty info to save
    public List<Bounty> equippedBounties = new List<Bounty>();
    public bool wentBountyThisRun = false;
    public int numBountyInteraction = 0;

    #endregion

    #region Home info to save
    public bool wentToTowerThisRun = false;
    public bool wentHomeThisRun = false;
    public int numHomeInteraction = 0;
    public List<string> cutscenesUnlocked = new List<string>();
    [SerializeField]
    public List<Attempts> attempts = new List<Attempts>();

    #endregion

    #region Enemy interaction info to save
    public int numEmpressInteraction = 0;
    public int numDeathToEmpress= 0;
    public int numHangedmanInteraction = 0;
    public int numDeathToHangedman = 0;
    public int numMagicianInteraction = 0;
    public int numDeathToMagician = 0;
    public int numFoolInteraction = 0;
    public int numDeathToFool = 0;
    public int numStarInteraction = 0;
    public int numDeathToStar = 0;
    public int numPriestessInteraction = 0;
    public int numDeathToPriestess = 0;
    public int numKingInteraction = 0;
    public int numDeathToKing = 0;

    #endregion

    #region Player menu options
    private bool loadoutStatus = false;
    #endregion


    private void Start()
    {
        loadoutStatus = true;
        ToggleLoadout();
    }

    private void Update()
    {
        hpText.text = "";
        if (inBattle)
        {
            hpText.text = "<sprite=45> " + h.hp + "/" + h.GetMaxHealth();
            gold.text = "<sprite=11> " + h.playerInventory.GetGoldAmount();
        }

        gem.text = "<sprite=9> " + gemstonesOwned;
        vials.text = "<sprite=7> " + etherVialsOwned;
        shards.text = "<sprite=5> " + etherShardsOwned;
        scraps.text = "<sprite=19> " + metalScrapsOwned;
        grimoire.text = "<sprite=13> " + grimoirePagesOwned;


        edenPng.SetActive(GetSuccessfulAttempts() > 0 && !savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed));

            
        if (inBattle && Input.GetKeyDown(KeyCode.Tab) && !PauseController.instance.isGamePaused())
        {
            //Show info
            ToggleLoadout();
        }
    }


    public TextMeshProUGUI physicalResist;
    public TextMeshProUGUI fireResist;
    public TextMeshProUGUI lightningResist;
    public TextMeshProUGUI iceResist;
    public TextMeshProUGUI poisonResist;
    public TextMeshProUGUI psychicResist;
    public TextMeshProUGUI divineResist;
    private void UpdateResistancesStats()
    {

        physicalResist.text = h.GetPhysicalResistance() ? "100" : "0";
        divineResist.text = h.GetDivineResistance() ? "100" : "0";
        fireResist.text = "" + h.GetBurningResistance();
        lightningResist.text = "" + h.GetShockedesistance();
        iceResist.text = "" + h.GetFrozenResistance();
        poisonResist.text = "" + h.GetPoisonResistance();
        psychicResist.text = "" + h.GetVulnerableResistance();
    }

    public void PlayerCanvasDisplayActive(bool val)
    {
        transform.GetChild(0).gameObject.SetActive(val);
    }

    public void PauseButton()
    {
        PauseController.instance.TogglePause();
    }

    public void SetInTower()
    {
        inBattle = true;
        modIcon.SetActive(UsingMod(selectedWeaponType));
        boonIcon.SetActive(churchBoon != 0);
        gold.gameObject.SetActive(true);
        artifactDisplayPanel.SetActive(true);
    }


    public void RenderRevives()
    {
        GameUtilities.DeleteAllChildGameObject(revivePanel);

        for (int x = 0; x < numRevives + numArtifactRevive; x++)
        {
            AddReviveIcon();
        }
    }

    public void RenderBounties()
    { 
        GameUtilities.DeleteAllChildGameObject(bountyPanel);

        for (int x = 0; x < equippedBounties.Count; x++)
        {
            var newObj = Instantiate(pfBountyHud);
            newObj.GetComponent<BountyHud>().InitBounty(equippedBounties[x]);
            newObj.transform.SetParent(bountyPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfBountyHud.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfBountyHud.GetComponent<RectTransform>().localScale;
        }
    }

    public void AddReviveIcon()
    {
        var newObj = Instantiate(pfRevive);
        newObj.transform.SetParent(revivePanel.transform);
        newObj.transform.SetAsFirstSibling();
        newObj.transform.localPosition = new Vector2(0, 0);
        newObj.GetComponent<RectTransform>().sizeDelta = pfRevive.GetComponent<RectTransform>().sizeDelta;
        newObj.GetComponent<RectTransform>().localScale = pfRevive.GetComponent<RectTransform>().localScale;
    }

    public void SetOutOfTower()
    {
        inBattle = false;
        modIcon.SetActive(false);
        gold.gameObject.SetActive(false);
        abilityDisplayPanel.transform.parent.gameObject.SetActive(false);
        artifactDisplayPanel.SetActive(false);
    }

    public void CheckBountyReward()
    {
        foreach(Bounty b in equippedBounties)
        {
            if (b.completed)
            {
                b.GetReward(this);
            }
        }
        equippedBounties.RemoveAll(item => item.completed == true);
    }

    public void SetGemstoneAmount(int val)
    {
        gemstonesOwned = Mathf.Clamp(val, 0, 999999);
    }

    public void ChangeGemstonesAmount(int val)
    {
        gemstonesOwned = Mathf.Clamp(gemstonesOwned + val, 0, 9999999);
    }

    public void SetEtherVialAmount(int val)
    {
        etherVialsOwned = Mathf.Clamp(val, 0, 999999);
    }

    public void ChangeVialsAmount(int val)
    {
        etherVialsOwned = Mathf.Clamp(etherVialsOwned + val, 0, 9999999);
    }

    public void SetEtherShardsAmount(int val)
    {
        etherShardsOwned = Mathf.Clamp(val, 0, 999999);
    }

    public void ChangeShardAmount(int val)
    {
        etherShardsOwned = Mathf.Clamp(etherShardsOwned + val, 0, 9999999);
    }

    public void SetMetalScrapsAmount(int val)
    {
        metalScrapsOwned = Mathf.Clamp(val, 0, 999999);
    }

    public void ChangeScrapsAmount(int val)
    {
        metalScrapsOwned = Mathf.Clamp(metalScrapsOwned + val, 0, 9999999);
    }

    public void SetGrimoirePageAmount(int val)
    {
        grimoirePagesOwned = Mathf.Clamp(val, 0, 999999);
    }

    public void ChangeGrimoirePageAmount(int val)
    {
        grimoirePagesOwned = Mathf.Clamp(grimoirePagesOwned + val, 0, 9999999);
    }

    public void AddArtifact(Artifact a)
    {
        h.playerInventory.AddArtifact(a);
        RenderArtifacts();
    }

   
    public void RemoveArtifact(Artifact a)
    {
        h.playerInventory.RemoveArtifact(a);
        RenderArtifacts();
    }

    public void AddSpecial(Special s)
    {

        h.AddSpecial(s);
        RenderLoadout();
    }

    public void RemoveSpecial(Special s)
    {
        h.RemoveSpecial(s);
        RenderLoadout();
    }

    private void RenderArtifacts()
    {
        GameUtilities.DeleteAllChildGameObject(artifactDisplayPanel);

        foreach (Artifact a in h.GetArtifacts())
        {
            var newObj = Instantiate(pfDisplayArtifact);
            newObj.GetComponent<UIDisplayArtifact>().Initialize(a, this);
            newObj.transform.SetParent(artifactDisplayPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfDisplayArtifact.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfDisplayArtifact.GetComponent<RectTransform>().localScale;
            
        }
    }

    public List<Consumable> GetConsumables()
    {
        List<Consumable> cList = new List<Consumable>(cityShopConsumables);
        cList.AddRange(h.playerInventory.combatShopConsumables);
        return cList;
    }

    public void RenderLoadout()
    {
        RenderAbilities();
        RenderConsumable();
        RenderAilment();
        RenderArtifacts();
        RenderBounties();
        CheckRevengeanceBonus();
        RenderEnchantmentLoadout();
    }

    public void RenderAbilities()
    {
        GameUtilities.DeleteAllChildGameObject(abilityDisplayPanel);

        if (h.baseAttack != null)
        {
            baseAttackDisplay.Initialize(h.baseAttack, this);
            baseAttackDisplay.gameObject.SetActive(true);
            baseAttackDisplay.ShowDesc();
        }

        foreach (IAbility a in h.specials)
        {
            var newObj = Instantiate(pfDisplayAbility);
            newObj.GetComponent<UiDisplayAbility>().Initialize(a, this);
            newObj.transform.SetParent(abilityDisplayPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfDisplayAbility.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfDisplayAbility.GetComponent<RectTransform>().localScale;

        }

    }

    public void RenderConsumable()
    {
        GameUtilities.DeleteAllChildGameObject(consumablePanel);


        foreach (Consumable a in GetConsumables())
        {
            var newObj = Instantiate(pfConsumableLoadout);
            newObj.GetComponent<ConsumableLoadout>().Init(this, a);
            newObj.transform.SetParent(consumablePanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfConsumableLoadout.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfConsumableLoadout.GetComponent<RectTransform>().localScale;

        }
    }

    public void RenderAilment()
    {
        GameUtilities.DeleteAllChildGameObject(ailmentPanel);
        foreach(Ailment a in h.playerInventory.ailments)
        {
            var newObj = Instantiate(pfAilmentItem);
            newObj.transform.SetParent(ailmentPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfAilmentItem.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfAilmentItem.GetComponent<RectTransform>().localScale;
            newObj.GetComponent<AilmentItem>().AssignAilment(a);
        }

    }

    public void AddAilment()
    {
        Ailment thisAilment = new Ailment(this, h.playerInventory.ailmentDiffMod);
        if (h.playerInventory.AddAilment(thisAilment))
        {
            RenderLoadout();
        }
    }

    public void ShowModDesc()
    {
        statusChance.gameObject.SetActive(false);
        turns.gameObject.SetActive(false);
        abilityname.text = "";
        turns.text = "";
        statusChance.text = "";
        numTarget.text = "";
        abilityRank.text = "";
        description.text = weaponMod.GetDesc() + "\n(Experimental weapon modifications.)"; 

    }
    public void ShowBoonDesc()
    {
        statusChance.gameObject.SetActive(false);
        turns.gameObject.SetActive(false);
        abilityname.text = "";
        turns.text = "";
        statusChance.text = "";
        abilityRank.text = "";
        numTarget.text = "";
        description.text = churchBoonDesc + "\n(The Church of the Star thank you for your gift.)";

    }
    
    public void ShowRevengeanceDesc()
    {
        statusChance.gameObject.SetActive(false);
        turns.gameObject.SetActive(false);
        abilityname.text = "";
        turns.text = "";
        statusChance.text = "";
        abilityRank.text = "";
        numTarget.text = "";
        description.text = "Heal 5 <sprite=45> for every enemy defeated for the next " + revengeanceTrait + " encounter(s). \n(A good death is not a failure, but a gift.)";
    }

    public void CheckRevengeanceBonus()
    {
        revengeanceIcon.SetActive(revengeanceTrait > 0);
    }

    public void PlayerReset(bool resetAfterTower)
    {
        if(resetAfterTower)
        {
            cityShopConsumables = new List<Consumable>();
            churchBoon = 0;
        }

        CheckBountyReward();
        SetOutOfTower();
        h.HeroDeathReset();
        RenderLoadout();
        GameUtilities.DeleteAllChildGameObject(revivePanel);
        healthOnExit = 5;
        goldOnExit = 20;
        numReroll = 0;
        numArtifactReroll = 0;
        numRevives = 0;
        numArtifactRevive = 0;
        usedArtifactRevive = false;
        loadoutStatus = true;
        ToggleLoadout();
    }

    public int GetWeaponRank(WeaponTypes weapon)
    {
        switch(weapon)
        {
            case WeaponTypes.Melancolia:
                return melancoliaWeaponRank;
            case WeaponTypes.Delta:
                return deltaWeaponRank;
            case WeaponTypes.Firestorm:
                return firestormWeaponRank;
            case WeaponTypes.Renegade:
                return renegadeWeaponRank;
        }
        return 0;
    }

    public void ToggleLoadout()
    {
        loadoutStatus = !loadoutStatus;
        CheckRevengeanceBonus();
        ailmentPanel.transform.parent.gameObject.SetActive(loadoutStatus);
        abilityDisplayPanel.transform.parent.gameObject.SetActive(loadoutStatus);
        abilityDisplayPanel.transform.parent.parent.GetChild(0).gameObject.SetActive(loadoutStatus);
        edenPng.transform.parent.gameObject.SetActive(loadoutStatus);
        UpdateResistancesStats();
    }

    public int swordBonus = 1;
    public int gunBonus= 3;
    public int fistBonus = 3;
    public int staffBonus = 3;

    public void GetStartingLoadout()
    {
        RenderRevives();
        RenderBounties();
        Attempts last = attempts[attempts.Count-2];
        if (!last.reachedTop && !last.surrender) revengeanceTrait = last.numberOfEncounters / 2;
        List<Attack> attacks = GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().allAttacks;
        h.baseAttack = Instantiate(attacks[Random.Range(0, attacks.Count - 1)]);
        int r = (int)h.baseAttack.GetRarity();
        int rank = GetWeaponRank(selectedWeaponType);
        r = Mathf.Clamp(r + rank, r, 5);
        h.baseAttack.SetRarity((Rarity)r);
        if (rank >= 5)
        {
            h.baseAttack.LevelUp();
            List<System.Action> popups = new List<System.Action>();
            popups.Add(() => h.baseAttack.TriggerNoHit(popupWindow.transform, h.baseAttack.GetName() + ": +1 Rank!", 12));
            GameUtilities.ShowLevelUpPopup(this, popups);        
        }

        if(rank > 1)
        {
            switch (selectedWeaponType)
            {
                case WeaponTypes.Melancolia:
                    h.GetToughness().AddModifier(new StatModifier(swordBonus * rank, StatModType.Flat, this));
                    break;
                case WeaponTypes.Delta:
                    h.GetCritChance().AddModifier(new StatModifier(gunBonus * rank, StatModType.Flat, this));
                    break;
                case WeaponTypes.Firestorm:
                    h.battleFireStatus.AddModifier(new StatModifier(staffBonus * rank, StatModType.Flat, this));
                    h.battleIceStatus.AddModifier(new StatModifier(staffBonus * rank, StatModType.Flat, this));
                    h.battleLightningStatus.AddModifier(new StatModifier(staffBonus * rank, StatModType.Flat, this));
                    h.battlePoisonStatus.AddModifier(new StatModifier(staffBonus * rank, StatModType.Flat, this));
                    h.battlePsychicStatus.AddModifier(new StatModifier(staffBonus * rank, StatModType.Flat, this));
                    break;
                case WeaponTypes.Renegade:
                    h.battleFireResist.AddModifier(new StatModifier(fistBonus * rank, StatModType.Flat, this));
                    h.battleIceResist.AddModifier(new StatModifier(fistBonus * rank, StatModType.Flat, this));
                    h.battleLightningResist.AddModifier(new StatModifier(fistBonus * rank, StatModType.Flat, this));
                    h.battlePoisonResist.AddModifier(new StatModifier(fistBonus * rank, StatModType.Flat, this));
                    h.battlePsychicResist.AddModifier(new StatModifier(fistBonus * rank, StatModType.Flat, this));
                    break;
            }
        }

        GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().ResetCollection();
        List<Special> tlist = GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().allSpecialsToUse;
        tlist = tlist.OrderBy(a => System.Guid.NewGuid()).ToList();
        for (int x = 0; x < GameController.GetStartingAbilitiesCount(this) && x < tlist.Count; x++)
        {
            int rarityBonus = cityShopConsumables.Exists(a => a.consumableIntValue == 2) ? 5 : 0;
            Special s = tlist[x];
            int max = Random.Range(0, 100) < (5*rank) + rarityBonus ? 5 : 4;
            s.SetRarity((Rarity)Random.Range(2, max));
            AddSpecial(s);
            bool removed = GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().RemoveSpecialFromCollection(s);
        }
        if (cityShopConsumables.Exists(a => a.consumableIntValue == 3))
        {
            AddArtifact(GameController.GetArtifactByRarity(Rarity.Common));
        }

        h.playerInventory.ActivateLateWeaponMod();
    }

    public void ResetHubStats()
    {
        wentShopkeepThisRun = false;
        wentHomeThisRun = false;
        wentToAcademyThisRun = false;
        wentToBlacksmithThisRun = false;
        wentToChurchThisRun = false;
        wentToTowerThisRun = false;

        giftedToAcademyThisRun = false;
        giftedToBlacksmithThisRun = false;
        giftedToChurchThisRun = false;
    }

    public int GetSuccessfulAttempts()
    {
        return attempts
             .Where(item => item.reachedTop && !item.surrender)
             .ToList().Count;
    }

    public int bestStreak = 0;
    public int GetCurrentStreak()
    {
        int streak = 0;
        for (int x  = 0; x < attempts.Count - 1; x++)
        {
            if(attempts[x].reachedTop && !attempts[x].surrender)
            {
                streak++;
            } else
            {
                if (streak > bestStreak) bestStreak = streak;
                streak = 0;
            }
        }
        if (streak > bestStreak) bestStreak = streak;
        return streak;
    }

    public int GetFailedAttempts()
    {
        return attempts
             .Where(item => !item.reachedTop && !item.surrender)
             .ToList().Count;
    }

    public int GetSurrenderedAttempts()
    {
        return attempts
             .Where(item => !item.reachedTop && item.surrender)
             .ToList().Count;
    }
    
    public int GetTotalAttempts()
    {
        return attempts.Count;
    }

    public bool HasUsedAbility(string abilityName)
    {
        foreach(Attempts attempt in attempts)
        {
            if(attempt.abilitiesName.Count > 0 && attempt.abilitiesName.Contains(abilityName)) return true;
        }
        return false;
    }

    public Attempts GetLastAttempt()
    {
        return attempts.Last();
    }

    public bool HasUsedArtifact(Artifact artifact)
    {
        foreach (Attempts attempt in attempts)
        {
            if (attempt.artifactsNames.Count > 0 && attempt.artifactsNames.Contains(artifact.GetName())) return true;
        }
        return false;
    }

    public bool HasDefeatedEnemy(string enemyNpc)
    {
        foreach (Attempts attempt in attempts)
        {
            if (attempt.defeatedEnemies.Count > 0 && attempt.defeatedEnemies.Contains(enemyNpc)) return true;
        }
        return false;
    }

    public int TotalTimeDefeatedEnemy(string enemyNpc)
    {
        int res = 0;
        foreach (Attempts attempt in attempts)
        {
            foreach(string s in attempt.defeatedEnemies)
            {
                if (s.Equals(enemyNpc)) res++;
            }
        }
        return res;
    }

    public bool UsingMod(WeaponTypes weapom)
    {
        bool modAvailable = false;

        switch (weapom)
        {
            case WeaponTypes.Melancolia:
                modAvailable = savedDialogueConditionsMet.Contains(DialogueConditions.UnlockMelancoliaMod);
                break;
            case WeaponTypes.Delta:
                modAvailable = savedDialogueConditionsMet.Contains(DialogueConditions.UnlockDeltaMod);
                break;
            case WeaponTypes.Firestorm:
                modAvailable = savedDialogueConditionsMet.Contains(DialogueConditions.UnlockFirestormMod);
                break;
            case WeaponTypes.Renegade:
                modAvailable = savedDialogueConditionsMet.Contains(DialogueConditions.UnlockRenegadeMod);
                break;
        }

        return useMod && modAvailable;
    }

    public GameObject deathBarkWindow;
    public void TriggerDeathBark()
    {
        List<DialogueBlock> barkList = new List<DialogueBlock>(h.GetNpc().onPlayerDeathBark);
        if(savedDialogueConditionsMet.Contains(DialogueConditions.StarRevealed))
        {
            barkList.RemoveAll(d => d.priority == DialoguePriority.Critical);
        }
        
        if(GetSuccessfulAttempts() <= 0)
        {
            barkList.RemoveAll(d => d.priority == DialoguePriority.High);
            barkList.RemoveAll(d => d.priority == DialoguePriority.Medium);
        } else if (!savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed))
        {
            barkList.RemoveAll(d => d.priority == DialoguePriority.High);
        } else if (savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed)) 
        {
            barkList.RemoveAll(d => d.priority == DialoguePriority.Medium);
        }

        int index = GetTotalAttempts() < 2 ? 2 : Random.Range(0, barkList.Count);
        int chance = GetTotalAttempts() < 2 ? 100 : 20;
        DialogueBlock bark = barkList[index];
        deathBarkWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bark.dialogueBlock;
        deathBarkWindow.SetActive(Random.Range(1, 101) <= chance);

    }

    private void RenderEnchantmentLoadout()
    {
        for(int x = 0; x < equippedBonuses.Count; x++)
        {
            enchantmentPanel.transform.GetChild(x).GetChild(0).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName(equippedBonuses[x]);
            enchantmentPanel.transform.GetChild(x).gameObject.SetActive(true);
        }
    }

    public GameObject enchantmentPanel;
    public void GetEnchantmentDesc(int index)
    {
        abilityname.text = GameUtilities.GetEnchantmentName(equippedBonuses[index]);
        description.text = GameUtilities.GetEnchantmentDesc(equippedBonuses[index]);
        description.text += "\n Total Bonus: +" + GetEnchantmentBonusAmount(equippedBonuses[index]);
        turns.text = "";
        statusChance.text = "";
        numTarget.text = "";
        abilityRank.text = "";
    }

    public int GetEnchantmentBonusAmount(string bonus)
    {
        switch (bonus)
        {
            case "burnBonus":
                return burnTurnIncrease;
            case "shockedBonus":
                return shockedTurnIncrease;
            case "vulnerableBonus":
                return vulnerableTurnIncrease;
            case "damnedBonus":
                return damnedStackIncrease;
            case "reviveBonus":
                return reviveIncrease;
            case "maxHealthBonus":
                return maxHealthIncrease;
            case "accuracyBonus":
                return accuracyIncrease;
            case "critChanceBonus":
                return critChanceIncrease;

            case "fireResistBonus":
                return fireResistIncrease;
            case "iceResistBonus":
                return iceResistIncrease;
            case "lightningResistBonus":
                return lightningResistIncrease;
            case "psychicResistBonus":
                return psychicResistIncrease;
            case "poisonResistBonus":
                return poisonResistIncrease;
            case "actionBonus":
                return actionIncrease;
            case "entryHealthBonus":
                return entryHealthIncrease;
            case "goldStart":
                return goldStart;

            case "fireChanceBonus":
                return fireStatusChance;
            case "iceChanceBonus":
                return iceStatusChance;
            case "poisonChanceBonus":
                return poisonStatusChance;
            case "lightningChanceBonus":
                return lightningStatusChance;
            case "psychicChanceBonus":
                return psychicStatusChance;
            case "entrygoldBonus":
                return entryGoldIncrease;
            case "attackLifestealBonus":
                return attackLifesteal;
            case "specialLifestealBonus":
                return specialLifesteal;
        }
        return -1;
    }
}




