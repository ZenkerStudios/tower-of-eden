using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSystem : MonoBehaviour
{
    private readonly int baseNumRewards = 3;
    public GameObject pfAbilityToLevelUp;
    public GameObject pfAbilityToSelect;
    public GameObject pfArtifactToSelect;
    public GameObject rewardPanel;

    public GameObject replacementWindow;
    public GameObject pfAbilityToReplace;
    public GameObject abilityReplacePanel;

    public GameObject pfRewardResult;
    public GameObject resultPanel;

    public TextMeshProUGUI title;
    public TextMeshProUGUI subtitle;

    public GameObject skipButton;
    public GameObject nextButton;
    public List<GameObject> floorStatues;
    public Image rewardIcon;

    private PlayerManager player;
    public List<IAbility> abilitiesOwned = new List<IAbility>();

    public int gold;
    private int gem;
    private int grimoire;
    private int scraps;
    private int shard;
    private int vial;
    private int ailmentChance;

    private bool foundNote = false;
    public bool isElite = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

        foundNote = player.savedPastConvos.Contains("ACRID_011_04")
            && player.savedPastConvos.Contains("TOWER_006_07")
            && GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode == Floors.Floor_02
          && !player.savedDialogueConditionsMet.Contains(DialogueConditions.AcridFirstNote)
          && (UnityEngine.Random.Range(0, 4) < 1);

        abilitiesOwned.Add(player.h.baseAttack);
        abilitiesOwned.AddRange(player.h.specials);
        rewardPanel.SetActive(false);
        replacementWindow.SetActive(false);
        skipButton.SetActive(true);
        GameUtilities.DeleteAllChildGameObject(rewardPanel);
        GameUtilities.DeleteAllChildGameObject(abilityReplacePanel);
        foreach(GameObject go in floorStatues)
        {
            go.SetActive(false);
        }

        //Structure
        transform.GetChild(1).gameObject.SetActive(false);
        //Selection Window
        transform.GetChild(2).gameObject.SetActive(false);
    }

    private void Start()
    {
        AudioManager.instance.PlaySfxSound("Reward");
        AudioManager.instance.LowerMusicVolume(0.2f);

    }

    private void Update()
    {

    }
   
    private GameObject resultItem;
    private void RenderResults(Reward r, int amount, bool nextEncounter, bool noReward)
    {
        GameUtilities.DeleteAllChildGameObject(resultPanel);

        //Exit Health Result
        resultItem = Instantiate(pfRewardResult);
        resultItem.transform.SetParent(resultPanel.transform);
        resultItem.transform.localPosition = new Vector2(0, 0);
        resultItem.GetComponent<RectTransform>().sizeDelta = pfRewardResult.GetComponent<RectTransform>().sizeDelta;
        resultItem.GetComponent<RectTransform>().localScale = pfRewardResult.GetComponent<RectTransform>().localScale;
        int val = player.healthOnExit;
        if (isElite) val *= 3;
        if (player.h.playerInventory.halfHealthEnc)
        {
            val /= 2;
        }
        resultItem.GetComponent<RewardResult>().rewardText.text =  "<sprite=" + 56 + "> " + val + " HP";

        //Exit Gold Result
        resultItem = Instantiate(pfRewardResult);
        resultItem.transform.SetParent(resultPanel.transform);
        resultItem.transform.localPosition = new Vector2(0, 0);
        resultItem.GetComponent<RectTransform>().sizeDelta = pfRewardResult.GetComponent<RectTransform>().sizeDelta;
        resultItem.GetComponent<RectTransform>().localScale = pfRewardResult.GetComponent<RectTransform>().localScale;
        int val2 = player.goldOnExit;
        if (isElite) val2 *= 3;
        if (player.h.playerInventory.halfGoldEnc)
        {
            val2 /= 2;
        }
        if (player.h.playerInventory.noGoldFloor == GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode)
        {
            val2 = 0;
        }
        resultItem.GetComponent<RewardResult>().rewardText.text = "<sprite=" + 57 + "> " + val2 + " Gold";

        //Door reward Result
        resultItem = Instantiate(pfRewardResult);
        resultItem.transform.SetParent(resultPanel.transform);
        resultItem.transform.localPosition = new Vector2(0, 0);
        resultItem.GetComponent<RectTransform>().sizeDelta = pfRewardResult.GetComponent<RectTransform>().sizeDelta;
        resultItem.GetComponent<RectTransform>().localScale = pfRewardResult.GetComponent<RectTransform>().localScale;

        if(noReward)
        {
            if (player.h.playerInventory.noGoldFloor == GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode)
            {
                amount = 0;
            }
            resultItem.GetComponent<RewardResult>().rewardText.text = "<sprite=" + 11 + "> " + amount + " Gold";
        } else
        {
            resultItem.GetComponent<RewardResult>().rewardText.text = "<sprite=" + r.imgIndex + "> " + amount + " " + r.GetDesc(r.reward)[0];
        }

        if (foundNote)
        {
            GameController.instance.DisplayRewardNotif("Found Acrid's notes!");
            player.savedDialogueConditionsMet.Add(DialogueConditions.AcridFirstNote);
            resultItem = Instantiate(pfRewardResult);
            resultItem.transform.SetParent(resultPanel.transform);
            resultItem.transform.localPosition = new Vector2(0, 0);
            resultItem.GetComponent<RectTransform>().sizeDelta = pfRewardResult.GetComponent<RectTransform>().sizeDelta;
            resultItem.GetComponent<RectTransform>().localScale = pfRewardResult.GetComponent<RectTransform>().localScale;
            resultItem.GetComponent<RewardResult>().rewardText.text = "<sprite=64> Acrid's notes!";
        }

        resultPanel.SetActive(true);
        nextButton.SetActive(true); 
        skipButton.SetActive(false);


        if (nextEncounter)
        {
            nextButton.SetActive(false);
            skipButton.SetActive(true);
        }
    }

    public void GetRandomReward(Difficulty diff, Reward reward, Floors floors, bool elite)
    {
        isElite = elite;
        floorStatues[((int)floors - 2)].SetActive(true);

        gold = 30 * (int)diff;
        gem = 30 * (int)diff;
        grimoire = 2 * (int)diff;
        scraps = 35 * (int)diff;
        shard = 1;
        vial = 1;
        if(isElite)
        {
            gold *= 2;
            gem += 25;
            grimoire += 1;
            scraps += 30;
            shard *= 2;
            vial *= 2;
        }
        title.text = reward.GetDesc(reward.reward)[0];
        subtitle.text = reward.GetDesc(reward.reward)[1];
        rewardIcon.sprite = reward.highlightImg;

        if (player.h.playerInventory.noGoldFloor == GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode)
        {
            gold = 0;
        }
        switch (reward.reward)
        {
            case RewardType.Ability:
                //Get 3 abilities to choose from
                RenderResults(reward, 1, false, false);
                List<Special> abilitiesToSelect = new List<Special>();
                abilitiesToSelect.AddRange(GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().allSpecialsToUse);
                if (abilitiesToSelect.Count <= 0)
                {
                    GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().ResetCollection();
                    abilitiesToSelect.AddRange(GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().allSpecialsToUse);
                }
                abilitiesToSelect = abilitiesToSelect.OrderBy(a => Guid.NewGuid()).ToList();
                rewardPanel.SetActive(true);
                int abilityCount = 0;
                foreach (Special a in abilitiesToSelect)
                {
                    if (abilityCount >= baseNumRewards)
                    {
                        break;
                    }
                    bool owns = HasAbility(a);
                    Rarity r = GameController.GetAbilityRarity(diff, player.h.playerInventory.GetRarityBonus());
                    if (isElite) r = (Rarity)Mathf.Clamp((int)r + 1,2, 6);
                    a.SetRarity(r);
                    var newObj = Instantiate(pfAbilityToSelect);
                    newObj.transform.SetParent(rewardPanel.transform);
                    newObj.transform.localPosition = new Vector2(0, 0);
                    newObj.GetComponent<RectTransform>().sizeDelta = pfAbilityToSelect.GetComponent<RectTransform>().sizeDelta;
                    newObj.GetComponent<RectTransform>().localScale = pfAbilityToSelect.GetComponent<RectTransform>().localScale;
                    newObj.GetComponent<AddAbility>().newAbility = a;
                    newObj.GetComponent<AddAbility>().rewardSystem = this;
                    newObj.GetComponent<AddAbility>().owned = owns;

                    abilityCount++;
                }
                if (player.h.playerInventory.ailments.Exists(a => a.conIndex == 3))
                {
                    player.h.playerInventory.ChangeGoldAmount(gold);
                    resultItem.GetComponent<RewardResult>().rewardText.text = "<sprite=" + 11 + "> " + gold + " Gold";
                    RenderResults(reward, gold, true, true);
                }
                break;
            case RewardType.Artifact:
                //Get 3 artifacts to choose from
                RenderResults(reward, 1, false, false);
                try
                {

                    int bonus = player.h.playerInventory.GetRarityBonus();
                    if (isElite) bonus += 10;
                    List<Artifact> artifactToSelect = GameController.GetArtifacts(bonus, baseNumRewards, floors);
                    rewardPanel.SetActive(true);
                    foreach (Artifact a in artifactToSelect)
                    {
                        var newObj = Instantiate(pfArtifactToSelect);
                        newObj.transform.SetParent(rewardPanel.transform);
                        newObj.transform.localPosition = new Vector2(0, 0);
                        newObj.GetComponent<RectTransform>().sizeDelta = pfArtifactToSelect.GetComponent<RectTransform>().sizeDelta;
                        newObj.GetComponent<RectTransform>().localScale = pfArtifactToSelect.GetComponent<RectTransform>().localScale;
                        newObj.GetComponent<AddArtifact>().artifact = a;
                        newObj.GetComponent<AddArtifact>().rewardSystem = this;
                    }
                }
                catch (Exception)
                {
                    player.h.playerInventory.ChangeGoldAmount(gold);
                    resultItem.GetComponent<RewardResult>().rewardText.text = "<sprite=" + 11 + "> " + gold + " Gold";
                    RenderResults(reward, gold, true, true);
                }
                break;
            case RewardType.Gold:
                player.h.playerInventory.ChangeGoldAmount(gold);
                RenderResults(reward, gold, true, false);
                break;
            case RewardType.GrimoirePage:
                player.ChangeGrimoirePageAmount(grimoire);
                RenderResults(reward, grimoire, true, false);
                ailmentChance = 15 + player.h.playerInventory.GetAilmentBonus() + player.h.playerInventory.ailmentChanceMod;
                //If diff 3 or higher also level up random skill
                if (player.chosenDifficulty > 2)
                {
                    List<Special> sList = new List<Special>(player.h.specials);
                    Special s = sList.OrderBy(a => Guid.NewGuid()).ToList()[0];
                    s.LevelUp();
                    List<System.Action> popups = new List<System.Action>();
                    popups.Add(() => s.TriggerNoHit(player.popupWindow.transform, s.GetName() + ": +1 Rank!", 12));
                    GameUtilities.ShowLevelUpPopup(player, popups);
                }
                break;
            case RewardType.Gemstones:
                player.ChangeGemstonesAmount(gem);
                RenderResults(reward, gem, true, false);
                ailmentChance = 15 + player.h.playerInventory.GetAilmentBonus() + player.h.playerInventory.ailmentChanceMod;
                //If diff 3 or higher also give random consumable
                if (player.chosenDifficulty > 2)
                {
                    Consumable.GiveRandomCombatConsumable(1, player);
                }

                foreach (Ailment ail in player.h.playerInventory.ailments)
                {
                    ail.gemsCollected += gem;
                }
                break;
            case RewardType.MetalScraps:
                player.ChangeScrapsAmount(scraps);
                RenderResults(reward, scraps, true, false);
                ailmentChance = 15 + player.h.playerInventory.GetAilmentBonus() + player.h.playerInventory.ailmentChanceMod;
                //If diff 2 or higher also level attack
                if (player.chosenDifficulty > 1)
                {
                    player.h.baseAttack.LevelUp();
                    List<System.Action> popups = new List<System.Action>();
                    popups.Add(() => player.h.baseAttack.TriggerNoHit(player.popupWindow.transform, player.h.baseAttack.GetName() + ": +1 Rank!", 12));
                    GameUtilities.ShowLevelUpPopup(player, popups);
                }
                foreach (Ailment ail in player.h.playerInventory.ailments)
                {
                    ail.scrapsCollected += scraps;
                }
                break;
            case RewardType.EtherShard:
                player.ChangeShardAmount(shard);
                RenderResults(reward, shard, true, false);

                //If diff 4 cure ailment
                if (player.chosenDifficulty == 4)   
                {
                    player.h.playerInventory.RemoveAilment();
                    player.RenderAilment();
                }
                else if (player.chosenDifficulty > 4)
                {
                    ailmentChance = 15 + player.h.playerInventory.GetAilmentBonus() + player.h.playerInventory.ailmentChanceMod;
                }

                foreach (Ailment ail in player.h.playerInventory.ailments)
                {
                    ail.shardCollected += shard;
                }
                break;
            case RewardType.EtherVial:
                player.ChangeVialsAmount(vial);
                RenderResults(reward, vial, true, false);
                ailmentChance = 15 + player.h.playerInventory.GetAilmentBonus() + player.h.playerInventory.ailmentChanceMod;
                foreach (Ailment ail in player.h.playerInventory.ailments)
                {
                    ail.vialCollected += vial;
                }
                //If diff 4 or higher also give 15 max health
                if (player.chosenDifficulty > 3)
                {
                    player.h.IncreaseMaxHealth(15);
                }
                break;
            case RewardType.MaxHealth: //Health infusion
                int infuse = player.healthPerInfusion;
                if (isElite) infuse += 25;
                if (player.h.playerInventory.ailments.Exists(a => a.conIndex == 1))
                {
                    infuse /= 2;
                }
                player.h.IncreaseMaxHealth(infuse);
                RenderResults(reward, 1, true, false);
                break;
            case RewardType.Powerup:
                RenderResults(reward, 1, false, false);
                rewardPanel.SetActive(true);
                abilitiesOwned = abilitiesOwned.OrderBy(a => Guid.NewGuid()).ToList();
                //Display first 3 index to level up
                int toLevelCount = 0;
                foreach (IAbility a in abilitiesOwned)
                {
                    bool shouldLevelUp = a.CanLevelUp();
                    if(isElite && a is Attack att)
                    {
                        shouldLevelUp = att.canLevelup;
                    } else if (isElite && a is Special sp)
                    {
                        shouldLevelUp = sp.canLevelup;
                    }

                    if (toLevelCount >= baseNumRewards)
                    {
                        break;
                    }
                    if (shouldLevelUp)
                    {
                        var newObj = Instantiate(pfAbilityToLevelUp);
                        newObj.transform.SetParent(rewardPanel.transform);
                        newObj.transform.localPosition = new Vector2(0, 0);
                        newObj.GetComponent<RectTransform>().sizeDelta = pfAbilityToLevelUp.GetComponent<RectTransform>().sizeDelta;
                        newObj.GetComponent<RectTransform>().localScale = pfAbilityToLevelUp.GetComponent<RectTransform>().localScale;
                        newObj.GetComponent<LevelUpAbility>().ability = a;
                        newObj.GetComponent<LevelUpAbility>().rewardSystem = this;
                        toLevelCount++;
                    }
                }
                if (player.h.playerInventory.ailments.Exists(a => a.conIndex == 4))
                {
                    player.h.playerInventory.ChangeGoldAmount(gold);
                    resultItem.GetComponent<RewardResult>().rewardText.text = "<sprite=" + 11 + "> " + gold + " Gold";
                    RenderResults(reward, gold, true, true);
                }
                break;
        }
    }

    public void Skip()
    {
        GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().ResetArtifactsNotSelected();
        ReturnToEncounterSelect();
    }

    private bool HasAbility(IAbility ability)
    {
        foreach (IAbility a in abilitiesOwned)
        {
            if (a.GetName().Equals(ability.GetName()))
            {
                return true;
            }
        }
        return false;
    }

    public void Continue()
    {
        resultPanel.SetActive(false);
        nextButton.SetActive(false);
        skipButton.SetActive(true);

    }

    public void ReturnToEncounterSelect()
    {
        StartCoroutine(EncounterAnim());
    }

    private IEnumerator EncounterAnim()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(5).gameObject.SetActive(false);

        FloorManager fm = GameObject.Find("Floor Manager").GetComponent<FloorManager>();
        fm.transitionAnimator.Play("Back to Enc");
        AudioManager.instance.PlaySfxSound("Transition_whoosh");

        yield return new WaitForSeconds(1f);
        fm.ExitRoom();
        if (player.chosenDifficulty > 3 && UnityEngine.Random.Range(1, 101) <= (ailmentChance - player.h.playerInventory.ailmentArtifactRedux))
        {
            player.AddAilment();
            fm.floorEventAnimator.Play("Ailment");

        }
        player.RenderLoadout();
        if (fm.IsBossBattle())
        {
            fm.BossDefeated();
        }
        if (GameObject.FindGameObjectWithTag("CombatSystem"))
        {
            Destroy(GameObject.FindGameObjectWithTag("CombatSystem"));
        }
        if (GameObject.FindGameObjectWithTag("UniqueRoom"))
        {
            Destroy(GameObject.FindGameObjectWithTag("UniqueRoom"));
        }
        fm.encountersWon++;
        player.revengeanceTrait--;
        fm.NextRoom();
    }

}


[Serializable]
public class Reward
{
    public Sprite highlightImg;
    public int highlightImgIndex;
    public Sprite img;
    public int imgIndex;
    public RewardType reward;

    public static string[] descs = new string[]
    {
        "Select one of 3 Skills to unlock.",
        "Select one of 3 artifacts.",
        "Currency to be used at the Shop in the tower.",
        "Currency to be used at the Shop in the town.",
        "Material used at the Academy to upgrade your grimoire.",
        "Materials used for the Blacksmith to upgrade weapons.",
        "Special gift to improve relationship with characters.",
        "Gift to improve relationship with characters.",
        "Increase maximum health.",
        "Increase mastery of an ability.",
        "Buy Skills or artifacts."
    };

    public static string[] names = new string[]
   {
        "Grail of Power",
        "Fabricator",
        "Gold",
        "Gemstones",
        "Grimoire Page",
        "Metal Scraps",
        "Ether Shard",
        "Ether Vial",
        "Health Infusion",
        "Mastery Rune",
        "Shop"
   };

    public Reward(Sprite[] s, RewardType r)
    {
        img = s[0];
        highlightImg = s[1];
        reward = r;
        GetDesc(r);
    }

    public List<string> GetDesc(RewardType r)
    {
        switch (r)
        {
            case RewardType.Ability:
                highlightImgIndex = 2;
                imgIndex = 3;
                return new List<string>() { names[0], descs[0] };
            case RewardType.Artifact:
                highlightImgIndex = 0;
                imgIndex = 1;
                return new List<string>() { names[1], descs[1] };
            case RewardType.Gold:
                highlightImgIndex = 10;
                imgIndex = 11;
                return new List<string>() { names[2], descs[2] };
            case RewardType.Gemstones:
                highlightImgIndex = 8;
                imgIndex = 9;
                return new List<string>() { names[3], descs[3] };
            case RewardType.GrimoirePage:
                highlightImgIndex = 12;
                imgIndex = 13;
                return new List<string>() { names[4], descs[4] };
            case RewardType.MetalScraps:
                highlightImgIndex = 18;
                imgIndex = 19;
                return new List<string>() { names[5], descs[5] };
            case RewardType.EtherShard:
                highlightImgIndex = 4;
                imgIndex = 5;
                return new List<string>() { names[6], descs[6] };
            case RewardType.EtherVial:
                highlightImgIndex = 6;
                imgIndex = 7;
                return new List<string>() { names[7], descs[7] };
            case RewardType.MaxHealth:
                highlightImgIndex = 14;
                imgIndex = 15;
                return new List<string>() { names[8], descs[8] };
            case RewardType.Powerup:
                highlightImgIndex = 16;
                imgIndex = 17;
                return new List<string>() { names[9], descs[9] };
            case RewardType.Shop:
                highlightImgIndex = 22;
                imgIndex = 23;
                return new List<string>() { names[10], descs[10] };
        }
        return new List<string>() { "", "" };
    }
}