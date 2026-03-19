using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class EnemyToken : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ICharacter
{
    #region variables 

    private Vector3 activeSpritePos = new Vector3(0,0,0);

    public GameObject isThinking;
    public GameObject dazedIcon;

    public GameObject pfIntent;
    public GameObject intentPanel;

    public GameObject tokenUi;
    public GameObject tokenInfo;

    public Slider healthSlider;

    public Sprite blueHeart;
    public Sprite blueFill;

    public Sprite yellowHeart;
    public Sprite yellowFill;

    public Sprite redHeart;
    public Sprite redFill;

    public Sprite emptyHeart;

    public Image healthHeart;
    public Image healthFill;

    public GameObject pfDamageType;
    public GameObject resistancePanel;


    public EnemyNpc enemyNpc;
    public Image mobSpriteImg;
    public Image bossSpriteImg;
    public Image activeSprite;

    public GameObject shield;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI shieldText;

    public GameObject turnIndicator;
    public GameObject targetIndicator;

    public GameObject conditionPanel;

    public CombatSystem cSystem;
    public int hp;
    public bool myTurn = false;
    public bool isDazed = false;

    private int[] battleHealthRegeneration = new int[2];

    private int freezeFor = 1;
    private int frozenStatus = 0;

    private int poisonFor = 1;
    private int poisonedStatus = 0;

    private int[] burningInfo = new int[2];
    private int[] burningStatus = new int[2];

    private float[] vulnerableInfo = new float[2];
    private float[] vulnerableStatus = new float[2];

    private int[] shockedInfo = new int[2];
    private int[] shockedStatus = new int[2];

    private int[] damnationInfo = new int[4];
    private int[] damnationStatus = new int[4];

    public List<string> conditions = new List<string>();

    public List<DamageTypeEnumValue> permanentResistances = new List<DamageTypeEnumValue>();
    public List<DamageTypeEnumValue> additionalResistances = new List<DamageTypeEnumValue>();

    public StatSystem battleIceResist;
    public StatSystem battleFireResist;
    public StatSystem battlePoisonResist;
    public StatSystem battlePsychicResist;
    public StatSystem battleLightningResist;

    public int traitIceResist;
    public int traitFireResist;
    public int traitPoisonResist;
    public int traitPsychicResist;
    public int traitLightningResist;

    public int iceResist;
    public int fireResist;
    public int poisonResist;
    public int psychicResist;
    public int lightningResist;

    public int iceStatus;
    public int fireStatus;
    public int poisonStatus;
    public int psychicStatus;
    public int lightningStatus;

    public int block;

    public int maxMeterGain;

    public StatSystem maxHealth;

    public StatSystem accuracy;

    public StatSystem critChance;

    public StatSystem actions;

    public StatSystem attLifesteal;

    public StatSystem spLifesteal;

    public StatSystem toughness;

    public StatSystem strength;

    public StatSystem dominance;

    private int specialMeter = 0;

    public bool abilityAfterDeath;

    public Special deathAbility;

    public Trait buffAfterDamage;

    public int buffDamageNumber;

    public Trait blockAfterDamage;

    public bool dazedResAbility;

    public bool echoAbillity;

    public EnemyNpc echoTokens;

    public bool canRevive;

    public EnemyNpc reviveTokens;

    public bool isTransforming;

    public int transformationTurn;

    public bool abilityOnSelfDestruct;

    public bool canSelfDestruct;

    public int selfDestructTurn;

    public List<Artifact> artifacts = new List<Artifact>();

    public List<Special> preparedMoves = new List<Special>();

    private List<string> movesOnCooldown = new List<string>();

    private bool isTargetted = false;

    public TextMeshProUGUI hpDesc;
    public TextMeshProUGUI accDesc;
    public TextMeshProUGUI ccDesc;
    public TextMeshProUGUI actDesc;
    public TextMeshProUGUI tghDesc;
    public TextMeshProUGUI lsDesc;
    public TextMeshProUGUI domDesc;

    public GameObject pfNpcInfo;
    public GameObject iceFx;
    public List<Trait> traits;
    public int extraTrait;

    public Animator tokenAnimator;
    #endregion

    private void Awake()
    {
        permanentResistances = new List<DamageTypeEnumValue>();
        mobSpriteImg.gameObject.SetActive(false);
        bossSpriteImg.gameObject.SetActive(false);
        cSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        tokenUi.SetActive(true);
        healthSlider.onValueChanged.AddListener(OnHealthChange);
        tokenAnimator = gameObject.GetComponent<Animator>();
    }

    private void AddToStatusChance(int val)
    {
        iceStatus += val;
        fireStatus += val;
        poisonStatus += val;
        lightningStatus += val;
        psychicStatus += val;
    }

    private void AddToStatusResist(int val)
    {
        if(iceResist != 0) iceResist += val;
        if (fireResist != 0) fireResist += val;
        if (poisonResist != 0) poisonResist += val;
        if (lightningResist != 0) lightningResist += val;
        if (psychicResist != 0) psychicResist += val;
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyNpc newNpc = Instantiate(enemyNpc);
        newNpc.enemySystem.InitializeEnemy(newNpc);
        enemyNpc = newNpc;
        if (enemyNpc.interactableNpc != null)
        {
            barkTriggerCooldown = enemyNpc.interactableNpc.barkCooldown;
        }
    
        if((GetName().Equals("Ether Clone") || GetName().Equals("Corrupted Eden") || GetName().Equals("Eden, The King"))
            && cSystem.player.savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed))
        {
            //If compendium is sealed, Izaak only sees ghost version
            bossSpriteImg.gameObject.SetActive(true);
            bossSpriteImg.sprite = enemyNpc.enemyIcon;
            activeSprite = bossSpriteImg;
        }
        else if (enemyNpc.isBoss)
        {
            bossSpriteImg.gameObject.SetActive(true);
            bossSpriteImg.sprite = enemyNpc.enemySprite;
            activeSprite = bossSpriteImg;

        } 
        else
        {
            mobSpriteImg.gameObject.SetActive(true);
            mobSpriteImg.sprite = enemyNpc.enemySprite;
            activeSprite = mobSpriteImg;
        }

        GetSpritePos();
        iceFx.SetActive(false);

        specialMeter = enemyNpc.startingSpecialMeter;
        permanentResistances = enemyNpc.resistances;

        block = enemyNpc.block;
        maxMeterGain = enemyNpc.maxMeterGain;
        maxHealth = enemyNpc.maxHealth;
        hp = (int)maxHealth.Value;
        accuracy = enemyNpc.accuracy;
        critChance = enemyNpc.critChance;
        actions = enemyNpc.actions;
        attLifesteal = enemyNpc.healChance;
        spLifesteal = enemyNpc.lifesteal;
        toughness = enemyNpc.toughness;

        poisonResist = enemyNpc.poisonResist;
        iceResist = enemyNpc.iceResist;
        fireResist = enemyNpc.fireResist;
        lightningResist = enemyNpc.lightningResist;
        psychicResist = enemyNpc.psychicResist;

        poisonStatus = enemyNpc.poisonStatus;
        iceStatus = enemyNpc.iceStatus;
        fireStatus = enemyNpc.fireStatus;
        lightningStatus = enemyNpc.lightningStatus;
        psychicStatus = enemyNpc.psychicStatus;

        freezeFor = enemyNpc.freezeFor;
        poisonFor = enemyNpc.poisonFor;
        burningInfo = enemyNpc.burningInfo;
        vulnerableInfo = enemyNpc.vulnerableInfo;
        shockedInfo = enemyNpc.shockedInfo;
        damnationInfo = enemyNpc.damnationInfo;

        abilityAfterDeath = enemyNpc.abilityAfterDeath;
        deathAbility = enemyNpc.deathAbility;
        dazedResAbility = enemyNpc.dazedResAbility;
        echoAbillity = enemyNpc.echoAbillity;
        echoTokens = enemyNpc.echoTokens;
        canRevive = enemyNpc.canRevive;
        reviveTokens = enemyNpc.reviveTokens;
        isTransforming = enemyNpc.isTransforming;
        transformationTurn = enemyNpc.transformationTurn;
        canSelfDestruct = enemyNpc.canSelfDestruct;
        abilityOnSelfDestruct = enemyNpc.abilityOnSelfDestruct;
        selfDestructTurn = enemyNpc.selfDestructTurn;

        traits = enemyNpc.traits;
        nameText.text = GetName();
        if (cSystem.player.chosenDifficulty == 2)
        {
            GetDominance().AddModifier(new StatModifier(2, StatModType.Flat, this));
            AddToStatusChance(5);
        }
        else if (cSystem.player.chosenDifficulty == 3)
        {
            GetDominance().AddModifier(new StatModifier(3, StatModType.Flat, this));
            extraTrait = 1;
            AddToStatusChance(10);
        }
        else if (cSystem.player.chosenDifficulty == 4)
        {
            GetDominance().AddModifier(new StatModifier(4, StatModType.Flat, this));
            enemyNpc.maxHealth.AddModifier(new StatModifier(enemyNpc.maxHealth.Value / 2, StatModType.Flat, this));
            extraTrait = 1;
            AddToStatusChance(15);
            AddToStatusResist(5);
        }
        else if (cSystem.player.chosenDifficulty == 5)
        {
            GetDominance().AddModifier(new StatModifier(5, StatModType.Flat, this));
            enemyNpc.maxHealth.AddModifier(new StatModifier(enemyNpc.maxHealth.Value / 2, StatModType.Flat, this));
            enemyNpc.toughness.AddModifier(new StatModifier(3, StatModType.Flat, this));
            extraTrait = 2;
            AddToStatusChance(15);
            AddToStatusResist(10);
        }


        SetHealth(hp);
        SetMaxHealth((int)maxHealth.Value);
        ApplyTrait();
        SetResistanceIcons();
        FloorEvent();
    }

    private void FloorEvent()
    {
        if (GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorEventTriggered && cSystem.encounterKey != Floors.Floor_03)
        {
            switch (cSystem.encounterKey)
            {

                case Floors.Floor_04:
                    string s = "Gain Dominance, Accuracy and increased Max Health for this encounter.";
                    GetAccuracy().AddModifier(new StatModifier(10, StatModType.Flat, cSystem));
                    GetHealthStats().AddModifier(new StatModifier(20, StatModType.Flat, cSystem));
                    GetDominance().AddModifier(new StatModifier(5, StatModType.Flat, cSystem));
                    ResetMaxHealth();
                    CreateOtherCond(Floors.Floor_04.ToString(), s, "71");
                    break;
            }
        }
    }

    public void SetResistanceIcons()
    {

        GameUtilities.DeleteAllChildGameObject(resistancePanel);
        foreach (DamageTypeEnumValue d in GetResistances())
        {
            var newObj = GameObject.Instantiate(pfDamageType);
            newObj.GetComponent<Image>().sprite = d.sprite;
            newObj.transform.SetParent(resistancePanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfDamageType.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfDamageType.GetComponent<RectTransform>().localScale;
        }
    }

    public void GetSpritePos()
    {
        activeSpritePos = activeSprite.transform.position;
    }

    //Updates character stats on UI
    public void UpdateInfo()
    {
        if (deathAnim) return;
        shieldText.text = "<sprite=44> Block: " + block;
        shield.SetActive(false);
        if (block > 0)
        {
            shield.SetActive(true);
        }
        if (myTurn)
        {
            turnIndicator.SetActive(true);
            targetIndicator.SetActive(false);
        }
        else
        {
            turnIndicator.SetActive(false);
        }
        SetHealth(hp);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateInfo();
    }

    private void ApplyTrait()
    {
        if(GetName().Equals("Sir Dorian Gold, The Fool"))
        {
            enemyNpc.attacks = enemyNpc.attacks.Take(Mathf.Clamp(cSystem.player.chosenDifficulty + 1, 2, 5)).ToList();
            enemyNpc.enemySystem.SetAllAbilities(enemyNpc);
        }

        //Apply to enemies
        List<int> traitIndex = new List<int> { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
        //Remove trait they already have from list
        foreach (Trait t in traits)
        {
            traitIndex.Remove((int)t.trait);
        }
        traitIndex = traitIndex.OrderBy(a => System.Guid.NewGuid()).ToList();
        for(int x = 0; x < extraTrait; x++)
        {
            traits.Add(new Trait((Traits)traitIndex[x]));
        }
        foreach (Trait t in traits)
        {
            t.ApplyPassive(this, cSystem.specialSystem);
        }
        ResetMaxHealth();
    }

    public void RemoveTrait(Traits t)
    {
        List<Trait> lst = traits.Where(trait => trait.trait == t).ToList();
        if(lst.Count != 0)
        {
            Trait tr = lst[0];
            traits.RemoveAll(trait => trait.trait == t);
            tr.RemovePassive(this, cSystem.specialSystem);
        }
    }

    public void AddTrait(Traits t)
    {
        Trait tr = new Trait(t);
        traits.Add(tr);
        tr.ApplyPassive(this, cSystem.specialSystem);
    }

    public string GetName()
    {
        return enemyNpc.GetName();
    }

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
        if (frozenStatus <= 0)
        {
            if (RemoveCondition("Frozen") && iceFx.activeInHierarchy)
            {
                toBreakIce = true;
            }
            else if(iceFx.activeInHierarchy)
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

    //Sets the health, mana, stamina bar values for UI
    public void SetHealth(int hp)
    {
        healthSlider.value = hp;
    }

    //Set max amounts for health, mana, stamina bars
    public void SetMaxHealth(int hp)
    {
        healthSlider.maxValue = hp;
        healthSlider.value = hp;
    }

    public void ResetMaxHealth()
    {
        hp = (int)maxHealth.Value;
        SetMaxHealth(hp);                                                                                                                                                                 
    }

    private int lastHp = 0;
    private void OnHealthChange(float health)
    {
        if(health < lastHp)
        {
            tokenAnimator.Play("Damaged_Anim");
            tokenAnimator.Play("Heart_Shake", 1);
            if (buffAfterDamage != null)
            {
                buffDamageNumber = Mathf.Clamp(buffDamageNumber + buffAfterDamage.powerAmount, 0, 25);
                GetDominance().AddModifier(new StatModifier(buffDamageNumber, StatModType.Flat, buffAfterDamage));
            }

            if(blockAfterDamage != null)
            {
                if (block < blockAfterDamage.powerAmount)
                {
                    SetBlock(blockAfterDamage.powerAmount);
                }
            }
        }
        lastHp = (int)health;
        if (health <= 0)
        {
            //Change to red UI
            healthHeart.sprite = emptyHeart;
            healthFill.sprite = redFill;
        }
        if (health < (healthSlider.maxValue * 0.3f))
        {
            //Change to red UI
            healthHeart.sprite = redHeart;
            healthFill.sprite = redFill;
        }
        else if (health < (healthSlider.maxValue * 0.6f) && (health > (healthSlider.maxValue * 0.3f)))
        {
            //Change to yellow
            healthHeart.sprite = yellowHeart;
            healthFill.sprite = yellowFill;
        }
        else
        {
            //Change to blue ui
            healthHeart.sprite = blueHeart;
            healthFill.sprite = blueFill;
        }
    }

    public void PulsateShield()
    {
        tokenAnimator.Play("Shield_Pulsate", 1);
    }
    public void PulsateConditions()
    {
        tokenAnimator.Play("Condition_Pulsate", 2);
    }

    public bool IsDead()
    {
        return hp <= 0;
    }

    private bool showingInfo;
    public void InfoButton()
    {
        if(!conditions.Contains("Frozen"))
        {
            iceFx.SetActive(false);
        }

        tokenAnimator.Play("View_Info");
        showingInfo = true;
        tokenUi.SetActive(false);
        SetResistanceIcons();
        hpDesc.text = "<sprite=45> HP: " + hp + "/" + GetMaxHealth();
        accDesc.text = "<sprite=46> Accuracy: " + GetAccuracy().Value;
        ccDesc.text = "<sprite=47> Crit Chance: " + GetCritChance().Value;
        actDesc.text = "<sprite=48> Actions: " + GetActions().Value;
        tghDesc.text = "<sprite=49> Toughness: " + GetToughness().Value;
        lsDesc.text = "<sprite=55> Lifesteal: " + GetSpecialLifeSteal().Value;
        domDesc.text = "<sprite=54> Dominance: " + GetDominance().Value;
    }

    public void RenderThinking()
    {
        dazedIcon.SetActive(false);
        intentPanel.SetActive(false);
        isThinking.SetActive(true);
    }

    public void RenderDazed()
    {
        preparedMoves = new List<Special>();
        dazedIcon.SetActive(true);
        intentPanel.SetActive(false);
        isThinking.SetActive(false);
    }

    public void RenderIntent()
    {
        dazedIcon.SetActive(false);
        isThinking.SetActive(false);
        GameUtilities.DeleteAllChildGameObject(intentPanel);
        foreach (Special s in preparedMoves)
        {
            var newObj = GameObject.Instantiate(pfIntent);
            newObj.GetComponent<Image>().sprite = s.GetSprite();
            newObj.transform.SetParent(intentPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfIntent.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfIntent.GetComponent<RectTransform>().localScale;
        }
        intentPanel.SetActive(true);
    }

    public void ResetTokenInfo()
    {
        if (showingInfo)
        {
            if (conditions.Contains("Frozen"))
            {
                iceFx.SetActive(true);
            }
            tokenAnimator.Play("Exit_Info");
            showingInfo = false;
            tokenUi.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ResetTokenInfo();
        if (eventData.button == PointerEventData.InputButton.Left)
        {

            if (cSystem.IsEnemyTargetable(this) && cSystem.abilityUsed != null && !cSystem.targetSelected)
            {
                cSystem.targetSelected = true;
                cSystem.targetIndex = transform.GetSiblingIndex();
                cSystem.StartAttack();
            } 
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            var newObj = GameObject.Instantiate(pfNpcInfo);
            newObj.GetComponent<NpcInfo>().Setup(this);
            newObj.transform.SetParent(GameObject.Find("Player Canvas").transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfNpcInfo.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfNpcInfo.GetComponent<RectTransform>().localScale;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (deathAnim) return;
        if (!myTurn)
        {
            targetIndicator.SetActive(true);
        }
        if (cSystem.abilityUsed != null)
        {
            cSystem.targetIndex = transform.GetSiblingIndex();
            cSystem.GetPotentialTargets(cSystem.abilityUsed, cSystem.targetIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (deathAnim) return;
        if (!isTargetted)
        {
            targetIndicator.SetActive(false);
        }
    }

    public void ShowAsPotentialTarget()
    {
        if (deathAnim) return;
        isTargetted = true;
        targetIndicator.SetActive(true);
    }

    public void HideTargetIndicator()
    {
        if (deathAnim) return;
        isTargetted = false;
        targetIndicator.SetActive(false);
    }


    public int GetMaxHealth()
    {
        return (int)maxHealth.Value;
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
        hp = (int)Mathf.Clamp(hp + val, 0, maxHealth.Value);
    }

    public int GetSpecialMeter()
    {
        return specialMeter;
    }
    public void SetMoves(int space, bool fullHealthTeam)
    {
        ValidateOnSelfModStatus(actions);
        specialMeter += Random.Range(1, maxMeterGain);
        preparedMoves = new List<Special>();
        List<string> movesToBeOnCooldown = new List<string>();
        bool hasRecovery = false;
        for (int x = 0; x < GetActions().Value; x++)
        {
            if (enemyNpc.bossSpecials.Count <= 0 || specialMeter < 14)
            {
                Special t = enemyNpc.attacks[Random.Range(0, enemyNpc.attacks.Count)];
                if(t.GetAbilityType() == AbilityTypes.Recovery)
                {
                    if(!hasRecovery)
                    {
                        hasRecovery = true;
                        if (t.GetTargetTypes() == TargetTypes.OnSelf)
                        {
                            if((t.numTarget == NumTarget.One && hp >= GetMaxHealth())
                                || (t.numTarget != NumTarget.One && fullHealthTeam && hp >= GetMaxHealth()))
                            {
                                hasRecovery = false;
                                t = enemyNpc.attacks[0];
                            }
                        }
                        else if((t.GetTargetTypes() == TargetTypes.OnTarget) && fullHealthTeam)
                        {
                            hasRecovery = false;
                            t = enemyNpc.attacks[0];
                        }
                    } else
                    {
                        t = enemyNpc.attacks[0];
                    }
                }
                else if (t.GetAbilityType() == AbilityTypes.Enchantment || t.GetAbilityType() == AbilityTypes.Affliction)
                {
                    if (movesOnCooldown.Contains(t.GetName()) || movesToBeOnCooldown.Contains(t.GetName()))
                    {
                        t = enemyNpc.attacks[0];
                    } else
                    {
                        movesToBeOnCooldown.Add(t.GetName());
                    }
                }

                preparedMoves.Add(t);
            }
            else
            {
                List<Special> super = enemyNpc.bossSpecials.OrderBy(a => System.Guid.NewGuid()).ToList();
                bool added = false;
                foreach(Special s in super)
                {
                    if (s.GetAbilityType() == AbilityTypes.Summon)
                    {
                        if (space > 0)
                        {
                            specialMeter = 0;
                            preparedMoves.Add(s);
                            added = true;
                            break;
                        }
                    }
                    else
                    {
                        specialMeter = 0;
                        preparedMoves.Add(s);
                        added = true;
                        break;
                    }
                }

                if(!added)
                {
                    preparedMoves.Add(enemyNpc.attacks[Random.Range(0, enemyNpc.attacks.Count)]);
                    added = true;
                }
               
            }
        }
        movesOnCooldown = movesToBeOnCooldown;
    }

    public Special GetMove(int index)
    {
        Special temp = preparedMoves[index];
        preparedMoves.RemoveAt(index);
        return temp;
    }

    public void ResetConditions()
    {
        conditions = new List<string>();
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
    }

    public void AddCondition(string condName)
    {
        conditions.Add(condName);
        conditions = conditions.Distinct().ToList();
    }

    public bool RemoveCondition(string condName)
    {
        for (int x = 0; x < conditionPanel.transform.childCount; x++)
        {
            ConditionIndicator condition = conditionPanel.transform.GetChild(x).GetComponent<ConditionIndicator>();
            if (condition.condName.Equals(condName))
            {
                GameObject child = conditionPanel.transform.GetChild(x).gameObject;
                Destroy(child);
                break;
            }
        }
        return conditions.Remove(condName);
    }

    public void CreateOtherCond(string condName, string desc, string sp)
    {
        if (!GetConditions().Contains(condName))
        {
            DamageTypeEnumValue dt = cSystem.specialSystem.allDamageTypes[0];
            var condToken = GameObject.Instantiate(dt.token);
            condToken.transform.SetParent(conditionPanel.transform);
            condToken.transform.localPosition = new Vector2(500, 0);
            condToken.GetComponent<RectTransform>().sizeDelta = dt.token.GetComponent<RectTransform>().sizeDelta;
            condToken.GetComponent<RectTransform>().localScale = dt.token.GetComponent<RectTransform>().localScale;
            condToken.GetComponent<RectTransform>().localRotation = dt.token.GetComponent<RectTransform>().localRotation;
            condToken.GetComponent<ConditionIndicator>().Init(condName, desc, this, sp);
            AddCondition(condName);
        }
    }

    public void CreateCondToken(DamageTypeEnumValue dt)
    {
        var condToken = GameObject.Instantiate(dt.token);
        condToken.transform.SetParent(conditionPanel.transform);
        condToken.transform.localPosition = new Vector2(500, 0);
        condToken.GetComponent<RectTransform>().sizeDelta = dt.token.GetComponent<RectTransform>().sizeDelta;
        condToken.GetComponent<RectTransform>().localScale = dt.token.GetComponent<RectTransform>().localScale;
        condToken.GetComponent<ConditionIndicator>().Init(dt, this);
        AddCondition(dt.GetConditionName());
    }

    public List<string> GetConditions()
    {
        return conditions;
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

    public EnemyNpc GetAllyToken()
    {
        System.Random rand = new System.Random();
        int x = rand.Next(enemyNpc.allyTokens.Count);
        return enemyNpc.allyTokens[x];
    }

    public EnemyNpc GetAllyTokenAtIndex(int x)
    {
        return enemyNpc.allyTokens[x];
    }

    public int GetFrozenResistance()
    {
        return Mathf.Clamp(iceResist + (int)battleIceResist.Value + traitIceResist, 0, 100);
    }

    public int GetBurningResistance()
    {
        return Mathf.Clamp(fireResist + (int)battleFireResist.Value + traitFireResist, 0, 100);
    }

    public int GetVulnerableResistance()
    {
        return Mathf.Clamp(psychicResist + (int)battlePsychicResist.Value + traitPsychicResist, 0, 100);
    }

    public int GetPoisonResistance()
    {
        return Mathf.Clamp(poisonResist + (int)battlePoisonResist.Value + traitPoisonResist, 0, 100);
    }

    public int GetShockedesistance()
    {
        return Mathf.Clamp(lightningResist + (int)battleLightningResist.Value + traitLightningResist, 0, 100);
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

    public void OnSelfStatsUpkeep()
    {
        ValidateOnSelfModStatus(critChance);
        ValidateOnSelfModStatus(accuracy);
        ValidateOnSelfModStatus(attLifesteal);
        ValidateOnSelfModStatus(spLifesteal);
        ValidateOnSelfModStatus(strength);
        ValidateOnSelfModStatus(dominance);
        ValidateOnSelfModStatus(toughness);
        ValidateOnSelfModStatus(maxHealth);
    }

    private void ValidateOnSelfModStatus(StatSystem s)
    {
        foreach (StatModifier m in s.statModifiers)
        {
            if (m.modSource is Special)
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

        RemoveAllBattleMods(critChance);
        RemoveAllBattleMods(accuracy);
        RemoveAllBattleMods(attLifesteal);
        RemoveAllBattleMods(spLifesteal);
        RemoveAllBattleMods(strength);
        RemoveAllBattleMods(dominance);
        RemoveAllBattleMods(toughness);
        RemoveAllBattleMods(maxHealth);

    }

    public void RemoveAllBattleMods(StatSystem s)
    {
        foreach (StatModifier m in s.publicStatModList)
        {
            if (m.modSource is Special)
            {
                s.RemoveAllModFromSource(m);
            }
        }
    }

    public StatSystem GetHealthStats()
    {
        return maxHealth;
    }

    public void IncreaseMaxHealth(int val)
    {
        GetHealthStats().baseValue += val;
        GetMaxHealth();
        ChangeHealth(val);
    }

    public void ResetAffinities()
    {
        return;
    }

    public int GetStatusChance(DamageTypeEnumValue dt)
    {
        if (dt.GetTypeName().Equals("Fire"))
        {
            return fireStatus;
        }
        else if (dt.GetTypeName().Equals("Ice"))
        {
            return iceStatus;
        }
        else if (dt.GetTypeName().Equals("Lightning"))
        {
            return lightningStatus;
        }
        else if (dt.GetTypeName().Equals("Poison"))
        {
            return poisonStatus;
        }
        else if (dt.GetTypeName().Equals("Psychic"))
        {
            return psychicStatus;
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

    public void Cleanse()
    {
        conditions = new List<string>();
        SetIsFrozen(0);
        SetIsPoisoned(0);
        SetIsBurning(new int[2] { 0, 0 });
        SetIsShocked(new int[2] { 0, 0 });
        SetIsVulnerable(new float[2] { 0, 0 });
        SetDamnation(new int[4] { 0, 0, 0, 0 });
    }


    private int barkTriggerCooldown;
    private int barkOnCooldown = 0;
    public int bonusBarkChance = 0;
    public Image barkTarget;
    public void VerifyBark(int playerHealth, List<Special> specials)
    {
        barkOnCooldown--;
        if(barkOnCooldown > 0)
        {
            return;
        }
        if(enemyNpc.interactableNpc != null && Random.Range(1, 101) <= (enemyNpc.interactableNpc.barkChance + bonusBarkChance))
        {
            if(playerHealth < 20)
            {
                barkOnCooldown = PlayerDeathBark();
            } else if (specials.Count > 0)
            {
                barkOnCooldown = AbilityBark();
            }
            else 
            {
                barkOnCooldown = RandomBark();
            }
        }
    }

    public int RandomBark()
    {
        DialogueBlock bark = enemyNpc.interactableNpc.GetRandomBark();
        if (bark != null)
        {
            DialogueManager.instance.ShowBarkWithOwner(enemyNpc.interactableNpc, barkTarget, bark);
            return barkTriggerCooldown;
        }
        return 0;
    }

    public int PlayerDeathBark()
    {
        DialogueBlock bark = enemyNpc.interactableNpc.GetOnPlayerDeathBark();
        if (bark != null)
        {
            DialogueManager.instance.ShowBarkWithOwner(enemyNpc.interactableNpc, barkTarget, bark);
            return barkTriggerCooldown;
        }
        return 0;
    }

    public int AbilityBark()
    {
        DialogueBlock bark = enemyNpc.interactableNpc.GetAbilityBark();
        if (bark != null)
        {
            DialogueManager.instance.ShowBarkWithOwner(enemyNpc.interactableNpc, barkTarget, bark);
            return barkTriggerCooldown;
        }
        return 0;
    }

    public void InitiateDeathDialogueSelf()
    {
        if (enemyNpc.isInteractable && enemyNpc.isBoss)
        {
            DialogueBlock block = enemyNpc.interactableNpc.GetDeathBark();
            if (block != null)
            {
                DialogueManager.instance.ShowDialogue(block);
            }
        }
    }

    public void InitiateDialogue()
    {
        if (enemyNpc.isInteractable)
        {
            DialogueBlock block = enemyNpc.interactableNpc.InitiateDialogue();
            if (block != null)
            {
                StartCoroutine(StartDialogue(block));
            }
        }
    }

    private IEnumerator StartDialogue(DialogueBlock block)
    {
        yield return new WaitForSeconds(1.8f);

        if(block != null)
        {
            DialogueManager.instance.ShowDialogue(block);
        }
    }

    public InteractableNpc GetNpc()
    {
        return enemyNpc.interactableNpc;
    }

    private readonly float dissolveTime = 0.75f;
    private readonly int dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    public bool deathAnim = false;

    public void KillThisNpc()
    {
        if (deathAnim) return;
        deathAnim = true;

        intentPanel.SetActive(false);
        tokenUi.transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
        Destroy(turnIndicator);
        Destroy(targetIndicator);
        isThinking.SetActive(false);
        hpDesc.transform.parent.parent.parent.gameObject.SetActive(false);
        cSystem.StartCoroutine(Vanish());
    }

    IEnumerator Vanish()
    {
        float elapsedTime = 0f;
        while(elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0, 1f, (elapsedTime / dissolveTime));
            if (elapsedTime > 0.5f) nameText.gameObject.SetActive(false);

            activeSprite.material.SetFloat(dissolveAmount, lerpedDissolve);
            yield return null;
        }
        gameObject.SetActive(false);
        cSystem.MoveOffscreen(gameObject);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public bool HasTrait(Traits trait)
    {
        foreach(Trait t in traits)
        {
            return t.trait == trait;
        }
        return false;
    }

    public int TraitAmount(Traits trait)
    {
        foreach (Trait t in traits)
        {
            if (t.trait == trait){ return t.powerAmount; }
        }
        return -1; 
    }
}

