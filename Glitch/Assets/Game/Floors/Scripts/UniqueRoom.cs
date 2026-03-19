using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UniqueRoom : MonoBehaviour
{
    public Image exArtIcon;
    public TextMeshProUGUI exArtDesc;
    public Animator windowAnimator;

    public Floors key;
    public Difficulty difficulty;
    public Reward rewardType;
    public GameObject rewardScreen;


    public Image roomImg;
    public Image fgRaycast;
    public Sprite floor1Sprite;
    public Sprite floor2Sprite;
    public Sprite floor3Sprite;
    public Sprite floor4Sprite;

    public TextMeshProUGUI desc;


    public GameObject roomPrompt;
    public GameObject approachButton;
    public GameObject ignoreButton;

    private PlayerManager player;
    private int gems = 100;
    private int gold = 200;
    private int heal = 75;
    private int vial = 1;
    private int page = 5;

    public GameObject buttonOptions;

    //Floor 1
    public GameObject goldButton;
    public GameObject rarityForAttackButton;
    public GameObject healButton;

    //Floor 2
    public GameObject levelByTwoButton;
    public GameObject rarityForThreeButton;
    public GameObject vialButton;

    //Floor 3
    public GameObject arcaneScrollButton;
    public GameObject levelForFiveButton;
    public GameObject replaceArtifactButton;

    //Floor 4
    public GameObject getReviveButton;
    public GameObject gemButton;

    private bool uleaConvo = false;
    private bool canObtainNote = false;

    private List<Artifact> artifactToSelect;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        GameObject.Find("Floor Manager").GetComponent<FloorManager>().isUniqueRoom = true;
        fgRaycast.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(player.h.playerInventory.healInUniqueRoom)
        {
            player.h.ChangeHealth(50);
        }

        roomPrompt.SetActive(true);
        buttonOptions.SetActive(false);
        goldButton.SetActive(false);
        rarityForAttackButton.SetActive(false);
        healButton.SetActive(false);
        levelByTwoButton.SetActive(false);
        rarityForThreeButton.SetActive(false);
        vialButton.SetActive(false);
        arcaneScrollButton.SetActive(false);
        levelForFiveButton.SetActive(false);
        replaceArtifactButton.SetActive(false);
        getReviveButton.SetActive(false);
        gemButton.SetActive(false);

        switch (key)
        {
            case Floors.Floor_01:
                roomImg.sprite = floor1Sprite;
                desc.text = "As you come across a small room, it becomes evident that it was once used as a makeshift campsite. " +
                    " However, it has been deserted for a long time now and only a few remnants remain." +
                    "\n" +
                    "\nYou see a pile of rubble in one corner and a trail of a luminous substance, casting a soft glow, leading away from it." +
                    "\nThe room has a desolate and forgotten atmosphere, " +
                    "but the trail of glowing substance sparks interest and leaves you wondering where it might lead.";
                goldButton.SetActive(true); //Search pile of rubble
                rarityForAttackButton.SetActive(true); //Use campfire
                healButton.SetActive(true); //Investigate substance trail
                break;
            case Floors.Floor_02:
                roomImg.sprite = floor2Sprite;
                desc.text = "Upon entering the laboratory room, you are struck by the feeling that it hasn't been used in some time. " +
                    "\nDust and cobwebs seem to have accumulated in the corners, and the equipment appears to have been untouched for quite a while." +
                    "\n" +
                    "\nSuddenly, your attention is drawn to a large vat in the center of the room, filled to the brim with a blue, viscous liquid." +
                    "\nThe surface of the liquid shimmers and reflects the dim light in the room, and you can't help but feel drawn to it, " +
                    "as if it's calling out to you.";
                levelByTwoButton.SetActive(true); //Touch your reflection
                rarityForThreeButton.SetActive(true); //Dip your weapon into it
                vialButton.SetActive(true); //Collect liquid
                break;
            case Floors.Floor_03:
                roomImg.sprite = floor3Sprite;
                desc.text = "As you enter the throne room, " +
                    "the humble braziers at the base of each of the six limestone columns cast a warm glow that illuminates the entire hall. " +
                    "\n" +
                    "\nYour eyes are drawn to a familiar figure slouching on an elegant throne beneath a towering portrait. " +
                    "A bird with iridescent green and yellow feathers gracefully alights on the arm of the throne, its feathers glimmering in the dim light." +
                    "\nThe shopkeeper makes you an offer, but at a price.";
                bool option1 = true;
                bool option2 = true;
                bool option3 = true;

                //If it is eden's first time seeing traveler
                if(player.GetSuccessfulAttempts() > 0 && !player.savedPastConvos.Contains("DORIAN_044_01"))
                {
                    DialogueManager.instance.ShowDialogue(
                           DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Fool, "DORIAN_044_01"));

                }
                if (player.h.specials.Count < 3)
                {
                    arcaneScrollButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "[Locked]: Need at least 3 Skills.";
                    arcaneScrollButton.GetComponent<Button>().interactable = false;
                    option1 = false;
                }
                if (player.h.playerInventory.GetGoldAmount() < 200)
                {
                    levelForFiveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "[Locked]: Need at least 200 Gold.";
                    levelForFiveButton.GetComponent<Button>().interactable = false;
                    option2 = false;
                }
                if(player.h.playerInventory.artifacts.Exists(a => a.rarity == Rarity.Exalted))
                {
                    replaceArtifactButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "[Locked]: Already own an Exalted Artifact.";
                    replaceArtifactButton.GetComponent<Button>().interactable = false;
                    option3 = false;
                }
                arcaneScrollButton.SetActive(true); //knowledge 
                levelForFiveButton.SetActive(true); //power
                replaceArtifactButton.SetActive(true); //treasure
                canObtainNote = player.savedDialogueConditionsMet.Contains(DialogueConditions.AcridFirstNote) && !player.savedDialogueConditionsMet.Contains(DialogueConditions.AcridSecondNote) && Random.Range(0, 3) < 1;

                if(canObtainNote)
                {
                    arcaneScrollButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += "   +  <sprite=64>";
                }

                artifactToSelect = GameObject.FindGameObjectWithTag("ArtifactDB").
                    GetComponent<ArtifactCollection>().tier5ArtifactsReadonly.OrderBy(a => System.Guid.NewGuid()).ToList();

                if (artifactToSelect == null)
                {
                    replaceArtifactButton.SetActive(false);
                    option3 = false;
                }
                if(!option1 && !option2 && !option3)
                {
                    Ignore();
                }
                break;
            case Floors.Floor_04:
                roomImg.sprite = floor4Sprite;
                desc.text = "As you step into the spacious and brightly lit room, " +
                    "your gaze falls upon an intricately adorned altar beneath an opulent baldachin." +
                    "\nA towering statue of the High Priestess stands watch over the altar, her left hand outstretched, " +
                    "bearing the marks of countless supplicants having grasped it. " +
                    "\nIt is clear that this altar holds great significance to the cultists " +
                    "and is frequently used for seeking the high priestess's divine favor.";
                getReviveButton.SetActive(true); //Obtain blessing
                gemButton.SetActive(true); //Desecrate the altar
                healButton.SetActive(true); //Rest 
                string text = "<color=#A2A1A1>[Rest] </color><color=#00FF5E>Heal 75 HP.</color>";
                healButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;

                FloorManager fm = GameObject.Find("Floor Manager").GetComponent<FloorManager>();
                EnemyNpc ulea = fm.floorSpecialEncounter[0].boss[0];
                uleaConvo = ulea.interactableNpc.InitiateDialogue() != null;
                if(uleaConvo)
                {
                    //Add star symbol to signify ulea interaction
                    healButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text.Replace("[Rest]", "[<sprite=60> Rest]");
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
       


    }

    public void GetGold()
    {
        fgRaycast.gameObject.SetActive(true);
        player.h.playerInventory.ChangeGoldAmount(gold);
        player.F101++;
        ShowDialogue(0);
        ReturnToEncounterSelect();
    }

    public void GetRarityForAttack()
    {
        fgRaycast.gameObject.SetActive(true);
        player.F102++;
        ShowDialogue(1);
        int r = (int)player.h.baseAttack.GetRarity();
        r = Mathf.Clamp(r + 1, 2, 6);
        player.h.baseAttack.SetRarity((Rarity)r);
        ReturnToEncounterSelect();
    }

    public void GetHeal()
    {
        fgRaycast.gameObject.SetActive(true);
        switch (key)
        {
            case Floors.Floor_01:
                player.F103++;
                ShowDialogue(2);
                player.h.ChangeHealth(heal);
                ReturnToEncounterSelect();
                break;
            case Floors.Floor_04:
                    
                FloorManager fm = GameObject.Find("Floor Manager").GetComponent<FloorManager>();
                if (player.savedDialogueConditionsMet.Contains(DialogueConditions.StarRevealed)
                    && !player.savedPastConvos.Contains("ULEA_009_11")
                    && uleaConvo)
                {
                    player.h.ChangeHealth(heal);
                    fm.encountersWon--;
                    player.playerManagerAnimator.Play("Fade In");
                    StartCoroutine(StartUleaEncounter(fm));
                }
                else
                {
                    player.h.ChangeHealth(heal);
                    ReturnToEncounterSelect();
                }
                break;
        }
    }

    private IEnumerator StartUleaEncounter(FloorManager fm)
    {
        yield return new WaitForSeconds(2f);
        //Show ulea BG for combat
        fm.battleBg.transform.GetChild(1).gameObject.SetActive(true);
        GameController.instance.StartEncounter(key, fm.floorSpecialEncounter[0].boss, rewardType, fm.floorSpecialEncounter[0].bossDiff, true);
        Destroy(GameObject.FindGameObjectWithTag("UniqueRoom"));
    }

    public void GetLevelByTwo()
    {
        fgRaycast.gameObject.SetActive(true);
        player.F201++;
        ShowDialogue(0);
        Attack a = player.h.baseAttack;
        if (a.CanLevelUp())
        {
            a.LevelUp();
            a.LevelUp();
            List<System.Action> popups = new List<System.Action>();
            popups.Add(() => a.TriggerNoHit(player.popupWindow.transform, a.GetName() + ": +1 Rank!", 12));
            popups.Add(() => a.TriggerNoHit(player.popupWindow.transform, a.GetName() + ": +1 Rank!", 12));
            GameUtilities.ShowLevelUpPopup(player, popups);
        }
        ReturnToEncounterSelect();
    }

    public void GetRarityForThree()
    {
        fgRaycast.gameObject.SetActive(true);
        player.F202++;
        ShowDialogue(1);
        List<IAbility> abilitiesOwned = new List<IAbility>();
        abilitiesOwned.AddRange(player.h.specials);
        abilitiesOwned = abilitiesOwned.OrderBy(a => System.Guid.NewGuid()).ToList();
        for (int x = 0; x < 3 && abilitiesOwned.Count > 2; x++)
        {
            IAbility a = abilitiesOwned[x];
            int r = (int)a.GetRarity();
            a.SetRarity((Rarity)(r + 1));
        }
        ReturnToEncounterSelect();
    }

    public void GetVial()
    {
        fgRaycast.gameObject.SetActive(true);
        player.F203++;
        ShowDialogue(2);
        player.ChangeVialsAmount(vial);
        ReturnToEncounterSelect();
    }

    public void GetArcaneScroll()
    {
        fgRaycast.gameObject.SetActive(true);
        player.F301++;
        ShowDialogue(0);
        List<IAbility> abilitiesOwned = new List<IAbility>();
        abilitiesOwned.AddRange(player.h.specials);
        abilitiesOwned = abilitiesOwned.OrderBy(a => System.Guid.NewGuid()).ToList();
        player.RemoveSpecial(player.h.specials[Random.Range(0, player.h.specials.Count)]);
        player.ChangeGrimoirePageAmount(page);
        if(canObtainNote)
        {
            List<System.Action> popups = new List<System.Action>();
            popups.Add(() => player.h.baseAttack.specialSystem.TriggerIconPopUpWithIndex(player.popupWindow.transform, true, 64));
            GameUtilities.ShowPopup(this, popups);
            GameController.instance.DisplayRewardNotif("Found Acrid's notes!");
            player.savedDialogueConditionsMet.Add(DialogueConditions.AcridSecondNote);
        }
        ReturnToEncounterSelect();
    }

    public void GetLevelForFive()
    {
        fgRaycast.gameObject.SetActive(true);
        player.F302++;
        ShowDialogue(1);
        player.h.playerInventory.ChangeGoldAmount(-gold);
        List<IAbility> abilitiesOwned = new List<IAbility>();
        abilitiesOwned.Add(player.h.baseAttack);
        abilitiesOwned.AddRange(player.h.specials);
        abilitiesOwned = abilitiesOwned.OrderBy(a => System.Guid.NewGuid()).ToList();
        for (int abilityToLevelUpCount = 0, abilitiesOwnedIndex = 0; abilityToLevelUpCount < 5 && abilitiesOwnedIndex < abilitiesOwned.Count; abilityToLevelUpCount++, abilitiesOwnedIndex++)
        {
            IAbility a = abilitiesOwned[abilitiesOwnedIndex];
            if(a.CanLevelUp())
            {
                a.LevelUp();
                List<System.Action> popups = new List<System.Action>();
                popups.Add(() => a.TriggerNoHit(player.popupWindow.transform, a.GetName() + ": +1 Rank!", 12));
                GameUtilities.ShowLevelUpPopup(player, popups);

            }
            else 
            {
                abilityToLevelUpCount--;
            }

        }      
        ReturnToEncounterSelect();
    }

    public void GetReplaceArtifact()
    {
        Artifact a = artifactToSelect[0];
        fgRaycast.gameObject.SetActive(true);
        player.F303++;
        ShowDialogue(2);
        player.AddArtifact(a);
        player.RenderLoadout();
        exArtIcon.sprite = a.GetSprite();
        exArtDesc.text = a.GetName() + "\n\n" + a.GetDesc();
        windowAnimator.gameObject.SetActive(true);
    }

    public void GetRevive()
    {
        fgRaycast.gameObject.SetActive(true);
        player.F401++;
        ShowDialogue(0);
        player.numRevives++;
        player.AddReviveIcon();
        ReturnToEncounterSelect();
    }

    public void GetGem()
    {
        fgRaycast.gameObject.SetActive(true);
        player.F402++;
        ShowDialogue(1);
        player.ChangeGemstonesAmount(gems);
        ReturnToEncounterSelect();
    }


    public Sprite uniqueRoomSprite;

    public void Approach()
    {
        buttonOptions.SetActive(true);
        roomPrompt.SetActive(false);
    }

    public void Ignore()
    {
        GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().ResetArtifactsNotSelected();
        GameObject.Find("Floor Manager").GetComponent<FloorManager>().encountersWon--;
        var window = GameObject.Instantiate(rewardScreen);
        window.transform.SetParent(transform);
        window.transform.localPosition = new Vector2(0, 0);
        window.GetComponent<RectTransform>().sizeDelta = rewardScreen.GetComponent<RectTransform>().sizeDelta;
        window.GetComponent<RectTransform>().localScale = rewardScreen.GetComponent<RectTransform>().localScale;
        window.GetComponent<RewardSystem>().GetRandomReward(difficulty, rewardType, key, false);
    }

    public void ReturnToEncounterSelect()
    {
        StartCoroutine(
               GameUtilities.WaitForConversation(() => StartCoroutine(EncounterAnim())));
    }

    private IEnumerator EncounterAnim()
    {
        GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().ResetArtifactsNotSelected();
        FloorManager fm = GameObject.Find("Floor Manager").GetComponent<FloorManager>();
        fm.transitionAnimator.Play("Back to Enc");
        player.RenderLoadout();
        yield return new WaitForSeconds(1f);
        fm.ExitRoom();
        Destroy(GameObject.FindGameObjectWithTag("UniqueRoom"));
        fm.NextRoom();

    }

    private void ShowDialogue(int val)
    {
        if (player.GetSuccessfulAttempts() < 2 || val > 2 || (Random.Range(0, 100) < 75)) return;

        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.CompendiumSealed)) return;

        switch (key)
        {
            case Floors.Floor_01:
                if (val == 0 && player.F101 > 1)
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Empress, "RAGNA_041_01"));
                }
                else if (val == 1 && player.F102 > 1)
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Empress, "RAGNA_042_01"));

                }
                else if (val == 2 && player.F103 > 1)
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Empress, "RAGNA_040_01"));
                }
                break;
            case Floors.Floor_02:
                if (val == 0 && player.F201 > 1 && player.F103 > 1)
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Magician, "ACRID_033_01"));
                }
                else if (val == 1 && player.F202 > 1)
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Magician, "ACRID_035_01"));
                }
                else if (val == 2 && player.F203 > 1 && player.academyAdmirationLevel > 5)
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Magician, "ACRID_034_01"));
                }
                break;
            case Floors.Floor_03:
                if (val == 0 && player.F301 > 1 && player.savedPastConvos.Contains("SHOP_003_05"))
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Fool, "DORIAN_042_01"));
                }
                else if (val == 1 && player.F302 > 1 && player.savedPastConvos.Contains("SHOP_003_05"))
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Fool, "DORIAN_041_01"));
                }
                else if (val == 2 && player.F303 > 1 && player.savedPastConvos.Contains("SHOP_003_05"))
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Fool, "DORIAN_040_01"));
                }
                break;
            case Floors.Floor_04:
                if (val == 0 && player.F401 > 1)
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Priestess, "LYRA_032_01"));
                }
                else if (val == 1 && player.F402 > 1)
                {
                    DialogueManager.instance.ShowDialogue(
                        DialogueManager.instance.GetDialogueBlockFor(InteractableNpcs.Priestess, "LYRA_033_01"));
                }
                break;
        }
        
    }
} 

