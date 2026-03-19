using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BountyItem : MonoBehaviour, IPointerClickHandler
{
    public Bounty bounty;
    public bool playerEquipped;
    public BountyHub tavern;
    public List<Sprite> bountyIcons;

    public TextMeshProUGUI bountyName;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI rewardAmount;
    public GameObject deleteButton;
    public Image bountyImg;
    public GameObject questImg;

    private PlayerManager player;
    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        deleteButton.SetActive(playerEquipped);
        bountyImg.sprite = bountyIcons[bounty.bountyType];
        questImg.SetActive(playerEquipped);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        bountyName.text = bounty.bountyName;
        desc.text = bounty.bountyInfo;
        switch (bounty.bountyRewards)
        {
            case RewardType.Gemstones:
                rewardAmount.text = "<sprite=9> " + bounty.reward;
                break;
            case RewardType.MetalScraps:
                rewardAmount.text = "<sprite=19> " + bounty.reward;
                break;
            case RewardType.EtherVial:
                rewardAmount.text = "<sprite=7> " + bounty.reward;
                break;
            case RewardType.EtherShard:
                rewardAmount.text = "<sprite=5> " + bounty.reward;
                break;
            case RewardType.GrimoirePage:
                rewardAmount.text = "<sprite=13> " + bounty.reward;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        questImg.SetActive(playerEquipped);
        deleteButton.SetActive(playerEquipped);
    }

    public void PickUpBounty()
    {
        if(player.equippedBounties.Count < player.maxBounties && !playerEquipped)
        {
            player.equippedBounties.Add(bounty);
            Destroy(gameObject);
            tavern.RenderBounties();
        }
    }
    public void DeleteBounty()
    {
        player.equippedBounties.Remove(bounty);
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PickUpBounty();
        } 
    }
}






public class Bounty
{
    public int bountyType = 0;
    public int tier = 1;

    public bool completed;
    public string bountyName;
    public string bountyInfo;

    public int critCondition;
    public int affectCondition;
    public int healCondition;
    public int avoidCondition;
    public int damageTypeCondition;
    public int affectTypeCondition;
    public int damageTakenCondition;
    public int damageDealtCondition;
    public int turnLimitCondition;
    public Floors floorCondition = Floors.Hub;
    public List<string> academyBuffCondition = new List<string>();
    public WeaponTypes weaponCondition = WeaponTypes.None;

    public RewardType bountyRewards;
    public int reward;
    private readonly string[] damageTypeSprite = new string[] { "39", "37", "42", "41", "38", "40", "43" };
    private readonly string[] buffs1 = new string[]
    {
        "burnBonus",
        "shockedBonus",
        "vulnerableBonus",
        "damnedBonus",
        "reviveBonus",
        "maxHealthBonus",
        "accuracyBonus",
        "critChanceBonus",     
    };  
    
    private readonly string[] buffs2 = new string[]
    {       
        "actionBonus",
        "fireResistBonus",
        "iceResistBonus",
        "lightningResistBonus",
        "psychicResistBonus",
        "poisonResistBonus",
        "entryHealthBonus",
        "goldStart",
    };   
    
    private readonly string[] buffs3 = new string[]
    {
        "fireChanceBonus",
        "iceChanceBonus",
        "poisonChanceBonus",
        "lightningChanceBonus",
        "psychicChanceBonus",
        "entrygoldBonus",
        "attackLifestealBonus",
        "specialLifestealBonus"
    };

    public Bounty() : this(new List<DialogueConditions>())
    {
    }

    public Bounty(List<DialogueConditions> conds)
    {
        tier = Random.Range(1, 6);
        //bountyType = Random.Range(1, 13);
        bountyType = 6;
        bountyRewards = (RewardType)Random.Range(7, 12);
        switch(bountyRewards)
        {
            case RewardType.Gemstones:
                reward = 20 * tier;
                break;
            case RewardType.MetalScraps:
                reward = 25 * tier;
                break;
            case RewardType.EtherVial:
                reward = tier;
                break;
            case RewardType.EtherShard:
                reward = 3;
                break;
            case RewardType.GrimoirePage:
                reward = Mathf.Clamp((int)(0.6f * tier), 1, 3);
                break;
        }

        critCondition = Random.Range(5, 10) + (tier/2);
        affectCondition = tier;
        affectTypeCondition = Random.Range(0, 6);
        healCondition = Random.Range(10, 20) * tier;
        avoidCondition = Random.Range(10, 15) + (tier/2);
        damageTypeCondition = Random.Range(0, 7);

        damageTakenCondition = Random.Range(100, 130) / tier;
        damageDealtCondition = Random.Range(30, 50) * tier;
        turnLimitCondition = Random.Range(10, 15) - tier;
        weaponCondition = (WeaponTypes)WeaponRange(conds);
        floorCondition = (Floors)tier + 1;
        academyBuffCondition = EnchantmentRange(conds);
        SetDesc();
    }

    private int WeaponRange(List<DialogueConditions> conds)
    {
        List<int> availableWeapons = new List<int>();
        availableWeapons.Add(1);
        if (conds != null && conds.Contains(DialogueConditions.UnlockDelta)) availableWeapons.Add(2);
        if (conds != null && conds.Contains(DialogueConditions.UnlockFirestorm)) availableWeapons.Add(3);
        if (conds != null && conds.Contains(DialogueConditions.UnlockRenegade)) availableWeapons.Add(4);
        return availableWeapons[Random.Range(0, availableWeapons.Count)];
    }

    private List<string> EnchantmentRange(List<DialogueConditions> conds)
    {
        List<string> availableEnc = new List<string>();
        availableEnc.AddRange(buffs1);
        if (conds != null && conds.Contains(DialogueConditions.GrimoirePageTwo)) availableEnc.AddRange(buffs2);
        if (conds != null && conds.Contains(DialogueConditions.GrimoirePageThree)) availableEnc.AddRange(buffs3);         
        return availableEnc.OrderBy(a => System.Guid.NewGuid()).ToList().GetRange(0, tier);
    }

    public void SetDesc()
    {
        switch (bountyType)
        {
            case 1:
                bountyName = "Defender Tier " + tier;
                bountyInfo = "Take " + damageTakenCondition + " damage or less in an encounter";
                break;
            case 2:
                bountyName = "Berserker Tier " + tier;
                bountyInfo = "Deal " + damageDealtCondition + " damage or more in an encounter";
                break;
            case 3:
                bountyName = "Hustler Tier " + tier;
                bountyInfo = "Win an encounter in " + turnLimitCondition + " turns or less";
                break;
            case 4:
                bountyName = "Explorer Tier " + tier;
                bountyInfo = "Beat Floor " + tier;
                break;
            case 5:
                bountyName = "Scholar Tier " + tier;
                bountyInfo = "Beat Floor 5 with the following enchantments:";
                AddAcademyBuffs();
                break;
            case 6:
                bountyName = "Weapon Master";
                bountyInfo = "Beat Floor 5 with the following weapon: " + weaponCondition.ToString();
                break;
            case 7:
                bountyName = "Proliferator  Tier " + tier;
                bountyInfo = "Apply " + affectCondition + "<sprite=" + damageTypeSprite[affectTypeCondition] + "> condition or more in an encounter";
                break;
            case 8:
                bountyName = "Enchanter Tier " + tier;
                damageDealtCondition /= 2;
                bountyInfo = "Deal " + damageDealtCondition + "<sprite=" + damageTypeSprite[damageTypeCondition] + "> damage or more in an encounter";
                break;
            case 9:
                bountyName = "Predator Tier " + tier;
                bountyInfo = "Get " + critCondition + " critical hits or more in an encounter";
                break;
            case 10:
                bountyName = "Prowler Tier " + tier;
                bountyInfo = "Avoid " + avoidCondition + " enemy attacks or more in an encounter";
                break;
            case 11:
                bountyName = "Healer Tier " + tier;
                bountyInfo = "Heal " + healCondition + " <sprite=45> or more in an encounter";
                break;
            case 12:
                bountyName = "Resolute";
                bountyInfo = "Hit every attack in an encounter";
                break;

        }
    }

    private void AddAcademyBuffs()
    {
        bountyInfo += GameUtilities.GetEnchantmentName(academyBuffCondition[0]);
        for (int x = 1; x < tier; x++)
        {
            bountyInfo += ", " + GameUtilities.GetEnchantmentName(academyBuffCondition[x]); 
        }
        bountyInfo += ".";

    }

    public bool CheckBountyConditions(CombatSystem cSystem)
    {

        if (completed) return true;

        switch (bountyType)
        {
            case 1:
                completed = System.Math.Abs(cSystem.totalDamageTaken) <= damageTakenCondition;
                break;
            case 2:
                completed = System.Math.Abs(cSystem.totalDamageDealt) >= damageDealtCondition;
                break;
            case 3:
                completed = cSystem.turnUsed <= turnLimitCondition;
                break;
            case 4:
                completed = (int)cSystem.encounterKey >= (int)floorCondition;
                break;
            case 5:
                completed = CheckBonusesCondition(cSystem.player.equippedBonuses);
                break;
            case 6:
                completed = (int)cSystem.encounterKey >= 6 && cSystem.player.selectedWeaponType == weaponCondition;
                break;
            case 7:
                completed = CheckAffectCondition(cSystem);
                break;
            case 8:
                completed = CheckDamageDealtCondition(cSystem);
                break;
            case 9:
                completed = cSystem.critHit >= critCondition;
                break;
            case 10:
                completed = cSystem.dodges >= avoidCondition;
                break;
            case 11:
                completed = cSystem.damageHealed >= healCondition;
                break;
            case 12:
                completed = cSystem.hitEveryAttack;
                break;

        }
        return false;
    }

    public void GetReward(PlayerManager player)
    {
        switch (bountyRewards)
        {
            case RewardType.Gemstones:
                player.ChangeGemstonesAmount(reward);
                break;
            case RewardType.MetalScraps:
                player.ChangeScrapsAmount(reward);
                break;
            case RewardType.EtherVial:
                player.ChangeVialsAmount(reward);
                break;
            case RewardType.EtherShard:
                player.ChangeShardAmount(reward);
                break;
            case RewardType.GrimoirePage:
                player.ChangeGrimoirePageAmount(reward);
                break;
        }
    }
    private bool CheckDamageDealtCondition(CombatSystem cs)
    {
        switch (damageTypeSprite[damageTypeCondition])
        {
            case "37":
                return System.Math.Abs(cs.fireDamageDealt) >= damageDealtCondition;
            case "38":
                return System.Math.Abs(cs.poisonDamageDealt) >= damageDealtCondition;
            case "39":
                return System.Math.Abs(cs.divineDamageDealt) >= damageDealtCondition;
            case "40":
                return System.Math.Abs(cs.psychicDamageDealt) >= damageDealtCondition;
            case "41":
                return System.Math.Abs(cs.lightningDamageDealt) >= damageDealtCondition;
            case "42":
                return System.Math.Abs(cs.iceDamageDealt) >= damageDealtCondition;
            case "43":
                return System.Math.Abs(cs.physicalDamageDealt) >= damageDealtCondition;
        }
        return false;
    }

    private bool CheckAffectCondition(CombatSystem cs)
    {
        switch(damageTypeSprite[affectTypeCondition])
        {
            case "37":
                return cs.fireCond >= affectCondition;
            case "38":
                return cs.poisonCond >= affectCondition;
            case "39":
                return cs.divineCond >= affectCondition;
            case "40":
                return cs.psychicCond >= affectCondition;
            case "41":
                return cs.lightningCond >= affectCondition;
            case "42":
                return cs.iceCond >= affectCondition;
        }
        return false;
    }

    private bool CheckBonusesCondition(List<string> bonuses)
    {
        foreach (string s in academyBuffCondition)
        {
            if (!bonuses.Contains(s))
            {
                return false;
            }
        }
        return true;
    }

}

       