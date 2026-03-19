using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AcademyHub : MonoBehaviour
{
    public TextMeshProUGUI descriptionBox;
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;

    public Animator windowAnimator;

    private PlayerManager player;

    public TextMeshProUGUI ownedGrimoire;

    #region all enchantments
    private readonly int burnCost = 5;
    private readonly int burnTurnIncreaseMax = 1;
    public TextMeshProUGUI burnBonusText;
    public TextMeshProUGUI burnBonusButtonText;
    public Toggle burnBonusToggle;
    public Button burnBonusButton;


    private readonly int shockedCost = 5;
    private readonly int shockedTurnIncreaseMax = 1;
    public TextMeshProUGUI shockedBonusText;
    public TextMeshProUGUI shockedBonusButtonText;
    public Toggle shockedBonusToggle;
    public Button shockedBonusButton;


    private readonly int vulnerableCost = 5;
    private readonly int vulnerableTurnIncreaseMax = 1;
    public TextMeshProUGUI vulnerableBonusText;
    public TextMeshProUGUI vulnerableBonusButtonText;
    public Toggle vulnerableBonusToggle;
    public Button vulnerableBonusButton;


    private readonly int damnedCost = 5;
    private readonly int damnedStackIncreaseMax = 1;
    public TextMeshProUGUI damnedBonusText;
    public TextMeshProUGUI damnedBonusButtonText;
    public Toggle damnedBonusToggle;
    public Button damnedBonusButton;


    private readonly int reviveCost = 20;
    private readonly int reviveIncreaseMax = 5;
    public TextMeshProUGUI reviveBonusText;
    public TextMeshProUGUI reviveBonusButtonText;
    public Toggle reviveBonusToggle;
    public Button reviveBonusButton;


    private readonly int maxHpCost = 1;
    private readonly int maxHealthIncreaseMax = 50;
    public TextMeshProUGUI maxHealthBonusText;
    public TextMeshProUGUI maxHealthBonusButtonText;
    public Toggle maxHealthBonusToggle;
    public Button maxHealthBonusButton;


    private readonly int accuracyCost = 1;
    private readonly int accuracyIncreaseMax = 10;
    public TextMeshProUGUI accuracyBonusText;
    public TextMeshProUGUI accuracyBonusButtonText;
    public Toggle accuracyBonusToggle;
    public Button accuracyBonusButton;


    private readonly int critCost = 1;
    private readonly int critChanceIncreaseMax = 20;
    public TextMeshProUGUI critChanceBonusText;
    public TextMeshProUGUI critChanceBonusButtonText;
    public Toggle critChanceBonusToggle;
    public Button critChanceBonusButton;


    private readonly int actionCost = 25;
    private readonly int actionIncreaseMax = 1;
    public TextMeshProUGUI actionBonusText;
    public TextMeshProUGUI actionBonusButtonText;
    public Toggle actionBonusToggle;
    public Button actionBonusButton;


    private readonly int fireCost = 1;
    private readonly int fireResitIncreaseMax = 20;
    public TextMeshProUGUI fireResistBonusText;
    public TextMeshProUGUI fireResistBonusButtonText;
    public Toggle fireResistBonusToggle;
    public Button fireResistBonusButton;


    private readonly int iceCost = 1;
    private readonly int iceResistIncreaseMax = 20;
    public TextMeshProUGUI iceResistBonusText;
    public TextMeshProUGUI iceResistBonusButtonText;
    public Toggle iceResistBonusToggle;
    public Button iceResistBonusButton;


    private readonly int lightningCost = 1;
    private readonly int lightningResistIncreaseMax = 20;
    public TextMeshProUGUI lightningResistBonusText;
    public TextMeshProUGUI lightningResistBonusButtonText;
    public Toggle lightningResistBonusToggle;
    public Button lightningResistBonusButton;


    private readonly int psychicCost = 1;
    private readonly int psychicResistIncreaseMax = 20;
    public TextMeshProUGUI psychicResistBonusText;
    public TextMeshProUGUI psychicResistBonusButtonText;
    public Toggle psychicResistBonusToggle;
    public Button psychicResistBonusButton;


    private readonly int poisonCost = 1;
    private readonly int poisonResistIncreaseMax = 20;
    public TextMeshProUGUI poisonResistBonusText;
    public TextMeshProUGUI poisonResistBonusButtonText;
    public Toggle poisonResistBonusToggle;
    public Button poisonResistBonusButton;


    private readonly int hpCost = 1;
    private readonly int entryHealthIncreaseMax = 5;
    public TextMeshProUGUI entryHealthBonusText;
    public TextMeshProUGUI entryHealthBonusButtonText;
    public Toggle entryHealthBonusToggle;
    public Button entryHealthBonusButton;


    private readonly int goldCost = 1;
    private readonly int goldStartMax = 50;
    public TextMeshProUGUI goldStartText;
    public TextMeshProUGUI goldStartButtonText;
    public Toggle goldStartToggle;
    public Button goldStartBonusButton;


    private readonly int fireChanceCost = 3;
    private readonly int fireChanceMax = 30;
    public TextMeshProUGUI fireChanceText;
    public TextMeshProUGUI fireChanceButtonText;
    public Toggle fireChanceToggle;
    public Button fireChanceBonusButton;


    private readonly int iceChanceCost = 2;
    private readonly int iceChanceMax = 30;
    public TextMeshProUGUI iceChanceText;
    public TextMeshProUGUI iceChanceButtonText;
    public Toggle iceChanceToggle;
    public Button iceChanceBonusButton;


    private readonly int lightningChanceCost = 2;
    private readonly int lightningChanceMax = 30;
    public TextMeshProUGUI lightningChanceText;
    public TextMeshProUGUI lightningChanceButtonText;
    public Toggle lightningChanceToggle;
    public Button lightningChanceBonusButton;


    private readonly int poisonChanceCost = 2;
    private readonly int poisonChanceMax = 30;
    public TextMeshProUGUI poisonChanceText;
    public TextMeshProUGUI poisonChanceButtonText;
    public Toggle poisonChanceToggle;
    public Button poisonChanceBonusButton;


    private readonly int psychicChanceCost = 2;
    private readonly int psychicChanceMax = 30;
    public TextMeshProUGUI psychicChanceText;
    public TextMeshProUGUI psychicChanceButtonText;
    public Toggle psychicChanceToggle;
    public Button psychicChanceBonusButton;


    private readonly int entryGoldCost = 1;
    private readonly int entryGoldMax = 15;
    public TextMeshProUGUI entryGoldText;
    public TextMeshProUGUI entryGoldButtonText;
    public Toggle entryGoldToggle;
    public Button entryGoldBonusButton;


    private readonly int attackLifestealCost = 10;
    private readonly int attackLifestealMax = 25;
    public TextMeshProUGUI attackLifestealText;
    public TextMeshProUGUI attackLifestealButtonText;
    public Toggle attackLifestealToggle;
    public Button attackLifestealBonusButton;


    private readonly int specialLifestealCost = 10;
    private readonly int specialLifestealMax = 25;
    public TextMeshProUGUI specialLifestealText;
    public TextMeshProUGUI specialLifestealButtonText;
    public Toggle specialLifestealToggle;
    public Button specialLifestealBonusButton;
    #endregion
    public TextMeshProUGUI acridNotes;

    private readonly int maxEquippedBonuses = 8;
    private List<string> equippedBonuses = new List<string>();

    [SerializeField]
    protected FriendlyToken friendlyToken;
    private DialogueManager dialogueManager;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        dialogueManager = DialogueManager.instance;

        equippedBonuses = new List<string>(player.equippedBonuses);

        burnBonusToggle.isOn = false;
        shockedBonusToggle.isOn = false;
        damnedBonusToggle.isOn = false;
        vulnerableBonusToggle.isOn = false;
        reviveBonusToggle.isOn = false;
        maxHealthBonusToggle.isOn = false;
        accuracyBonusToggle.isOn = false;
        critChanceBonusToggle.isOn = false;

        fireResistBonusToggle.isOn = false;
        iceResistBonusToggle.isOn = false;
        lightningResistBonusToggle.isOn = false;
        psychicResistBonusToggle.isOn = false;
        poisonResistBonusToggle.isOn = false;
        actionBonusToggle.isOn = false;
        entryHealthBonusToggle.isOn = false;
        goldStartToggle.isOn = false;

        fireChanceToggle.isOn = false;
        iceChanceToggle.isOn = false;
        lightningChanceToggle.isOn = false;
        psychicChanceToggle.isOn = false;
        poisonChanceToggle.isOn = false;
        entryGoldToggle.isOn = false;
        attackLifestealToggle.isOn = false;
        specialLifestealToggle.isOn = false;

        SetEnchantmentUiName();
    }

    // Start is called before the first frame update
    void Start()
    {

        if (player.wentToAcademyThisRun)
        {
            friendlyToken.newInteraction = false;
            friendlyToken.urgentSymbol.SetActive(false);
        }

        foreach (string s in equippedBonuses)
        {
            switch (s)
            {
                case "burnBonus":
                    burnBonusToggle.isOn = true;
                    break;
                case "shockedBonus":
                    shockedBonusToggle.isOn = true;
                    break;
                case "vulnerableBonus":
                    vulnerableBonusToggle.isOn = true;
                    break;
                case "damnedBonus":
                    damnedBonusToggle.isOn = true;
                    break;
                case "reviveBonus":
                    reviveBonusToggle.isOn = true;
                    break;
                case "maxHealthBonus":
                    maxHealthBonusToggle.isOn = true;
                    break;
                case "accuracyBonus":
                    accuracyBonusToggle.isOn = true;
                    break;
                case "critChanceBonus":
                    critChanceBonusToggle.isOn = true;
                    break;
                case "actionBonus":
                    actionBonusToggle.isOn = true;
                    break;
                case "fireResistBonus":
                    fireResistBonusToggle.isOn = true;
                    break;
                case "iceResistBonus":
                    iceResistBonusToggle.isOn = true;
                    break;
                case "lightningResistBonus":
                    lightningResistBonusToggle.isOn = true;
                    break;
                case "psychicResistBonus":
                    psychicResistBonusToggle.isOn = true;
                    break;
                case "poisonResistBonus":
                    poisonResistBonusToggle.isOn = true;
                    break;
                case "entryHealthBonus":
                    entryHealthBonusToggle.isOn = true;
                    break;
                case "goldStart":
                    goldStartToggle.isOn = true;
                    break;
                case "fireChanceBonus":
                    fireChanceToggle.isOn = true;
                    break;
                case "iceChanceBonus":
                    iceChanceToggle.isOn = true;
                    break;
                case "poisonChanceBonus":
                    poisonChanceToggle.isOn = true;
                    break;
                case "lightningChanceBonus":
                    lightningChanceToggle.isOn = true;
                    break;
                case "psychicChanceBonus":
                    psychicChanceToggle.isOn = true;
                    break;
                case "entrygoldBonus":
                    entryGoldToggle.isOn = true;
                    break;
                case "attackLifestealBonus":
                    attackLifestealToggle.isOn = true;
                    break;
                case "specialLifestealBonus":
                    specialLifestealToggle.isOn = true;
                    break;
            }
        }

        GoToPanel1();

        if(player.savedDialogueConditionsMet.Contains(DialogueConditions.AcridSecondNote))
        {
            acridNotes.text = "2/2";
        } else if (player.savedDialogueConditionsMet.Contains(DialogueConditions.AcridFirstNote))
        {
            acridNotes.text = "1/2";
        } else
        {
            acridNotes.text = "0/2";
        }

    }

    // Update is called once per frame
    void Update()
    {
        ownedGrimoire.text = player.grimoirePagesOwned + "";

        //Amount text
        burnBonusText.text = player.burnTurnIncrease + "";
        shockedBonusText.text = player.shockedTurnIncrease + "";
        vulnerableBonusText.text = player.vulnerableTurnIncrease + "";
        damnedBonusText.text = player.damnedStackIncrease + "";
        reviveBonusText.text = player.reviveIncrease + "";
        maxHealthBonusText.text = player.maxHealthIncrease + "";
        accuracyBonusText.text = player.accuracyIncrease + "";
        critChanceBonusText.text = player.critChanceIncrease + "";

        actionBonusText.text = player.actionIncrease + "";
        entryHealthBonusText.text = player.entryHealthIncrease + "";
        fireResistBonusText.text = player.fireResistIncrease + "";
        iceResistBonusText.text = player.iceResistIncrease + "";
        poisonResistBonusText.text = player.poisonResistIncrease + "";
        psychicResistBonusText.text = player.psychicResistIncrease + "";
        lightningResistBonusText.text = player.lightningResistIncrease + "";
        goldStartText.text = player.goldStart + "";

        fireChanceText.text = player.fireStatusChance + "";
        iceChanceText.text = player.iceStatusChance + "";
        lightningChanceText.text = player.lightningStatusChance + "";
        poisonChanceText.text = player.poisonStatusChance + "";
        psychicChanceText.text = player.psychicStatusChance + "";
        entryGoldText.text = player.entryGoldIncrease + "";
        attackLifestealText.text = player.attackLifesteal + "";
        specialLifestealText.text = player.specialLifesteal + "";


        burnBonusButtonText.text = burnCost + "";
        shockedBonusButtonText.text = shockedCost + "";
        vulnerableBonusButtonText.text = vulnerableCost + "";
        damnedBonusButtonText.text = damnedCost + "";
        reviveBonusButtonText.text = reviveCost + "";
        maxHealthBonusButtonText.text = maxHpCost + "";
        accuracyBonusButtonText.text = accuracyCost + "";
        critChanceBonusButtonText.text = critCost + "";

        fireResistBonusButtonText.text = fireCost + "";
        iceResistBonusButtonText.text = iceCost + "";
        lightningResistBonusButtonText.text = lightningCost + "";
        psychicResistBonusButtonText.text = psychicCost + "";
        poisonResistBonusButtonText.text = poisonCost + "";
        actionBonusButtonText.text = actionCost + "";
        entryHealthBonusButtonText.text = hpCost + "";
        goldStartButtonText.text = goldCost + "";

        fireChanceButtonText.text = fireChanceCost + "";
        iceChanceButtonText.text = iceChanceCost + "";
        lightningChanceButtonText.text = lightningChanceCost + "";
        poisonChanceButtonText.text = poisonChanceCost + "";
        psychicChanceButtonText.text = psychicChanceCost + "";
        entryGoldButtonText.text = entryGoldCost + "";
        attackLifestealButtonText.text = attackLifestealCost + "";
        specialLifestealButtonText.text = specialLifestealCost + "";


        burnBonusButtonText.color = new Color32(255, 255, 255, 255);
        shockedBonusButtonText.color = new Color32(255, 255, 255, 255);
        vulnerableBonusButtonText.color = new Color32(255, 255, 255, 255);
        damnedBonusButtonText.color = new Color32(255, 255, 255, 255);
        reviveBonusButtonText.color = new Color32(255, 255, 255, 255);
        maxHealthBonusButtonText.color = new Color32(255, 255, 255, 255);
        accuracyBonusButtonText.color = new Color32(255, 255, 255, 255);
        critChanceBonusButtonText.color = new Color32(255, 255, 255, 255);

        fireResistBonusButtonText.color = new Color32(255, 255, 255, 255);
        iceResistBonusButtonText.color = new Color32(255, 255, 255, 255);
        lightningResistBonusButtonText.color = new Color32(255, 255, 255, 255);
        psychicResistBonusButtonText.color = new Color32(255, 255, 255, 255);
        poisonResistBonusButtonText.color = new Color32(255, 255, 255, 255);
        actionBonusButtonText.color = new Color32(255, 255, 255, 255);
        entryHealthBonusButtonText.color = new Color32(255, 255, 255, 255);
        goldStartButtonText.color = new Color32(255, 255, 255, 255);

        fireChanceButtonText.color = new Color32(255, 255, 255, 255);
        iceChanceButtonText.color = new Color32(255, 255, 255, 255);
        lightningChanceButtonText.color = new Color32(255, 255, 255, 255);
        poisonChanceButtonText.color = new Color32(255, 255, 255, 255);
        psychicChanceButtonText.color = new Color32(255, 255, 255, 255);
        entryGoldButtonText.color = new Color32(255, 255, 255, 255);
        attackLifestealButtonText.color = new Color32(255, 255, 255, 255);
        specialLifestealButtonText.color = new Color32(255, 255, 255, 255);

        burnBonusButton.interactable = true;
        shockedBonusButton.interactable = true;
        vulnerableBonusButton.interactable = true;
        damnedBonusButton.interactable = true;
        reviveBonusButton.interactable = true;
        maxHealthBonusButton.interactable = true;
        accuracyBonusButton.interactable = true;
        critChanceBonusButton.interactable = true;

        fireResistBonusButton.interactable = true;
        iceResistBonusButton.interactable = true;
        lightningResistBonusButton.interactable = true;
        psychicResistBonusButton.interactable = true;
        poisonResistBonusButton.interactable = true;
        actionBonusButton.interactable = true;
        entryHealthBonusButton.interactable = true;
        goldStartBonusButton.interactable = true;

        fireChanceBonusButton.interactable = true;
        iceChanceBonusButton.interactable = true;
        lightningChanceBonusButton.interactable = true;
        poisonChanceBonusButton.interactable = true;
        psychicChanceBonusButton.interactable = true;
        entryGoldBonusButton.interactable = true;
        attackLifestealBonusButton.interactable = true;
        specialLifestealBonusButton.interactable = true;


        //Button text when cost is too high
        if (player.grimoirePagesOwned < burnCost && player.burnTurnIncrease != burnTurnIncreaseMax)
        {
            burnBonusButtonText.color = new Color32(255, 0, 0, 255);
            burnBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < shockedCost && player.shockedTurnIncrease != shockedTurnIncreaseMax)
        {
            shockedBonusButtonText.color = new Color32(255, 0, 0, 255);
            shockedBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < vulnerableCost && player.vulnerableTurnIncrease != vulnerableTurnIncreaseMax)
        {
            vulnerableBonusButtonText.color = new Color32(255, 0, 0, 255);
            vulnerableBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < damnedCost && player.damnedStackIncrease != damnedStackIncreaseMax)
        {
            damnedBonusButtonText.color = new Color32(255, 0, 0, 255);
            damnedBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < reviveCost && player.reviveIncrease != reviveIncreaseMax)
        {
            reviveBonusButtonText.color = new Color32(255, 0, 0, 255);
            reviveBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < maxHpCost && player.maxHealthIncrease != maxHealthIncreaseMax)
        {
            maxHealthBonusButtonText.color = new Color32(255, 0, 0, 255);
            maxHealthBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < accuracyCost && player.accuracyIncrease != accuracyIncreaseMax)
        {
            accuracyBonusButtonText.color = new Color32(255, 0, 0, 255);
            accuracyBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < critCost && player.critChanceIncrease != critChanceIncreaseMax)
        {
            critChanceBonusButtonText.color = new Color32(255, 0, 0, 255);
            critChanceBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < actionCost && player.actionIncrease != actionIncreaseMax)
        {
            actionBonusButtonText.color = new Color32(255, 0, 0, 255);
            actionBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < fireCost && player.fireResistIncrease != fireResitIncreaseMax)
        {
            fireResistBonusButtonText.color = new Color32(255, 0, 0, 255);
            fireResistBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < iceCost && player.iceResistIncrease != iceResistIncreaseMax)
        {
            iceResistBonusButtonText.color = new Color32(255, 0, 0, 255);
            iceResistBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < lightningCost && player.lightningResistIncrease != lightningResistIncreaseMax)
        {
            lightningResistBonusButtonText.color = new Color32(255, 0, 0, 255);
            lightningResistBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < psychicCost && player.psychicResistIncrease != psychicResistIncreaseMax)
        {
            psychicResistBonusButtonText.color = new Color32(255, 0, 0, 255);
            psychicResistBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < poisonCost && player.poisonResistIncrease != poisonResistIncreaseMax)
        {
            poisonResistBonusButtonText.color = new Color32(255, 0, 0, 255);
            poisonResistBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < hpCost && player.entryHealthIncrease != entryHealthIncreaseMax)
        {
            entryHealthBonusButtonText.color = new Color32(255, 0, 0, 255);
            entryHealthBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < goldCost && player.goldStart != goldStartMax)
        {
            goldStartButtonText.color = new Color32(255, 0, 0, 255);
            goldStartBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < fireChanceCost && player.fireStatusChance != fireChanceMax)
        {
            fireChanceButtonText.color = new Color32(255, 0, 0, 255);
            fireChanceBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < iceChanceCost && player.iceStatusChance != iceChanceMax)
        {
            iceChanceButtonText.color = new Color32(255, 0, 0, 255);
            iceChanceBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < lightningChanceCost && player.lightningStatusChance != lightningChanceMax)
        {
            lightningChanceButtonText.color = new Color32(255, 0, 0, 255);
            lightningChanceBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < poisonChanceCost && player.poisonStatusChance != poisonChanceMax)
        {
            poisonChanceButtonText.color = new Color32(255, 0, 0, 255);
            poisonChanceBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < psychicChanceCost && player.psychicStatusChance != psychicChanceMax)
        {
            psychicChanceButtonText.color = new Color32(255, 0, 0, 255);
            psychicChanceBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < entryGoldCost && player.entryGoldIncrease != entryGoldMax)
        {
            entryGoldButtonText.color = new Color32(255, 0, 0, 255);
            entryGoldBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < attackLifestealCost && player.attackLifesteal != attackLifestealMax)
        {
            attackLifestealButtonText.color = new Color32(255, 0, 0, 255);
            attackLifestealBonusButton.interactable = false;
        }

        if (player.grimoirePagesOwned < specialLifestealCost && player.specialLifesteal != specialLifestealMax)
        {
            specialLifestealButtonText.color = new Color32(255, 0, 0, 255);
            specialLifestealBonusButton.interactable = false;
        }



        //Button text when maxed out
        if (player.burnTurnIncrease >= burnTurnIncreaseMax)
        {
            //Max
            burnBonusButtonText.text = "Max";
            burnBonusButton.gameObject.SetActive(false);
            burnBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.shockedTurnIncrease >= shockedTurnIncreaseMax)
        {
            //Max
            shockedBonusButtonText.text = "Max";
            shockedBonusButton.gameObject.SetActive(false);
            shockedBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.vulnerableTurnIncrease >= vulnerableTurnIncreaseMax)
        {
            //Max
            vulnerableBonusButtonText.text = "Max";
            vulnerableBonusButton.gameObject.SetActive(false);
            vulnerableBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.damnedStackIncrease >= damnedStackIncreaseMax)
        {
            //Max
            damnedBonusButtonText.text = "Max";
            damnedBonusButton.gameObject.SetActive(false);
            damnedBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.reviveIncrease >= reviveIncreaseMax)
        {
            //Max
            reviveBonusButtonText.text = "Max";
            reviveBonusButton.gameObject.SetActive(false);
            reviveBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.maxHealthIncrease >= maxHealthIncreaseMax)
        {
            //Max
            maxHealthBonusButtonText.text = "Max";
            maxHealthBonusButton.gameObject.SetActive(false);
            maxHealthBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.accuracyIncrease >= accuracyIncreaseMax)
        {
            //Max
            accuracyBonusButtonText.text = "Max";
            accuracyBonusButton.gameObject.SetActive(false);
            accuracyBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.critChanceIncrease >= critChanceIncreaseMax)
        {
            //Max
            critChanceBonusButtonText.text = "Max";
            critChanceBonusButton.gameObject.SetActive(false);
            critChanceBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.actionIncrease >= actionIncreaseMax)
        {
            //Max
            actionBonusButtonText.text = "Max";
            actionBonusButton.gameObject.SetActive(false);
            actionBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.fireResistIncrease >= fireResitIncreaseMax)
        {
            //Max
            fireResistBonusButtonText.text = "Max";
            fireResistBonusButton.gameObject.SetActive(false);
            fireResistBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.iceResistIncrease >= iceResistIncreaseMax)
        {
            //Max
            iceResistBonusButtonText.text = "Max";
            iceResistBonusButton.gameObject.SetActive(false);
            iceResistBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.lightningResistIncrease >= lightningResistIncreaseMax)
        {
            //Max
            lightningResistBonusButtonText.text = "Max";
            lightningResistBonusButton.gameObject.SetActive(false);
            lightningResistBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.psychicResistIncrease >= psychicResistIncreaseMax)
        {
            //Max
            psychicResistBonusButtonText.text = "Max";
            psychicResistBonusButton.gameObject.SetActive(false);
            psychicResistBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.poisonResistIncrease >= poisonResistIncreaseMax)
        {
            //Max
            poisonResistBonusButtonText.text = "Max";
            poisonResistBonusButton.gameObject.SetActive(false);
            poisonResistBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.entryHealthIncrease >= entryHealthIncreaseMax)
        {
            //Max
            entryHealthBonusButtonText.text = "Max";
            entryHealthBonusButton.gameObject.SetActive(false);
            entryHealthBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.goldStart >= goldStartMax)
        {
            //Max
            goldStartButtonText.text = "Max";
            goldStartBonusButton.gameObject.SetActive(false);
            goldStartBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.fireStatusChance >= fireChanceMax)
        {
            //Max
            fireChanceButtonText.text = "Max";
            fireChanceBonusButton.gameObject.SetActive(false);
            fireChanceBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.iceStatusChance >= iceChanceMax)
        {
            //Max
            iceChanceButtonText.text = "Max";
            iceChanceBonusButton.gameObject.SetActive(false);
            iceChanceBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.lightningStatusChance >= lightningChanceMax)
        {
            //Max
            lightningChanceButtonText.text = "Max";
            lightningChanceBonusButton.gameObject.SetActive(false);
            lightningChanceBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.poisonStatusChance >= poisonChanceMax)
        {
            //Max
            poisonChanceButtonText.text = "Max";
            poisonChanceBonusButton.gameObject.SetActive(false);
            poisonChanceBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.psychicStatusChance >= psychicChanceMax)
        {
            //Max
            psychicChanceButtonText.text = "Max";
            psychicChanceBonusButton.gameObject.SetActive(false);
            psychicChanceBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.entryGoldIncrease >= entryGoldMax)
        {
            //Max
            entryGoldButtonText.text = "Max";
            entryGoldBonusButton.gameObject.SetActive(false);
            entryGoldBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.attackLifesteal >= attackLifestealMax)
        {
            //Max
            attackLifestealButtonText.text = "Max";
            attackLifestealBonusButton.gameObject.SetActive(false);
            attackLifestealBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }

        if (player.specialLifesteal >= specialLifestealMax)
        {
            //Max
            specialLifestealButtonText.text = "Max";
            specialLifestealBonusButton.gameObject.SetActive(false);
            specialLifestealBonusButton.transform.parent.GetChild(3).gameObject.SetActive(false);
        }
    }

    public void UpgradeBonus(string bonus)
    {
        switch (bonus)
        {
            case "burnBonus":
                player.ChangeGrimoirePageAmount(-burnCost);
                player.burnTurnIncrease = Mathf.Clamp(player.burnTurnIncrease + 1, 0, burnTurnIncreaseMax);
                break;
            case "shockedBonus":
                player.ChangeGrimoirePageAmount(-shockedCost);
                player.shockedTurnIncrease = Mathf.Clamp(player.shockedTurnIncrease + 1, 0, shockedTurnIncreaseMax);
                break;
            case "vulnerableBonus":
                player.ChangeGrimoirePageAmount(-vulnerableCost);
                player.vulnerableTurnIncrease = Mathf.Clamp(player.vulnerableTurnIncrease + 1, 0, vulnerableTurnIncreaseMax);
                break;
            case "damnedBonus":
                player.ChangeGrimoirePageAmount(-damnedCost);
                player.damnedStackIncrease = Mathf.Clamp(player.damnedStackIncrease + 1, 0, damnedStackIncreaseMax);
                break;
            case "reviveBonus":
                player.ChangeGrimoirePageAmount(-reviveCost);
                player.reviveIncrease = Mathf.Clamp(player.reviveIncrease + 1, 0, reviveIncreaseMax);
                break;
            case "maxHealthBonus":
                player.ChangeGrimoirePageAmount(-maxHpCost);
                player.maxHealthIncrease = Mathf.Clamp(player.maxHealthIncrease + 5, 0, maxHealthIncreaseMax);
                break;
            case "accuracyBonus":
                player.ChangeGrimoirePageAmount(-accuracyCost);
                player.accuracyIncrease = Mathf.Clamp(player.accuracyIncrease + 2, 0, accuracyIncreaseMax);
                break;
            case "critChanceBonus":
                player.ChangeGrimoirePageAmount(-critCost);
                player.critChanceIncrease = Mathf.Clamp(player.critChanceIncrease + 2, 0, critChanceIncreaseMax);
                break;

            case "fireResistBonus":
                player.ChangeGrimoirePageAmount(-fireCost);
                player.fireResistIncrease = Mathf.Clamp(player.fireResistIncrease + 5, 0, fireResitIncreaseMax);
                break;
            case "iceResistBonus":
                player.ChangeGrimoirePageAmount(-iceCost);
                player.iceResistIncrease = Mathf.Clamp(player.iceResistIncrease + 5, 0, iceResistIncreaseMax);
                break;
            case "lightningResistBonus":
                player.ChangeGrimoirePageAmount(-lightningCost);
                player.lightningResistIncrease = Mathf.Clamp(player.lightningResistIncrease + 5, 0, lightningResistIncreaseMax);
                break;
            case "psychicResistBonus":
                player.ChangeGrimoirePageAmount(-psychicCost);
                player.psychicResistIncrease = Mathf.Clamp(player.psychicResistIncrease + 5, 0, psychicResistIncreaseMax);
                break;
            case "poisonResistBonus":
                player.ChangeGrimoirePageAmount(-poisonCost);
                player.poisonResistIncrease = Mathf.Clamp(player.poisonResistIncrease + 5, 0, poisonResistIncreaseMax);
                break;
            case "actionBonus":
                player.ChangeGrimoirePageAmount(-actionCost);
                player.actionIncrease = Mathf.Clamp(player.actionIncrease + 1, 0, actionIncreaseMax);
                break;
            case "entryHealthBonus":
                player.ChangeGrimoirePageAmount(-hpCost);
                player.entryHealthIncrease = Mathf.Clamp(player.entryHealthIncrease + 1, 0, entryHealthIncreaseMax);
                break;
            case "goldStart":
                player.ChangeGrimoirePageAmount(-goldCost);
                player.goldStart = Mathf.Clamp(player.goldStart + 10, 0, goldStartMax);
                break;

            case "fireChanceBonus":
                player.ChangeGrimoirePageAmount(-fireChanceCost);
                player.fireStatusChance = Mathf.Clamp(player.fireStatusChance + 5, 0, fireChanceMax);
                break;
            case "iceChanceBonus":
                player.ChangeGrimoirePageAmount(-iceChanceCost);
                player.iceStatusChance = Mathf.Clamp(player.iceStatusChance + 5, 0, iceChanceMax);
                break;
            case "poisonChanceBonus":
                player.ChangeGrimoirePageAmount(-poisonChanceCost);
                player.poisonStatusChance = Mathf.Clamp(player.poisonStatusChance + 5, 0, poisonChanceMax);
                break;
            case "lightningChanceBonus":
                player.ChangeGrimoirePageAmount(-lightningChanceCost);
                player.lightningStatusChance = Mathf.Clamp(player.lightningStatusChance + 5, 0, lightningChanceMax);
                break;
            case "psychicChanceBonus":
                player.ChangeGrimoirePageAmount(-psychicChanceCost);
                player.psychicStatusChance = Mathf.Clamp(player.psychicStatusChance + 5, 0, psychicChanceMax);
                break;
            case "entrygoldBonus":
                player.ChangeGrimoirePageAmount(-entryGoldCost);
                player.entryGoldIncrease = Mathf.Clamp(player.entryGoldIncrease + 5, 0, entryGoldMax);
                break;
            case "attackLifestealBonus":
                player.ChangeGrimoirePageAmount(-attackLifestealCost);
                player.attackLifesteal = Mathf.Clamp(player.attackLifesteal + 5, 0, attackLifestealMax);
                break;
            case "specialLifestealBonus":
                player.ChangeGrimoirePageAmount(-specialLifestealCost);
                player.specialLifesteal = Mathf.Clamp(player.specialLifesteal + 5, 0, specialLifestealMax);
                break;
        }

        TriggerSelectionBark();
    }

    public bool canBark = true;

    private void TriggerSelectionBark()
    {
        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockAcademy) && Random.Range(0, 100) < friendlyToken.interactableNpc.barkChance && canBark)
        {
            DialogueBlock bark = friendlyToken.interactableNpc.GetAbilityBark();
            if (bark != null)
            {
                dialogueManager.ShowBarkWithOwner(
                    friendlyToken.interactableNpc, friendlyToken.characterImage, bark);
            }
        }
    }

    public void EquipBonus(string bonus)
    {
        switch (bonus)
        {
            case "burnBonus":
                AddBonusToEquipped(burnBonusToggle, bonus);
                break;
            case "shockedBonus":
                AddBonusToEquipped(shockedBonusToggle, bonus);
                break;
            case "vulnerableBonus":
                AddBonusToEquipped(vulnerableBonusToggle, bonus);
                break;
            case "damnedBonus":
                AddBonusToEquipped(damnedBonusToggle, bonus);
                break;
            case "reviveBonus":
                AddBonusToEquipped(reviveBonusToggle, bonus);
                break;
            case "maxHealthBonus":
                AddBonusToEquipped(maxHealthBonusToggle, bonus);
                break;
            case "accuracyBonus":
                AddBonusToEquipped(accuracyBonusToggle, bonus);
                break;
            case "critChanceBonus":
                AddBonusToEquipped(critChanceBonusToggle, bonus);
                break;

            case "actionBonus":
                AddBonusToEquipped(actionBonusToggle, bonus);
                break;
            case "fireResistBonus":
                AddBonusToEquipped(fireResistBonusToggle, bonus);
                break;
            case "iceResistBonus":
                AddBonusToEquipped(iceResistBonusToggle, bonus);
                break;
            case "lightningResistBonus":
                AddBonusToEquipped(lightningResistBonusToggle, bonus);
                break;
            case "psychicResistBonus":
                AddBonusToEquipped(psychicResistBonusToggle, bonus);
                break;
            case "poisonResistBonus":
                AddBonusToEquipped(poisonResistBonusToggle, bonus);
                break;
            case "entryHealthBonus":
                AddBonusToEquipped(entryHealthBonusToggle, bonus);
                break;
            case "goldStart":
                AddBonusToEquipped(goldStartToggle, bonus);
                break;

            case "fireChanceBonus":
                AddBonusToEquipped(fireChanceToggle, bonus);
                break;
            case "iceChanceBonus":
                AddBonusToEquipped(iceChanceToggle, bonus);
                break;
            case "poisonChanceBonus":
                AddBonusToEquipped(poisonChanceToggle, bonus);
                break;
            case "lightningChanceBonus":
                AddBonusToEquipped(lightningChanceToggle, bonus);
                break;
            case "psychicChanceBonus":
                AddBonusToEquipped(psychicChanceToggle, bonus);
                break;
            case "entrygoldBonus":
                AddBonusToEquipped(entryGoldToggle, bonus);
                break;
            case "attackLifestealBonus":
                AddBonusToEquipped(attackLifestealToggle, bonus);
                break;
            case "specialLifestealBonus":
                AddBonusToEquipped(specialLifestealToggle, bonus);
                break;
        }

        TriggerSelectionBark();
    }

    private void AddBonusToEquipped(Toggle toggle, string bonus)
    {
        if (toggle.isOn)
        {
            if (player.equippedBonuses.Count >= maxEquippedBonuses)
            {
                MaxBonus();
                toggle.isOn = false;
                return;
            }
            player.equippedBonuses.Add(bonus);
        }
        else
        {
            player.equippedBonuses.Remove(bonus);
        }
    }

    private void MaxBonus()
    {
        DialogueBlock bark = friendlyToken.interactableNpc.GetDeathBark();
        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockAcademy) && bark != null && canBark)
        {
            dialogueManager.ShowBarkWithOwner(
                friendlyToken.interactableNpc, friendlyToken.characterImage, bark);
        }

    }

    public void ApplyBonus()
    {
        player.PlayerReset(false);

        if (burnBonusToggle.isOn)
        {
            player.h.SetBurningInfo(new int[] { 2 + player.burnTurnIncrease, 5 });
        }

        if (shockedBonusToggle.isOn)
        {
            player.h.SetShockedInfo(new int[] { 2 + player.shockedTurnIncrease, 10 });
        }

        if (vulnerableBonusToggle.isOn)
        {
            player.h.SetVulnerableInfo(new float[] { 2 + player.vulnerableTurnIncrease, 1.5f });
        }

        if (damnedBonusToggle.isOn)
        {
            player.h.SetDamnationInfo(new int[] { 2, 1 + player.damnedStackIncrease, 5, 10 });
        }

        //weapon mod happens here
        if(player.UsingMod(player.selectedWeaponType))
        {
            player.weaponMod.ActivateMod(player);
        }

        if (reviveBonusToggle.isOn)
        {
            player.numRevives += player.reviveIncrease;
        }

        if (maxHealthBonusToggle.isOn)
        {
            player.h.IncreaseMaxHealth(player.maxHealthIncrease);
            player.h.SetToMaxHealth();
        }

        if (accuracyBonusToggle.isOn)
        {
            player.h.accuracy.AddModifier(new StatModifier(player.accuracyIncrease, StatModType.Flat, this));
        }

        if (critChanceBonusToggle.isOn)
        {
            player.h.critChance.AddModifier(new StatModifier(player.critChanceIncrease, StatModType.Flat, this));
        }

        if (actionBonusToggle.isOn)
        {
            player.h.actions.AddModifier(new StatModifier(player.actionIncrease, StatModType.Flat, this));
        }

        if (fireResistBonusToggle.isOn)
        {
            player.h.fireResist += player.fireResistIncrease;
        }

        if (iceResistBonusToggle.isOn)
        {
            player.h.iceResist += player.iceResistIncrease;
        }

        if (lightningResistBonusToggle.isOn)
        {
            player.h.lightningResist += player.lightningResistIncrease;
        }

        if (psychicResistBonusToggle.isOn)
        {
            player.h.psychicResist += player.psychicResistIncrease;
        }

        if (poisonResistBonusToggle.isOn)
        {
            player.h.poisonResist += player.poisonResistIncrease;
        }

        if (entryHealthBonusToggle.isOn)
        {
            player.healthOnExit += player.entryHealthIncrease;
        }

        if (goldStartToggle.isOn)
        {
            player.h.playerInventory.SetGoldAmount(player.goldStart);
        }




        if (fireChanceToggle.isOn)
        {
            player.h.fireStatus += player.fireStatusChance;
        }

        if (iceChanceToggle.isOn)
        {
            player.h.iceStatus += player.iceStatusChance;
        }

        if (lightningChanceToggle.isOn)
        {
            player.h.lightningStatus += player.lightningStatusChance;
        }

        if (poisonChanceToggle.isOn)
        {
            player.h.poisonStatus += player.poisonStatusChance;
        }

        if (psychicChanceToggle.isOn)
        {
            player.h.psychicStatus += player.psychicStatusChance;
        }

        if (entryGoldToggle.isOn)
        {
            player.goldOnExit += player.entryGoldIncrease;
        }

        if (attackLifestealToggle.isOn)
        {
            player.h.attLifesteal.AddModifier(new StatModifier(player.attackLifesteal, StatModType.Flat, this));
        }

        if (specialLifestealToggle.isOn)
        {
            player.h.spLifesteal.AddModifier(new StatModifier(player.specialLifesteal, StatModType.Flat, this));
        }
        player.h.SetAffinityReadonly();

    }

    private void SetEnchantmentUiName()
    {
        burnBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("burnBonus");
        shockedBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("shockedBonus");
        damnedBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("damnedBonus");
        vulnerableBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("vulnerableBonus");
        reviveBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("reviveBonus");
        maxHealthBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("maxHealthBonus");
        accuracyBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("accuracyBonus");
        critChanceBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("critChanceBonus");

        fireResistBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("fireResistBonus");
        iceResistBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("iceResistBonus");
        lightningResistBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("lightningResistBonus");
        psychicResistBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("psychicResistBonus");
        poisonResistBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("poisonResistBonus");
        actionBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("actionBonus");
        entryHealthBonusToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("entryHealthBonus");
        goldStartToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("goldStart");

        fireChanceToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("fireChanceBonus");
        iceChanceToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("iceChanceBonus");
        lightningChanceToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("lightningChanceBonus");
        psychicChanceToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("psychicChanceBonus");
        poisonChanceToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("poisonChanceBonus");
        entryGoldToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("entrygoldBonus");
        attackLifestealToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("attackLifestealBonus");
        specialLifestealToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameUtilities.GetEnchantmentName("specialLifestealBonus");
    }

    public void GetEnchantmentDesc(string val)
    {
        descriptionBox.text = GameUtilities.GetEnchantmentDesc(val);
    }

    public void RemoveAllBonusMods(StatSystem s)
    {
        foreach (StatModifier m in s.statModifiers)
        {
            if (m.modSource is AcademyHub)
            {
                m.toRemove = true;
                s.SetModChangeTrue();
            }
        }
        s.statModifiers.RemoveAll(x => x.toRemove == true);
    }

    public GameObject academyMenu;
    public GameObject buttonPage2;
    public GameObject buttonPage3;

    public void OpenWindow()
    {
        AudioManager.instance.PlaySfxSound("Button_Click");
        friendlyToken.CheckGiftable();
        academyMenu.SetActive(true);

        if (!player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockAcademy))
        {
            //hide academy menu
            academyMenu.SetActive(false);
        }

        if (friendlyToken.Visited())
        {
            ShowWindow();
        }
        else
        {
            player.wentToAcademyThisRun = true;
            if (friendlyToken.TriggerVisitDialogue())
            {
                StartCoroutine(GameUtilities.WaitForConversation(() => TriggerShowWindow()));
            }
            else
            {
                ShowWindow();
            }

            if (friendlyToken.admiration.currentRank > 0)
            {
                player.numAcademyInteraction++;
                academyMenu.SetActive(true);
            }
        }


    }

    private void TriggerShowWindow()
    {
        if (player.savedPastConvos.Contains("AGNES_026_07")
            && !player.cutscenesUnlocked.Contains(Cutscenes.Fifteen.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Fifteen);
            DialogueBlock block =
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Academy, "AGNES_027_01");
            StartCoroutine(
                GameUtilities.WaitForCutscene(() => DialogueManager.instance.ShowDialogue(block)));
            StartCoroutine(GameUtilities.WaitForConversation(() => ShowWindow()));
        }
        else
        {
            ShowWindow();
        }
    }

    public void ShowWindow()
    {
        HubManager.interactingWith = friendlyToken;
        //play open animation
        buttonPage2.SetActive(true);
        buttonPage3.SetActive(true);

        if (!player.savedDialogueConditionsMet.Contains(DialogueConditions.GrimoirePageTwo))
        {
            //hide academy menu
            buttonPage2.SetActive(false);
        }

        if (!player.savedDialogueConditionsMet.Contains(DialogueConditions.GrimoirePageThree))
        {
            //hide academy menu
            buttonPage3.SetActive(false);
        }
        GoToPanel1();
        windowAnimator.Play("Academy Open");
        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockAcademy) && Random.Range(0, 100) < friendlyToken.interactableNpc.barkChance && canBark)
        {
            DialogueBlock bark = friendlyToken.interactableNpc.GetRandomBark();
            if (bark != null)
            {
                dialogueManager.ShowBarkWithOwner(
                    friendlyToken.interactableNpc, friendlyToken.characterImage, bark);
            }

        }
    }

    public void CloseWindow()
    {
        //play close animation 
        windowAnimator.Play("Academy Close");
        StartCoroutine(
            GameUtilities.WaitForConversation(
                () => GameObject.FindGameObjectWithTag("HubManager").GetComponent<HubManager>().CheckMiscConditions()));

        ;
    }

    public void GoToPanel1()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
        panel3.SetActive(false);
    }

    public void GoToPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
        panel3.SetActive(false);
    }

    public void GoToPanel3()
    {
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(true);
    }
}
