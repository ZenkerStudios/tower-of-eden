using System.Collections.Generic;
using System.Linq;

public class SaveUnpacker
{
    PlayerManager player;

    public SaveUnpacker(PlayerData data, PlayerManager p)
    {
        player = p;
        UnpackPlayer(data);
        UnpackChurch(data);
        UnpackBlacksmith(data);
        UnpackBountyAndShop(data);
        UnpackHome(data);
        UnpackEnemy(data);

        UnpackAcademy(data);

    }


    private void UnpackPlayer(PlayerData data)
    {
        player.chosenDifficulty = data.difficulty;
        player.highestDifficultyWon = data.difficultyWon;
        player.highestFloorReached = (Floors)data.highestFloor;
        player.selectedWeaponType = EnumConstants.ParseEnum<WeaponTypes>(data.selectedWeaponType);
        player.gemstonesOwned = data.gemstonesOwned;
        player.etherVialsOwned = data.etherVialsOwned;
        player.metalScrapsOwned = data.scrapsOwned;
        player.grimoirePagesOwned = data.grimoirePagesOwned;
        player.etherShardsOwned = data.shardsOwned;
        player.useMod = data.mod;
        player.bestStreak = data.streak;


        foreach (string dc in data.dialogueConditionsMet)
        {
            player.savedDialogueConditionsMet.Add(EnumConstants.ParseEnum<DialogueConditions>(dc));
        }

        foreach (string s in data.invalidConvos)
        {
            player.savedInvalidConvos.Add(s);
        }

        foreach (string s in data.pastConvos)
        {
            player.savedPastConvos.Add(s);
        }

        for (int x = 0; x < data.consumables.GetLength(0); x++)
        {
            int consumableVal = data.consumables[x, 0];
            Consumable c = new Consumable(consumableVal, consumableVal + 1, data.consumables[x, 1]);
            c.owner = player;
            player.cityShopConsumables.Add(c);
        }

        player.F101 = data.f101;
        player.F102 = data.f102;
        player.F103 = data.f103;

        player.F201 = data.f201;
        player.F202 = data.f202;
        player.F203 = data.f203;

        player.F301 = data.f301;
        player.F302 = data.f302;
        player.F303 = data.f303;

        player.F401 = data.f401;
        player.F402 = data.f402;
    }

    private void UnpackChurch(PlayerData data)
    {
        player.numChurchInteraction = data.churchInteraction; 
        player.churchAdmirationLevel = data.churchAdmiration;
        player.wentToChurchThisRun = data.talkedToChurch;
        player.giftedToChurchThisRun = data.giftedChurch;
        player.churchBoon = data.boonFromChurch;
        player.churchBoonDesc = data.boonDescFromChurch;
        player.revengeanceTrait = data.revengeance;
    }

    private void UnpackBlacksmith(PlayerData data)
    {
        player.firestormWeaponRank = data.mageWeaponRank;
        player.melancoliaWeaponRank = data.fighterWeaponRank;
        player.deltaWeaponRank = data.engineerWeaponRank;
        player.renegadeWeaponRank = data.brawlerWeaponRank;
        player.numBlacksmithInteraction = data.blacksmithInteraction;
        player.blacksmithAdmirationLevel = data.blacksmithAdmiration;
        player.wentToBlacksmithThisRun = data.talkedToBlacksmith;
        player.giftedToBlacksmithThisRun = data.giftedBlacksmith;
    }

    private void UnpackAcademy(PlayerData data)
    {
        player.burnTurnIncrease = data.burnTurnIncrease;
        player.shockedTurnIncrease = data.shockedTurnIncrease;
        player.vulnerableTurnIncrease = data.vulnerableTurnIncrease;
        player.damnedStackIncrease = data.damnedStackIncrease;
        player.reviveIncrease = data.reviveIncrease;
        player.maxHealthIncrease = data.maxHealthIncrease;
        player.accuracyIncrease = data.hitChanceIncrease;
        player.critChanceIncrease = data.critChanceIncrease;

        player.fireResistIncrease = data.fireResistIncrease;
        player.iceResistIncrease = data.iceResistIncrease;
        player.lightningResistIncrease = data.lightningResistIncrease;
        player.psychicResistIncrease = data.psychicResistIncrease;
        player.poisonResistIncrease = data.poisonResistIncrease;
        player.actionIncrease = data.actionIncrease;
        player.entryHealthIncrease = data.entryHealthIncrease;
        player.goldStart = data.goldStart;

        player.fireStatusChance = data.fireStatusChance;
        player.iceStatusChance = data.iceStatusChance;
        player.lightningStatusChance = data.lightningStatusChance;
        player.poisonStatusChance = data.poisonStatusChance;
        player.psychicStatusChance = data.psychicStatusChance;
        player.entryGoldIncrease = data.entryGoldIncrease;
        player.attackLifesteal = data.attackLifesteal;
        player.specialLifesteal = data.specialLifesteal;

        player.wentToAcademyThisRun = data.talkedToAcademy;
        player.giftedToAcademyThisRun = data.giftedAcademy;
        player.numAcademyInteraction = data.academyInteraction;
        player.academyAdmirationLevel = data.academyAdmiration;

        foreach (string s in data.equippedBonuses)
        {
            player.equippedBonuses.Add(s);
        }

    }

    private void UnpackBountyAndShop(PlayerData data)
    {
        player.combatShopInteraction = data.combatShopNum;
        player.wentShopkeepThisRun = data.talkedToShop;
        player.numShopkeepInteraction = data.shopInteraction;
        player.wentBountyThisRun = data.visitedBounty;
        player.numBountyInteraction = data.bountyInteraction;

        if(data.bounties != null)
        {
            for(int x = 0; x < data.bounties.GetLength(0); x++)
            {
                Bounty b = new Bounty();



                b.bountyType = data.bounties[x, 0];
                b.tier = data.bounties[x, 1];
                b.completed = data.bounties[x, 2] == 1;
                b.bountyRewards = (RewardType)data.bounties[x, 3];
                b.reward = data.bounties[x,4];

                switch (b.bountyType)
                {
                    case 1:
                        b.damageTakenCondition = int.Parse(data.bountyInfo[x,0]);
                        break;
                    case 2:
                        b.damageDealtCondition = int.Parse(data.bountyInfo[x,0]);
                        break;
                    case 3:
                        b.turnLimitCondition = int.Parse(data.bountyInfo[x,0]);
                        break;
                    case 4:
                        b.floorCondition = EnumConstants.ParseEnum<Floors>(data.bountyInfo[x, 0]);
                        break;
                    case 5:
                        List<string> conditions = new List<string>();
                        for (int count = 0; count < 5; count++)
                        {
                            conditions.Add(data.bountyInfo[x, count]);
                        }
                        conditions.RemoveAll(item => item.Equals(""));
                        b.academyBuffCondition = conditions;
                        break;
                    case 6:
                        b.weaponCondition = EnumConstants.ParseEnum<WeaponTypes>(data.bountyInfo[x, 0]);
                        break;
                    case 7:
                        b.affectCondition = int.Parse(data.bountyInfo[x,0]);
                        b.affectTypeCondition = int.Parse(data.bountyInfo[x,1]);
                        break;
                    case 8:
                        b.damageDealtCondition = int.Parse(data.bountyInfo[x, 0]);
                        b.damageTypeCondition = int.Parse(data.bountyInfo[x, 1]);
                        break;
                    case 9:
                        b.critCondition = int.Parse(data.bountyInfo[x, 0]);
                        break;
                    case 10:
                        b.avoidCondition = int.Parse(data.bountyInfo[x, 0]);
                        break;
                    case 11:
                        b.healCondition = int.Parse(data.bountyInfo[x, 0]);
                        break;
                }
                b.SetDesc();
                player.equippedBounties.Add(b);

            }

        }
    }

    private void UnpackHome(PlayerData data)
    {
        player.wentToTowerThisRun = data.talkedToTower;
        player.wentHomeThisRun = data.talkedToHome;
        player.numHomeInteraction = data.homeInteraction;
        player.cutscenesUnlocked.AddRange(data.cutscenes.ToList());


        //unpack attempts
        for (int x = 0; x < data.attemptCount; x++)
        {
            Attempts a = new Attempts();

            a.numberOfEncounters = data.attemptNumEnc[x];
            a.reachedTop = data.attemptReachedTop[x];
            a.surrender = data.attemptSurrender[x];
            a.killedByName = data.attemptKilledByName[x];
            a.floor = (Floors)data.attemptFloor[x];
            a.weapon = (WeaponTypes)data.attemptWeapon[x];
            a.difficulty = (Difficulty)data.attemptDifficulty[x];
            a.mod = data.attemptMod[x];



            a.enchanments = data.attemptEnchanments[x].ToList();
            a.defeatedEnemies = data.attemptDefeatedEnemies[x].ToList();
            a.abilitiesName = data.attemptAbilitiesName[x].ToList();
            a.abilitiesRank = data.attemptAbilitiesRank[x].ToList();
            a.abilitiesSpriteIndex = data.attemptAbilitiesSpriteIndex[x].ToList();
            a.artifactsNames = data.attemptArtifactsNames[x].ToList();
            a.artifactsSpriteIndex = data.attemptArtifactsSpriteIndex[x].ToList();


            player.attempts.Add(a);
        }

    }

    private void UnpackEnemy(PlayerData data)
    {
        player.numDeathToEmpress = data.deathToEmpress;
        player.numEmpressInteraction = data.empressInteraction;
        player.numDeathToHangedman = data.deathToHangedman;
        player.numHangedmanInteraction = data.hangedmanInteraction;
        player.numDeathToMagician = data.deathToMagician;
        player.numMagicianInteraction = data.magicianInteraction;
        player.numDeathToFool = data.deathToFool;
        player.numFoolInteraction = data.foolInteraction;
        player.numDeathToStar = data.deathToStar;
        player.numStarInteraction= data.starInteraction;
        player.numDeathToPriestess = data.deathToPriestess;
        player.numPriestessInteraction = data.priestessInteraction;
        player.numDeathToKing = data.deathToKing;
        player.numKingInteraction = data.kingInteraction;
    }


}
