using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatTutorial : MonoBehaviour
{
    public PlayerManager player;
    public Attack att;
    public GameObject pfTutorialReward;

    public List<Special> specialsReward;
    public Special special1;
    public Special special2;
    public Special special3;
    public Special special4;
    public Special special5;

    public GameObject tutorialPrompt;
    public List<EnemyNpc> enc;

    public GameObject encSelect;
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public CombatSystem combatSystem;

    public GameObject floorPage1;
    public GameObject floorPage2;

    private DialogueManager dialogueManager;

    public DialogueBlock introDialogue;
    public DialogueBlock exitDialogue;
    public DialogueBlock nextPathDialogue;

    private bool showTutorialPrompt;
    private bool startedTutorial;
    private bool finishCombat;
    private bool endConvo;
    public Sprite[] rewardSprite;

    private void Awake()
    {
        encSelect.SetActive(false);
        page3.SetActive(false);
        page2.SetActive(false);
        page1.SetActive(false);
        tutorialPrompt.SetActive(false);
        HideFloorTutorial();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        dialogueManager = DialogueManager.instance;
        //Checks if this is player's first time to show tutorial
        if (player.GetTotalAttempts() <= 1)
        {
            dialogueManager.ShowDialogue(introDialogue);
            showTutorialPrompt = true;

        }
        else
        {
            encSelect.SetActive(true);
            Destroy(gameObject);

        }
    }

    private void Update()
    {
        //Makes sure player doesn't die during tutorial
        if(player.h.hp < 25)
        {
            player.h.hp = 25;
        }

        //Wait to show prompt to accept or reject tutorial
        if(showTutorialPrompt && dialogueManager.IsDialogueOver(introDialogue.nextDialogueNode.GetDialogueId()))
        {
            showTutorialPrompt = false;
            tutorialPrompt.SetActive(true);
        }

        //Check to see that all enemies in the tutorial are dead to give reward
        if(startedTutorial && combatSystem != null && combatSystem.AllNPCDead())
        {
            try
            {
                startedTutorial = false;
                RewardSystem rs = GameObject.Find("Reward System(Clone)").GetComponent<RewardSystem>();
                rs.skipButton.SetActive(false);
                rs.nextButton.SetActive(true);
                rs.nextButton.GetComponent<Button>().onClick.AddListener(() => rs.skipButton.SetActive(false));
                GameUtilities.DeleteAllChildGameObject(rs.rewardPanel);
                foreach (Special a in specialsReward)
                {
                    var newObj = Instantiate(pfTutorialReward);
                    newObj.transform.SetParent(rs.rewardPanel.transform);
                    newObj.transform.localPosition = new Vector2(0, 0);
                    newObj.GetComponent<RectTransform>().sizeDelta = pfTutorialReward.GetComponent<RectTransform>().sizeDelta;
                    newObj.GetComponent<RectTransform>().localScale = pfTutorialReward.GetComponent<RectTransform>().localScale;
                    newObj.GetComponent<AddAbility>().newAbility = a;
                    newObj.GetComponent<AddAbility>().rewardSystem = rs;
                    newObj.GetComponent<Button>().onClick.AddListener(() => GetReward(a));
                }
            } catch(System.NullReferenceException)
            {
                startedTutorial = true;
                return;
            }

        }


        //Wait to end combat tutorial
        if (finishCombat && dialogueManager.IsDialogueOver(exitDialogue.GetDialogueId()))
        {
            StartCoroutine(FinishCombatTutorial());
        }

        //Wait to return in encounter select
        if (endConvo && dialogueManager.IsDialogueOver(nextPathDialogue.GetDialogueId()))
        {
            GoToFloorPage1();
        }

    }

    private IEnumerator FinishCombatTutorial()
    {
        finishCombat = false;
        GameObject.Find("Floor Manager").GetComponent<FloorManager>().transitionAnimator.Play("Back to Enc");
        AudioManager.instance.IncreaseMusicVolume(0.5f);
        player.playerManagerAnimator.Play("Floor_1_Start");
        yield return new WaitForSeconds(3.5f);
        encSelect.SetActive(true);
        yield return new WaitForSeconds(.3f);
        dialogueManager.ShowDialogue(nextPathDialogue);
        floorPage1.transform.parent.parent.GetChild(0).gameObject.SetActive(false);
        floorPage1.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        endConvo = true;
    }

    public void EndTutorial()
    {
        dialogueManager.inConversation = false;
        Destroy(gameObject);
        AudioManager.instance.IncreaseMusicVolume(0.5f);
    }

    public void RejectTutorial()
    {
        tutorialPrompt.SetActive(false);

        player.h.baseAttack = Instantiate(att);
        player.h.baseAttack.SetRarity(Rarity.Common);

        player.AddSpecial(special1);
        player.AddSpecial(special2);
        player.AddSpecial(special3);
        player.AddSpecial(special4);
        player.AddSpecial(special5);

        transform.GetChild(0).gameObject.SetActive(true);
        dialogueManager.ShowDialogue(exitDialogue);
        finishCombat = true;
    }

    public void FinishTutorial()
    {
        player.AddSpecial(special3);
        player.AddSpecial(special4);
        player.AddSpecial(special5);
        player.h.ChangeHealth(999);
        player.h.fireStatus = 0;
        player.h.GetAccuracy().RemoveAllModFromSource(this);

        dialogueManager.ShowDialogue(exitDialogue);
        finishCombat = true;
    }

    public void BeginTutorial()
    {
        tutorialPrompt.SetActive(false);
        encSelect.SetActive(false);

        player.h.baseAttack = Instantiate(att);
        player.h.baseAttack.SetRarity(Rarity.Common);

        player.AddSpecial(special1);
        player.AddSpecial(special2);

        player.h.GetAccuracy().AddModifier(new StatModifier(35, StatModType.Flat,10, this));
        player.h.fireStatus += 100;
        transform.GetChild(0).gameObject.SetActive(false);
        Reward r = new Reward(rewardSprite, RewardType.Ability);
        GameController.instance.StartEncounter(Floors.Floor_01, enc, r, Difficulty.Easy, false);
        combatSystem = GameObject.Find("Combat System(Clone)").GetComponent<CombatSystem>();
        ShowCombatTutorial();
        startedTutorial = true;
    }

    private void GetReward(Special s)
    {
        player.AddSpecial(s);
        Destroy(GameObject.FindGameObjectWithTag("CombatSystem"));
        FinishTutorial();

    }

    public void ShowCombatTutorial()
    {
        page1.transform.parent.gameObject.SetActive(true);
        GoToCombatPage1();
    }

    public void HideCombatTutorial()
    {
        page1.transform.parent.gameObject.SetActive(false);
        page3.SetActive(false);
        page2.SetActive(false);
        page1.SetActive(false);
        GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>().textAnimator.Play("Player Text Slide");

    }

    public void GoToCombatPage1()
    {
        page3.SetActive(false);
        page2.SetActive(false);
        page1.SetActive(true);
    }

    public void GoToCombatPage2()
    {
        page3.SetActive(false);
        page1.SetActive(false);
        page2.SetActive(true);
    }

    public void GoToCombatPage3()
    {
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(true);
    }

    public void HideFloorTutorial()
    {
        floorPage1.transform.parent.gameObject.SetActive(false);
        floorPage1.SetActive(false);
        floorPage2.SetActive(false);
    }

    public void GoToFloorPage1()
    {
        floorPage1.transform.parent.parent.GetChild(0).gameObject.SetActive(false);
        floorPage1.transform.parent.parent.GetChild(1).gameObject.SetActive(false);

        floorPage1.transform.parent.gameObject.SetActive(true);
        floorPage2.SetActive(false);
        floorPage1.SetActive(true);
    }

    public void GoToFloorPage2()
    {
        endConvo = false;

        floorPage1.transform.parent.parent.GetChild(0).gameObject.SetActive(false);
        floorPage1.transform.parent.parent.GetChild(1).gameObject.SetActive(false);

        floorPage1.transform.parent.gameObject.SetActive(true);
        floorPage1.SetActive(false);
        floorPage2.SetActive(true);
    }

   

}
