using System;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData  
{

    public PlayerData(PlayerManager player)
    {
        SaveBlacksmithInfo(player);
        SaveChurchInfo(player);
        SaveAcademyInfo(player);
        SaveBountyAndShopInfo(player);
        SaveHomeInfo(player);
        SaveEnemyInfo(player);

        SaveManagerInfo(player);
    }


   

    #region
    //Player manager info to save
    public string lastPlayed;
    public string selectedWeaponType;
    public int gemstonesOwned = 0;
    public int etherVialsOwned = 0;
    public int grimoirePagesOwned = 0;
    public int scrapsOwned = 0;
    public int shardsOwned = 0;
    public string[] dialogueConditionsMet;
    public string[] invalidConvos;
    public string[] pastConvos;
    public string lastKilled;
    public bool markDoor;
    public bool encounterDoor;
    public bool enteredDoor;
    public int streak = 0;
    public int difficulty = 0;
    public int difficultyWon = 0;
    public int highestFloor = 0;
    public int[,] consumables;
    public bool mod;

    public int f101;
    public int f102;
    public int f103;

    public int f201;
    public int f202;
    public int f203;

    public int f301;
    public int f302;
    public int f303;

    public int f401;
    public int f402;
    #endregion
    private void SaveManagerInfo(PlayerManager player)
    {
        mod = player.useMod;
        difficulty = player.chosenDifficulty;
        difficultyWon = player.highestDifficultyWon;
        highestFloor = Mathf.Clamp((int)player.highestFloorReached, 2, 7);
        player.savedDialogueConditionsMet = player.savedDialogueConditionsMet.Distinct().ToList();
        dialogueConditionsMet = new string[player.savedDialogueConditionsMet.Count];
        player.savedInvalidConvos = player.savedInvalidConvos.Distinct().ToList();
        invalidConvos = new string[player.savedInvalidConvos.Count];
        player.savedPastConvos = player.savedPastConvos.Distinct().ToList();
        pastConvos = new string[player.savedPastConvos.Count];
        int index = 0;
        foreach (DialogueConditions dc in player.savedDialogueConditionsMet)
        {
            dialogueConditionsMet[index] = dc.ToString();
            index++;
        }

        index = 0;
        foreach (string st in player.savedInvalidConvos)
        {
            invalidConvos[index] = st;
            index++;
        }

        index = 0;
        foreach (string st in player.savedPastConvos)
        {
            pastConvos[index] = st;
            index++;
        }

        selectedWeaponType = player.selectedWeaponType.ToString();
        gemstonesOwned = player.gemstonesOwned;
        etherVialsOwned = player.etherVialsOwned;
        grimoirePagesOwned = player.grimoirePagesOwned;
        scrapsOwned = player.metalScrapsOwned;
        shardsOwned = player.etherShardsOwned;


        lastPlayed = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");

        consumables = new int[player.cityShopConsumables.Count, 2];
        //saving consumables
        for(int x = 0; x < player.cityShopConsumables.Count; x++)
        {
            Consumable c = player.cityShopConsumables[x];
            consumables[x, 0] = c.consumableIntValue;
            consumables[x, 1] = c.amount;
        }

        f101 = player.F101;
        f102 = player.F102;
        f103 = player.F103;
        
        f201 = player.F201;
        f202 = player.F202;
        f203 = player.F203;

        f301 = player.F301;
        f302 = player.F302;
        f303 = player.F303;
        
        f401 = player.F401;
        f402 = player.F402;
    
    }

    #region
    //Church info to save
    public int churchInteraction = 0;
    public int churchAdmiration = 0;
    public bool talkedToChurch;
    public bool giftedChurch;
    public int boonFromChurch;
    public string boonDescFromChurch;
    public int revengeance = 0;
    #endregion
    private void SaveChurchInfo(PlayerManager player)
    {
        streak = player.bestStreak;
        churchInteraction = player.numChurchInteraction;
        talkedToChurch = player.wentToChurchThisRun;
        giftedChurch = player.giftedToChurchThisRun;
        churchAdmiration = player.churchAdmirationLevel;
        boonFromChurch = player.churchBoon;
        boonDescFromChurch = player.churchBoonDesc;
        boonDescFromChurch = player.churchBoonDesc;
        revengeance = player.revengeanceTrait;
    }

    #region
    //Academy Info to Save
    public int burnTurnIncrease = 0;
    public int shockedTurnIncrease = 0;
    public int vulnerableTurnIncrease = 0;
    public int damnedStackIncrease = 0;
    public int reviveIncrease = 0;
    public int maxHealthIncrease = 0;
    public int hitChanceIncrease = 0;
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

    public string[] equippedBonuses;

    public int academyInteraction = 0;
    public int academyAdmiration = 0;
    public bool talkedToAcademy;
    public bool giftedAcademy;

    #endregion
    private void SaveAcademyInfo(PlayerManager player)
    {
        burnTurnIncrease = player.burnTurnIncrease;
        shockedTurnIncrease = player.shockedTurnIncrease;
        vulnerableTurnIncrease = player.vulnerableTurnIncrease;
        damnedStackIncrease = player.damnedStackIncrease;
        reviveIncrease = player.reviveIncrease;
        maxHealthIncrease = player.maxHealthIncrease;
        hitChanceIncrease = player.accuracyIncrease;
        critChanceIncrease = player.critChanceIncrease;

        fireResistIncrease = player.fireResistIncrease;
        iceResistIncrease = player.iceResistIncrease;
        lightningResistIncrease = player.lightningResistIncrease;
        psychicResistIncrease = player.psychicResistIncrease;
        poisonResistIncrease = player.poisonResistIncrease;
        actionIncrease = player.actionIncrease;
        entryHealthIncrease = player.entryHealthIncrease;
        goldStart = player.goldStart;

        fireStatusChance = player.fireStatusChance;
        iceStatusChance = player.iceStatusChance;
        lightningStatusChance = player.lightningStatusChance;
        poisonStatusChance = player.poisonStatusChance;
        psychicStatusChance = player.psychicStatusChance;
        entryGoldIncrease = player.entryGoldIncrease;
        attackLifesteal = player.attackLifesteal;
        specialLifesteal = player.specialLifesteal;


        equippedBonuses = new string[player.equippedBonuses.Count];


        academyInteraction = player.numAcademyInteraction;
        academyAdmiration = player.academyAdmirationLevel;
        talkedToAcademy = player.wentToAcademyThisRun;
        giftedAcademy = player.giftedToAcademyThisRun;

        int index = 0;
        foreach(string s in player.equippedBonuses)
        {
            equippedBonuses[index] = s;
            index++;
        }

    }

    #region
    //Blacksmith info to save
    public int mageWeaponRank = 0;
    public int fighterWeaponRank = 0;
    public int engineerWeaponRank = 0;
    public int brawlerWeaponRank = 0;
    public int blacksmithInteraction = 0;
    public int blacksmithAdmiration = 0;
    public bool talkedToBlacksmith;
    public bool giftedBlacksmith;

    #endregion
    private void SaveBlacksmithInfo(PlayerManager player)
    {
        mageWeaponRank = player.firestormWeaponRank;
        fighterWeaponRank = player.melancoliaWeaponRank;
        engineerWeaponRank = player.deltaWeaponRank;
        brawlerWeaponRank = player.renegadeWeaponRank;
        blacksmithInteraction = player.numBlacksmithInteraction;
        blacksmithAdmiration = player.blacksmithAdmirationLevel;
        talkedToBlacksmith = player.wentToBlacksmithThisRun;
        giftedBlacksmith = player.giftedToBlacksmithThisRun;
    }

    #region
    //barkeep info to save
    public int shopInteraction = 0;
    public bool talkedToShop;
    public int bountyInteraction = 0;
    public bool visitedBounty;
    public int[,] bounties;
    public string[,] bountyInfo;
    public int combatShopNum = 0;
    #endregion
    private void SaveBountyAndShopInfo(PlayerManager player)
    {
        shopInteraction = player.numShopkeepInteraction;
        talkedToShop = player.wentShopkeepThisRun;
        bountyInteraction = player.numBountyInteraction;
        visitedBounty = player.wentBountyThisRun;
        combatShopNum = player.combatShopInteraction;
        if(player.equippedBounties != null)
        {
            bounties = new int[player.equippedBounties.Count, 5];
            bountyInfo = new string[player.equippedBounties.Count, 5];
            int index = 0;
            foreach(Bounty b in player.equippedBounties)
            {
                int completed = 0;
                if (b.completed) completed = 1;

                bounties[index, 0] = b.bountyType;
                bounties[index, 1] = b.tier;
                bounties[index, 2] = completed;
                bounties[index, 3] = (int)b.bountyRewards;
                bounties[index, 4] = b.reward;

                switch (b.bountyType)
                {
                    case 1:
                        bountyInfo[index, 0] = b.damageTakenCondition + "";
                        bountyInfo[index, 1] = b.damageTakenCondition + "";
                        bountyInfo[index, 2] = b.damageTakenCondition + "";
                        bountyInfo[index, 3] = b.damageTakenCondition + "";
                        bountyInfo[index, 4] = b.damageTakenCondition + "";
                        break;
                    case 2:
                        bountyInfo[index, 0] = b.damageDealtCondition + "";
                        bountyInfo[index, 1] = b.damageDealtCondition + "";
                        bountyInfo[index, 2] = b.damageDealtCondition + "";
                        bountyInfo[index, 3] = b.damageDealtCondition + "";
                        bountyInfo[index, 4] = b.damageDealtCondition + "";
                        break;
                    case 3:
                        bountyInfo[index, 0] = b.turnLimitCondition + "";
                        bountyInfo[index, 1] = b.turnLimitCondition + "";
                        bountyInfo[index, 2] = b.turnLimitCondition + "";
                        bountyInfo[index, 3] = b.turnLimitCondition + "";
                        bountyInfo[index, 4] = b.turnLimitCondition + "";
                        break;
                    case 4:
                        bountyInfo[index, 0] = b.floorCondition.ToString();
                        bountyInfo[index, 1] = b.floorCondition.ToString();
                        bountyInfo[index, 2] = b.floorCondition.ToString();
                        bountyInfo[index, 3] = b.floorCondition.ToString();
                        bountyInfo[index, 4] = b.floorCondition.ToString();
                        break;
                    case 5:
                        int count = 0;
                        foreach(string s in b.academyBuffCondition)
                        {
                            bountyInfo[index, count] = s;
                            count++;
                        }
                        for(int x = count; x < 5; x++)
                        {
                            bountyInfo[index, count] = "";
                            count++;
                        }
                        break;
                    case 6:
                        bountyInfo[index, 0] = b.weaponCondition.ToString();
                        bountyInfo[index, 1] = b.weaponCondition.ToString();
                        bountyInfo[index, 2] = b.weaponCondition.ToString();
                        bountyInfo[index, 3] = b.weaponCondition.ToString();
                        bountyInfo[index, 4] = b.weaponCondition.ToString();
                        break;
                    case 7:
                        bountyInfo[index, 0] = b.affectCondition.ToString();
                        bountyInfo[index, 1] = b.affectTypeCondition.ToString();
                        bountyInfo[index, 2] = b.affectCondition.ToString();
                        bountyInfo[index, 3] = b.affectCondition.ToString();
                        bountyInfo[index, 4] = b.affectCondition.ToString();
                        break;
                    case 8:
                        bountyInfo[index, 0] = b.damageDealtCondition.ToString();
                        bountyInfo[index, 1] = b.damageTypeCondition.ToString();
                        bountyInfo[index, 2] = b.damageDealtCondition.ToString();
                        bountyInfo[index, 3] = b.damageDealtCondition.ToString();
                        bountyInfo[index, 4] = b.damageDealtCondition.ToString();
                        break;
                    case 9:
                        bountyInfo[index, 0] = b.critCondition.ToString();
                        bountyInfo[index, 1] = b.critCondition.ToString();
                        bountyInfo[index, 2] = b.critCondition.ToString();
                        bountyInfo[index, 3] = b.critCondition.ToString();
                        bountyInfo[index, 4] = b.critCondition.ToString();
                        break;
                    case 10:
                        bountyInfo[index, 0] = b.avoidCondition.ToString();
                        bountyInfo[index, 1] = b.avoidCondition.ToString();
                        bountyInfo[index, 2] = b.avoidCondition.ToString();
                        bountyInfo[index, 3] = b.avoidCondition.ToString();
                        bountyInfo[index, 4] = b.avoidCondition.ToString();
                        break;
                    case 11:
                        bountyInfo[index, 0] = b.healCondition.ToString();
                        bountyInfo[index, 1] = b.healCondition.ToString();
                        bountyInfo[index, 2] = b.healCondition.ToString();
                        bountyInfo[index, 3] = b.healCondition.ToString();
                        bountyInfo[index, 4] = b.healCondition.ToString();
                        break;
                }

                index++;
            }
        }
       
    }

    #region
    //Home info to save
    public int homeInteraction = 0;
    public int homeAdmiration = 0;
    public bool talkedToHome;
    public bool talkedToTower;
    public bool giftedHome;
    public string[] cutscenes;

    public int[] attemptNumEnc;
    public bool[] attemptReachedTop;
    public bool[] attemptSurrender;
    public string[] attemptKilledByName;
    public string[] attemptMod;
    public int[] attemptFloor;
    public int[] attemptWeapon;
    public int[] attemptDifficulty;

    public string[][] attemptAbilitiesName;
    public string[][] attemptAbilitiesRank;
    public string[][] attemptArtifactsNames;
    public string[][] attemptEnchanments;
    public string[][] attemptDefeatedEnemies;
    public int[][] attemptArtifactsSpriteIndex;
    public int[][] attemptAbilitiesSpriteIndex;

    public int attemptCount = 0;
    #endregion

    private void SaveHomeInfo(PlayerManager player)
    {
        homeInteraction = player.numHomeInteraction;
        talkedToHome = player.wentHomeThisRun;
        talkedToTower = player.wentToTowerThisRun;

        player.cutscenesUnlocked = player.cutscenesUnlocked.Distinct().ToList();
        cutscenes = new string[player.cutscenesUnlocked.Count];
        int x = 0;
        foreach (string n in player.cutscenesUnlocked)
        {
            cutscenes[x] = n;
            x++;
        }

        //save attempts
        attemptCount = player.attempts.Count;


        //Initialize arrays
        attemptNumEnc = new int[attemptCount];
        attemptReachedTop = new bool[attemptCount];
        attemptSurrender = new bool[attemptCount];
        attemptKilledByName = new string[attemptCount];
        attemptFloor = new int[attemptCount];
        attemptWeapon = new int[attemptCount];
        attemptDifficulty = new int[attemptCount];
        attemptMod = new string[attemptCount];

        attemptAbilitiesName = new string[attemptCount][];
        attemptAbilitiesRank = new string[attemptCount][];
        attemptArtifactsNames = new string[attemptCount][];
        attemptEnchanments = new string[attemptCount][];
        attemptDefeatedEnemies = new string[attemptCount][];
        attemptArtifactsSpriteIndex = new int[attemptCount][];
        attemptAbilitiesSpriteIndex = new int[attemptCount][];

        //Assign values to arrays
        x = 0;
        foreach (Attempts attempt in player.attempts)
        {
            attemptNumEnc[x] = attempt.numberOfEncounters;
            attemptReachedTop[x] = attempt.reachedTop;
            attemptSurrender[x] = attempt.surrender;
            attemptKilledByName[x] = attempt.killedByName;
            attemptFloor[x] = (int)attempt.floor;
            attemptWeapon[x] = (int)attempt.weapon;
            attemptDifficulty[x] = (int)attempt.difficulty;
            attemptMod[x] = attempt.mod;


            attemptAbilitiesName[x] = attempt.abilitiesName.ToArray();
            attemptAbilitiesRank[x] = attempt.abilitiesRank.ToArray();
            attemptArtifactsNames[x] = attempt.artifactsNames.ToArray();
            attemptEnchanments[x] = attempt.enchanments.ToArray();
            attemptDefeatedEnemies[x] = attempt.defeatedEnemies.ToArray();
            attemptArtifactsSpriteIndex[x] = attempt.artifactsSpriteIndex.ToArray();
            attemptAbilitiesSpriteIndex[x] = attempt.abilitiesSpriteIndex.ToArray();
            x++;
        }
    }

    #region
    //Enemy info to save
    public int empressInteraction = 0;
    public int deathToEmpress = 0;
    public int hangedmanInteraction = 0;
    public int deathToHangedman = 0;
    public int magicianInteraction = 0;
    public int deathToMagician = 0;
    public int foolInteraction = 0;
    public int deathToFool = 0;
    public int starInteraction = 0;
    public int deathToStar = 0;
    public int priestessInteraction = 0;
    public int deathToPriestess = 0;
    public int kingInteraction = 0;
    public int deathToKing = 0;
    #endregion

    private void SaveEnemyInfo(PlayerManager player)
    {
        empressInteraction = player.numEmpressInteraction;
        deathToEmpress = player.numDeathToEmpress;
        hangedmanInteraction = player.numHangedmanInteraction;
        deathToHangedman = player.numDeathToHangedman;
        magicianInteraction = player.numMagicianInteraction;
        deathToMagician = player.numDeathToMagician;
        foolInteraction = player.numFoolInteraction;
        deathToFool = player.numDeathToFool;
        starInteraction = player.numStarInteraction;
        deathToStar = player.numDeathToStar;
        priestessInteraction = player.numPriestessInteraction;
        deathToPriestess = player.numDeathToPriestess;
        kingInteraction = player.numKingInteraction;
        deathToKing = player.numDeathToKing;
    }


}
