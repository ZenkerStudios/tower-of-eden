    using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public List<Sprite> bgs = new List<Sprite>();
    public PlayerManager player;

    public GameObject pfAbilityToReplace;
    public GameObject abilityToReplacePanel;
    public List<Special> offeredAbilities = new List<Special>();

    public GameObject pfAbility;
    public GameObject pfArtifact;


    public GameObject additionalOption;
    public GameObject healthSprite;
    public GameObject pageSprite;
    public GameObject gemSprite;
    public GameObject abilityRemoval;
    public GameObject abilityToRemovePanel;
    public TextMeshProUGUI additionalOptionCostText;
    public TextMeshProUGUI additionalOptionNameText;
    public TextMeshProUGUI ailmentRemovalCostText;
    private int Cost = 25;
    private int ailmentRemovalCost = 75;
    public int baseHeal = 25;
    private RewardType reward;
    public TextMeshProUGUI descTextBox;
    public GameObject scrollPanel;


    public Difficulty floorDifficulty;
   
    private int abilityPrice = 5;
    private int artifactPrice = 8;

    private int baseNumRewards = 3;
    private FloorManager fm;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI gem;
    public TextMeshProUGUI vials;
    public TextMeshProUGUI shards;
    public TextMeshProUGUI scraps;
    public TextMeshProUGUI grimoire;

    public Scrollbar scrollbar;


    private void Awake()
    {
        fm = GameObject.Find("Floor Manager").GetComponent<FloorManager>();
        transform.GetChild(0).GetComponent<Image>().sprite = bgs[(int)fm.floorCode - 2];
        AudioManager.instance.LowerMusicVolume(0);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        abilityToReplacePanel.transform.parent.gameObject.SetActive(false);
        abilityToRemovePanel.transform.parent.gameObject.SetActive(false);
        additionalOption.SetActive(false);
        player.combatShopInteraction++;

        if(player.chosenDifficulty < 4)
        {
            ailmentRemovalCostText.transform.parent.gameObject.SetActive(false);
        }

        if (player.chosenDifficulty > 4)
        {
            Cost += 10;
            abilityPrice += 2;
            artifactPrice += 1;
        }

        if (player.h.playerInventory.ailments.Exists(a => a.conIndex == 9))
        {
            Cost += 15;
            abilityPrice += 3;
            artifactPrice += 2;
        }

        if (player.h.playerInventory.ailments.Count <= 0)
        {
            ailmentRemovalCostText.transform.parent.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        ailmentRemovalCostText.transform.parent.GetChild(2).GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        if (player.h.playerInventory.ailments.Count <= 0)
        {
            ailmentRemovalCostText.transform.parent.GetChild(2).GetComponent<Image>().color = new Color32(175, 175, 175, 255);

        }

        additionalOptionCostText.color = new Color32(255, 255, 255, 255);
        if (player.h.playerInventory.GetGoldAmount() < Cost)
        {
            additionalOptionCostText.color = new Color32(255, 0, 0, 255);
        }

        ailmentRemovalCostText.color = new Color32(255, 255, 255, 255);
        ailmentRemovalCostText.text = "<sprite=11> " + ailmentRemovalCost;
        if (player.h.playerInventory.GetGoldAmount() < ailmentRemovalCost)
        {
            ailmentRemovalCostText.color = new Color32(255, 0, 0, 255);
        }

          
        hpText.text = "<sprite=45> " + player.h.hp + "/" + player.h.GetMaxHealth();
        gold.text = "<sprite=11> " + player.h.playerInventory.GetGoldAmount();

        gem.text = "<sprite=9> " + player.gemstonesOwned;
        vials.text = "<sprite=7> " + player.etherVialsOwned;
        shards.text = "<sprite=5> " + player.etherShardsOwned;
        scraps.text = "<sprite=19> " + player.metalScrapsOwned;
        grimoire.text = "<sprite=13> " + player.grimoirePagesOwned;
    }

    private string additionalOptionDesc;

    // Start is called before the first frame update
    IEnumerator Start()
    {

        //offer health
        int rand = UnityEngine.Random.Range(0, 100);
        if (rand <= 40)
        {
            additionalOptionCostText.text = "<sprite=11>" + Cost;
            additionalOptionNameText.text = "Health";
            additionalOptionDesc = "Heal " + (baseHeal + (7 * (int)floorDifficulty)) + " HP.";
            reward = RewardType.MaxHealth;
            healthSprite.SetActive(true);
            gemSprite.SetActive(false);
            pageSprite.SetActive(false);
            abilityRemoval.SetActive(false);
        } else if(rand > 40 && rand <= 70)
        {
            additionalOptionCostText.text = "<sprite=11>" + Cost;
            additionalOptionNameText.text = "Gemstones";
            additionalOptionDesc = "Trade for " + 20 + " Gemstones.";
            reward = RewardType.Gemstones;
            healthSprite.SetActive(false);
            gemSprite.SetActive(true);
            pageSprite.SetActive(false);
            abilityRemoval.SetActive(false);
        }
        else if (rand > 70 && rand <= 90)
        {
            additionalOptionCostText.text = "<sprite=11>" + Cost;
            additionalOptionNameText.text = "Grimoire Page";
            additionalOptionDesc = "Buy " + 1 + " Grimoire page.";
            reward = RewardType.GrimoirePage;
            healthSprite.SetActive(false);
            gemSprite.SetActive(false);
            pageSprite.SetActive(true);
            abilityRemoval.SetActive(false);
        }
        else
        {
            Cost = 0;
            additionalOptionCostText.text = "Free";
            additionalOptionNameText.text = "Spiritual Cleansing";
            additionalOptionDesc = "Purge abilities for <sprite=11> Gold";
            reward = RewardType.Gold;
            healthSprite.SetActive(false);
            gemSprite.SetActive(false);
            pageSprite.SetActive(false);
            abilityRemoval.SetActive(true);
        }

        additionalOption.SetActive(true);
        //Get abilities to choose from
        List<Special> abilitiesToSelect = new List<Special>();
        abilitiesToSelect.AddRange(GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().allSpecials);
        abilitiesToSelect = abilitiesToSelect.OrderBy(a => Guid.NewGuid()).ToList();
        int abilityCount = 0;

        //Get artifacts to choose from
        try
        {
            List<Artifact> artifactToSelect = GameController.GetArtifacts(player.h.playerInventory.GetRarityBonus(), baseNumRewards, fm.floorCode);
            foreach (Artifact a in artifactToSelect)
            {
                var newObj = GameObject.Instantiate(pfArtifact);
                newObj.transform.SetParent(scrollPanel.transform);
                newObj.transform.localPosition = new Vector2(0, 0);
                newObj.GetComponent<RectTransform>().sizeDelta = pfArtifact.GetComponent<RectTransform>().sizeDelta;
                newObj.GetComponent<RectTransform>().localScale = pfArtifact.GetComponent<RectTransform>().localScale;
                ArtifactShopItem item = newObj.GetComponent<ArtifactShopItem>();
                item.cost = artifactPrice * 5 * (int)a.rarity;
                item.artifact = a;
                item.shop = this;
            }
        }
        catch (Exception)
        {
        }


        foreach (Special ab in abilitiesToSelect)
        {
            Special a = Instantiate(ab);
            if (abilityCount >= baseNumRewards)
            {
                break;
            }

            offeredAbilities.Add(a);
            Rarity r = GameController.GetAbilityRarity(floorDifficulty, player.h.playerInventory.GetRarityBonus());
            a.SetRarity(r);
            var newObj = GameObject.Instantiate(pfAbility);
            newObj.transform.SetParent(scrollPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfAbility.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfAbility.GetComponent<RectTransform>().localScale;
            AbilityShopItem item = newObj.GetComponent<AbilityShopItem>();
            item.shop = this;
            item.newAbility = a;
            item.cost = abilityPrice * 5 * (int)r;
            if (HasAbility(a))
            {
                if(a.canLevelup)
                {
                    newObj.GetComponent<AbilityShopItem>().owned = true;
                }
                else
                {
                    offeredAbilities.Remove(a);
                    abilityCount--;
                    Destroy(newObj);
                }
            }


            abilityCount++;
        }

        yield return new WaitForSeconds(.5f); //Delay for options to be generated before scrolling to top of list
        scrollbar.value = 1;

        yield return new WaitForSeconds(1.5f);

    }

    public void ShowAdditionalDesc()
    {
        descTextBox.text = additionalOptionDesc;
    }
    private bool HasAbility(IAbility ability)
    {
        foreach (IAbility a in player.h.specials)
        {
            if (a.GetName().Equals(ability.GetName()))
            {
                return true;
            }
        }
        return false;
    }

    public void BuyAdditionalOption()
    {
        if (player.h.playerInventory.GetGoldAmount() >= Cost)
        {
            player.h.playerInventory.ChangeGoldAmount(-Cost);

            switch(reward)
            {
                case RewardType.MaxHealth:
                    player.h.ChangeHealth(baseHeal + (7 * (int)floorDifficulty));
                    additionalOption.SetActive(false);
                    break;
                case RewardType.Gemstones:
                    player.ChangeGemstonesAmount(20);
                    additionalOption.SetActive(false);
                    break;
                case RewardType.GrimoirePage:
                    player.ChangeGrimoirePageAmount(1);
                    additionalOption.SetActive(false);
                    break;
                case RewardType.Gold:
                    SetToRemoveAbility();
                    break;
            }
        }
    }

    private void SetToRemoveAbility()
    {
        
        abilityToRemovePanel.transform.parent.gameObject.SetActive(true);
        player.h.OrderSpecials();

        for(int index = 0; index < player.h.specials.Count; index++)
        {
            Special s = player.h.specials[index];
            Transform abilityObject = abilityToRemovePanel.transform.GetChild(0).GetChild(0).GetChild(index);

            abilityObject.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = s.GetSprite();
            abilityObject.GetChild(1).GetComponent<TextMeshProUGUI>().text = s.GetName();
            abilityObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = s.GetDesc();
            abilityObject.GetChild(3).GetComponent<TextMeshProUGUI>().text = "<sprite=11> " + GetGoldRewardForAbility(s);
            abilityObject.gameObject.SetActive(true);
        }
    }

    private int GetGoldRewardForAbility(Special special)
    {
        int gold = 25 * (int)special.rarity;
        if(!special.canLevelup)
        {
            gold += 50;
        } else
        {
            gold += 10 * special.GetMasteryRankInt();
        }

        return gold;
    }

    public void RemoveThisAbility(int index)
    {
        player.h.OrderSpecials();
        Special s = player.h.specials[index];
        player.h.playerInventory.ChangeGoldAmount(GetGoldRewardForAbility(s));
        player.h.RemoveSpecial(s);
        AudioManager.instance.PlaySfxSound("Coin");
        abilityToRemovePanel.transform.parent.gameObject.SetActive(false);
        additionalOption.SetActive(false);
        player.RenderLoadout();
    }

    public void RemoveAilment()
    {
        if (player.h.playerInventory.GetGoldAmount() >= ailmentRemovalCost)
        {
            player.h.playerInventory.RemoveAilment();
            player.h.GetMaxHealth();
            player.h.ChangeHealth(0);
            player.RenderLoadout();
        }

        if (player.h.playerInventory.ailments.Count <= 0)
        {
            ailmentRemovalCostText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void LeaveShop()
    {
        //Destroy all instantiated items

        int len = offeredAbilities.Count;

        for(int x = 0; 0 < len;)
        {
            Destroy(offeredAbilities[x]);
            offeredAbilities.RemoveAt(x);
            len = offeredAbilities.Count;
        }

         GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().ResetArtifactsNotSelected();

        fm.ExitRoom();
        fm.encountersWon++;
        Destroy(gameObject);
        fm.NextRoom();
    }

    public void EndReplacement()
    {
        abilityToReplacePanel.transform.parent.gameObject.SetActive(false);
        GameUtilities.DeleteAllChildGameObject(abilityToReplacePanel.transform.GetChild(0).GetChild(0).gameObject);
    }

    public List<Special> GetAbilitiesOwned()
    {
        return player.h.specials;

    }
}
