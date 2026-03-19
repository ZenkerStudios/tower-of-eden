using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(2)]
public class HubManager : MonoBehaviour
{

    public FriendlyToken home;
    public FriendlyToken smith;
    public FriendlyToken academy;
    public FriendlyToken church;
    public FriendlyToken tower;

    public GameObject smithVial;
    public GameObject smithShard;
    public GameObject academyVial;
    public GameObject academyShard;
    public GameObject churchVial;
    public GameObject churchShard;
    public HomeHub homeHub;
    public GameObject towerHub;
    public GameObject bountyHub;

    public bool inCity;

    private PlayerManager player;

    public TextMeshProUGUI gold;
    public TextMeshProUGUI gem;
    public TextMeshProUGUI vials;
    public TextMeshProUGUI shards;
    public TextMeshProUGUI scraps;
    public TextMeshProUGUI grimoire;

    public static FriendlyToken interactingWith;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        StartCoroutine(GameUtilities.WaitForConversation(() => homeHub.NextExitDialogue()));
        StartCoroutine(GameUtilities.WaitForConversation(() => CheckMiscConditions()));
    }

    // Start is called before the first frame update
    void Start()
    {
        if (inCity)
        {
            CheckNewDialogues();
        }
    }

    // Update is called once per frame
    void Update()
    {
        gem.text = "<sprite=9> " + player.gemstonesOwned;
        vials.text = "<sprite=7> " + player.etherVialsOwned;
        shards.text = "<sprite=5> " + player.etherShardsOwned;
        scraps.text = "<sprite=19> " + player.metalScrapsOwned;
        grimoire.text = "<sprite=13> " + player.grimoirePagesOwned;

    }

    public void ToggleCurrencyVisibility(bool val)
    {
        gem.transform.parent.parent.gameObject.SetActive(val);
    }

    public void CheckNewDialogues()
    {
        if (smith.nextDialogue != null
            && (smith.nextDialogue.GetDialogueId().Equals("ARLO_020_01") 
            || smith.nextDialogue.GetDialogueId().Equals("ARLO_021_01")))
        {
            academy.characterImage.gameObject.SetActive(false);
            academy.urgentSymbol.gameObject.SetActive(false);
            player.wentToAcademyThisRun = true;
            academy.giftedThisRun = true;
            academy.nextDialogue = null;
            academyVial.SetActive(false);
            academyShard.SetActive(false);
            academy.gameObject.GetComponent<AcademyHub>().canBark = false;
            academyShard.transform.parent.parent.gameObject.SetActive(false);
        }
    }

    public void CheckMiscConditions()
    {
        if(player.GetSuccessfulAttempts() == 1
            && player.cutscenesUnlocked.Contains(Cutscenes.Two.ToString())
            && !player.cutscenesUnlocked.Contains(Cutscenes.Three.ToString()))
        {
            CutsceneManager.instance.Play(Cutscenes.Three);
            StartCoroutine(
                GameUtilities.WaitForCutscene(() => DialogueManager.instance.ShowDialogue(
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Home, "HOME_016_01"))));

            StartCoroutine(
                GameUtilities.WaitForCutscene(() => academy.InitializeTokenDialogue()));
            StartCoroutine(
                GameUtilities.WaitForCutscene(() => tower.InitializeTokenDialogue()));
        }
        if (player.GetSuccessfulAttempts() > 0)
        {
            SteamIntegration.UnlockThisAchievement(Achievements.FIRST_WIN.ToString());
        }

        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.IzaakConfrontsEden) 
            && !player.savedDialogueConditionsMet.Contains(DialogueConditions.IzaakReturnsToTower))
        {
            towerHub.transform.GetChild(0).GetComponent<Button>().interactable = false;
            towerHub.GetComponent<ButtonHover>().isInteractable = false;
            academy.InitializeTokenDialogue();
        }
        if(player.savedDialogueConditionsMet.Contains(DialogueConditions.AgnesConcerned) 
            && !player.savedDialogueConditionsMet.Contains(DialogueConditions.IzaakReassuresEden))
        {
            DialogueManager.instance.ShowDialogue(
                DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Home, "HOME_011_01"));

        }

    }


}
