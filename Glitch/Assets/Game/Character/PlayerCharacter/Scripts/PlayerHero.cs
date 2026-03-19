using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

public class PlayerHero : MonoBehaviour, ICharacter
{
    #region variables

    private int maxAbilityCount = 8;

    [SerializeField]
    protected PlayableCharacter readonlyPC;
    private PlayableCharacter pc;
    public int hp;

    public Sprite heroSprite;
    public PlayerInventory playerInventory;
    public List<Special> specials = new List<Special>();
    
    private int[] battleHealthRegeneration = new int[2];

    private int freezeFor = 1;
    protected int freezeForReadonly = 1;
    private int frozenStatus = 0;

    private int poisonFor = 1;
    protected int poisonForReadonly = 1;
    private int poisonedStatus = 0;

    private int[] burningInfo = new int[2];
    protected int[] burningInfoReadonly = new int[2];
    private int[] burningStatus = new int[2];

    private float[] vulnerableInfo = new float[2];
    protected float[] vulnerableInfoReadonly = new float[2];
    private float[] vulnerableStatus = new float[2];

    private int[] shockedInfo = new int[2];
    protected int[] shockedInfoReadonly = new int[2];
    private int[] shockedStatus = new int[2];

    private int[] damnationInfo = new int[4];
    protected int[] damnationInfoReadonly = new int[4];
    private int[] damnationStatus = new int[4];

    public int iceStatus;
    public int fireStatus;
    public int poisonStatus;
    public int psychicStatus;
    public int lightningStatus;

    public StatSystem battleIceStatus;
    public StatSystem battleFireStatus;
    public StatSystem battlePoisonStatus;
    public StatSystem battlePsychicStatus;
    public StatSystem battleLightningStatus;

    public int iceResist;
    public int fireResist;
    public int poisonResist;
    public int psychicResist;
    public int lightningResist;

    public StatSystem battleIceResist;
    public StatSystem battleFireResist;
    public StatSystem battlePoisonResist;
    public StatSystem battlePsychicResist;
    public StatSystem battleLightningResist;
    public StatSystem battlePhysicalResist;
    public StatSystem battleDivineResist;

    public int block;

    public StatSystem maxHealth;

    public StatSystem accuracy;

    public StatSystem critChance;

    public StatSystem actions;

    public StatSystem attLifesteal;

    public StatSystem spLifesteal;

    public StatSystem toughness;

    public StatSystem strength;

    public StatSystem dominance;

    public List<DamageTypeEnumValue> permanentResistances = new List<DamageTypeEnumValue>();
    public List<DamageTypeEnumValue> additionalResistances = new List<DamageTypeEnumValue>();

    public Attack baseAttack;

    public List<string> conditions = new List<string>();

    public bool isDazed;

    public GameObject iceFx;

    #endregion

    private void Awake()
    {
        playerInventory = new PlayerInventory();
        specials = new List<Special>();
        permanentResistances = new List<DamageTypeEnumValue>();
        conditions = new List<string>();
        additionalResistances = new List<DamageTypeEnumValue>();
        pc = Instantiate(readonlyPC);
        maxAbilityCount = 8;
    }
    // Start is called before the first frame update
    void Start()
    {
        heroSprite = pc.characterSprite;
        
        maxHealth = pc.maxHealth;
        SetToMaxHealth();

        accuracy = pc.accuracy;
        critChance = pc.critChance;
        actions = pc.actions;
        attLifesteal = pc.lifesteal;
        spLifesteal = pc.healChance;
        toughness = pc.toughness;
        strength = new StatSystem();
        dominance = new StatSystem();


        poisonStatus = pc.poisonStatus;
        iceStatus = pc.iceStatus;
        fireStatus = pc.fireStatus;
        lightningStatus = pc.lightningStatus;
        psychicStatus = pc.psychicStatus;

        poisonResist = pc.poisonResist;
        iceResist = pc.iceResist;
        fireResist = pc.fireResist;
        lightningResist = pc.lightningResist;
        psychicResist = pc.psychicResist;

        freezeFor = pc.freezeFor;
        poisonFor = pc.poisonFor;
        burningInfo = pc.burningInfo;
        vulnerableInfo = pc.vulnerableInfo;
        shockedInfo = pc.shockedInfo;
        damnationInfo = pc.damnationInfo;

        battleIceStatus = new StatSystem();
        battleFireStatus = new StatSystem();
        battlePoisonStatus = new StatSystem();
        battlePsychicStatus = new StatSystem();
        battleLightningStatus = new StatSystem();

        battleIceResist = new StatSystem();
        battleFireResist = new StatSystem();
        battlePoisonResist = new StatSystem();
        battlePsychicResist = new StatSystem();
        battleLightningResist = new StatSystem();
        battlePhysicalResist = new StatSystem();
        battleDivineResist = new StatSystem();

        permanentResistances = pc.resistances;
        SetAffinityReadonly();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetToMaxHealth()
    {
        hp = (int)maxHealth.Value;
    }

    public string GetName()
    {
        return pc.name.Replace("(Clone)", "");
    }

    #region combat statuses
    public void PlayIceFX()
    {
        toBreakIce = false;
        iceFx.SetActive(false);
        iceFx.SetActive(true);
    }

    public void BreakIce()
    {
        iceFx.GetComponent<Animator>().Play("Ice Break");
        AudioManager.instance.PlaySfxSound("Ice_Break");
        StartCoroutine(IceFxOff());
    }

    private IEnumerator IceFxOff()
    {
        yield return new WaitForSeconds(1f);
        iceFx.SetActive(false);
    }

    public bool toBreakIce = false;
    public bool CheckConditions()
    {
        if(frozenStatus <= 0)
        {
            if (RemoveCondition("Frozen") && iceFx.activeInHierarchy)
            {
                toBreakIce = true;
            }
            else if (iceFx.activeInHierarchy)
            {
                BreakIce();
            }
            else
            {
                iceFx.SetActive(false);
            }
            frozenStatus = 0;
        }


        if (poisonedStatus <= 0)
        {
            RemoveCondition("Poisoned");
            poisonedStatus = 0;
        }

        if (burningStatus[0] <= 0)
        {
            RemoveCondition("Burning");
            burningStatus = new int[2] { 0, 0 };
        }

        if (vulnerableStatus[0] <= 0)
        {
            RemoveCondition("Vulnerable");
            vulnerableStatus = new float[2] { 0, 0 };
        }

        if (damnationStatus[0] <= 0)
        {
            RemoveCondition("Condemned");
            damnationStatus = new int[4] { 0, 0, 0, 0 };
        }

        if (shockedStatus[0] <= 0)
        {
            RemoveCondition("Shocked");
            shockedStatus = new int[2] { 0, 0 };
        }

        if (battleHealthRegeneration[0] <= 0)
        {
            RemoveCondition("Regenerating");
            battleHealthRegeneration = new int[2] { 0, 0 };
        }
        return false;
    }

    //Health Regen
    public int[] GetHealthRegen()
    {
        return battleHealthRegeneration;
    }

    public void SetHealthRegen(int[] healthRegen)
    {
        battleHealthRegeneration = healthRegen;
    }

    public int GetMaxHealth()
    {
        int max = (int)maxHealth.Value;
        return max;
    }

    //Frozen
    public void SetIsFrozen(int turn)
    {
        frozenStatus = turn;
    }

    public int GetIsFrozen()
    {
        return frozenStatus;
    }

    public void SetFrozenTurns(int turn)
    {
        freezeFor = turn;
    }

    public int GetFrozenTurns()
    {
        return freezeFor;
    }

    //Burning
    public void SetIsBurning(int[] burning)
    {
        burningStatus = burning;
    }

    public int[] GetIsBurning()
    {
        return burningStatus;
    }

    public void SetBurningInfo(int[] burning)
    {
        burningInfo = burning;
    }

    public int[] GetBurningInfo()
    {
        return new int[] { burningInfo[0], burningInfo[1] };
    }

    //Vulnerable
    public void SetIsVulnerable(float[] vulnerable)
    {
        vulnerableStatus = vulnerable;
    }

    public float[] GetIsVulnerable()
    {
        return vulnerableStatus;
    }

    public void SetVulnerableInfo(float[] vulnerable)
    {
        vulnerableInfo = vulnerable;
    }

    public float[] GetVulnerableInfo()
    {
        return new float[] { vulnerableInfo[0], vulnerableInfo[1] };
    }

    //Poisoned
    public void SetIsPoisoned(int turn)
    {
        poisonedStatus = turn;
    }

    public int GetIsPoisoned()
    {
        return poisonedStatus;
    }

    public void SetPoisonedTurns(int turn)
    {
        poisonFor = turn;
    }

    public int GetPoisonedTurns()
    {
        return poisonFor;
    }

    //Shocked
    public void SetIsShocked(int[] shocked)
    {
        shockedStatus = shocked;
    }

    public int[] GetIsShocked()
    {
        return shockedStatus;
    }

    public void SetShockedInfo(int[] shocked)
    {
        shockedInfo = shocked;
    }

    public int[] GetShockedInfo()
    {
        return new int[] { shockedInfo[0], shockedInfo[1] };
    }

    //Smite
    public void SetDamnation(int[] smite)
    {
        damnationStatus = smite;
    }

    public int[] GetDamnation()
    {
        return damnationStatus;
    }

    public void SetDamnationInfo(int[] smite)
    {
        damnationInfo = smite;
    }

    public int[] GetDamnationInfo()
    {
        return new int[] { damnationInfo[0], damnationInfo[1], damnationInfo[2], damnationInfo[3] };
    }

    public void ResetConditions()
    {
        conditions = new List<string>();
    }
    #endregion

   

    #region combat stats
    public StatSystem GetCritChance()
    {
        return critChance;
    }

    public StatSystem GetActions()
    {
        return actions;
    }

    public void ChangeHealth(int val)
    {
        hp = (int)Mathf.Clamp(hp + val, 0, pc.maxHealth.Value);
    }

    public StatSystem GetDominance()
    {
        return dominance;
    }

    public StatSystem GetStrength()
    {
        return strength;
    }

    public void SetBlock(int val)
    {
        block = val;
    }

    public int GetBlock()
    {
        return block;
    }

    public List<DamageTypeEnumValue> GetResistances()
    {
        List<DamageTypeEnumValue> comb = new List<DamageTypeEnumValue>();
        comb.AddRange(permanentResistances);
        comb.AddRange(new List<DamageTypeEnumValue>(this.additionalResistances));
        return comb.Distinct().ToList();
    }

    public bool HasResistance(string dt)
    {
        foreach (DamageTypeEnumValue type in GetResistances())
        {
            if (dt.Equals(type.GetTypeName())) return true;
        }
        return false;
    }

    public void AddBattleResistance(DamageTypeEnumValue dt, int dur, int amount, object src)
    {
        additionalResistances.Add(dt);
        additionalResistances = additionalResistances.Distinct().ToList();

        if (dt.GetTypeName().Equals("Fire"))
        {
            battleFireResist.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Ice"))
        {
            battleIceResist.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Lightning"))
        {
            battleLightningResist.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Poison"))
        {
            battlePoisonResist.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Psychic"))
        {
            battlePsychicResist.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Physical"))
        {
            battlePhysicalResist.AddModifier(new StatModifier(100, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Divine"))
        {
            battleDivineResist.AddModifier(new StatModifier(100, StatModType.Flat, dur, src));
        }
    }

    public void AddCondition(string condName)
    {
        conditions.Add(condName);
        conditions = conditions.Distinct().ToList();
    }

    public bool RemoveCondition(string condName)
    {
        return conditions.Remove(condName);
    }

    public void CreateOtherCond(string condName, string desc, string sp)
    {
        if (!GetConditions().Contains(condName))
        {
            var condToken = GameObject.Instantiate(baseAttack.damageType.token);
            condToken.transform.SetParent(GameObject.FindGameObjectWithTag("HeroConditionPanel").transform);
            condToken.transform.localPosition = new Vector2(500, 0);
            condToken.GetComponent<RectTransform>().sizeDelta = baseAttack.damageType.token.GetComponent<RectTransform>().sizeDelta;
            condToken.GetComponent<RectTransform>().localScale = baseAttack.damageType.token.GetComponent<RectTransform>().localScale;
            condToken.GetComponent<ConditionIndicator>().Init(condName, desc, this, sp);
            AddCondition(condName);
        }
    }

    public void CreateCondToken(DamageTypeEnumValue dt)
    {
        var condToken = GameObject.Instantiate(dt.token);
        condToken.transform.SetParent(GameObject.FindGameObjectWithTag("HeroConditionPanel").transform);
        condToken.transform.localPosition = new Vector2(500, 0);
        condToken.GetComponent<RectTransform>().sizeDelta = dt.token.GetComponent<RectTransform>().sizeDelta;
        condToken.GetComponent<RectTransform>().localScale = dt.token.GetComponent<RectTransform>().localScale;
        condToken.GetComponent<ConditionIndicator>().Init(dt, this);
        AddCondition(dt.GetConditionName());
    }

    public void Cleanse()
    {
        conditions = new List<string>();
        SetIsFrozen(0);
        SetIsPoisoned(0);
        SetIsBurning(new int[2] { 0, 0 });
        SetIsShocked(new int[2] { 0, 0 });
        SetIsVulnerable(new float[2] { 0, 0});
        SetDamnation(new int[4] { 0, 0, 0, 0 });
    }

    public List<string> GetConditions()
    {
        return conditions;
    }

    public int GetFrozenResistance()
    {
        return Mathf.Clamp(iceResist + (int)battleIceResist.Value + playerInventory.iceModResist, 0, 100);
    }

    public int GetBurningResistance()
    {
        return Mathf.Clamp(fireResist + (int)battleFireResist.Value + playerInventory.fireModResist, 0, 100);
    }

    public int GetVulnerableResistance()
    {
        return Mathf.Clamp(psychicResist + (int)battlePsychicResist.Value + playerInventory.psychicModResist, 0, 100);
    }

    public int GetPoisonResistance()
    {
        return Mathf.Clamp(poisonResist + (int)battlePoisonResist.Value + playerInventory.poisonModResist, 0, 100); 
    }

    public int GetShockedesistance()
    {
        return Mathf.Clamp(lightningResist + (int)battleLightningResist.Value + playerInventory.lightningModResist, 0, 100); 
    }

    public bool GetPhysicalResistance()
    {
        return HasResistance("Physical") || battlePhysicalResist.Value > 0;
    }

    public bool GetDivineResistance()
    {
        return HasResistance("Divine") || battleDivineResist.Value > 0;
    }

    public StatSystem GetAttackLifestrike()
    {
        return attLifesteal;
    }

    public StatSystem GetSpecialLifeSteal()
    {
        return spLifesteal;
    }

    public StatSystem GetToughness()
    {
        return toughness;
    }

    public StatSystem GetAccuracy()
    {
        return accuracy;
    }

    public void OnSelfStatsUpkeep()
    {

        ValidateOnSelfModStatus(critChance);
        ValidateOnSelfModStatus(accuracy);
        ValidateOnSelfModStatus(attLifesteal);
        ValidateOnSelfModStatus(spLifesteal);
        ValidateOnSelfModStatus(strength);
        ValidateOnSelfModStatus(dominance);
        ValidateOnSelfModStatus(toughness);
        ValidateOnSelfModStatus(actions);
        ValidateOnSelfModStatus(maxHealth);

        ValidateOnSelfModStatus(battleDivineResist, baseAttack.specialSystem.allDamageTypes[0]);
        ValidateOnSelfModStatus(battleFireResist, baseAttack.specialSystem.allDamageTypes[1]);
        ValidateOnSelfModStatus(battleIceResist, baseAttack.specialSystem.allDamageTypes[2]);
        ValidateOnSelfModStatus(battleLightningResist, baseAttack.specialSystem.allDamageTypes[3]);
        ValidateOnSelfModStatus(battlePhysicalResist, baseAttack.specialSystem.allDamageTypes[4]);
        ValidateOnSelfModStatus(battlePoisonResist, baseAttack.specialSystem.allDamageTypes[5]);
        ValidateOnSelfModStatus(battlePsychicResist, baseAttack.specialSystem.allDamageTypes[6]);

        ValidateOnSelfModStatus(battleFireStatus);
        ValidateOnSelfModStatus(battleIceStatus);
        ValidateOnSelfModStatus(battleLightningStatus);
        ValidateOnSelfModStatus(battlePoisonStatus);
        ValidateOnSelfModStatus(battlePsychicStatus);



    }

    private void ValidateOnSelfModStatus(StatSystem s, DamageTypeEnumValue dt)
    {
        foreach (StatModifier m in s.statModifiers)
        {
            if (m.modSource is Special || m.modSource is ArtifactItem)
            {
                if (m.duration < 1)
                {
                    m.toRemove = true;
                    s.SetModChangeTrue();
                    additionalResistances.Remove(dt);
                }
                m.duration--;
            } 
        }
        s.statModifiers.RemoveAll(x => x.toRemove == true);
    }

    private void ValidateOnSelfModStatus(StatSystem s)
    {
        foreach (StatModifier m in s.statModifiers)
        {
            if (m.modSource is Special || m.modSource is ArtifactItem)
            {
                if (m.duration < 1)
                {
                    m.toRemove = true;
                    s.SetModChangeTrue();
                }
                m.duration--;
            }
        }
        s.statModifiers.RemoveAll(x => x.toRemove == true);
    }

  

    public void ClearBattleModifiers()
    {
        block = 0;
        RemoveAllBattleMods(critChance);
        RemoveAllBattleMods(accuracy);
        RemoveAllBattleMods(attLifesteal);
        RemoveAllBattleMods(spLifesteal);
        RemoveAllBattleMods(strength);
        RemoveAllBattleMods(dominance);
        RemoveAllBattleMods(toughness);
        RemoveAllBattleMods(actions);
        RemoveAllBattleMods(maxHealth);
    }

    public void RemoveAllBattleMods(StatSystem s)
    {
        foreach (StatModifier m in s.statModifiers)
        {
            if (m.modSource is Special ||  m.modSource is ArtifactItem || m.modSource is CombatSystem)
            {
                m.toRemove = true;
                s.SetModChangeTrue();
            }
        }
        s.statModifiers.RemoveAll(x => x.toRemove == true);
    }

    public StatSystem GetHealthStats()
    {
        return maxHealth;
    }
    #endregion

    public List<Artifact> GetArtifacts()
    {
        return playerInventory.artifacts;
    }

    public void IncreaseMaxHealth(int val)
    {
        GetHealthStats().baseValue += val;
        GetMaxHealth();
        ChangeHealth(val);
    }

    public void HeroDeathReset()
    {
        foreach (Special s in specials)
        {
            Destroy(s);
        }
        Destroy(baseAttack);
        Destroy(pc);
        abilitySlot = 2;
        heroLevel = 0;
        slotXp = 0;
        Awake();
        Start();
    }

    public void ResetAffinities()
    {
        freezeFor = freezeForReadonly;
        poisonFor = poisonForReadonly;
        burningInfoReadonly.CopyTo(burningInfo, 0);
        shockedInfoReadonly.CopyTo(shockedInfo, 0);
        vulnerableInfoReadonly.CopyTo(vulnerableInfo, 0);
        damnationInfoReadonly.CopyTo(damnationInfo, 0);
    }

    public void SetAffinityReadonly()
    {
        freezeForReadonly = freezeFor;
        poisonForReadonly = poisonFor;
        burningInfo.CopyTo(burningInfoReadonly, 0);
        shockedInfo.CopyTo(shockedInfoReadonly, 0);
        vulnerableInfo.CopyTo(vulnerableInfoReadonly, 0);
        damnationInfo.CopyTo(damnationInfoReadonly, 0);
    }

    public int GetStatusChance(DamageTypeEnumValue dt)
    {
        if (GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode == Floors.Floor_02)
        {
            return 0;
        }
        else if (dt.GetTypeName().Equals("Fire"))
        {
            return fireStatus + (int)battleFireStatus.Value + playerInventory.fireStatus + playerInventory.fireModStatus;
        }
        else if (dt.GetTypeName().Equals("Ice"))
        {
            return iceStatus + (int)battleIceStatus.Value + playerInventory.iceStatus + playerInventory.iceModStatus;
        }
        else if (dt.GetTypeName().Equals("Lightning"))
        {
            return lightningStatus + (int)battleLightningStatus.Value + playerInventory.lightningStatus + playerInventory.lightningModStatus;
        }
        else if (dt.GetTypeName().Equals("Poison"))
        {
            return poisonStatus + (int)battlePoisonStatus.Value + playerInventory.poisonStatus + playerInventory.poisonModStatus;
        }
        else if (dt.GetTypeName().Equals("Psychic"))
        {
            return psychicStatus + (int)battlePsychicStatus.Value + playerInventory.psychicStatus + playerInventory.psychicModStatus;
        }
        return 0;
    }

    public void SetStatusChance(DamageTypeEnumValue dt, int val)
    {
        if (dt.GetTypeName().Equals("Fire"))
        {
            fireStatus = val;
        }
        else if (dt.GetTypeName().Equals("Ice"))
        {
            iceStatus = val;
        }
        else if (dt.GetTypeName().Equals("Lightning"))
        {
            lightningStatus = val;
        }
        else if (dt.GetTypeName().Equals("Poison"))
        {
            poisonStatus = val;
        }
        else if (dt.GetTypeName().Equals("Psychic"))
        {
            psychicStatus = val;
        }
    }

    public void SetBattleStatusChance(DamageTypeEnumValue dt, int dur, int amount, object src)
    {
        if (dt.GetTypeName().Equals("Fire"))
        {
            battleFireStatus.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Ice"))
        {
            battleIceStatus.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Lightning"))
        {
            battleLightningStatus.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Poison"))
        {
            battlePoisonStatus.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
        else if (dt.GetTypeName().Equals("Psychic"))
        {
            battlePsychicStatus.AddModifier(new StatModifier(amount, StatModType.Flat, dur, src));
        }
    }

    #region abilities
    public int abilitySlot = 2;
    public List<Special> generatedSpecials = new List<Special>();

    public int heroLevel = 0;
    public int slotXp = 0;
    private readonly int[] slotXpRequirements = new int[] { 100, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190};
    public void ChangeAbilitySlotXp(int val)
    {
        int req = MaxXp();
        slotXp = Mathf.Clamp(slotXp + val, 0, req);
        if(slotXp >= req)
        {
            heroLevel++;
            abilitySlot = Mathf.Clamp(abilitySlot + 1, 0, specials.Count);
            if(abilitySlot < specials.Count)
            {
                slotXp = 0;
            }
        }
    }

    public int MaxXp()
    {
       return slotXpRequirements[abilitySlot - 2];
    }

    public int GetAbilitySlotCount()
    {
        return abilitySlot;
    }

    public void GenerateSpecials()
    {
        if (abilitySlot >= specials.Count)
        {
            generatedSpecials = specials;
        }
        else
        {
            generatedSpecials = specials.OrderBy(a => Guid.NewGuid()).ToList().GetRange(0, Math.Min(abilitySlot, specials.Count));
        }
    }

    public void AddSpecial(Special s)
    {
        if (specials.Count < GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().h.GetMaxAbilityCount())
        {
            Special sp = Instantiate(s);
            specials.Add(sp);
        }
        else
        {
            GameUtilities.ShowMessage(MessageLevel.Error, "Reached Max Special Capacity. Failed to Update.");
        }
    }

    public void OrderSpecials()
    {
        specials = specials.OrderBy(a => Guid.NewGuid()).OrderByDescending(name => name.GetName()).ToList();
    }

    public void RemoveSpecial(Special s)
    {
        specials.Remove(s);
        GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().AddSpecialToCollection(s);
        Destroy(s);
    }
    public int GetMaxAbilityCount()
    {
        List<Ailment> ailments = playerInventory.ailments.Where(a => a.conIndex == 7).ToList();
        if (ailments.Count > 0)
        {
            return ailments[0].maxAbility;
        }
        else
        {
            return maxAbilityCount;
        }
    }

    public void SetMaxAbilityCount(int val)
    {
        maxAbilityCount = val;
    }

    #endregion

    public InteractableNpc GetNpc()
    {
        return pc.heroNpc;
    }
}
