using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "newDialogueBlock", menuName = "Dialogue/DialogueBlock", order = 1)]
public class DialogueBlock : ScriptableObject
{
    private string dialogueId;
    public InteractableNpc speakerNpc;

    public DialoguePriority priority;
    public int priorityWeight;

    public bool combatShopInteraction;
    public int numCombatShopInteractionNeeded;

    public InteractableNpc interactingWithNpc;
    
    public bool victoryRequirement;
    public string enemyVictoryRequirement;
    public int numVictoryRequired;

    public bool strictNumInteraction;
    public int strictNumInteractionNeeded;

    public bool softNumInteraction;
    public int softNumInteractionNeeded;

    public bool numberOfAttempts;
    public int numberOfAttemptsNeeded;

    public bool numberOfSuccessfulAttempts;
    public int numberOfSuccessfulAttemptsNeeded;

    public bool numberDeathsRequired;
    public int numberDeathsNeeded;

    public WeaponTypes weaponType;
    public bool lastRunWeapon;
    public bool weaponRankRequirement;
    public int weaponRankNeeded;

    public bool lastKilledRequirement;
    public string lastKilledBy;

    public bool blacksmithAdmirationRequirement;
    public int blacksmithAdmirationLevelNeeded;

    public bool academyAdmirationRequirement;
    public int academyAdmirationLevelNeeded;

    public bool churchAdmirationRequirement;
    public int churchAdmirationLevelNeeded;

    public bool deathToEmpressRequirement;
    public int numDeathToEmpressNeeded;

    public bool deathToHangedmanRequirement;
    public int numDeathToHangedmanNeeded;

    public bool deathToMagicianRequirement;
    public int numDeathToMagicianNeeded;

    public bool deathToFoolRequirement;
    public int numDeathToFoolNeeded;

    public bool deathToPriestessRequirement;
    public int numDeathToPriestessNeeded;

    public bool deathToStarRequirement;
    public int numDeathToStarNeeded;


    public bool deathToKingRequirement;
    public int numDeathToKingNeeded;

    public Difficulty difficultyRequirement;
    
    public Cutscenes cutsceneRequirement;
    public Floors floorRequirement;


    public List<DialogueBlock> priorConversationsNeeded;

    public bool hasReward;
    public bool showReward;
    public Sprite rewardSprite;
    public string rewardMessage;
    public DialogueConditions rewardEnum;

    [SerializeField]
    [TextArea(5, 10)]
    public string dialogueBlock;

    public bool isLastDialogueNode;

    public bool isBark;
    public float barkTimer;

    public DialogueBlock prevDialogueNode;

    public DialogueBlock nextDialogueNode;

    public List<DialogueConditions> dialogueConditions;

    public bool canInvalidate;
    public List<DialogueBlock> invalidateDialogueNodes;


    private void Awake()
    {
    }

    private void OnDestroy()
    {
    }

    public string GetDialogueId()
    {
        dialogueId = name.Replace("(Clone)", "");
        return dialogueId;
    }

    public void BarkUsed()
    {
        try
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().savedPastConvos.Add(GetDialogueId());
            InvalidateList();
            GetDialogueReward();
        } catch 
        {

        }
    }

    public void DialogueBlockUsed()
    {
        //Critical are quest relevant. Moved to invalid after 1 use
        //High can loop and are quest reminders. Moved to low priority after 1 use
        //Medium are friendly reminders, tips. Go to low after 2 use
        //Lows endlessly loop
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().savedPastConvos.Add(GetDialogueId());
        switch (priority)
        {
            case DialoguePriority.Critical:
                Invalidate(GetDialogueId());
                break;
            case DialoguePriority.High:
                MoveToMedium(true, GetDialogueId());
                break;
            case DialoguePriority.Medium:
                MoveToMedium(false, GetDialogueId());
                break;
            case DialoguePriority.Low:
                break;
            case DialoguePriority.Invalid:
                break;
        }
        InvalidateList();
        GetDialogueReward();
    }

    private void Invalidate(string id)
    {
        if (!GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().savedInvalidConvos.Contains(id))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().savedInvalidConvos.Add(id);
        }
    }

    private void MoveToMedium(bool mid, string id)
    {
        if(mid)
        {
            if (!DialogueManager.instance.mediumConversation.Contains(id))
            {
                DialogueManager.instance.mediumConversation.Add(id);
            }
        } else
        {
            if (!DialogueManager.instance.lowConversation.Contains(id))
            {
                DialogueManager.instance.lowConversation.Add(id);
            }
        }

    }

    private void InvalidateList()
    {
        if(!canInvalidate)
        {
            return;
        }
        foreach(DialogueBlock d in invalidateDialogueNodes)
        {
            Invalidate(d.GetDialogueId());
        }
    }

    public void GetDialogueReward()
    {
        if(hasReward && !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().savedDialogueConditionsMet.Contains(rewardEnum))
        {
            //show the image for reward and message
            if(showReward)
            {
                GameController.instance.DisplayRewardNotif(rewardMessage);
                
            }
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().savedDialogueConditionsMet.Add(rewardEnum);
            switch (rewardEnum)
            {
                case DialogueConditions.UnlockDelta:
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().deltaWeaponRank = 1;
                    break;
                case DialogueConditions.UnlockFirestorm:
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().firestormWeaponRank = 1;
                    break;
                case DialogueConditions.UnlockRenegade:
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().renegadeWeaponRank = 1;
                    break;
                case DialogueConditions.IzaakReturnsToTower:
                    GameObject.FindGameObjectWithTag("HubManager").GetComponent<HubManager>().towerHub.transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
                    GameObject.FindGameObjectWithTag("HubManager").GetComponent<HubManager>().towerHub.GetComponent<ButtonHover>().isInteractable = true;
                    GameObject.FindGameObjectWithTag("HubManager").GetComponent<HubManager>().tower.InitializeTokenDialogue();
                    break;
                case DialogueConditions.UnlockHome:
                    GameObject.FindGameObjectWithTag("HubManager").GetComponent<HubManager>().homeHub.transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
                    GameObject.FindGameObjectWithTag("HubManager").GetComponent<HubManager>().homeHub.GetComponent<ButtonHover>().isInteractable = true;
                    GameObject.FindGameObjectWithTag("HubManager").GetComponent<HubManager>().homeHub.SetCutsceneWiki();
                    break;
            }

        }

    }

    public bool AreConditionsMet(PlayerManager player)
    {
        //Check if invalid 
        if(combatShopInteraction && numCombatShopInteractionNeeded < player.combatShopInteraction)
        {
            return false;
        }
        
        //Check if invalid 
        if(lastRunWeapon && player.GetLastAttempt().weapon != weaponType)
        {
            return false;
        }

        //Check if invalid 
        if(weaponRankRequirement && player.GetWeaponRank(weaponType) != weaponRankNeeded)
        {
            return false;
        }

        //Check if invalid 
        if (cutsceneRequirement != Cutscenes.None && 
            !player.cutscenesUnlocked.Contains(cutsceneRequirement.ToString()))
        {
            return false;
        }

        //Check if invalid 
        if ((int)floorRequirement > (int)player.highestFloorReached)
        {
            return false;
        }

        //Check if invalid 
        if (player.savedInvalidConvos.Contains(GetDialogueId()))
        {
            return false;
        }

        //Check if death requirement
        if(numberDeathsRequired && player.GetFailedAttempts() < numberDeathsNeeded)
        {
            return false;
        }

        //Check if last killed by matches
        if (lastKilledRequirement && !player.attempts.Last().killedByName.Equals(lastKilledBy))
        {
            return false;
        }

        //Check number of admiration level
        if (player.churchAdmirationLevel < churchAdmirationLevelNeeded && churchAdmirationRequirement)
        {
            return false;
        }

        if (player.academyAdmirationLevel < academyAdmirationLevelNeeded && academyAdmirationRequirement)
        {
            return false;
        }

        if (player.blacksmithAdmirationLevel < blacksmithAdmirationLevelNeeded && blacksmithAdmirationRequirement)
        {
            return false;
        }

        if (interactingWithNpc != null)
        {
            //Check number of interaction level
            switch (interactingWithNpc.thisNpc)
            {
                case InteractableNpcs.Academy:
                    if ((strictNumInteractionNeeded != player.numAcademyInteraction && strictNumInteraction)
                        || (player.numAcademyInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Blacksmith:
                    if ((strictNumInteractionNeeded != player.numBlacksmithInteraction && strictNumInteraction)
                        || (player.numBlacksmithInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Home:
                    if ((strictNumInteractionNeeded != player.numHomeInteraction && strictNumInteraction)
                        || (player.numHomeInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Church:
                    if ((strictNumInteractionNeeded != player.numChurchInteraction && strictNumInteraction)
                        || (player.numChurchInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Shop:
                    if ((strictNumInteractionNeeded != player.numShopkeepInteraction && strictNumInteraction)
                        || (player.numShopkeepInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Empress:
                    if ((strictNumInteractionNeeded != player.numEmpressInteraction && strictNumInteraction)
                        || (player.numEmpressInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.HangedMan:
                    if ((strictNumInteractionNeeded != player.numHangedmanInteraction && strictNumInteraction)
                        || (player.numHangedmanInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Magician:
                    if ((strictNumInteractionNeeded != player.numMagicianInteraction && strictNumInteraction)
                        || (player.numMagicianInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Fool:
                    if ((strictNumInteractionNeeded != player.numFoolInteraction && strictNumInteraction)
                        || (player.numFoolInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Priestess:
                    if ((strictNumInteractionNeeded != player.numPriestessInteraction && strictNumInteraction)
                        || (player.numPriestessInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.Star:
                    if ((strictNumInteractionNeeded != player.numStarInteraction && strictNumInteraction)
                        || (player.numStarInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.King:
                    if ((strictNumInteractionNeeded != player.numKingInteraction && strictNumInteraction)
                        || (player.numKingInteraction < softNumInteractionNeeded && softNumInteraction))
                    {
                        return false;
                    }
                    break;
                case InteractableNpcs.None:
                    break;
            }
        }

        //Check death requirements
        if (deathToEmpressRequirement && player.numDeathToEmpress < numDeathToEmpressNeeded)
        {
            return false;
        }
        if (deathToHangedmanRequirement && player.numDeathToHangedman < numDeathToHangedmanNeeded)
        {
            return false;
        }
        if (deathToMagicianRequirement && player.numDeathToMagician < numDeathToMagicianNeeded)
        {
            return false;
        }
        if (deathToFoolRequirement && player.numDeathToFool < numDeathToFoolNeeded)
        {
            return false;
        }
        if (deathToPriestessRequirement && player.numDeathToPriestess < numDeathToPriestessNeeded)
        {
            return false;
        }
        if (deathToStarRequirement && player.numDeathToStar < numDeathToStarNeeded)
        {
            return false;
        }
        if (deathToKingRequirement && player.numDeathToKing < numDeathToKingNeeded)
        {
            return false;
        }


        //Check if invalid 
        if (victoryRequirement && player.TotalTimeDefeatedEnemy(enemyVictoryRequirement) < numVictoryRequired)
        {
            return false;
        }

        //Check difficulty condition
        if (player.chosenDifficulty < (int)difficultyRequirement)
        {
            return false;
        }

        //Check run conditions
        if (numberOfAttempts && (player.GetTotalAttempts() < numberOfAttemptsNeeded))
        {
            return false;
        }

        //Check successful run conditions
        if (numberOfSuccessfulAttempts && (player.GetSuccessfulAttempts() < numberOfSuccessfulAttemptsNeeded))
        {
            return false;
        }

        //Check previous dialogues conditions
        if(priorConversationsNeeded.Count > 0)
        {
            foreach(DialogueBlock db in priorConversationsNeeded)
            {
                if(!player.savedPastConvos.Contains(db.GetDialogueId()))
                {
                    return false;
                }
            }
        }

        //Check dialogue conditions
        foreach (DialogueConditions checkConditions in dialogueConditions)
        {
            if(!player.savedDialogueConditionsMet.Contains(checkConditions))
            {
                return false;
            }
        }
        return true;
    }
}
