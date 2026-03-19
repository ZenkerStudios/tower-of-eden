using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableItem : MonoBehaviour
{
    public Consumable consumable;
    public TextMeshProUGUI consumableName;

    public Button itemButton;
    public Image shopItemSprite;
    public List<Sprite> consumableSprite;
    public TextMeshProUGUI costText;
    public int minIndex, maxIndex;
    public PlayerManager player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        itemButton.interactable = true;
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        costText.color = new Color32(255, 255, 255, 255);


        if (consumable.consumableIntValue > 4 && player.h.playerInventory.GetGoldAmount() < consumable.cost)
        {
            costText.color = new Color32(255, 0, 0, 255);

        }
        else if (consumable.consumableIntValue < 5 && player.gemstonesOwned < consumable.cost)
        {
            costText.color = new Color32(255, 0, 0, 255);

        }
    }

    public void Init()
    {
        consumable = new Consumable(minIndex, maxIndex);
        consumableName.text = consumable.GetName(0);
        int iconIndex = 11;
        if (minIndex >= 0 && maxIndex <= 5)
        {
            iconIndex = 9;
        }

        costText.text = "<sprite=" + iconIndex + "> " + consumable.cost;
        shopItemSprite.sprite = consumableSprite[consumable.consumableIntValue];

    }

    public void ShowDesc()
    {
        GameObject.Find("ShopDesc").GetComponent<TextMeshProUGUI>().text = consumable.GetDesc(0); 
    }

    private void EquipConsumable()
    {
        itemButton.interactable = false;
        Consumable toRemove = null;
        switch (consumable.consumable)
        {
            //City shop consumables
            case Consumables.ailmentRedux:
            case Consumables.hazardRedux:
            case Consumables.rarityIncrease:
            case Consumables.randomArtifact:
            case Consumables.randomSpecial:
                foreach (Consumable c in consumable.owner.cityShopConsumables)
                {
                    if (c.consumable == consumable.consumable)
                    {
                        toRemove = c;
                    }
                }
                consumable.owner.cityShopConsumables.Remove(toRemove);
                consumable.owner.ChangeGemstonesAmount(-consumable.cost);
                consumable.owner.cityShopConsumables.Add(consumable);
                break;

            //Combat shop consumables
            case Consumables.statusIncrease:
            case Consumables.accuracyIncrease:
            case Consumables.critIncrease:
            case Consumables.strengthIncrease:
            case Consumables.dominanceIncrease:
            case Consumables.toughnessIncrease:
            case Consumables.lifestrikeIncrease:
            case Consumables.lifestealIncrease:
                foreach (Consumable c in consumable.owner.h.playerInventory.combatShopConsumables)
                {
                    if (c.consumable == consumable.consumable)
                    {
                        toRemove = c;
                    }
                }
                consumable.owner.h.playerInventory.combatShopConsumables.Remove(toRemove);
                consumable.owner.h.playerInventory.ChangeGoldAmount(-consumable.cost);
                consumable.owner.h.playerInventory.combatShopConsumables.Add(consumable);
                break;
        }
        foreach (Ailment ail in player.h.playerInventory.ailments)
        {
            ail.consumableBought = true;
        }

        player.RenderConsumable();
    }

    public void Buy()
    {
        switch (consumable.boughtWithCurrency)
        {
            case RewardType.Gold:
                if(player.h.playerInventory.GetGoldAmount() >= consumable.cost)
                {
                    consumable.owner = player;
                    EquipConsumable();
                }
                break;
            case RewardType.Gemstones:
                if (player.gemstonesOwned >= consumable.cost)
                {
                    consumable.owner = player;
                    EquipConsumable();
                }
                break;
        }
    }


}

[Serializable]
public class Consumable
{
    public int consumableIntValue;
    public RewardType boughtWithCurrency;
    public int cost;
    public Consumables consumable;
    public int totalTurns;
    public int statusType;
    public PlayerManager owner;
    public int amount;
    private int previousStatusChance;

    public Consumable(int min, int max)
    {
        boughtWithCurrency = RewardType.Gold;
        totalTurns = 4;
        consumableIntValue = UnityEngine.Random.Range(min, max);
        consumable = (Consumables)consumableIntValue;
        cost = 35;
        switch (consumable)
        {
            //City shop consumables
            case Consumables.ailmentRedux:
                amount = UnityEngine.Random.Range(5, 11);
                boughtWithCurrency = RewardType.Gemstones;
                cost = 25;
                break;
            case Consumables.hazardRedux:
                amount = UnityEngine.Random.Range(5, 11);
                boughtWithCurrency = RewardType.Gemstones;
                cost = 25;
                break;
            case Consumables.rarityIncrease:
                amount = UnityEngine.Random.Range(5, 11);
                boughtWithCurrency = RewardType.Gemstones;
                cost = 25;
                break;
            case Consumables.randomArtifact:
                boughtWithCurrency = RewardType.Gemstones;
                cost = 50;
                break;
            case Consumables.randomSpecial:
                boughtWithCurrency = RewardType.Gemstones;
                cost = 50;
                break;

            //Combat shop consumables
            case Consumables.statusIncrease:
                amount = UnityEngine.Random.Range(10, 21);
                statusType = UnityEngine.Random.Range(13, 18);
                break;
            case Consumables.accuracyIncrease:
                amount = UnityEngine.Random.Range(15, 26);
                break;
            case Consumables.critIncrease:
                amount = UnityEngine.Random.Range(10, 20);
                break;
            case Consumables.strengthIncrease:
                amount = UnityEngine.Random.Range(3, 8);
                break;
            case Consumables.dominanceIncrease:
                amount = UnityEngine.Random.Range(3, 8);
                break;
            case Consumables.toughnessIncrease:
                amount = UnityEngine.Random.Range(3, 8);
                break;
            case Consumables.lifestrikeIncrease:
                amount = UnityEngine.Random.Range(25, 35);
                break;
            case Consumables.lifestealIncrease:
                amount = UnityEngine.Random.Range(25, 35);
                break;
        }

    }

    public Consumable(int min, int max, int val)
    {
        amount = val;
        boughtWithCurrency = RewardType.Gold;
        totalTurns = 4;
        consumableIntValue = UnityEngine.Random.Range(min, max);
        consumable = (Consumables)consumableIntValue;
        switch (consumable)
        {
            //City shop consumables
            case Consumables.ailmentRedux:
                boughtWithCurrency = RewardType.Gemstones;
                cost = 25;
                break;
            case Consumables.hazardRedux:
                boughtWithCurrency = RewardType.Gemstones;
                cost = 25;
                break;
            case Consumables.rarityIncrease:
                boughtWithCurrency = RewardType.Gemstones;
                cost = 25;
                break;
            case Consumables.randomArtifact:
                boughtWithCurrency = RewardType.Gemstones;
                cost = 50;
                break;
            case Consumables.randomSpecial:
                boughtWithCurrency = RewardType.Gemstones;
                cost = 50;
                break;

            //Combat shop consumables
            case Consumables.statusIncrease:
                statusType = UnityEngine.Random.Range(13, 18);
                cost = 20;
                break;
            case Consumables.accuracyIncrease:
                cost = 20;
                break;
            case Consumables.critIncrease:
                cost = 20;
                break;
            case Consumables.strengthIncrease:
                cost = 20;
                break;
            case Consumables.dominanceIncrease:
                cost = 20;
                break;
            case Consumables.toughnessIncrease:
                cost = 20;
                break;
            case Consumables.lifestrikeIncrease:
                cost = 20;
                break;
            case Consumables.lifestealIncrease:
                cost = 20;
                break;
        }

    }

    public void DisableConsumable()
    {
        switch (consumable)
        {
            //City shop consumables
            case Consumables.ailmentRedux:
                owner.h.playerInventory.SetAilmentBonus(0);
                break;
            case Consumables.hazardRedux:
                owner.h.playerInventory.SetHazardBonus(0);
                break;
            case Consumables.rarityIncrease:
                owner.h.playerInventory.SetRarityBonus(0);
                break;

            //Combat shop consumables
            case Consumables.statusIncrease:
                switch(statusType)
                {
                    case 13: //Fire
                        owner.h.playerInventory.fireStatus = previousStatusChance;
                        break;
                    case 14: //Ice
                        owner.h.playerInventory.iceStatus = previousStatusChance;
                        break;
                    case 15: //Lightning
                        owner.h.playerInventory.lightningStatus = previousStatusChance;
                        break;
                    case 16: //Poison
                        owner.h.playerInventory.poisonStatus = previousStatusChance;
                        break;
                    case 17: //Psychic
                        owner.h.playerInventory.psychicStatus = previousStatusChance;
                        break;
                }
                break;
            case Consumables.accuracyIncrease:
                owner.h.GetAccuracy().RemoveAllModFromSource(this);                
                break;
            case Consumables.critIncrease:
                owner.h.GetCritChance().RemoveAllModFromSource(this);                
                break;
            case Consumables.strengthIncrease:
                owner.h.GetStrength().RemoveAllModFromSource(this);                
                break;
            case Consumables.dominanceIncrease:
                owner.h.GetDominance().RemoveAllModFromSource(this);                
                break;
            case Consumables.toughnessIncrease:
                owner.h.GetToughness().RemoveAllModFromSource(this);                
                break;
            case Consumables.lifestrikeIncrease:
                owner.h.GetAttackLifestrike().RemoveAllModFromSource(this);                
                break;
            case Consumables.lifestealIncrease:
                owner.h.GetSpecialLifeSteal().RemoveAllModFromSource(this);                
                break;
        }

    }

    public void ActivateConsumable()
    {
        DisableConsumable();
        if (owner.h.playerInventory.ailments.Exists(a => a.conIndex == 8))
        {
            return;
        }
        switch (consumable)
        {
            //City shop consumables
            case Consumables.ailmentRedux:
                owner.h.playerInventory.SetAilmentBonus(amount);
                break;
            case Consumables.hazardRedux:
                owner.h.playerInventory.SetHazardBonus(amount);
                break;
            case Consumables.rarityIncrease:
                owner.h.playerInventory.SetRarityBonus(amount);
                break;

            //Combat shop consumables
            case Consumables.statusIncrease:
                switch (statusType)
                {
                    case 13: //Fire
                        previousStatusChance = owner.h.playerInventory.fireStatus;
                        owner.h.playerInventory.fireStatus += amount;
                        break;
                    case 14: //Ice
                        previousStatusChance = owner.h.playerInventory.iceStatus;
                        owner.h.playerInventory.iceStatus += amount;
                        break;
                    case 15: //Lightning
                        previousStatusChance = owner.h.playerInventory.lightningStatus;
                        owner.h.playerInventory.lightningStatus += amount;
                        break;
                    case 16: //Poison
                        previousStatusChance = owner.h.playerInventory.poisonStatus;
                        owner.h.playerInventory.poisonStatus += amount;
                        break;
                    case 17: //Psychic
                        previousStatusChance = owner.h.playerInventory.psychicStatus;
                        owner.h.playerInventory.psychicStatus += amount;
                        break;
                }
                break;
            case Consumables.accuracyIncrease:
                owner.h.GetAccuracy().AddModifier(new StatModifier(amount, StatModType.Flat, this));
                break;
            case Consumables.critIncrease:
                owner.h.GetCritChance().AddModifier(new StatModifier(amount, StatModType.Flat, this));
                break;
            case Consumables.strengthIncrease:
                owner.h.GetStrength().AddModifier(new StatModifier(amount, StatModType.Flat, this));
                break;
            case Consumables.dominanceIncrease:
                owner.h.GetDominance().AddModifier(new StatModifier(amount, StatModType.Flat, this));
                break;
            case Consumables.toughnessIncrease:
                owner.h.GetToughness().AddModifier(new StatModifier(amount, StatModType.Flat, this));
                break;
            case Consumables.lifestrikeIncrease:
                owner.h.GetAttackLifestrike().AddModifier(new StatModifier(amount, StatModType.Flat, this));
                break;
            case Consumables.lifestealIncrease:
                owner.h.GetSpecialLifeSteal().AddModifier(new StatModifier(amount, StatModType.Flat, this));
                break;
        }

        totalTurns--;
    }

    public string GetDesc(int val)
    {
        int turns = val + totalTurns;
        switch (consumable)
        {
            //City shop consumables
            case Consumables.ailmentRedux:
                return "Reduced chance of being affected by Ailments <sprite=65> by " + amount + "%.";
            case Consumables.hazardRedux:
                return "Reduced chance of being affected by floor hazards by " + amount + "%.";
            case Consumables.rarityIncrease:
                return "Increased chance of getting Artifacts and Abilities of higher rarity by " + amount + "%."; ;
            case Consumables.randomArtifact:
                return "Begin run with a random common Artifact.";
            case Consumables.randomSpecial:
                return "Begin run with an additional Skill.";

            //Combat shop consumables
            case Consumables.statusIncrease:
                return "Increased chance to apply condition with <sprite=" + GetDamageIcon(statusType) + "> by " + amount + "% for the next " + turns + " room(s).";
            case Consumables.accuracyIncrease:
                return "Increased Accuracy <sprite=" + 46 + "> by " + amount + "% for the next " + turns + " room(s).";
            case Consumables.critIncrease:
                return "Increased Critical Chance <sprite=" + 47 + "> by " + amount + "% for the next " + turns + " room(s).";
            case Consumables.strengthIncrease:
                return "Increased Strength <sprite=" + 51 + "> by " + amount + " for the next " + turns + " room(s).";
            case Consumables.dominanceIncrease:
                return "Increased Dominance <sprite=" + 54 + "> by " + amount + " for the next " + turns + " room(s).";
            case Consumables.toughnessIncrease:
                return "Increased Toughness <sprite=" + 49 + "> by " + amount + " for the next " + turns + " room(s).";
            case Consumables.lifestrikeIncrease:
                return "Increased Lifestrike <sprite=" + 50 + "> by " + amount + "% for the next " + turns + " room(s).";
            case Consumables.lifestealIncrease:
                return "Increased Lifesteal <sprite=" + 55 + "> by " + amount + "% for the next " + turns + " room(s).";
        }
        return "";
    }

    public string GetName(int val)
    {
        switch (consumable)
        {
            //City shop consumables
            case Consumables.ailmentRedux:
                return "Potion of Warding";
            case Consumables.hazardRedux:
                return "Potion of Awareness";
            case Consumables.rarityIncrease:
                return "Potion of Luck"; 
            case Consumables.randomArtifact:
                return "Bag of Goodies";
            case Consumables.randomSpecial:
                return "Potion of Power";

            //Combat shop consumables
            case Consumables.statusIncrease:
                return "Potion of Elements";
            case Consumables.accuracyIncrease:
                return "Potion of Clarity";
            case Consumables.critIncrease:
                return "Potion of Might";
            case Consumables.strengthIncrease:
                return "Potion of Strength";
            case Consumables.dominanceIncrease:
                return "Potion of Dominance";
            case Consumables.toughnessIncrease:
                return "Potion of Toughness";
            case Consumables.lifestrikeIncrease:
                return "Potion of Lifestrike";
            case Consumables.lifestealIncrease:
                return "Potion of Lifesteal";
        }
        return "";
    }

    private int GetDamageIcon(int statusType)
    {
        switch (statusType)
        {
            case 13: //Fire
                return 37;
            case 14: //Ice
                return 42;
            case 15: //Lightning
                return 41;
            case 16: //Poison
                return 38;
            case 17: //Psychic
                return 40;
        }
        return 0;
    }

    public static void GiveRandomCombatConsumable(int amount, PlayerManager player)
    {
        List<int> options = new List<int>() { 5, 6, 7, 8, 9, 10, 11, 12 };
        for(int x = 0; x < amount; x++)
        {
            int op = options[x];
            Consumable consumable = new Consumable(op, op + 1);
            consumable.owner = player;
            Consumable toRemove = null;
            switch (consumable.consumable)
            {
                //Combat shop consumables
                case Consumables.statusIncrease:
                case Consumables.accuracyIncrease:
                case Consumables.critIncrease:
                case Consumables.strengthIncrease:
                case Consumables.dominanceIncrease:
                case Consumables.toughnessIncrease:
                case Consumables.lifestrikeIncrease:
                case Consumables.lifestealIncrease:
                    foreach (Consumable c in consumable.owner.h.playerInventory.combatShopConsumables)
                    {
                        if (c.consumable == consumable.consumable)
                        {
                            toRemove = c;
                        }
                    }
                    consumable.owner.h.playerInventory.combatShopConsumables.Remove(toRemove);
                    consumable.owner.h.playerInventory.combatShopConsumables.Add(consumable);
                    break;
            }
        }
            player.RenderLoadout();

    }
}