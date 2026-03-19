using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChurchHub : MonoBehaviour
{

    public GameObject hubWindow;
    public Animator windowAnimator;

    private PlayerManager player;
    public GameObject churchTarget;

    public Button gemGift;
    
    public Button scrapGift;
    
    public Button pageGift;
    
    public Button shardGift;
    
    public Button vialGift;


    [SerializeField]
    protected FriendlyToken friendlyToken;
    private DialogueManager dialogueManager;
    public SpecialSystem specialSystem;

   
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        dialogueManager = DialogueManager.instance;
        scrapGift.gameObject.SetActive(player.churchAdmirationLevel > 0);
        gemGift.gameObject.SetActive(player.churchAdmirationLevel > 1);
        vialGift.gameObject.SetActive(player.churchAdmirationLevel > 2);
        shardGift.gameObject.SetActive(player.churchAdmirationLevel > 3);
        pageGift.gameObject.SetActive(player.churchAdmirationLevel > 4);

        scrapGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = scrapCost + "";
        gemGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gemstoneCost + "";
        vialGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = vialCost + "";
        shardGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = shardCost + "";
        pageGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = pageCost + "";

        boonIndex = player.churchBoon;

    }

    // Update is called once per frame
    void Update()
    {
        SetCosts();

    }

    private void SetCosts()
    {
        gemGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        vialGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        shardGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        pageGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        scrapGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);

        if (player.gemstonesOwned < gemstoneCost)
        {
            gemGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            gemGift.enabled = false;
        }
        if (player.etherVialsOwned < vialCost)
        {
            vialGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            vialGift.enabled = false;
        }
        if (player.etherShardsOwned < shardCost)
        {
            shardGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            shardGift.enabled = false;
        }
        if (player.grimoirePagesOwned < pageCost)
        {
            pageGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            pageGift.enabled = false;
        }
        if (player.metalScrapsOwned < scrapCost)
        {
            scrapGift.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 0, 0, 255);
            scrapGift.enabled = false;
        }
    }

    private int gemstoneCost = 20;
    private int scrapCost = 30;
    private int vialCost = 3;
    private int shardCost = 1;
    private int pageCost = 2;
 
    public void OpenWindow()
    {
        AudioManager.instance.PlaySfxSound("Button_Click");
        friendlyToken.CheckGiftable();
        if (player.wentToChurchThisRun)
        {
            ShowWindow();
        }
        else
        {
            player.wentToChurchThisRun = true;
            if (friendlyToken.TriggerVisitDialogue())
            {
                StartCoroutine(GameUtilities.WaitForConversation(() => ShowWindow()));
            }
            else
            {
                ShowWindow();
            }

            player.numChurchInteraction++;
        }
    }

    public void ShowWindow()
    {
        HubManager.interactingWith = friendlyToken;
        //play open animation
        windowAnimator.Play("Church Open");
        TriggerChurchBark();
    }

    public void TriggerChurchBark()
    {
        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockChurch) && Random.Range(0, 100) < friendlyToken.interactableNpc.barkChance)
        {
            DialogueBlock bark = friendlyToken.interactableNpc.GetRandomBark();
            if (bark != null)
            {
                dialogueManager.ShowBarkWithOwner(
               friendlyToken.interactableNpc, friendlyToken.characterImage, bark);
            }
        }
    }

    private static string[] boonDesc = new string[]
    {
        "Gain 5 <sprite=49>",
        "Gain 25 more maximum <sprite=45>",
        "Gain more Ability XP",
        "Gain 1 Ability Slot",
        "Gain 1 <sprite=48>"
    };
    private int boonIndex = 0;

    public void GiftToFew(int gifts)
    {
        TriggerChurchBark();
        boonIndex = gifts;
        player.churchBoon = boonIndex;
        player.churchBoonDesc = boonDesc[gifts-7];

        switch(gifts)
        {
            case 7:
                player.ChangeGemstonesAmount(-gemstoneCost);
                break;
            case 8:
                player.ChangeScrapsAmount(-scrapCost);
                break;
            case 9:
                player.ChangeVialsAmount(-vialCost);
                break;
            case 10:
                player.ChangeShardAmount(-shardCost);
                break;
            case 11:
                player.ChangeGrimoirePageAmount(-pageCost);
                break;
        }
        
        List<System.Action> popups = new List<System.Action>();
        popups.Add(() => specialSystem.TriggerIconPopUpWithIndex(churchTarget.transform, true, 59));

        GameUtilities.ShowPopup(this, popups);

        gemGift.gameObject.SetActive(false);
        scrapGift.gameObject.SetActive(false);
        vialGift.gameObject.SetActive(false);
        shardGift.gameObject.SetActive(false);
        pageGift.gameObject.SetActive(false);
    }

    public void ApplyChurchBoons()
    {

        if (player.churchBoon < 7 && player.churchBoon > 11) return;

        switch ((RewardType)player.churchBoon)
        {
            case RewardType.Gemstones:
                //buff toughness
                player.h.toughness.AddModifier(new StatModifier(5, StatModType.Flat, this));
                break;
            case RewardType.MetalScraps:
                //buff hp
                player.h.IncreaseMaxHealth(25);
                player.h.SetToMaxHealth();
                break;
            case RewardType.EtherVial:
                //buff ability slot xp
                player.h.playerInventory.bonusSlotXp = true;
                break;
            case RewardType.EtherShard:
                //begin with +1 slot
                player.h.abilitySlot += 1;
                break;
            case RewardType.GrimoirePage:
                //buff actions
                player.h.actions.AddModifier(new StatModifier(1, StatModType.Flat, this));
                break;
        }
    }

    private void TriggerCloseWindow()
    {
        if (player.savedPastConvos.Contains("AGNES_026_07")
            && player.savedPastConvos.Contains("ABSALOM_010_05")
            && player.savedPastConvos.Contains("ARLO_017_05")
            && !player.cutscenesUnlocked.Contains(Cutscenes.Sixteen.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Sixteen);
            StartCoroutine(GameUtilities.WaitForCutscene(() => windowAnimator.Play("Church Close")));
            SteamIntegration.UnlockThisAchievement(Achievements.FINAL_ENDING.ToString());

        }
        else
        {
            windowAnimator.Play("Church Close");
        }
    }

    public void CloseWindow()
    {
        //play close animation 
        TriggerCloseWindow();

    }

}
