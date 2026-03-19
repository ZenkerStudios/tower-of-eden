using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]

public class CombatSystem : CombatStateMachine, IPointerClickHandler
{
    public int numberOfMisses;
    public int critReroll, oldAttackCount, oldSpecialCount, oldDomKillCount, oldStrKillCount;

    public bool hitEveryAttack = true;
    public bool elite = false;
    public int turnUsed, totalDamageTaken, totalDamageDealt, divineDamageDealt, fireDamageDealt, iceDamageDealt, lightningDamageDealt, physicalDamageDealt, poisonDamageDealt, psychicDamageDealt;
    public int dodges, critHit, damageHealed, divineCond, fireCond, iceCond, lightningCond, poisonCond, psychicCond;
    
    public bool isMiniBoss;
    public bool playerTurn = true;
    public Floors encounterKey = Floors.Floor_01;
    public Reward reward;
    public string lastKilledBy;

    public PlayerManager player;
    public PlayerHero hero;
    public Image heroIcon;
    public Transform heroTransform;
    public List<EnemyNpc> enemyList = new List<EnemyNpc>();
    public List<string> defeatedEnemies = new List<string>();
    public int playerKillCount = 0;
    public int maxEnemyCount;

    protected static int descTimer = 5;

    static CombatSystem instance;
    public Difficulty difficulty = Difficulty.Easy;

    public IAbility abilityUsed = null;
    public int targetIndex = 0;
    public bool targetSelected = false;
    public int enemyTurnIndex = 0;

    //Returns how many enemies the player has killed
    public virtual int enemyCount
    {
        get
        {
            return enemyPanel.transform.childCount;
        }
    }

    public GameObject eventAlert;

    public Animator textAnimator;
    public Animator hudAnimator;

    public GameObject rewardScreen;

    public GameObject enemyPanel;
    public GameObject pfEnemyToken;
    public SpecialSystem specialSystem;

    public GameObject heroHud;
    public GameObject heroShield;
    public TextMeshProUGUI heroShieldText;

    public GameObject heroCondPanel;
    public Slider heroHealthSlider;
    public Slider heroXpSlider;

    public Button heroAttack;
    public Button heroSpecialOne;
    public Button heroSpecialTwo;
    public Button heroSpecialThree;
    public Button heroSpecialFour;
    public Button heroSpecialFive;
    public Button heroSpecialSix;
    public Button heroSpecialSeven;
    public Button heroSpecialEight;
    public Button heroSpecialNine;
    public Button heroSpecialTen;

    public Button rerollButton;
    public int rerollThisTurn;

    public TextMeshProUGUI turnText;
    public GameObject abilityDescription;
    public GameObject conditionDescription;
    public GameObject characterDescription;
    public TextMeshProUGUI hpDesc;
    public TextMeshProUGUI accDesc;
    public TextMeshProUGUI ccDesc;
    public TextMeshProUGUI actDesc;
    public TextMeshProUGUI tghDesc;
    public TextMeshProUGUI alsDesc;
    public TextMeshProUGUI slsDesc;
    public TextMeshProUGUI strDesc;
    public TextMeshProUGUI domDesc;

    public GameObject iceFx;
    public FloorManager fm;

    public List<EnemyNpc> ragnaSpecialSummons;
    public List<EnemyNpc> dorianModes;
    public List<string> dorianModesUsed;
    public List<Sprite> lyraUleaTransformation;
    public int verse = 0;
    public int seal = 0;
    public bool isApotheosis;
    public int totalApotheosisTurn = 0;

    void Awake()
    {
        /**
         * called when an instance awakes in the game
         * set our static reference to our newly initialized instance
         */
        rerollButton.transform.parent.GetChild(1).gameObject.SetActive(true);
        eventAlert.SetActive(false);
        instance = this;


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DisableDesc();
    }

    // Start is called before the first frame update
    void Start()
    {
        fm = GameObject.Find("Floor Manager").GetComponent<FloorManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        player.h.iceFx = iceFx;
        iceFx.SetActive(false);
        heroSpecialNine.transform.parent.parent.gameObject.SetActive(player.h.GetMaxAbilityCount() > 8);
        heroSpecialTen.transform.parent.parent.gameObject.SetActive(player.h.GetMaxAbilityCount() > 8);

        heroXpSlider.maxValue = player.h.MaxXp();
        DisableDesc();

        maxEnemyCount = GameController.GetMaxEnemiesCount();
        SetUpBattle();
        StartCombat();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeroBattleStats();

        turnText.text = "Turn " + turnUsed;

        heroXpSlider.value = hero.slotXp + xpGainedThisEncounter;
    }

    public void StartCombat()
    {
        SetState(new BeginBattle(instance));
    }

    public void CanReroll()
    {
        rerollButton.interactable = false;
        rerollButton.transform.parent.GetChild(1).gameObject.SetActive(true);
        if (rerollThisTurn > 0)
        {
            rerollButton.transform.parent.GetChild(1).gameObject.SetActive(false);
            rerollButton.interactable = true;
        }
    }

    public void PerformReroll(int subtract)
    {
        abilityUsed = null;
        DeselectAllTargets();
        DisableDesc();
        DisableAbilitySelection();
        hero.GenerateSpecials();
        rerollThisTurn -= subtract;
        CanReroll();
        UpdateAbilitySprites();
    }
    public int xpGainedThisEncounter;
    private bool attackLastUsed;
    private bool specialLastUsed;
    public void GainXp()
    {
        if (abilityUsed == null) return;

        int xpTogain = 3;

        if (abilityUsed is Attack && attackLastUsed) xpTogain = 1;
        else if (abilityUsed is Special && specialLastUsed) xpTogain = 2;

        if (abilityUsed.GetAbilityType() == AbilityTypes.Affliction) xpTogain++;

        if (player.h.playerInventory.bonusSlotXp) xpTogain += 2;

        attackLastUsed = abilityUsed is Attack;
        specialLastUsed = abilityUsed is Special;
        xpGainedThisEncounter += xpTogain;
    }

    public void ActionButtonsInteractable(bool val)
    {
        rerollButton.interactable = val;
        heroAttack.interactable = val;
        heroSpecialOne.interactable = val;
        heroSpecialTwo.interactable = val;
        heroSpecialThree.interactable = val;
        heroSpecialFour.interactable = val;
        heroSpecialFive.interactable = val;
        heroSpecialSix.interactable = val;
        heroSpecialSeven.interactable = val;
        heroSpecialEight.interactable = val;
        heroSpecialNine.interactable = val;
        heroSpecialTen.interactable = val;

        CanReroll();
    }

    public void DisableAbilitySelection()
    {
        heroAttack.transform.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialOne.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialTwo.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialThree.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialFour.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialFive.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialSix.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialSeven.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialEight.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialNine.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        heroSpecialTen.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
    }
    public void UpdateAbilitySprites()
    {
        heroAttack.image.sprite = hero.baseAttack.abilitySprite;

        heroSpecialOne.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialTwo.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialThree.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialFour.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialFive.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialSix.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialSeven.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialEight.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialNine.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        heroSpecialTen.transform.parent.parent.GetChild(1).gameObject.SetActive(false);



        for (int x = 0; x < hero.generatedSpecials.Count; x++)
        {
            switch (x)
            {
                case 0:
                    heroSpecialOne.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialOne.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 1:
                    heroSpecialTwo.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialTwo.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 2:
                    heroSpecialThree.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialThree.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 3:
                    heroSpecialFour.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialFour.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 4:
                    heroSpecialFive.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialFive.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 5:
                    heroSpecialSix.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialSix.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 6:
                    heroSpecialSeven.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialSeven.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 7:
                    heroSpecialEight.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialEight.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 8:
                    heroSpecialNine.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialNine.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
                case 9:
                    heroSpecialTen.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                    heroSpecialTen.image.sprite = hero.generatedSpecials[x].abilitySprite;
                    break;
            }
        }

    }

    //Sets up battle UI and displays everything necessary and play animations
    public void SetUpBattle()
    {
        ShowHeroInfo();
        GetNextRoundOfEnemies();
        SpawnAllEnemies();
    }

    private int lastHp = 0;
    private void OnHealthChange(float health)
    {
        if (lastHp > 25 && health <= 25)
        {
            AudioManager.instance.PlaySfxSound("Heart_Beat");
        }
        else if (lastHp <= 25 && health > 25)
        {
            AudioManager.instance.StopSfxSound("Heart_Beat");
        }
        lastHp = (int)health;
    }

    public void ShowHeroInfo()
    {

        hero = player.h;
        lastHp = hero.hp;
        heroHealthSlider.onValueChanged.AddListener(OnHealthChange);

        heroIcon.sprite = hero.heroSprite;
        heroHealthSlider.maxValue = hero.GetMaxHealth();
        heroHealthSlider.value = hero.hp;
        UpdateHeroBattleStats();
    }
    //Get a list of enemies, either random or designated encounter
    public void GetNextRoundOfEnemies()
    {
        //Checks if enemy list already has a special or boss encounter assigned to it. If not gives random one.
        try
        {
            if (enemyList.Count <= 0)
            {
                enemyList = fm.GetRandomEnemiesForFloor();
            }
        }
        catch (ArgumentNullException)
        {
            enemyList = fm.GetRandomEnemiesForFloor();
        }
    }

    //Sets battleHUD and tokens for enemies
    public void SpawnAllEnemies()
    {
        foreach (EnemyNpc npc in enemyList)
        {
            SpawnEnemy(npc, player.h.playerInventory.enemyDazedStart, false);
        }

        StartCoroutine(PlaySpawnAnim());

    }

    private IEnumerator PlaySpawnAnim()
    {
        EnemyToken hasDialogue = null;
        yield return new WaitForSeconds(1f);
        for (int x = 0; x < enemyPanel.transform.childCount; x++)
        {
            enemyPanel.transform.GetChild(x).gameObject.SetActive(true);
            EnemyToken eHUD = enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            if (eHUD.enemyNpc.isBoss && eHUD.enemyNpc.isInteractable)
            {
                hasDialogue = eHUD;
            }
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(.3f);
        if (hasDialogue != null) hasDialogue.InitiateDialogue();

    }

    public GameObject SpawnEnemy(EnemyNpc npc, bool dazed, bool displayOnSpawn)
    {
        CheckBossInteraction(npc);
        GameObject newObj = GameObject.Instantiate(pfEnemyToken);
        EnemyToken token = newObj.GetComponent<EnemyToken>();
        token.enemyNpc = npc;
        token.isDazed = dazed;
        token.dazedIcon.SetActive(dazed);
        token.isThinking.SetActive(!dazed);
        token.hp = 1;
        token.turnIndicator.SetActive(false);
        newObj.transform.SetParent(enemyPanel.transform);
        newObj.transform.localPosition = new Vector2(0, 0);
        newObj.GetComponent<RectTransform>().sizeDelta = pfEnemyToken.GetComponent<RectTransform>().sizeDelta;
        newObj.GetComponent<RectTransform>().localScale = pfEnemyToken.GetComponent<RectTransform>().localScale;
        newObj.gameObject.SetActive(displayOnSpawn);
        AudioManager.instance.PlaySfxSound(AbilityTypes.Summon.ToString());
        if (token.GetNpc() != null && token.GetNpc().thisNpc == InteractableNpcs.Priestess)
        {
            token.CreateOtherCond("Verse", "Lyra sings her Final Verse after 3 Perfect Pitch. (Perfect Pitch to Final Verse: /efx)", "61");
        }
        if (token.GetNpc() != null && token.GetNpc().thisNpc == InteractableNpcs.Eden)
        {
            token.CreateOtherCond("Seal", "After the 10th cast, Eden will seal the compendium.\nCast: /efx", "63");
        }
        return newObj;

    }

    private void CheckBossInteraction(EnemyNpc enemy)
    {
        if (!enemy.isInteractable || enemy.interactableNpc == null) return;
        //Check number of interaction level
        switch (enemy.interactableNpc.thisNpc)
        {
            case InteractableNpcs.Empress:
                player.numEmpressInteraction++;
                break;
            case InteractableNpcs.HangedMan:
                player.numHangedmanInteraction++;
                break;
            case InteractableNpcs.Magician:
                player.numMagicianInteraction++;
                break;
            case InteractableNpcs.Fool:
                player.numFoolInteraction++;
                break;
            case InteractableNpcs.Priestess:
                player.numPriestessInteraction++;
                break;
            case InteractableNpcs.Star:
                player.numStarInteraction++;
                break;
            case InteractableNpcs.King:
                player.numKingInteraction++;
                break;
            case InteractableNpcs.None:
                break;
        }
    }

    //Returns true if all enemies are dead, false otherwise
    public bool AllNPCDead()
    {
        for (int x = 0; x < enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            if (!eHUD.IsDead())
            {
                return false;
            }
        }
        return true;
    }

    //Returns true if all heroes are dead, else false
    public bool HeroDead()
    {
        if (hero.hp <= 0)
        {
            if (!player.usedArtifactRevive && player.numArtifactRevive > 0)
            {
                int max = hero.GetMaxHealth()/2;
                hero.ChangeHealth(max);
                player.usedArtifactRevive = true;
                player.numArtifactRevive = 0;
                TriggerRevive();
            }
            else if (player.numRevives > 0)
            {
                int max = hero.GetMaxHealth()/2;
                hero.ChangeHealth(max);
                player.numRevives--;
                TriggerRevive();
            }

        }
        return hero.hp <= 0;
    }

    private void TriggerRevive()
    {
        player.RenderRevives();
    }

    private List<EnemyToken> targetList = new List<EnemyToken>();
    public void TriggerAttack()
    {
        SetAbilityDesc();

        if (AllNPCDead())
        {
            StartAttack();
        }

        if (hero.baseAttack.GetTargetTypes() is TargetTypes.OnSelf)
        {
            abilityUsed = hero.baseAttack;
            GainXp();
            targetIndex = 0;
            if (hero.baseAttack.GetVfx() != null)
            {
                GetCombatState().CreateFx(hero.baseAttack.GetVfx(), heroIcon.transform.parent.parent.transform);
            }
            abilityUsed = null;
            StartCoroutine(hero.baseAttack.TriggerAbility(hero, heroTransform, hero, heroTransform, PercentileChanceToHappen(hero.critChance.Value), this));
            StartAttack();
            AudioManager.instance.PlaySfxSound(hero.baseAttack.GetAudio());
            hero.playerInventory.attacksUsedThisEncounter++;
        }
        else
        {
            abilityUsed = hero.baseAttack;
            targetIndex = 0;
            GetPotentialTargets(hero.baseAttack, 0);
            targetList = GetAllTargets();
        }
    }


    public void TriggerSpecial(int index)
    {
        SetAbilityDesc();

        if (AllNPCDead())
        {
            StartAttack();
        }

        Special sp = hero.generatedSpecials[index];
        if (sp.GetTargetTypes() is TargetTypes.OnSelf)
        {
            targetIndex = 0;
            abilityUsed = sp;
            GainXp();
            if (sp.GetVfx() != null)
            {
                GetCombatState().CreateFx(sp.GetVfx(), heroIcon.transform.parent.parent.transform);
            }
            abilityUsed = null;
            StartCoroutine(sp.TriggerAbility(hero, heroTransform, hero, heroTransform, PercentileChanceToHappen(hero.critChance.Value), this));
            StartAttack();
            AudioManager.instance.PlaySfxSound(sp.GetAudio());
            hero.playerInventory.specialsUsedThisEncounter++;

        }
        else
        {
            abilityUsed = sp;
            targetIndex = 0;
            GetPotentialTargets(sp, 0);
            targetList = GetAllTargets();
        }
    }

    public bool PercentileChanceToHappen(float percentile)
    {
        return UnityEngine.Random.Range(1, 101) <= percentile;
    }

    public List<EnemyNpc> deadEnemyNpcs;
    private void DestroyEnemy(EnemyToken eHUD)
    {
        if (eHUD == null || eHUD.deathAnim)
        {
            return;
        }

        if (eHUD.GetName().Equals("Hellthorn Banshee 2")
            || eHUD.GetName().Equals("Scraps"))
        {
            deadEnemyNpcs.Add(Instantiate(eHUD.enemyNpc.reviveTokens));
        }
        else
        {
            deadEnemyNpcs.Add(eHUD.enemyNpc);
        }
        defeatedEnemies.Add(eHUD.GetName());
                   
        if (player.revengeanceTrait > 0) hero.ChangeHealth(5);

        eHUD.KillThisNpc();
    }

    [SerializeField] private Transform offscreenParent;
    public void MoveOffscreen(GameObject go)
    {
        go.transform.SetParent(offscreenParent);
        go.transform.localPosition = new Vector2(0, 0);
        go.GetComponent<RectTransform>().sizeDelta = go.GetComponent<RectTransform>().sizeDelta;
        go.GetComponent<RectTransform>().localScale = go.GetComponent<RectTransform>().localScale;
    }

    //Updates NPC stats after damage phase
    public void UpdateNpcBattleStats()
    {
        bool spawn = false;
        bool echo = false;
        List<EnemyToken> toDestroyForRevive = new List<EnemyToken>();
        List<EnemyToken> toDestroyForEcho = new List<EnemyToken>();
        for (int x = 0; x < enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            GetCombatState().ApotheosisTurn(eHUD);
            eHUD.HideTargetIndicator();
            eHUD.UpdateInfo();
            if (eHUD.IsDead())
            {
                if (eHUD.canRevive)
                {
                    spawn = true;
                    toDestroyForRevive.Add(eHUD);
                }
                else if (eHUD.echoAbillity)
                {
                    echo = true;
                    toDestroyForEcho.Add(eHUD);
                }
                else
                {
                    if (eHUD.abilityAfterDeath)
                    {
                        if (eHUD.GetName().Equals("Cocoon") && !playerTurn)
                        {
                            StartCoroutine(eHUD.deathAbility.
                                TriggerAbility(eHUD, eHUD.transform,
                                enemyPanel.transform.GetChild(0).GetComponent<EnemyToken>(),
                                enemyPanel.transform.GetChild(0).transform,
                                PercentileChanceToHappen(eHUD.critChance.Value), this));
                        }
                        else
                        {
                            TriggerAbility(eHUD, eHUD.deathAbility);
                        }
                    }
                    else if (eHUD.abilityOnSelfDestruct && !playerTurn)
                    {
                        if (eHUD.deathAbility.abilityType == AbilityTypes.Summon)
                        {
                            spawn = true;
                            toDestroyForRevive.Add(eHUD);
                        }
                        else
                        {
                            TriggerAbility(eHUD, eHUD.deathAbility);
                        }
                    }
                    eHUD.InitiateDeathDialogueSelf();
                    playerKillCount++;
                    if (!isMiniBoss)
                    {
                        isMiniBoss = fm.bossNames.Contains(eHUD.GetName());
                    }

                    StartCoroutine(GameUtilities.WaitForConversation(() => DestroyEnemy(eHUD)));


                }
            }
        }

        if (spawn)
        {
            foreach (EnemyToken eHUD in toDestroyForRevive)
            {
                EnemyNpc reviveToken = eHUD.reviveTokens;
                int index = eHUD.transform.GetSiblingIndex();
                GameObject newObj = SpawnEnemy(reviveToken, eHUD.dazedResAbility, true);
                Destroy(eHUD.enemyNpc);
                Destroy(eHUD.gameObject);
                newObj.transform.SetSiblingIndex(index);
            }
        }

        if (echo)
        {
            foreach (EnemyToken eHUD in toDestroyForEcho)
            {
                playerKillCount++;
                EnemyNpc echoToken = eHUD.echoTokens;
                int index = eHUD.transform.GetSiblingIndex();
                GameObject newObj = SpawnEnemy(echoToken, eHUD.dazedResAbility, true);
                Destroy(eHUD.enemyNpc);
                Destroy(eHUD.gameObject);
                newObj.transform.SetSiblingIndex(index);
            }

        }

    }

    private void TriggerAbility(EnemyToken eHUD, IAbility ability)
    {
        if (PercentileChanceToHappen(eHUD.accuracy.Value - eHUD.GetIsShocked()[1]))
        {
            bool willHurt = ability.GetAbilityType() == AbilityTypes.Offensive
                || ability.GetAbilityType() == AbilityTypes.Affliction;
            if (hero.GetBlock() > 0 && willHurt)
            {
                cState.BlockAttack(hero, heroTransform);
            }
            else
            {
                StartCoroutine(ability.
                    TriggerAbility(eHUD, eHUD.transform, hero, heroTransform, PercentileChanceToHappen(eHUD.critChance.Value), this));
            }
        }
        else
        {
            ability.TriggerNoHit(hero.transform, "Miss");
        }
    }

    //Updates NPC stats after damage phase
    public void UnshowInfo()
    {
        for (int x = 0; x < enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            eHUD.ResetTokenInfo();
        }

    }

    public void PrepareEnemyAttacks()
    {
        for (int x = 0; x < enemyPanel.transform.childCount; x++)
        {
            EnemyToken eHUD = enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            if (eHUD.isDazed)
            {
                eHUD.dazedIcon.SetActive(true);
            }
            else if (eHUD.preparedMoves.Count <= 0 && eHUD.GetActions().Value > 0)
            {
                cState.IsEnemySquadFullHealth();
                cState.damagedEnemies.Remove(eHUD);
                eHUD.SetMoves(maxEnemyCount - enemyCount, cState.damagedEnemies.Count <= 0);
                if (eHUD.GetName().Equals("Ragna, The Empress"))
                {
                    EnemyToken cocoon = cState.isCocoonAvailable();
                    if (cocoon && cocoon.selfDestructTurn <= 1)
                    {
                        eHUD.preparedMoves = new List<Special>();
                        eHUD.preparedMoves.Add(eHUD.enemyNpc.specialAbility);
                        for(int newAbilities = 1; newAbilities < eHUD.GetActions().Value; newAbilities++)
                        {
                            eHUD.preparedMoves.Add(eHUD.enemyNpc.attacks[0]);
                        }
                    }

                    if (cState.areSpiderlingsAvailable().Count > 0 && eHUD.GetSpecialMeter() > 13)
                    {
                        eHUD.preparedMoves = new List<Special>();
                        eHUD.preparedMoves.Add(eHUD.enemyNpc.deathAbility);
                        for (int newAbilities = 1; newAbilities < eHUD.GetActions().Value; newAbilities++)
                        {
                            eHUD.preparedMoves.Add(eHUD.enemyNpc.attacks[0]);
                        }
                    }
                }
                else if (eHUD.GetName().Equals("Lyra, The High Priestess") && verse > 2)
                {
                    eHUD.preparedMoves = new List<Special>();
                    eHUD.preparedMoves.Add(eHUD.enemyNpc.specialAbility);
                }
                else if (eHUD.GetName().Equals("Eden"))
                {
                    eHUD.preparedMoves = new List<Special>();
                    eHUD.preparedMoves.AddRange(eHUD.enemyNpc.attacks);
                }
                eHUD.RenderIntent();
            }
            else
            {
                eHUD.RenderThinking();
            }
        }
    }
    public void RemoveHeroCondition(string condName)
    {
        for (int x = 0; x < heroCondPanel.transform.childCount; x++)
        {
            ConditionIndicator condition = heroCondPanel.transform.GetChild(x).GetComponent<ConditionIndicator>();
            if (condition.condName.Equals(condName))
            {
                GameObject child = heroCondPanel.transform.GetChild(x).gameObject;
                Destroy(child);
                break;
            }
        }
    }

    public void UpdateHeroBattleStats()
    {
        heroShield.SetActive(false);
        if (hero.block > 0)
        {
            heroShield.SetActive(true);
            heroShieldText.text = hero.block + "";
        }
        heroHealthSlider.value = hero.hp;
        heroHealthSlider.maxValue = hero.GetMaxHealth();
    }

    public void DeselectAllTargets()
    {
        UpdateHeroBattleStats();
        UpdateNpcBattleStats();
    }

    public static readonly string uleaCondName = "Star Revealed";
    public static readonly string uleaCondDesc = "Izaak cannot attack Ulea.";

    private bool CheckUleaCondition(string npc)
    {
        return hero.GetConditions().Contains(uleaCondName) && npc.Equals("Ulea, The Star");
    }

    public bool IsEnemyTargetable(EnemyToken enemy)
    {
        return targetList.Contains(enemy);
    }

    public List<EnemyToken> GetAllTargets()
    {
        List<EnemyToken> targets = new List<EnemyToken>();

        for (int x = 0; x < enemyPanel.transform.childCount; x++)
        {
            EnemyToken enemy = enemyPanel.transform.GetChild(x).GetComponent<EnemyToken>();
            if (!CheckUleaCondition(enemy.GetName()))
            {
                enemy.ShowAsPotentialTarget();
                targets.Add(enemy);
            }
        }

        return targets;
    }

    public List<EnemyToken> GetPotentialTargets(IAbility ability, int startingIndex)
    {
        DeselectAllTargets();
        List<EnemyToken> targets = new List<EnemyToken>();

        if (startingIndex >= (enemyPanel.transform.childCount - (int)ability.GetNumTarget()))
        {
            startingIndex = Mathf.Clamp(enemyPanel.transform.childCount - (int)ability.GetNumTarget(), 0, enemyPanel.transform.childCount - 1); ;
        }

        for (int x = 0; x < (int)ability.GetNumTarget() && startingIndex < enemyPanel.transform.childCount; x++, startingIndex++)
        {
            EnemyToken enemy = enemyPanel.transform.GetChild(startingIndex).GetComponent<EnemyToken>();
            if (!CheckUleaCondition(enemy.GetName()))
            {
                enemy.ShowAsPotentialTarget();
                targets.Add(enemy);
            }
        }

        return targets;
    }

    public EnemyToken GetNextEnemy()
    {
        EnemyToken token = enemyPanel.transform.GetChild(enemyTurnIndex).GetComponent<EnemyToken>();
        token.GetSpritePos();
        enemyTurnIndex++;
        return token;
    }


    public EnemyToken GetCurrentEnemy()
    {
        EnemyToken token = enemyPanel.transform.GetChild(enemyTurnIndex).GetComponent<EnemyToken>();
        token.GetSpritePos();
        return token;
    }

    public bool LastEnemyToGo()
    {
        return enemyTurnIndex >= enemyPanel.transform.childCount;
    }

    public void DisableDesc()
    {
        abilityDescription.SetActive(false);
        conditionDescription.SetActive(false);
        characterDescription.SetActive(false);
    }

    public void SetAbilityDesc()
    {
        DisableDesc();
        abilityDescription.SetActive(true);
    }

    public void SetConditionDesc()
    {
        DisableDesc();
        conditionDescription.SetActive(true);
    }

    public void SetCharacterDesc(ICharacter character)
    {
        DisableDesc();
        if (character is PlayerHero h)
        {
            hpDesc.text = "<sprite=45> HP: " + h.hp + "/" + h.GetMaxHealth();
            accDesc.text = "<sprite=46> Accuracy: " + h.GetAccuracy().Value;
            ccDesc.text = "<sprite=47> Crit Chance: " + h.GetCritChance().Value;
            actDesc.text = "<sprite=48> Actions: " + h.GetActions().Value;
            tghDesc.text = "<sprite=49> Toughness: " + h.GetToughness().Value;
            slsDesc.text = "<sprite=55> Lifesteal: " + h.GetSpecialLifeSteal().Value;
            domDesc.text = "<sprite=54> Dominance: " + h.GetDominance().Value;
            alsDesc.text = "<sprite=50> Lifestrike: " + h.GetAttackLifestrike().Value;
            strDesc.text = "<sprite=51> Strength: " + h.GetStrength().Value;
        }
        characterDescription.SetActive(true);
    }

    public void BeginFloorSpecialEnc()
    {
        GameUtilities.DeleteAllChildGameObject(enemyPanel);
        fm.floorEventTriggered = false;
        enemyList = new List<EnemyNpc>();
        enemyList.AddRange(fm.floorSpecialEncounter[0].boss);
        foreach (EnemyNpc enemyNpc in enemyList)
        {
            SpawnEnemy(enemyNpc, true, true);
        }
    }

    public void BeginDorianSpecialEnc()
    {
        GameUtilities.DeleteAllChildGameObject(enemyPanel);
        difficulty = Difficulty.Accursed;
        enemyList = new List<EnemyNpc>();
        enemyList.AddRange(fm.finalBosses[0].boss);
        foreach (EnemyNpc enemyNpc in enemyList)
        {
            SpawnEnemy(enemyNpc, false, true);
        }
    }

    public void TrackBountyDamage(ICharacter target, DamageTypeEnumValue damageType, int amount)
    {
        if (target is EnemyToken && amount < 0)
        {
            totalDamageDealt += amount;
            switch (damageType.spriteIndex)
            {
                case "39":
                    divineDamageDealt += amount;
                    break;
                case "37":
                    fireDamageDealt += amount;
                    break;
                case "42":
                    iceDamageDealt += amount;
                    break;
                case "41":
                    lightningDamageDealt += amount;
                    break;
                case "43":
                    physicalDamageDealt += amount;
                    break;
                case "38":
                    poisonDamageDealt += amount;
                    break;
                case "40":
                    psychicDamageDealt += amount;
                    break;
            }
        } else if (target is PlayerHero)
        {
            if (amount > 0)
            {
                TrackBountyHeal(target, amount);
            } else
            {
                totalDamageTaken += amount;
            }
                
        }
    }
    

    public void TrackBountyCondition(string damageType)
    {
        switch (damageType)
        {
            case "39":
                divineCond++;
                break;
            case "37":
                fireCond++;
                break;
            case "42":
                iceCond++;
                break;
            case "41":
                lightningCond++;
                break;
            case "38":
                poisonCond++;
                break;
            case "40":
                psychicCond++;
                break;
        }
    }

    public void TrackBountyHeal(ICharacter target, int amount)
    {
        if (target is PlayerHero && amount > 0)
        {
            damageHealed += amount;
        }
    }
}
