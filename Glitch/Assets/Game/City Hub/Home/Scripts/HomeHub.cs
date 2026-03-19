using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeHub : MonoBehaviour
{

    public GameObject hubWindow;
    public Animator windowAnimator;

    public GameObject npcPanel;
    public GameObject cutscenePanel;
    private int[] cutscenes = new int[] { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
    private string[] cutsceneNames = new string[]
  {
        "1. Intro",
        "2. Eden Defeated",
        "3. Unlikely Visitor",
        "4. Confrontation",
        "5. Dorian's Honesty",
        "6. Ulea Defeated",
        "7. The Lovers",
        "8. Compendium Stabilized",
        "9. Izaak's Denial",
        "10. Ragna Defeated",
        "11. Acrid Defeated",
        "12. Dorian Defeated",
        "13. Lyra Defeated",
        "14. Izaak and Agnes",
        "15. Izaak's Revolt"
  };
    private PlayerManager player;
    [SerializeField]
    protected FriendlyToken friendlyToken;
    private DialogueManager dialogueManager;


    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = DialogueManager.instance;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

        bool active = player.GetTotalAttempts() >= 10 || player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockHome);
        gameObject.GetComponent<ButtonHover>().isInteractable = active;
        transform.GetChild(0).gameObject.GetComponent<Button>().interactable = active;

        SetEnemyWiki();
        SetCutsceneWiki();
        attemptIndex = player.attempts.Count;
        GeneratePreviousSetOfAttemptList();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int selectedCutscene = 0;
    public Image cutsceneImg;
    public TextMeshProUGUI cutsceneName;

    public void SelectCutscene(int cutscene)
    {
        selectedCutscene = cutscene;
        cutsceneName.text = CutsceneManager.instance.GetCutsceneNames(selectedCutscene-1);
        cutsceneImg.sprite = CutsceneManager.instance.cutscenePng[selectedCutscene-1];
    }


    public void PlayCutscene()
    {
        StartCoroutine(PlayScene((Cutscenes)selectedCutscene));
    }

    private IEnumerator PlayScene(Cutscenes cutscene)
    {
        yield return new WaitForSeconds(0.1f);
        CutsceneManager.instance.Play(cutscene);
    }

    public NpcWiki viewingNpc;
    public AbilityInfo viewingAbility;
    public ArtifactCodex viewingArtifact;
    public void ExpandNpcInfo()
    {
        viewingNpc.ExpandInfo();
    }

    public void OpenWindow()
    {
        AudioManager.instance.PlaySfxSound("Button_Click");
        if (player.wentHomeThisRun)
        {
            ShowWindow();
        }
        else
        {
            player.wentHomeThisRun = true;
            if (friendlyToken.TriggerVisitDialogue())
            {
                StartCoroutine(GameUtilities.WaitForConversation(() => ShowWindow()));
            }
            else
            {
                ShowWindow();
            }
            player.numHomeInteraction++;
        }

    }

    public void NextExitDialogue()
    {
        if (!player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockHome)
            && (player.GetTotalAttempts() >= 10))
        {
            dialogueManager.ShowDialogue(dialogueManager.GetDialogueBlockFor(InteractableNpcs.Home, "HOME_002_01"));
        }

        if (friendlyToken.nextDialogue != null
            && !friendlyToken.nextDialogue.GetDialogueId().Equals("HOME_012_01")
            && !friendlyToken.nextDialogue.GetDialogueId().Equals("HOME_013_01")
            && !friendlyToken.nextDialogue.GetDialogueId().Equals("HOME_014_01"))
        {
            friendlyToken.InitializeTokenDialogue();
            dialogueManager.ShowDialogue(friendlyToken.nextDialogue);
            friendlyToken.urgentSymbol.SetActive(false);
            friendlyToken.nextDialogue = null;

        }


    }

    private void SetEnemyWiki()
    {
        for (int index = 0; index < npcPanel.transform.childCount; index++)
        {
            npcPanel.transform.GetChild(index).GetComponentInChildren<TextMeshProUGUI>().text = "???";
            npcPanel.transform.GetChild(index).GetComponent<Button>().interactable = false;

            NpcWiki npc = npcPanel.transform.GetChild(index).GetComponentInChildren<NpcWiki>();
            string npcName = npc.GetName();


            if (player.HasDefeatedEnemy(npc.enemyCharacter.GetName()))
            {
                switch(npcName)
                {
                    case "Dorian Gauntlet":
                        npcName = "Sir Dorian Gold, The Fool (GA)";
                        break;
                    case "Dorian Guard":
                        npcName = "Sir Dorian Gold, The Fool (SH)";
                        break;
                    case "Dorian Gun":
                        npcName = "Sir Dorian Gold, The Fool (GU)";
                        break;
                    case "Dorian Spear":
                        npcName = "Sir Dorian Gold, The Fool (SP)";
                        break;
                    case "Dorian Sword":
                        npcName = "Sir Dorian Gold, The Fool (SW)";
                        break;
                }

                if (npcName.Equals("Hellthorn Banshee 1")
                || npcName.Equals("Hellthorn Banshee 2"))
                {
                    npcName = "Hellthorn Banshee";
                }
                npcPanel.transform.GetChild(index).GetComponentInChildren<TextMeshProUGUI>().text = npcName;
                npcPanel.transform.GetChild(index).GetComponent<Button>().interactable = true;
            }
        }
    }

    public void SetCutsceneWiki()
    {
        for (int index = 0; index < cutscenes.Length; index++)
        {
            string cutscene = ((Cutscenes)cutscenes[index]).ToString();
            if (player.cutscenesUnlocked.Contains(cutscene))
            {
                cutscenePanel.transform.GetChild(index).GetComponent<Button>().interactable = true;
                cutscenePanel.transform.GetChild(index).GetChild(0).GetComponent<TextMeshProUGUI>().text = cutsceneNames[index];
            } else
            {
                cutscenePanel.transform.GetChild(index).GetChild(0).GetComponent<TextMeshProUGUI>().text = "???";

            }

        }
    }

    public void ShowWindow()
    {
        //play open animation
        windowAnimator.Play("Home Open");
        ResetAllBooks();
    }

    public List<Sprite> defaultIcons = new List<Sprite>();
    public void ResetAllBooks()
    {
        HideDeskOverlay();

        //Activity
        attemptIndex = player.attempts.Count;
        GeneratePreviousSetOfAttemptList();
        GetAttempt(player.attempts[attemptIndex], attemptIndex+1);

        //Enemies
        viewingNpc.nameBox.text = "???";
        viewingNpc.descBox.text = "???";
        viewingNpc.headshotContainer.sprite = defaultIcons[0];
        viewingNpc.descBox.transform.parent.parent.GetChild(3).gameObject.SetActive(false);
        viewingNpc.transform.parent.parent.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

        //Weapons
        viewingAbility.typeTextBox.text = "???";
        viewingAbility.nameTextBox.text = "???";
        viewingAbility.descTextBox.text = "???";
        viewingAbility.targetTextBox.text = "???";
        viewingAbility.statusChanceTextBox.text = "???";
        viewingAbility.durationTextBox.text = "???";
        viewingAbility.abilityImage.sprite = defaultIcons[1];
        viewingAbility.transform.parent.parent.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

        //Artifacts
        viewingArtifact.artifactImg.sprite = defaultIcons[2];
        viewingArtifact.nameText.text = "???";
        viewingArtifact.descText.text = "???";
        viewingArtifact.transform.parent.parent.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

        //Cutscenes
        SelectCutscene(1);
        cutscenePanel.transform.parent.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);


    }

    private void HideDeskOverlay()
    {
        hubWindow.transform.GetChild(1).gameObject.SetActive(false);
        hubWindow.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        hubWindow.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
        hubWindow.transform.GetChild(1).GetChild(3).gameObject.SetActive(false);
        hubWindow.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
        hubWindow.transform.GetChild(1).GetChild(5).gameObject.SetActive(false);
        hubWindow.transform.GetChild(1).GetChild(6).gameObject.SetActive(false);
    }

    public void CloseWindow()
    {
        //play close animation 
        windowAnimator.Play("Home Close");
    }

    public GameObject attemptPanel;

    public TextMeshProUGUI attemptNumberText;
    public TextMeshProUGUI outcomeText;
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI modText;
    public TextMeshProUGUI difficultyText;

    public TextMeshProUGUI enchanmentOneText;
    public TextMeshProUGUI enchanmentTwoText;

    public TextMeshProUGUI abilitiesOneText;
    public TextMeshProUGUI abilitiesTwoText;

    public TextMeshProUGUI artifactText;

    private int attemptIndex = 0;
    private readonly int attemptsPerSet = 5;
    public void GenerateNextSetOfAttemptList()
    {
        attemptIndex = Mathf.Clamp(attemptIndex + attemptsPerSet, 0, player.attempts.Count-1);
        GenerateSetOfAttempts();
    }

    public void GeneratePreviousSetOfAttemptList()
    {
        attemptIndex = Mathf.Clamp(attemptIndex - attemptsPerSet, 0, player.attempts.Count-1);
        GenerateSetOfAttempts();
    }

    private void GenerateSetOfAttempts()
    {
        GameUtilities.ToggleActiveAllChildGameObject(attemptPanel, false);
        attemptPanel.transform.parent.GetChild(1).gameObject.SetActive(true);
        attemptPanel.transform.parent.GetChild(2).gameObject.SetActive(true);
        for (int count = 0, index = attemptIndex; count < attemptsPerSet && index < player.attempts.Count; count++, index++)
        {
            AttemptUI attemptUI = attemptPanel.transform.GetChild(count).GetComponent<AttemptUI>();
            attemptUI.gameObject.SetActive(true);
            attemptUI.Initialize(index, player.attempts[index]);
        }

        if (attemptIndex + attemptsPerSet >= player.attempts.Count)
        {
            attemptPanel.transform.parent.GetChild(2).gameObject.SetActive(false);
        }
        if (attemptIndex <= 0)
        {
            attemptPanel.transform.parent.GetChild(1).gameObject.SetActive(false);
        }
    }


    public void GetAttempt(Attempts attempt, int attemptIndex)
    {
        attemptNumberText.transform.parent.parent.parent.GetChild(0).gameObject.SetActive(true);
        attemptNumberText.transform.parent.parent.parent.GetChild(1).gameObject.SetActive(false);
        attemptNumberText.transform.parent.parent.parent.GetChild(2).gameObject.SetActive(true);
        attemptNumberText.transform.parent.parent.parent.GetChild(3).gameObject.SetActive(false);

        attemptNumberText.text = "#" + (attemptIndex);
        outcomeText.text = attempt.GetOutcome();
        floorText.text = "Floor " + ((int)attempt.floor - 1);
        weaponText.text = attempt.weapon.ToString();
        modText.text = attempt.mod;
        difficultyText.text = GameUtilities.difficultyNames[(int)attempt.difficulty];


        string abilityOne = "";
        string abilityTwo= "";
        string abilityThree = "";
        string enchantOne = "";
        string enchantTwo = "";
        string artifact = "";

        for (int x = 0; x < attempt.enchanments.Count; x++)
        {
            if(x < 4)
            {
                enchantOne += "<sprite=67>" + GameUtilities.GetEnchantmentName(attempt.enchanments[x])  + "\n";
            } else
            {
                enchantTwo += "<sprite=67>" + GameUtilities.GetEnchantmentName(attempt.enchanments[x]) + "\n";
            }
        }

        for (int x = 0; x < attempt.abilitiesName.Count; x++)
        {
            if (x < 3)
            {
                abilityOne += "<sprite=" + attempt.abilitiesSpriteIndex[x] + "> " 
                    + attempt.abilitiesName[x] + "\n";
            }
            else if(x >= 3 && x < 6)
            {
                abilityTwo += "<sprite=" + attempt.abilitiesSpriteIndex[x] + "> "
                    + attempt.abilitiesName[x] + "\n";
            } 
            else
            {
                abilityThree += "<sprite=" + attempt.abilitiesSpriteIndex[x] + "> "
                    + attempt.abilitiesName[x] + "\n";
            }
        }

        for (int x = 0; x < attempt.artifactsSpriteIndex.Count; x++)
        {
            artifact += "<sprite=" + attempt.artifactsSpriteIndex[x] + "> ";
        }


        abilitiesOneText.text = abilityOne;
        abilitiesTwoText.text = abilityTwo;
        enchanmentOneText.text = enchantOne;
        enchanmentTwoText.text = enchantTwo;
        artifactText.text = artifact;
    }
}

