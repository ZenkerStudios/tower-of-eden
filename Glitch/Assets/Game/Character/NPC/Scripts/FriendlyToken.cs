using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class FriendlyToken : MonoBehaviour, ICharacter
{
    public InteractableNpc interactableNpc;

    public AdmirationSystem admiration;

    public bool giftable;

    public bool giftedThisRun = false;

    public bool newInteraction = false;
    public GameObject urgentSymbol;

    public GameObject giveVial;
    public GameObject giveShard;
    public TextMeshProUGUI admirationLevelText;

    private PlayerManager player;

    private DialogueManager dialogueManager;

    public DialogueBlock nextDialogue;

    public Image characterImage;

    public HubManager hub;

    private void Awake()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        dialogueManager = DialogueManager.instance;
        characterImage.sprite = interactableNpc.npcSprite;
        admiration = new AdmirationSystem(interactableNpc.maxAdmiration);
        InitializeTokenDialogue();
    }

    public void InitializeTokenDialogue()
    {
        urgentSymbol.SetActive(false);
        GetDialogue();
        if (Visited()) return;
        newInteraction = nextDialogue != null && (nextDialogue.priority == DialoguePriority.Critical || nextDialogue.priority == DialoguePriority.High);
        urgentSymbol.SetActive(newInteraction);

    }

    // Start is called before the first frame update
    void Start()
    {
        CheckGiftable();
    }

    private void GetDialogue()
    {
        nextDialogue = interactableNpc.InitiateDialogue();
        if ((nextDialogue != null)
            && nextDialogue.priority != DialoguePriority.Critical  && nextDialogue.priority != DialoguePriority.High
            && Random.Range(0, 100) < interactableNpc.lowDialogueChance)
        {
            nextDialogue = null;
        }
    }
    public void CheckGiftable()
    {
        giveVial.SetActive(false);
        giveShard.SetActive(false);
        
        switch (interactableNpc.thisNpc)
        {
            case InteractableNpcs.Academy:
                giveVial.transform.parent.parent.parent.gameObject.SetActive(false);
                admiration.currentRank = player.academyAdmirationLevel;
                giftedThisRun = player.giftedToAcademyThisRun;
                break;
            case InteractableNpcs.Blacksmith:
                giveVial.transform.parent.parent.parent.gameObject.SetActive(false);
                admiration.currentRank = player.blacksmithAdmirationLevel;
                giftedThisRun = player.giftedToBlacksmithThisRun;
                break;
            case InteractableNpcs.Church:
                giveVial.transform.parent.parent.parent.gameObject.SetActive(false);
                admiration.currentRank = player.churchAdmirationLevel;
                giftedThisRun = player.giftedToChurchThisRun;
                break;
        }

        admirationLevelText.text = admiration.currentRank + "";
        giftable = interactableNpc.giftable && admiration.currentRank < interactableNpc.maxAdmiration;

        //If player has stuff to give and this NPC hasn't been gifted yet
        if (giftable && !giftedThisRun)
        {
            if (player.etherVialsOwned > 0 && (admiration.currentRank <= interactableNpc.familiarAdmirationThreshold))
            {
                giveVial.transform.parent.parent.parent.gameObject.SetActive(true);
                giveVial.SetActive(true);
            }
            else if (player.etherShardsOwned > 0 && (admiration.currentRank > interactableNpc.familiarAdmirationThreshold))
            {
                giveVial.transform.parent.parent.parent.gameObject.SetActive(true);
                giveShard.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        admirationLevelText.text = admiration.currentRank + "";
        UpdateAdmiration();
        if(giftable && !giftedThisRun && Input.GetKeyDown(KeyCode.F) && HubManager.interactingWith == this)
        {
            if (player.etherVialsOwned > 0 && (admiration.currentRank <= interactableNpc.familiarAdmirationThreshold))
            {
                GiveVial();
            }
            else if (player.etherShardsOwned > 0 && (admiration.currentRank > interactableNpc.familiarAdmirationThreshold))
            {
                GiveShard();
            }
        }
    }

    private void UpdateAdmiration()
    {
        switch (interactableNpc.thisNpc)
        {
            case InteractableNpcs.Academy:
                player.academyAdmirationLevel = admiration.currentRank;
                player.giftedToAcademyThisRun = giftedThisRun;
                break;
            case InteractableNpcs.Blacksmith:
                player.blacksmithAdmirationLevel = admiration.currentRank;
                player.giftedToBlacksmithThisRun = giftedThisRun;
                break;
            case InteractableNpcs.Church:
                player.churchAdmirationLevel = admiration.currentRank;
                player.giftedToChurchThisRun = giftedThisRun;
                break;
        }
    }
    
    public bool Visited()
    {
        switch (interactableNpc.thisNpc)
        {
            case InteractableNpcs.Academy:
                return (player.wentToAcademyThisRun && nextDialogue?.priority != DialoguePriority.Critical) || player.savedPastConvos.Contains(nextDialogue?.GetDialogueId());
            case InteractableNpcs.Blacksmith:
                return player.wentToBlacksmithThisRun;
            case InteractableNpcs.Church:
                return player.wentToChurchThisRun;
            case InteractableNpcs.Home:
                return player.wentHomeThisRun;
            case InteractableNpcs.Shop:
                return player.wentShopkeepThisRun;
            case InteractableNpcs.Tower:
                return player.wentToTowerThisRun;
        }
        return true;
    }

    public string GetName()
    {
        return interactableNpc.GetName(); 
    }

    public string GetCharacterName()
    {
        return interactableNpc.GetCharacterName();
    }

    public bool TriggerVisitDialogue()
    {
        if(nextDialogue != null)
        {
            newInteraction = false;
            urgentSymbol.SetActive(newInteraction);
            dialogueManager.ShowDialogue(nextDialogue);
            return true;
        }

        return false;
    }

    public void GiveVial()
    {
        giveVial.SetActive(false);
        giveVial.transform.parent.parent.parent.gameObject.SetActive(false);

        if (player.etherVialsOwned > 0 && !giftedThisRun)
        {
            player.etherVialsOwned--;
            admiration.currentRank++;
            UpdateAdmiration();
        }
        giftedThisRun = true;
        TriggerBark();
    }


    public void GiveShard()
    {
        giveVial.transform.parent.parent.parent.gameObject.SetActive(false);

        giveShard.SetActive(false);
        if (player.etherShardsOwned > 1 && !giftedThisRun)
        {
            player.etherShardsOwned--;
            admiration.currentRank++;
            UpdateAdmiration();
        }
        giftedThisRun = true;
        TriggerBark();
    }

    private void TriggerBark()
    {
        bool unlocked = false;
        switch (interactableNpc.thisNpc)
        {
            case InteractableNpcs.Blacksmith:
                unlocked = player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockBlacksmith);
                if(player.blacksmithAdmirationLevel == 1)
                {
                    dialogueManager.ShowBarkWithOwner(interactableNpc, characterImage, interactableNpc.FindOnPlayerDeathBark("ARLO_BARK_014"));
                    unlocked = false;
                }
                else if (player.blacksmithAdmirationLevel == 5)
                {
                    dialogueManager.ShowBarkWithOwner(interactableNpc, characterImage, interactableNpc.FindOnPlayerDeathBark("ARLO_BARK_015"));
                    unlocked = false;
                }
                break;
            case InteractableNpcs.Academy:
                unlocked = player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockAcademy);
                if (player.academyAdmirationLevel == 5)
                {
                    dialogueManager.ShowBarkWithOwner(interactableNpc, characterImage, interactableNpc.FindOnPlayerDeathBark("AGNES_BARK_013"));
                    unlocked = false;
                }
                break;
            case InteractableNpcs.Church:
                unlocked = player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockChurch);
                if (player.churchAdmirationLevel == 1)
                {
                    dialogueManager.ShowBarkWithOwner(interactableNpc, characterImage, interactableNpc.FindOnPlayerDeathBark("ABSALOM_BARK_011"));
                    unlocked = false;
                }
                break;
        }

        DialogueBlock bark = interactableNpc.GetOnPlayerDeathBark();
        if (unlocked && bark != null)
        {
            dialogueManager.ShowBarkWithOwner(interactableNpc, characterImage, bark);
        }
    }

    public InteractableNpc GetNpc()
    {
        return interactableNpc;
    }

    public void AddBattleResistance(DamageTypeEnumValue dt, int dur, int amount, object src)
    {
        throw new System.NotImplementedException();
    }

    public void AddCondition(string dt)
    {
        throw new System.NotImplementedException();
    }

    public void ChangeHealth(int hp)
    {
        throw new System.NotImplementedException();
    }

    public bool CheckConditions()
    {
        throw new System.NotImplementedException();
    }

    public StatSystem GetStrength()
    {
        throw new System.NotImplementedException();
    }

    public StatSystem GetAttackLifestrike()
    {
        throw new System.NotImplementedException();
    }

    public int GetBlock()
    {
        throw new System.NotImplementedException();
    }

    public int[] GetBurningInfo()
    {
        throw new System.NotImplementedException();
    }

    public int GetBurningResistance()
    {
        throw new System.NotImplementedException();
    }

    public StatSystem GetAccuracy()
    {
        throw new System.NotImplementedException();
    }

    public List<string> GetConditions()
    {
        throw new System.NotImplementedException();
    }

    public StatSystem GetCritChance()
    {
        throw new System.NotImplementedException();
    }

    public StatSystem GetToughness()
    {
        throw new System.NotImplementedException();
    }

    public int[] GetDamnation()
    {
        throw new System.NotImplementedException();
    }

    public int[] GetDamnationInfo()
    {
        throw new System.NotImplementedException();
    }

    public int GetFrozenResistance()
    {
        throw new System.NotImplementedException();
    }

    public int GetFrozenTurns()
    {
        throw new System.NotImplementedException();
    }

    public StatSystem GetHealthStats()
    {
        throw new System.NotImplementedException();
    }

    public int[] GetIsBurning()
    {
        throw new System.NotImplementedException();
    }

    public int GetIsFrozen()
    {
        throw new System.NotImplementedException();
    }

    public int GetIsPoisoned()
    {
        throw new System.NotImplementedException();
    }

    public int[] GetIsShocked()
    {
        throw new System.NotImplementedException();
    }

    public float[] GetIsVulnerable()
    {
        throw new System.NotImplementedException();
    }


    public StatSystem GetActions()
    {
        throw new System.NotImplementedException();
    }

    public int GetPoisonedTurns()
    {
        throw new System.NotImplementedException();
    }

    public int GetPoisonResistance()
    {
        throw new System.NotImplementedException();
    }

    public List<DamageTypeEnumValue> GetResistances()
    {
        throw new System.NotImplementedException();
    }

    public int GetShockedesistance()
    {
        throw new System.NotImplementedException();
    }

    public int[] GetShockedInfo()
    {
        throw new System.NotImplementedException();
    }

    public StatSystem GetDominance()
    {
        throw new System.NotImplementedException();
    }

    public StatSystem GetSpecialLifeSteal()
    {
        throw new System.NotImplementedException();
    }

    public float[] GetVulnerableInfo()
    {
        throw new System.NotImplementedException();
    }

    public int GetVulnerableResistance()
    {
        throw new System.NotImplementedException();
    }

    public bool HasResistance(string dt)
    {
        throw new System.NotImplementedException();
    }

    public void IncreaseMaxHealth(int val)
    {
        throw new System.NotImplementedException();
    }

    public bool RemoveCondition(string st)
    {
        throw new System.NotImplementedException();
    }

    public void ResetAffinities()
    {
        throw new System.NotImplementedException();
    }

    public void SetBlock(int val)
    {
        throw new System.NotImplementedException();
    }

    public void SetBurningInfo(int[] burning)
    {
        throw new System.NotImplementedException();
    }

    public void SetDamnation(int[] smite)
    {
        throw new System.NotImplementedException();
    }

    public void SetDamnationInfo(int[] smite)
    {
        throw new System.NotImplementedException();
    }

    public void SetFrozenTurns(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void SetIsBurning(int[] burning)
    {
        throw new System.NotImplementedException();
    }

    public void SetIsFrozen(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void SetIsPoisoned(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void SetIsShocked(int[] shocked)
    {
        throw new System.NotImplementedException();
    }

    public void SetIsVulnerable(float[] vulnerable)
    {
        throw new System.NotImplementedException();
    }

    public void SetPoisonedTurns(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void SetShockedInfo(int[] shocked)
    {
        throw new System.NotImplementedException();
    }

    public void SetVulnerableInfo(float[] vulnerable)
    {
        throw new System.NotImplementedException();
    }

    public int GetStatusChance(DamageTypeEnumValue dt)
    {
        throw new System.NotImplementedException();
    }

    public void SetStatusChance(DamageTypeEnumValue dt, int val)
    {
        throw new System.NotImplementedException();
    }

    public void Cleanse()
    {
        throw new System.NotImplementedException();
    }

    public int[] GetHealthRegen()
    {
        throw new System.NotImplementedException();
    }

    public void SetHealthRegen(int[] healthRegen)
    {
        throw new System.NotImplementedException();
    }

    public void CreateCondToken(DamageTypeEnumValue dt)
    {
        throw new System.NotImplementedException();
    }

    public void CreateOtherCond(string condName, string desc, string sp)
    {
        throw new System.NotImplementedException();
    }

    public void PlayIceFX()
    {
        throw new System.NotImplementedException();
    }

    public void ResetConditions()
    {
        throw new System.NotImplementedException();
    }
}
