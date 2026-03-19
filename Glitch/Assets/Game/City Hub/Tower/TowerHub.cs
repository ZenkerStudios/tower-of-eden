using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TowerHub : MonoBehaviour
{

    public GameObject hubWindow;
    public GameObject edenPng;
    public GameObject increaseArrow;
    public GameObject decreaseArrow;
    public Animator windowAnimator;

    private PlayerManager player;
    [SerializeField]
    protected FriendlyToken friendlyToken;
    public TextMeshProUGUI difficultyTextName;
    public TextMeshProUGUI difficultyTextNumber;
    public TextMeshProUGUI difficultyTextDescription;
    private string[] diffDescription = new string[]
    {
        "- Conquer the Tower as you know it",

        "- 8% increased chance for high rarity drops\n" +
        "- Enemies have increased element status chance\n" +
        "- Enemies gain 2 bonus Dominance\n" +
        "- Metal Scraps pick ups will upgrade your Attack ",

        "- 28% increased chance for high rarity drops\n" +
        "- Enemies have increased element status chance\n" +
        "- Enemies gain 3 bonus Dominance\n" +
        "- Enemies have 1 Random Trait\n" +
        "- Metal Scraps pick ups will upgrade your Attack\n" +
        "- Grimoire Page pick ups will upgrade a random Skill\n" +
        "- Gems pick ups give a random Shop Consumable",

        "- 45% increased chance for high rarity drops\n" +
        "- Enemies have increased element status chance\n" +
        "- Enemies have increased element resistance\n" +
        "- Enemies gain 4 bonus Dominance\n" +
        "- Enemies have 1 Random Trait\n" +
        "- Metal Scraps pick ups will upgrade your Attack\n" +
        "- Grimoire Page pick ups will upgrade a random Skill\n" +
        "- Gems pick ups give a random Shop Consumable\n" +
        "- Ether Vial pick ups increase Max Health by 15\n" +
        "- Enemies gain +50% Max HP\n" +
        "- Ether Vial, Grimoire Page, Metal Scraps, and Gems can give Ailments during run\n" +
        "- Ailment removal is available at Shop\n" +
        "- Ether Shards can remove Ailments",

        "- 45% increased chance for high rarity drops\n" +
        "- Enemies have increased element status chance\n" +
        "- Enemies have increased element resistance\n" +
        "- Enemies gain 5 bonus Dominance\n" +
        "- Enemies have 2 Random Trait\n" +
        "- Metal Scraps pick ups will upgrade your Attack\n" +
        "- Grimoire Page pick ups will upgrade a random Skill\n" +
        "- Gems pick ups give a random Shop Consumable\n" +
        "- Ether Vial pick ups increase Max Health by 15\n" +
        "- Enemies gain +50% Max HP\n" +
        "- Ether Vial, Grimoire Page, Metal Scraps, and Gems can give Ailments during run\n" +
        "- Ailment removal is available at Shop\n" +
        "- Ether Shards can no longer remove Ailments\n" +
        "- Enemies gain 3 bonus Toughness\n" +
        "- Shop is more expensive"
    };



    // Start is called before the first frame update
    void Start()
    {
        Destroy(GameObject.Find("Non-Destructible Collections"));

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        SetDifficulty(0);

    }

    // Update is called once per frame
    void Update()
    {
        difficultyTextName.text = "Current Difficulty: " + GameUtilities.difficultyNames[(int)player.chosenDifficulty]; 
        difficultyTextNumber.text = player.chosenDifficulty + "";
        difficultyTextDescription.text = diffDescription[player.chosenDifficulty - 1];

        edenPng.SetActive(player.GetSuccessfulAttempts() > 0 && !player.savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed));

    }

    public void SetDifficulty(int diff)
    {
        if (player.GetSuccessfulAttempts() < 1)
        {
            decreaseArrow.SetActive(false);
            increaseArrow.SetActive(false);
            difficultyTextNumber.gameObject.SetActive(false);
        }
        else
        {
            decreaseArrow.SetActive(true);
            increaseArrow.SetActive(player.chosenDifficulty < player.highestDifficultyWon);

            player.chosenDifficulty = Mathf.Clamp(player.chosenDifficulty + diff, 1, 5);

            increaseArrow.SetActive(player.chosenDifficulty < player.highestDifficultyWon + 1);
            if (player.chosenDifficulty == 1)
            {
                //hide bottom arrow
                decreaseArrow.SetActive(false);
            }

            if (player.chosenDifficulty > 4)
            {
                //hide top arrow
                increaseArrow.SetActive(false);
            }
        }
    }

    public void OpenWindow()
    {
        AudioManager.instance.PlaySfxSound("Button_Click");
        if (player.wentToTowerThisRun)
        {
            ShowWindow();
        } else
        {
            player.wentToTowerThisRun = true;
            if (friendlyToken.TriggerVisitDialogue())
            {
                StartCoroutine(GameUtilities.WaitForConversation(() => ShowWindow()));
            }
            else
            {
                ShowWindow();
            }
        }

    }

    public void ShowWindow()
    {
        //play open animation
        if(player.GetSuccessfulAttempts() < 1)
        {
            EnterTower();
        } else
        {
            windowAnimator.Play("Tower Open");
        }
        
    }

    public void EnterTower()
    {

        //Stop current anims
        GameUtilities.DeleteAllChildGameObject(GameObject.Find("Popup window"));

        player.attempts.Add(new Attempts());
        GameObject.Find("Academy POI").GetComponent<AcademyHub>().ApplyBonus();
        GameObject.Find("Church POI").GetComponent<ChurchHub>().ApplyChurchBoons();
        player.SetInTower();
        GameController.instance.LoadGame(Floors.Floor_01);
    }

    public void CloseWindow()
    {
        //play close animation 
        windowAnimator.Play("Tower Close");

    }

}