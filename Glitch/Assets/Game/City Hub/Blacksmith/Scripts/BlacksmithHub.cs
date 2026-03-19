using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class BlacksmithHub : MonoBehaviour
{

    public GameObject hubWindow;
    public Animator windowAnimator;

    public GameObject fighterWeaponIndicator;
    public GameObject engineerWeaponIndicator;
    public GameObject mageWeaponIndicator;
    public GameObject brawlerWeaponIndicator;

    public TextMeshProUGUI fighterWeaponRankCost;
    public TextMeshProUGUI engineerWeaponRankCost;
    public TextMeshProUGUI mageWeaponRankCost;
    public TextMeshProUGUI brawlerWeaponRankCost;

    public TextMeshProUGUI fighterWeaponRank;
    public TextMeshProUGUI engineerWeaponRank;
    public TextMeshProUGUI mageWeaponRank;
    public TextMeshProUGUI brawlerWeaponRank;


    private PlayerManager player;
    [SerializeField]
    protected FriendlyToken friendlyToken;
    private DialogueManager dialogueManager;

    private readonly int[] rankCosts = new int[] { 0, 25, 250, 625, 1000, 1875, 0};

    public List<Sprite> frameRarity;
    public List<Image> imgFrame;

    public GameObject staffSchema;

    private WeaponMod melancoliaMod;
    private WeaponMod deltaMod;
    private WeaponMod firestormMod;
    private WeaponMod renegadeMod;

    public GameObject fighterWeaponMod;
    public GameObject engineerWeaponMod;
    public GameObject mageWeaponMod;
    public GameObject brawlerWeaponMod;

    public TextMeshProUGUI descriptionWindow;
    public Toggle modToggle;

    private void Awake()
    {
        staffSchema.SetActive(false);
        dialogueManager = DialogueManager.instance;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        //Show broken staff for quest if has 
        staffSchema.SetActive(
            player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockStaffQuest)
            && !player.savedDialogueConditionsMet.Contains(DialogueConditions.ObtainedStaffMaterial));

        RerollWeaponMod(WeaponTypes.Melancolia);
        RerollWeaponMod(WeaponTypes.Delta);
        RerollWeaponMod(WeaponTypes.Firestorm);
        RerollWeaponMod(WeaponTypes.Renegade);
        ShowModToggle();
        fighterWeaponMod.SetActive(player.UsingMod(WeaponTypes.Melancolia));
        engineerWeaponMod.SetActive(player.UsingMod(WeaponTypes.Delta));
        mageWeaponMod.SetActive(player.UsingMod(WeaponTypes.Firestorm));
        brawlerWeaponMod.SetActive(player.UsingMod(WeaponTypes.Renegade));
    }

    // Start is called before the first frame update
    void Start()
    {

        if (player.wentToBlacksmithThisRun)
        {
            friendlyToken.newInteraction = false;
            friendlyToken.urgentSymbol.SetActive(false);
        }
        fighterWeaponIndicator.transform.parent.transform.parent.gameObject.SetActive(false);
        engineerWeaponIndicator.transform.parent.transform.parent.gameObject.SetActive(false);
        mageWeaponIndicator.transform.parent.transform.parent.gameObject.SetActive(false);
        brawlerWeaponIndicator.transform.parent.transform.parent.gameObject.SetActive(false);
        CheckMaxRanks();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetWeaponRank(WeaponTypes.Melancolia) > 0)
        {
            imgFrame[0].sprite = frameRarity[player.GetWeaponRank(WeaponTypes.Melancolia) - 1];
            fighterWeaponRank.text = "Melancolia Rank: " + player.GetWeaponRank(WeaponTypes.Melancolia);
            fighterWeaponIndicator.transform.parent.transform.parent.gameObject.SetActive(true);
        }

        if (player.GetWeaponRank(WeaponTypes.Delta) > 0)
        {
            imgFrame[1].sprite = frameRarity[player.GetWeaponRank(WeaponTypes.Delta) - 1];
            engineerWeaponRank.text = "Delta Rank: " + player.GetWeaponRank(WeaponTypes.Delta);
            engineerWeaponIndicator.transform.parent.transform.parent.gameObject.SetActive(true);
        }

        if (player.GetWeaponRank(WeaponTypes.Firestorm) > 0)
        {
            imgFrame[2].sprite = frameRarity[player.GetWeaponRank(WeaponTypes.Firestorm) - 1];
            mageWeaponRank.text = "Firestorm Rank: " + player.GetWeaponRank(WeaponTypes.Firestorm);
            mageWeaponIndicator.transform.parent.transform.parent.gameObject.SetActive(true);
        }

        if (player.GetWeaponRank(WeaponTypes.Renegade) > 0)
        {
            imgFrame[3].sprite = frameRarity[player.GetWeaponRank(WeaponTypes.Renegade) - 1];
            brawlerWeaponRank.text = "Renegades Rank: " + player.GetWeaponRank(WeaponTypes.Renegade);
            brawlerWeaponIndicator.transform.parent.transform.parent.gameObject.SetActive(true);
        }
        CheckMaxRanks();
        CheckSchemaRequirement();
        CheckModCost();
    }

    private void CheckSchemaRequirement()
    {
        staffSchema.transform.GetChild(0).GetComponent<Button>().interactable = true;
        staffSchema.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        staffSchema.transform.GetChild(3).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);

        if (player.etherShardsOwned < 1)
        {
            staffSchema.transform.GetChild(3).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            staffSchema.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }

        if (player.metalScrapsOwned < 250)
        {
            staffSchema.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            staffSchema.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
    }

    public void CheckMaxRanks()
    {
        fighterWeaponRankCost.text = rankCosts[player.melancoliaWeaponRank] + "";
        engineerWeaponRankCost.text = rankCosts[player.deltaWeaponRank] + "";
        mageWeaponRankCost.text = rankCosts[player.firestormWeaponRank] + "";
        brawlerWeaponRankCost.text = rankCosts[player.renegadeWeaponRank] + "";

        fighterWeaponRankCost.color = new Color32(255, 255, 255, 255);
        engineerWeaponRankCost.color = new Color32(255, 255, 255, 255);
        mageWeaponRankCost.color = new Color32(255, 255, 255, 255);
        brawlerWeaponRankCost.color = new Color32(255, 255, 255, 255);

        fighterWeaponRankCost.transform.parent.gameObject.GetComponent<Button>().interactable = true;
        engineerWeaponRankCost.transform.parent.gameObject.GetComponent<Button>().interactable = true;
        mageWeaponRankCost.transform.parent.gameObject.GetComponent<Button>().interactable = true;
        brawlerWeaponRankCost.transform.parent.gameObject.GetComponent<Button>().interactable = true;

        //Button visual when price is out of range
        if (player.metalScrapsOwned < rankCosts[player.melancoliaWeaponRank] && player.GetWeaponRank(WeaponTypes.Melancolia) <= 4)
        {
            fighterWeaponRankCost.color = new Color32(255, 0, 0, 255);
            fighterWeaponRankCost.transform.parent.gameObject.GetComponent<Button>().interactable = false;
        }

        if (player.metalScrapsOwned < rankCosts[player.deltaWeaponRank] && player.GetWeaponRank(WeaponTypes.Delta) <= 4)
        {
            engineerWeaponRankCost.color = new Color32(255, 0, 0, 255);
            engineerWeaponRankCost.transform.parent.gameObject.GetComponent<Button>().interactable = false;
        }

        if (player.metalScrapsOwned < rankCosts[player.firestormWeaponRank] && player.GetWeaponRank(WeaponTypes.Firestorm) <= 4)
        {
            mageWeaponRankCost.color = new Color32(255, 0, 0, 255);
            mageWeaponRankCost.transform.parent.gameObject.GetComponent<Button>().interactable = false;
        }

        if (player.metalScrapsOwned < rankCosts[player.renegadeWeaponRank] && player.GetWeaponRank(WeaponTypes.Renegade) <= 4)
        {
            brawlerWeaponRankCost.color = new Color32(255, 0, 0, 255);
            brawlerWeaponRankCost.transform.parent.gameObject.GetComponent<Button>().interactable = false;
        }

        //Button visual when maxed
        if (player.GetWeaponRank(WeaponTypes.Melancolia) > 4)
        {
            //Max
            fighterWeaponRankCost.text = "Max";
            fighterWeaponRankCost.transform.parent.gameObject.SetActive(false);
            fighterWeaponRankCost.transform.parent.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (player.GetWeaponRank(WeaponTypes.Delta) > 4)
        {
            //Max
            engineerWeaponRankCost.text = "Max";
            engineerWeaponRankCost.transform.parent.gameObject.SetActive(false);
            engineerWeaponRankCost.transform.parent.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (player.GetWeaponRank(WeaponTypes.Firestorm) > 4)
        {
            //Max
            mageWeaponRankCost.text = "Max";
            mageWeaponRankCost.transform.parent.gameObject.SetActive(false);
            mageWeaponRankCost.transform.parent.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (player.GetWeaponRank(WeaponTypes.Renegade) > 4)
        {
            //Max
            brawlerWeaponRankCost.text = "Max";
            brawlerWeaponRankCost.transform.parent.gameObject.SetActive(false);
            brawlerWeaponRankCost.transform.parent.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void SelectedWeaponIndicator()
    {
        fighterWeaponIndicator.SetActive(false);
        engineerWeaponIndicator.SetActive(false);
        mageWeaponIndicator.SetActive(false);
        brawlerWeaponIndicator.SetActive(false);
        player.weaponMod = null;
        switch (player.selectedWeaponType)
        {
            case WeaponTypes.Melancolia:
                fighterWeaponIndicator.SetActive(true);
                break;
            case WeaponTypes.Delta:
                engineerWeaponIndicator.SetActive(true);
                break;
            case WeaponTypes.Firestorm:
                mageWeaponIndicator.SetActive(true);
                break;
            case WeaponTypes.Renegade:
                brawlerWeaponIndicator.SetActive(true);
                break;
        }

        player.weaponMod = GetWeaponMod(player.selectedWeaponType, player.UsingMod(player.selectedWeaponType));
    }

    public void BuildStaff()
    {
        player.ChangeShardAmount(-1);
        player.ChangeScrapsAmount(-250);
        staffSchema.SetActive(false);
        dialogueManager.ShowBarkWithOwner(
             friendlyToken.interactableNpc, friendlyToken.characterImage, 
             friendlyToken.interactableNpc.FindAbilityBark("ARLO_BARK_003"));
        player.savedDialogueConditionsMet.Add(DialogueConditions.ObtainedStaffMaterial);
    }


    private void CheckModCost()
    {
        fighterWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = rerollModCost + "";
        engineerWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = rerollModCost + "";
        mageWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = rerollModCost + "";
        brawlerWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = rerollModCost + "";
        
        fighterWeaponMod.transform.GetChild(3).GetComponent<Button>().interactable = true;
        engineerWeaponMod.transform.GetChild(3).GetComponent<Button>().interactable = true;
        mageWeaponMod.transform.GetChild(3).GetComponent<Button>().interactable = true;
        brawlerWeaponMod.transform.GetChild(3).GetComponent<Button>().interactable = true;

        fighterWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        engineerWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        mageWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        brawlerWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);

        if (player.metalScrapsOwned < rerollModCost)
        {
            fighterWeaponMod.transform.GetChild(3).GetComponent<Button>().interactable = false;
            engineerWeaponMod.transform.GetChild(3).GetComponent<Button>().interactable = false;
            mageWeaponMod.transform.GetChild(3).GetComponent<Button>().interactable = false;
            brawlerWeaponMod.transform.GetChild(3).GetComponent<Button>().interactable = false;

            fighterWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            engineerWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            mageWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            brawlerWeaponMod.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);

            
        }
    }

    private void ShowModToggle()
    {
        bool modAvailable = player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockDeltaMod)
            || player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockFirestormMod)
            || player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockMelancoliaMod)
            || player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockRenegadeMod);

        modToggle.gameObject.SetActive(modAvailable);
        modToggle.isOn = player.useMod;
        player.weaponMod = GetWeaponMod(player.selectedWeaponType, player.UsingMod(player.selectedWeaponType));
        ModToggleOnClick(player.useMod);
    }

    private int rerollModCost = 200;
    public void RerollThisMod(int mod)
    {
        if (player.metalScrapsOwned < rerollModCost) return;

        player.ChangeScrapsAmount(-rerollModCost);
        RerollWeaponMod((WeaponTypes)mod);
        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockBlacksmith) && UnityEngine.Random.Range(0, 100) < friendlyToken.interactableNpc.barkChance)
        {
            DialogueBlock bark = friendlyToken.interactableNpc.GetDeathBark();
            if (bark != null)
            {
                dialogueManager.ShowBarkWithOwner(
               friendlyToken.interactableNpc, friendlyToken.characterImage, bark);
            }

        }
        ShowModDesc(mod);
    }

    private void RerollWeaponMod(WeaponTypes weapon)
    {
        switch(weapon)
        {
            case WeaponTypes.Melancolia:
                melancoliaMod = new WeaponMod(WeaponTypes.Melancolia);
                break;
            case WeaponTypes.Delta:
                deltaMod = new WeaponMod(WeaponTypes.Delta);
                break;
            case WeaponTypes.Firestorm:
                firestormMod = new WeaponMod(WeaponTypes.Firestorm);
                break;
            case WeaponTypes.Renegade:
                renegadeMod = new WeaponMod(WeaponTypes.Renegade);
                break;
        }
    }

    public WeaponMod GetWeaponMod(WeaponTypes weapon, bool modAvail)
    {
        if(!modAvail)
        {
            return null;
        }

        switch (weapon)
        {
            case WeaponTypes.Melancolia:
                return melancoliaMod;
            case WeaponTypes.Delta:
                return deltaMod;
            case WeaponTypes.Firestorm:
                return firestormMod;
            case WeaponTypes.Renegade:
                return renegadeMod;
        }

        return null;
    }

    public void ShowModDesc(int weapon)
    {
        switch ((WeaponTypes)weapon)
        {
            case WeaponTypes.Melancolia:
                descriptionWindow.text = melancoliaMod.GetDesc();
                break;
            case WeaponTypes.Delta:
                descriptionWindow.text = deltaMod.GetDesc();
                break;
            case WeaponTypes.Firestorm:
                descriptionWindow.text = firestormMod.GetDesc();
                break;
            case WeaponTypes.Renegade:
                descriptionWindow.text = renegadeMod.GetDesc();
                break;
        }
    }

    public void ModToggleOnClick(bool val)
    {
        player.useMod = val;
        player.weaponMod = GetWeaponMod(player.selectedWeaponType, player.UsingMod(player.selectedWeaponType));
       
        fighterWeaponMod.SetActive(player.UsingMod(WeaponTypes.Melancolia));
        engineerWeaponMod.SetActive(player.UsingMod(WeaponTypes.Delta));
        mageWeaponMod.SetActive(player.UsingMod(WeaponTypes.Firestorm));
        brawlerWeaponMod.SetActive(player.UsingMod(WeaponTypes.Renegade));
    }

    public int GetRankCost(int r)
    {
        return rankCosts[r];
    }

    public void OpenWindow()
    {
        AudioManager.instance.PlaySfxSound("Button_Click");
        friendlyToken.CheckGiftable();
        if (player.wentToBlacksmithThisRun)
        {
            ShowWindow();
        }
        else
        {
            player.wentToBlacksmithThisRun = true;
            if (friendlyToken.TriggerVisitDialogue())
            {
                StartCoroutine(GameUtilities.WaitForConversation(() => ShowWindow()));
            }
            else
            {
                ShowWindow();
            }

            player.numBlacksmithInteraction++;
        }

    }


    public void ShowWindow()
    {
        HubManager.interactingWith = friendlyToken;
        //play open animation
        windowAnimator.Play("Blacksmith Open");
        SelectedWeaponIndicator();
        ShowModToggle();

        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockBlacksmith) && UnityEngine.Random.Range(0, 100) < friendlyToken.interactableNpc.barkChance)
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
        windowAnimator.Play("Blacksmith Close");

    }
}




[Serializable]
public class WeaponMod
{
    public BlacksmithConMods conMod;
    public BlacksmithProMods proMod;

    public PlayerManager player;
    private int proAmount;
    private int conAmount;

    public static int[] melencoliaCon = new int[] { 0, 1, 2, 5, 6, 7, 8, 9, 10, 11 };
    public static int[] firestormCon = new int[]{ 0, 1, 3, 12, 13, 14, 15, 16, 17, 18 };
    public static int[] deltaCon = new int[]{ 0, 1, 4, 3, 19, 20, 21, 22, 23, 24 };
    public static int[] renegadeCon = new int[] { 0, 1, 2, 3, 4, 5, 25, 26, 27, 28 };

    public static int[] melencoliaPro = new int[] { 0, 1, 6, 7, 2, 8, 9, 10, 11, 12 };
    public static int[] firestormPro = new int[] { 0, 1, 13, 14, 3, 15, 16, 17, 5, 18 };
    public static int[] deltaPro = new int[] { 0, 1, 19, 20, 3, 21, 22, 4, 23, 24 };
    public static int[] renegadePro = new int[] { 0, 1, 25, 26, 27, 28, 2, 4, 5, 29 };


    public WeaponMod(WeaponTypes weapon)
    {
        int conIndex = UnityEngine.Random.Range(0, 10);
        int proIndex = UnityEngine.Random.Range(0, 10);

        switch (weapon)
        {
            case WeaponTypes.Melancolia:
                conMod = (BlacksmithConMods)melencoliaCon[conIndex];
                proMod = (BlacksmithProMods)melencoliaPro[proIndex];
                break;
            case WeaponTypes.Delta:
                conMod = (BlacksmithConMods)deltaCon[conIndex];
                proMod = (BlacksmithProMods)deltaPro[proIndex];
                break;
            case WeaponTypes.Firestorm:
                conMod = (BlacksmithConMods)firestormCon[conIndex];
                proMod = (BlacksmithProMods)firestormPro[proIndex];
                break;
            case WeaponTypes.Renegade:
                conMod = (BlacksmithConMods)renegadeCon[conIndex];
                proMod = (BlacksmithProMods)renegadePro[proIndex];
                break;
        }

        SetConAmount();
        SetProAmount();

    }

    public void ActivateMod(PlayerManager pm)
    {
        player = pm;
      
        if (player.UsingMod(player.selectedWeaponType))
        {
            player.h.playerInventory.weaponMod = this;
            ActivatePro();
            ActivateCon();
        }

    }

    public string GetDesc()
    {
        return "Con: " + GetConDesc() + "\n\n" + "Pro: " + GetProDesc();
    }
    private void ActivatePro()
    {
        switch (proMod)
        {
            //Common
            case BlacksmithProMods.halfHpDmg:
                player.h.playerInventory.halfHpDmg = true;
                break;
            case BlacksmithProMods.dmgPerAilment:
                player.h.playerInventory.dmgPerAilment = true;
                break;
            case BlacksmithProMods.attackTarget:
                player.h.playerInventory.attackTarget = true;
                break;
            case BlacksmithProMods.extraMaxHealth:
                player.h.GetHealthStats().AddModifier(new StatModifier(proAmount, StatModType.Flat, this));
                break;
            case BlacksmithProMods.extraHealthPerEncounter:
                player.healthOnExit += proAmount;
                break;
            case BlacksmithProMods.extraFireResist:
                player.h.playerInventory.fireModResist = proAmount;
                player.h.playerInventory.hasResistance = true;
                player.h.playerInventory.resistanceIndex = 1;
                break;

            //Sword
            case BlacksmithProMods.blockPerTurn:
                if (player.h.playerInventory.blockPerTurn < 1)
                {
                    player.h.playerInventory.blockPerTurn = 1;
                }
                break;
            case BlacksmithProMods.extraLifesteal:
                player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(proAmount, StatModType.Flat, this));
                break;
            case BlacksmithProMods.burnSpread:
                player.h.playerInventory.burnSpread = 15;
                break;
            case BlacksmithProMods.vulnerableExtended:
                float[] vInfo = player.h.GetVulnerableInfo();
                player.h.SetVulnerableInfo(new float[] { vInfo[0] + 2, vInfo[1] });
                break;
            case BlacksmithProMods.healOnCrit:
                player.h.playerInventory.healOnCrit = proAmount;
                break;
            case BlacksmithProMods.extraLightningResist:
                player.h.playerInventory.lightningModResist = proAmount;
                player.h.playerInventory.hasResistance = true;
                player.h.playerInventory.resistanceIndex = 3;
                break;
            case BlacksmithProMods.extraDivineResist:
                player.h.playerInventory.hasResistance = true;
                player.h.playerInventory.resistanceIndex = 0;
                break;

            //Staff
            case BlacksmithProMods.extraDmgPerGold:
                player.h.playerInventory.extraDmgPerGold = proAmount;
                break;
            case BlacksmithProMods.vulnerableConfusion:
                player.h.playerInventory.vulnerableConfusion = 50;
                break;
            case BlacksmithProMods.extraLifestrike:
                player.h.GetAttackLifestrike().AddModifier(new StatModifier(proAmount, StatModType.Flat, this));
                break;
            case BlacksmithProMods.attackDaze:
                player.h.playerInventory.attackDazeChance = proAmount;
                break;
            case BlacksmithProMods.extraToughness:
                player.h.GetToughness().AddModifier(new StatModifier(proAmount, StatModType.Flat, this));
                break;
            case BlacksmithProMods.extraPoisonResist:
                player.h.playerInventory.poisonModResist = proAmount;
                player.h.playerInventory.hasResistance = true;
                player.h.playerInventory.resistanceIndex = 5;
                break;


            //Gun
            case BlacksmithProMods.poisonSpread:
                player.h.playerInventory.poisonSpread = 15;
                break;
            case BlacksmithProMods.critOnFrozen:
                player.h.playerInventory.critOnFrozen = true;
                break;
            case BlacksmithProMods.attackPsychicType:
                player.h.playerInventory.attackPsychicType = true;
                break;
            case BlacksmithProMods.extraCrit:
                player.h.GetCritChance().AddModifier(new StatModifier(proAmount, StatModType.Flat, this));
                break;
            case BlacksmithProMods.extraPsychicResist:
                player.h.playerInventory.psychicModResist = proAmount;
                player.h.playerInventory.hasResistance = true;
                player.h.playerInventory.resistanceIndex = 6;
                break;
            case BlacksmithProMods.extraIceResist:
                player.h.playerInventory.iceModResist = proAmount;
                player.h.playerInventory.resistanceIndex = 2;
                player.h.playerInventory.hasResistance = true;
                break;

            //Fists
            case BlacksmithProMods.blockPerEnc:
                player.h.playerInventory.blockPerEnc = proAmount;
                break;
            case BlacksmithProMods.critOnPoison:
                player.h.playerInventory.critOnPoison = true;
                break;
            case BlacksmithProMods.shockSpread:
                player.h.playerInventory.shockSpread = 15;
                break;
            case BlacksmithProMods.attackPoisonType:
                player.h.playerInventory.attackPoisonType = true;
                break;
            case BlacksmithProMods.extraPhysicalResist:
                player.h.playerInventory.resistanceIndex = 4;
                player.h.playerInventory.hasResistance = true;
                break;

        }
    }

    private void SetProAmount()
    {
        switch (proMod)
        {
            //Common
            case BlacksmithProMods.extraMaxHealth:
                proAmount = UnityEngine.Random.Range(30, 40);
                break;
            case BlacksmithProMods.extraHealthPerEncounter:
                proAmount = UnityEngine.Random.Range(15, 20);
                break;
            case BlacksmithProMods.extraFireResist:
                proAmount = UnityEngine.Random.Range(35, 50);
                break;

            //Sword
            case BlacksmithProMods.extraLifesteal:
                proAmount = UnityEngine.Random.Range(40, 60);
                break;
            case BlacksmithProMods.healOnCrit:
                proAmount = UnityEngine.Random.Range(10, 25);
                break;
            case BlacksmithProMods.extraLightningResist:
                proAmount = UnityEngine.Random.Range(35, 50);
                break;

            //Staff
            case BlacksmithProMods.extraDmgPerGold:
                proAmount = UnityEngine.Random.Range(5, 15);
                break;
            case BlacksmithProMods.extraLifestrike:
                proAmount = UnityEngine.Random.Range(20, 50);
                break;
            case BlacksmithProMods.attackDaze:
                proAmount = UnityEngine.Random.Range(15, 25);
                break;
            case BlacksmithProMods.extraToughness:
                proAmount = UnityEngine.Random.Range(3, 8);
                break;
            case BlacksmithProMods.extraPoisonResist:
                proAmount = UnityEngine.Random.Range(35, 50);
                break;


            //Gun
            case BlacksmithProMods.extraCrit:
                proAmount = UnityEngine.Random.Range(30, 40);
                break;
            case BlacksmithProMods.extraPsychicResist:
                proAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithProMods.extraIceResist:
                proAmount = UnityEngine.Random.Range(35, 50);
                break;

            //Fists
            case BlacksmithProMods.blockPerEnc:
                proAmount = UnityEngine.Random.Range(2, 5);
                break;

        }
    }

    private void ActivateCon()
    {
        switch (conMod)
        {
            //Common
            case BlacksmithConMods.ailmentChance:
                player.h.playerInventory.ailmentChanceMod = 10;
                break;
            case BlacksmithConMods.ailmentTask:
                player.h.playerInventory.ailmentDiffMod = true;
                break;
            case BlacksmithConMods.takeMoreFireDmg:
                player.h.playerInventory.takeExtraDamageTypes.Add("Fire");
                break;
            case BlacksmithConMods.lowerLightningDmg:
                player.h.playerInventory.inflictLessDamageTypes.Add("Lightning");
                break;
            case BlacksmithConMods.takeMorePhysicalDmg:
                player.h.playerInventory.takeExtraDamageTypes.Add("Physical");
                break;
            case BlacksmithConMods.lowerMaxHealth:
                player.h.GetHealthStats().AddModifier(new StatModifier(-conAmount, StatModType.Flat, this));
                break;

            //Sword
            case BlacksmithConMods.halfHealthEnc:
                player.h.playerInventory.halfHealthEnc = true;
                break;
            case BlacksmithConMods.lowerRevive:
                player.numRevives -= 1;
                break;
            case BlacksmithConMods.takeMorePoisonDmg:
                player.h.playerInventory.takeExtraDamageTypes.Add("Poison");
                break;
            case BlacksmithConMods.lowerDivineDmg:
                player.h.playerInventory.inflictLessDamageTypes.Add("Divine");
                break;
            case BlacksmithConMods.lowerPhysicalDmg:
                player.h.playerInventory.inflictLessDamageTypes.Add("Physical");
                break;
            case BlacksmithConMods.lowerPsychicChance:
                player.h.playerInventory.psychicModStatus = -conAmount;
                break;

            //Staff
            case BlacksmithConMods.halfGoldEnc:
                player.h.playerInventory.halfGoldEnc = true;
                break;
            case BlacksmithConMods.lowerAcademyPassive:
                List<string> newBonus = new List<string>();
                if(player.equippedBonuses.Count > 4)
                {
                    newBonus.Add(player.equippedBonuses[0]);
                    newBonus.Add(player.equippedBonuses[1]);
                    newBonus.Add(player.equippedBonuses[2]);
                    newBonus.Add(player.equippedBonuses[3]);
                }
                player.equippedBonuses = newBonus;
                break;
            case BlacksmithConMods.firestormLowerLifesteal:
                player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(-conAmount, StatModType.Flat, this));
                break;
            case BlacksmithConMods.lowerFireDmg:
                player.h.playerInventory.inflictLessDamageTypes.Add("Fire");
                break;
            case BlacksmithConMods.lowerIceDmg:
                player.h.playerInventory.inflictLessDamageTypes.Add("Ice");
                break;
            case BlacksmithConMods.takeMoreDivineDmg:
                player.h.playerInventory.takeExtraDamageTypes.Add("Divine");
                break;
            case BlacksmithConMods.lowerAccuracy:
                player.h.GetAccuracy().AddModifier(new StatModifier(-conAmount, StatModType.Flat, this));
                break;

            //Gun
            case BlacksmithConMods.lowerLifestrike:
                player.h.GetAttackLifestrike().AddModifier(new StatModifier(-conAmount, StatModType.Flat, this));
                break;
            case BlacksmithConMods.lowerAttackTarget:
                player.h.playerInventory.lowerAttackTarget = true;
                break;
            case BlacksmithConMods.lowerPoisonDmg:
                player.h.playerInventory.inflictLessDamageTypes.Add("Poison");
                break;
            case BlacksmithConMods.lowerCrit:
                player.h.GetCritChance().AddModifier(new StatModifier(-conAmount, StatModType.Flat, this));
                break;
            case BlacksmithConMods.attackSelfDmg:
                player.h.playerInventory.attackSelfDmg = -5;
                break;
            case BlacksmithConMods.takeMorePsychicDmg:
                player.h.playerInventory.takeExtraDamageTypes.Add("Psychic");
                break;

            //Fists
            case BlacksmithConMods.lowerDivineStack:
                player.h.playerInventory.lowerDivineStack = true;
                break;
            case BlacksmithConMods.renegadeLowerLifesteal:
                player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(-conAmount, StatModType.Flat, this));
                break;
            case BlacksmithConMods.lowerVulnerableMult:
                player.h.playerInventory.lowerVulnerableMult = true;
                break;
            case BlacksmithConMods.lowerPsychicDmg:
                player.h.playerInventory.inflictLessDamageTypes.Add("Psychic");
                break;
        }
    }

    private void SetConAmount()
    {
        switch (conMod)
        {
            //Common
            case BlacksmithConMods.takeMoreFireDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.lowerLightningDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.takeMorePhysicalDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.lowerMaxHealth:
                conAmount = UnityEngine.Random.Range(25, 50);
                break;

            //Sword
            case BlacksmithConMods.takeMorePoisonDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.lowerDivineDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.lowerPhysicalDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.lowerPsychicChance:
                conAmount = UnityEngine.Random.Range(10, 25);
                break;

            //Staff
            case BlacksmithConMods.firestormLowerLifesteal:
                conAmount = UnityEngine.Random.Range(15, 30);
                break;
            case BlacksmithConMods.lowerFireDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.lowerIceDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.takeMoreDivineDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.lowerAccuracy:
                conAmount = UnityEngine.Random.Range(20, 30);
                break;

            //Gun
            case BlacksmithConMods.lowerLifestrike:
                conAmount = UnityEngine.Random.Range(15, 30);
                break;
            case BlacksmithConMods.lowerPoisonDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
            case BlacksmithConMods.lowerCrit:
                conAmount = UnityEngine.Random.Range(20, 30);
                break;
            case BlacksmithConMods.takeMorePsychicDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;

            //Fists
            case BlacksmithConMods.renegadeLowerLifesteal:
                conAmount = UnityEngine.Random.Range(25, 45);
                break;
            case BlacksmithConMods.lowerPsychicDmg:
                conAmount = UnityEngine.Random.Range(35, 50);
                break;
        }
    }

    private string GetProDesc()
    {
        switch (proMod)
        {
            //Common
            case BlacksmithProMods.halfHpDmg:
                return "Do 30% more damage at 50% <sprite=45>.";
            case BlacksmithProMods.dmgPerAilment:
                return "Do 10% more damage per Ailment.";
            case BlacksmithProMods.attackTarget:
                return "Attack targets up to 2 enemies.";
            case BlacksmithProMods.extraMaxHealth:
                return "Gain " + proAmount + "% more maximum <sprite=45>.";
            case BlacksmithProMods.extraHealthPerEncounter:
                return "Gain " + proAmount + " more <sprite=45> per encounter.";
            case BlacksmithProMods.extraFireResist:
                return "Gain " + proAmount + "% more <sprite=37> Resist.";

            //Sword
            case BlacksmithProMods.blockPerTurn:
                return "Gain 1 Block <sprite=44> every turn.";
            case BlacksmithProMods.extraLifesteal:
                return "Gain " + proAmount + "% more <sprite=55>.";
            case BlacksmithProMods.burnSpread:
                return "<sprite=37> condition has 15% chance to spread to a new target every turn.";
            case BlacksmithProMods.vulnerableExtended:
                return "<sprite=40> condition remain on target longer.";
            case BlacksmithProMods.healOnCrit:
                return "Critical hits on Attacks heal for " + proAmount + "<sprite=45>.";
            case BlacksmithProMods.extraLightningResist:
                return "Gain " + proAmount + "% more <sprite=41> Resist.";
            case BlacksmithProMods.extraDivineResist:
                return "Gain <sprite=39> Resist.";

            //Staff
            case BlacksmithProMods.extraDmgPerGold:
                return "Do " + proAmount + "% more damage per <sprite=11>.";
            case BlacksmithProMods.vulnerableConfusion:
                return "Enemies affected by <sprite=40> condition have a 50% chance of targeting their allies.";
            case BlacksmithProMods.extraLifestrike:
                return "Gain " + proAmount + "% more <sprite=50>.";
            case BlacksmithProMods.attackDaze:
                return "Attacks have a  " + proAmount + "% chance to <sprite=28> target.";
            case BlacksmithProMods.extraToughness:
                return "Gain " + proAmount + " more <sprite=49>.";
            case BlacksmithProMods.extraPoisonResist:
                return "Gain " + proAmount + "% more <sprite=38> Resist.";


            //Gun
            case BlacksmithProMods.poisonSpread:
                return "<sprite=38> condition has a 15% chance to spread to a new target every turn.";
            case BlacksmithProMods.critOnFrozen:
                return "Attacks on enemies affected by <sprite=42> condition is always a critical hit.";
            case BlacksmithProMods.attackPsychicType:
                return "Attacks deal <sprite=40> Damage.";
            case BlacksmithProMods.extraCrit:
                return "Gain " + proAmount + "% more <sprite=47>.";
            case BlacksmithProMods.extraPsychicResist:
                return "Gain " + proAmount + "% more <sprite=40> Resist.";
            case BlacksmithProMods.extraIceResist:
                return "Gain " + proAmount + "% more <sprite=42> Resist.";

            //Fists
            case BlacksmithProMods.blockPerEnc:
                return "Start every encounter with " + proAmount + " <sprite=44>.";
            case BlacksmithProMods.critOnPoison:
                return "Attacks on enemies affected by <sprite=38> condition is always a critical hit.";
            case BlacksmithProMods.shockSpread:
                return "<sprite=41> condition has a 15% chance to spread to a new target every turn.";
            case BlacksmithProMods.attackPoisonType:
                return "Attacks deal <sprite=38> Damage.";
            case BlacksmithProMods.extraPhysicalResist:
                return "Gain <sprite=43> Resist.";

        }
        return "";
    }

    private string GetConDesc()
    {
        switch (conMod)
        {
            //Common
            case BlacksmithConMods.ailmentChance:
                return "More likely to be affected by Ailments <sprite=65>.";
            case BlacksmithConMods.ailmentTask:
                return "Ailment tasks are harder to complete.";
            case BlacksmithConMods.takeMoreFireDmg:
                return "Take " + conAmount + "% more <sprite=37> damage.";
            case BlacksmithConMods.lowerLightningDmg:
                return "Deal " + conAmount + "% less <sprite=41> damage.";
            case BlacksmithConMods.takeMorePhysicalDmg:
                return "Take " + conAmount + "% more <sprite=43> damage.";
            case BlacksmithConMods.lowerMaxHealth:
                return "Start with " + conAmount + "% less maximum <sprite=45>.";
              
            //Sword
            case BlacksmithConMods.halfHealthEnc:
                return "<sprite=45> recovery per encounter is halved.";
            case BlacksmithConMods.lowerRevive:
                return "Start with 1 less <sprite=32>.";
            case BlacksmithConMods.takeMorePoisonDmg:
                return "Take " + conAmount + "% more <sprite=38> damage.";
            case BlacksmithConMods.lowerDivineDmg:
                return "Deal " + conAmount + "% less <sprite=39> damage.";
            case BlacksmithConMods.lowerPhysicalDmg:
                return "Deal " + conAmount + "% less <sprite=43> damage.";
            case BlacksmithConMods.lowerPsychicChance:
                return "Chance to apply <sprite=40> condition is reduced by " + conAmount + "%.";

            //Staff
            case BlacksmithConMods.halfGoldEnc:
                return "<sprite=11> obtained per encounter is halved.";
            case BlacksmithConMods.lowerAcademyPassive:
                return "Can only have 4 Grimoire Enchantment.";
            case BlacksmithConMods.firestormLowerLifesteal:
                return "Start with " + conAmount + "% less <sprite=55>.";
            case BlacksmithConMods.lowerFireDmg:
                return "Deal " + conAmount + "% less <sprite=37> damage.";
            case BlacksmithConMods.lowerIceDmg:
                return "Deal " + conAmount + "% less <sprite=42> damage.";
            case BlacksmithConMods.takeMoreDivineDmg:
                return "Take " + conAmount + "% more <sprite=39> damage.";
            case BlacksmithConMods.lowerAccuracy:
                return "Start with " + conAmount + "% less <sprite=46>.";

            //Gun
            case BlacksmithConMods.lowerLifestrike:
                return "Start with " + conAmount + "% less <sprite=50>.";
            case BlacksmithConMods.lowerAttackTarget:
                return "Attack targets only 1 enemy.";
            case BlacksmithConMods.lowerPoisonDmg:
                return "Deal " + conAmount + "% less <sprite=38> damage.";
            case BlacksmithConMods.lowerCrit:
                return "Start with " + conAmount + "% less <sprite=47>.";
            case BlacksmithConMods.attackSelfDmg:
                return "Attacks deal 5 damage to self.";
            case BlacksmithConMods.takeMorePsychicDmg:
                return "Take " + conAmount + "% more <sprite=40> damage.";

            //Fists
            case BlacksmithConMods.lowerDivineStack:
                return "Maximux <sprite=39> stack is reduced to 5.";
            case BlacksmithConMods.renegadeLowerLifesteal:
                return "Start with " + conAmount + "% less <sprite=55>.";
            case BlacksmithConMods.lowerVulnerableMult:
                return "Damage to enemies affected by <sprite=40> condition is reduced to 1.25x.";
            case BlacksmithConMods.lowerPsychicDmg:
                return "Deal " + conAmount + "% less <sprite=40> damage.";
        }
        return "";
    }

    public int GetProAmount()
    {
        return proAmount;
    }

    public int GetConAmount()
    {
        return conAmount;
    }
}

