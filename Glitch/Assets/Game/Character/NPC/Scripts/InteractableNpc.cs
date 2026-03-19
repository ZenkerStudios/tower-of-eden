using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newInteractableNpc", menuName = "NPC/InteractableNpc", order = 2)]
public class InteractableNpc : ScriptableObject
{
    [SerializeField]
    [TextArea(5, 10)]
    public string background;

    public InteractableNpcs thisNpc;
    
    public string characterName;

    public Sprite npcSprite;
    public int npcSpriteIndex;

    public int maxAdmiration;

    public int familiarAdmirationThreshold;

    public bool giftable;

    public int lowDialogueChance;
    public int barkChance;
    public int barkCooldown;

    public List<DialogueBlock> barks;

    public List<DialogueBlock> deathBarks;

    public List<DialogueBlock> abilityBarks;

    public List<DialogueBlock> onPlayerDeathBark;

    public bool isBarking = false;

    private void Awake()
    {
        isBarking = false;
    }

    private void OnDestroy()
    {
    }

    public DialogueBlock FindRandomBark(string id)
    {
        foreach (DialogueBlock d in barks)
        {
            if (d.GetDialogueId().Equals(id))
            {
                return d;
            }
        }
        return null;
    }

    public DialogueBlock FindDeathBark(string id)
    {
        foreach (DialogueBlock d in deathBarks)
        {
            if (d.GetDialogueId().Equals(id))
            {
                return d;
            }
        }
        return null;
    }

    public DialogueBlock FindAbilityBark(string id)
    {
        foreach (DialogueBlock d in abilityBarks)
        {
            if (d.GetDialogueId().Equals(id))
            {
                return d;
            }
        }
        return null;
    }

    public DialogueBlock FindOnPlayerDeathBark(string id)
    {
        foreach (DialogueBlock d in onPlayerDeathBark)
        {
            if (d.GetDialogueId().Equals(id))
            {
                return d;
            }
        }
        return null;
    }

    public DialogueBlock GetRandomBark()
    {
        return InitiateDialogue(barks);
    }

    public DialogueBlock GetDeathBark()
    {
        return InitiateDialogue(deathBarks);
    }


    public DialogueBlock GetAbilityBark()
    {
        return InitiateDialogue(abilityBarks);
    }

    public DialogueBlock GetOnPlayerDeathBark()
    {
        return InitiateDialogue(onPlayerDeathBark);
    }

    public DialogueBlock InitiateDialogue()
    {
        return InitiateDialogue(DialogueManager.instance.GetConversationStarters(thisNpc));
    }

    public DialogueBlock InitiateDialogue(List<DialogueBlock> dialogueList)
    {
        PlayerManager player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        DialogueManager dialogueManager = DialogueManager.instance;
        //Need to go through all possible dialogue option, tally those that are valid and select by priority.
        List<DialogueBlock> criticalBlocks = new List<DialogueBlock>();
        List<DialogueBlock> highBlocks = new List<DialogueBlock>();
        List<DialogueBlock> mediumBlocks = new List<DialogueBlock>();
        List<DialogueBlock> lowBlocks = new List<DialogueBlock>();

        foreach (DialogueBlock block in dialogueList)
        {
            if (block.speakerNpc == null) block.speakerNpc = this;
            //Tally all valid dialogues
            if (block.AreConditionsMet(player))
            {
                switch (block.priority)
                {
                    case DialoguePriority.Critical:
                        criticalBlocks.Add(block);
                        break;
                    case DialoguePriority.High:
                        if (dialogueManager.mediumConversation.Contains(block.GetDialogueId()))
                        {
                            mediumBlocks.Add(block);
                        }
                        else if (dialogueManager.lowConversation.Contains(block.GetDialogueId()))
                        {
                            lowBlocks.Add(block);
                        }
                        else
                        {
                            if (player.savedPastConvos.Contains(block.GetDialogueId()))
                            {
                                lowBlocks.Add(block);
                            }
                            else
                            {
                                highBlocks.Add(block);
                            }
                        }
                        break;
                    case DialoguePriority.Medium:
                        if (dialogueManager.lowConversation.Contains(block.GetDialogueId()))
                        {
                            lowBlocks.Add(block);
                        }
                        else
                        {
                            if (player.savedPastConvos.Contains(block.GetDialogueId()))
                            {
                                lowBlocks.Add(block);
                            }
                            else
                            {
                                mediumBlocks.Add(block);
                            }
                        }
                        break;
                    case DialoguePriority.Low:
                        lowBlocks.Add(block);
                        break;
                    case DialoguePriority.Invalid:
                        break;
                }
            }

        }
        //critical and high order by priority
        criticalBlocks = criticalBlocks.OrderByDescending(s => s.priorityWeight).ToList();
        highBlocks = highBlocks.OrderByDescending(s => s.priorityWeight).ToList();
        //Medium and low random order 
        mediumBlocks = mediumBlocks.OrderBy(a => System.Guid.NewGuid()).ToList();
        lowBlocks = lowBlocks.OrderBy(a => System.Guid.NewGuid()).ToList();
        int chance = 40;
        DialogueBlock res = null;
        if (criticalBlocks.Count > 0)
        {
            return criticalBlocks[0];
        }
        else if (highBlocks.Count > 0)
        {
            DialogueBlock temp = highBlocks[Random.Range(0, highBlocks.Count)];
            if (player.savedPastConvos.Contains(temp.GetDialogueId()))
            {
                lowBlocks.Add(temp);
                highBlocks.Remove(temp);
            }
            res = temp;
        }
        else if (mediumBlocks.Count > 0)
        {
            res = mediumBlocks[Random.Range(0, mediumBlocks.Count)];
            chance = 30;
        }
        else if (lowBlocks.Count > 0)
        {
            res = lowBlocks[Random.Range(0, lowBlocks.Count)];
            chance = 20;
        }
        if (Random.Range(1, 101) <= chance)
        {
            return res = null;
        }
        return res;
    }

    public string GetName()
    {
        return name.Replace("(Clone)", "");
    }

    public string GetCharacterName()
    {
        return characterName;
    }

    public string GetBackground()
    {
        return background;
    }

}
