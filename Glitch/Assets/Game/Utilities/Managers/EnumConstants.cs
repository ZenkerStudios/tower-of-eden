//Game ENUMS
public enum RewardType { Shop = 1, Powerup = 2, Ability = 3, MaxHealth = 4, Artifact = 5, Gold = 6, Gemstones = 7, MetalScraps = 8, EtherVial = 9, EtherShard = 10, GrimoirePage = 11}
public enum Difficulty { Easy = 1, Medium = 2, Hard = 3, Gauntlet = 4, Accursed = 5}
public enum Rarity { Default = 1, Common = 2, Uncommon = 3, Rare = 4, Legendary = 5, Exalted = 6, }
public enum WeaponTypes { Melancolia = 1, Delta = 2, Firestorm = 3, Renegade = 4, None = 5}
public enum AbilityTypes { Offensive, Recovery, Enchantment, Affliction, Summon }
public enum TargetTypes { OnSelf, OnTarget }
public enum NumTarget { One = 1, Two = 2, Three = 3, All = 4}
public enum StatModType { Flat = 100, PercentageMult = 200, PercentageIncrease = 300 }
public enum CharacterStats { Health = 0, Accuracy = 1, CritChance = 2, Actions = 3, Block = 4, Resistance = 5, Toughness = 6,
    Lifestrike = 7, Strength = 8, Lifesteal = 9, Dominance = 10, Dazed = 11, Gold = 12, Cleanse = 13, MultiHit = 14, NewSkill = 15, HealthRegen = 16
}
public enum Traits { Volatile = 0, Pacifist = 1, Undying = 2, Devourer = 3, Amalgamation = 4, Nexus_Arm = 5, Apotheosis = 6, Kingly = 7,
    Relentless = 8, Unruly = 9, Vigilant = 10, Tenacious = 11,
    Pious = 12, Stoic = 13, Zealous = 14, Aberrant = 15, Intrepid = 16, Adept = 17, Mender = 18, Indomitable = 19, Sheltered = 20, Safeguard = 21
}
public enum Floors { Start = 0, Hub = 1, Floor_01 = 2, Floor_02 = 3, Floor_03 = 4, Floor_04 = 5, Floor_05 = 6, End = 7}
public enum CombatUiComponent { Attack, Special1, Special2, Special3, Special4, Special5, Special6, Special7, Special8, Special9, Special10, Character }
public enum MessageLevel { Info, Warning, Error }

//City: 0-4, Tower: 5-12
public enum Consumables {
    ailmentRedux = 0, hazardRedux = 1, rarityIncrease = 2, randomArtifact = 3, randomSpecial = 4,
    statusIncrease = 5, accuracyIncrease = 6, critIncrease = 7, strengthIncrease = 8,
    dominanceIncrease = 9, toughnessIncrease = 10, lifestrikeIncrease = 11, lifestealIncrease = 12
}


public enum BlacksmithConMods
{
    ailmentChance = 0, ailmentTask = 1, takeMoreFireDmg = 2, lowerLightningDmg = 3, takeMorePhysicalDmg = 4, lowerMaxHealth = 5,

    //Sword: 0, 1, 2, 5, 6, 7, 8, 9, 10, 11
    halfHealthEnc = 6, lowerRevive = 7, takeMorePoisonDmg = 8, lowerDivineDmg = 9, lowerPhysicalDmg = 10, lowerPsychicChance = 11,

    //Staff: 0, 1, 3, 12, 13, 14, 15, 16, 17, 18
    halfGoldEnc = 12, lowerAcademyPassive = 13, firestormLowerLifesteal = 14, lowerFireDmg = 15, lowerIceDmg = 16, takeMoreDivineDmg = 17, lowerAccuracy = 18,

    //Gun: 0, 1, 4, 3, 19, 20, 21, 22, 23, 24
    lowerLifestrike = 19, lowerAttackTarget = 20, lowerPoisonDmg = 21, lowerCrit = 22, attackSelfDmg = 23, takeMorePsychicDmg = 24,

    //Fist: 0, 1, 2, 3, 4, 5, 25, 26, 27, 28
    lowerDivineStack = 25, renegadeLowerLifesteal = 26, lowerVulnerableMult = 27, lowerPsychicDmg = 28,
}

public enum BlacksmithProMods
{
    halfHpDmg = 0, dmgPerAilment = 1, attackTarget = 2, extraMaxHealth = 3, extraHealthPerEncounter = 4, extraFireResist = 5,

    //Sword: 0, 1, 6, 7, 2, 8, 9, 10, 11, 12
    blockPerTurn = 6, extraLifesteal = 7, burnSpread = 8, vulnerableExtended = 9, healOnCrit = 10, extraLightningResist = 11, extraDivineResist = 12,

    //Staff: 0, 1, 13, 14, 3, 15, 16, 17, 5, 18
    extraDmgPerGold = 13, vulnerableConfusion = 14, extraLifestrike = 15, attackDaze = 16, extraToughness = 17, extraPoisonResist = 18,

    //Gun: 0, 1, 19, 20, 3, 21, 22, 4, 23, 24
    poisonSpread = 19, critOnFrozen = 20, attackPsychicType = 21, extraCrit = 22, extraPsychicResist = 23, extraIceResist = 24,

    //Fist: 0, 1, 25, 26, 27, 28, 2, 4, 5, 29
    blockPerEnc = 25, critOnPoison = 26, shockSpread = 27, attackPoisonType = 28, extraPhysicalResist = 29,
}


//Dialogue and friendly NPC ENUMS
public enum DialogueConditions { None,
    UnlockChurch, UnlockBlacksmith, UnlockHome, UnlockAcademy, UnlockBounties, UnlockShop,
    UnlockDelta, UnlockFirestorm, UnlockRenegade, GrimoirePageTwo, GrimoirePageThree, UnlockStaffQuest, ObtainedStaffMaterial,
    RagnaChallenge, RagnaChallengeWon, RagnaChallengeLost, StarRevealed, IzaakConfrontsEden, IzaakReturnsToTower,
    DorianDecided, DorianResolve, SoldierResolve, FirstUleaDefeat, AcridFirstNote, AcridSecondNote, AgnesConcerned,
    IzaakReassuresEden, CompendiumStabilized, CompendiumSealed, 
    UnlockDeltaMod, UnlockFirestormMod, UnlockRenegadeMod, UnlockMelancoliaMod,
    FinalUlea
}
public enum InteractableNpcs { Blacksmith, Academy, Church, Empress, HangedMan, Magician, Fool, Priestess, Star, King, Home, Shop, Tower, Izaak, Eden, None}
public enum DialoguePriority { Critical, High, Medium, Low, Invalid}
public enum Cutscenes
{
    None = 0, One = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8,
    Nine = 9, Ten = 10, Eleven = 11, Twelve = 12, Thirteen = 13, Fourteen = 14, Fifteen = 15, Sixteen = 16
}

public enum Achievements
{
    GAME_START, FIRST_WIN, CONTINUED_ATTEMPT, FINAL_ENDING, ACADEMY_PAGES, BLACKSMITH_WEAPONS, WEAPON_MODS, CHURCH_DONATION, PORTRAIT, FINAL_ULEA
}
public class EnumConstants 
{
    public static T ParseEnum<T>(string value)
    {
        return (T)System.Enum.Parse(typeof(T), value, true);
    }
}
