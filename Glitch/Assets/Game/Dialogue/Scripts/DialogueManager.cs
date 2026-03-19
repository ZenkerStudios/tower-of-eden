using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
[DefaultExecutionOrder(-48)]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField]
    protected List<DialogueBlock> allRagnaConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allAcridConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allBosneyConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allDorianConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allLyraConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allUleaConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allEdenConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allAcademyConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allBlacksmithConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allHomeConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allTowerConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allShopConversationStarters;
    [SerializeField]
    protected List<DialogueBlock> allChurchConversationStarters;

    public List<DialogueBlock> conversationStarters;

    public List<string> mediumConversation;
    public List<string> lowConversation;

    public DialogueBlock previousDialogueBlock;
    private DialogueBlock viewingDialogueBlock;

    public bool inConversation = false;

    public GameObject pfBarkBubble;

    public GameObject DialogueContainer;
    public GameObject spriteImg;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public List<InteractableNpc> npcs = new List<InteractableNpc>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        conversationStarters.AddRange(allRagnaConversationStarters);
        conversationStarters.AddRange(allAcridConversationStarters);
        conversationStarters.AddRange(allBosneyConversationStarters);
        conversationStarters.AddRange(allDorianConversationStarters);
        conversationStarters.AddRange(allLyraConversationStarters);
        conversationStarters.AddRange(allUleaConversationStarters);
        conversationStarters.AddRange(allEdenConversationStarters);

        conversationStarters.AddRange(allAcademyConversationStarters);
        conversationStarters.AddRange(allBlacksmithConversationStarters);
        conversationStarters.AddRange(allHomeConversationStarters);
        conversationStarters.AddRange(allTowerConversationStarters);
        conversationStarters.AddRange(allShopConversationStarters);
        conversationStarters.AddRange(allChurchConversationStarters);

        DialogueContainer.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetBarks();
    }

    public void ResetBarks()
    {
        foreach (InteractableNpc npc in npcs)
        {
            npc.isBarking = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseController.instance.isGamePaused()) return;

        if(!CutsceneManager.instance.inCutscene && inConversation && Input.GetKeyDown(KeyCode.Space))
        {
            NextInConversation();
        }
    }

    //We take a monobehaviour so the owner starts coroutine, if owner dies coroutine ends to avoid null exception
    public void ShowBarkWithOwner(InteractableNpc ownerNpc, MonoBehaviour owner, DialogueBlock block)
    {
        if(block.isBark)
        {
            owner.StartCoroutine(ShowBark(ownerNpc, owner.transform, block));
        }
        else
        {
            ShowDialogue(block);
        }

    }

    public void ShowDialogue(DialogueBlock block)
    {
        inConversation = true;
        viewingDialogueBlock = block;
        //display dialogue, wait for click to proceed then use
        dialogueText.text = viewingDialogueBlock.dialogueBlock;
        nameText.text = viewingDialogueBlock.speakerNpc.GetCharacterName();

        int index = viewingDialogueBlock.speakerNpc.npcSpriteIndex;
        GameUtilities.ToggleActiveAllChildGameObject(spriteImg, false);
        spriteImg.transform.GetChild(index).gameObject.SetActive(true);

        //show everything
        DialogueContainer.SetActive(true);
    }

    private IEnumerator ShowBark(InteractableNpc ownerNpc, Transform owner, DialogueBlock block)
    {
        if(!ownerNpc.isBarking)
        {
            ownerNpc.isBarking = true;

            var newObj = Instantiate(pfBarkBubble);
            newObj.transform.SetParent(owner.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<BarkBubble>().Init(ownerNpc, block);
            newObj.GetComponent<RectTransform>().sizeDelta = pfBarkBubble.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfBarkBubble.GetComponent<RectTransform>().localScale;
            yield return null;
        }
    }

    public void NextInConversation()
    {
        //hide everything
        AudioManager.instance.PlaySfxSound("Click");
        DialogueContainer.SetActive(false);
        viewingDialogueBlock.DialogueBlockUsed();

        if (viewingDialogueBlock.isLastDialogueNode)
        {
            previousDialogueBlock = viewingDialogueBlock;
            viewingDialogueBlock = null;
            inConversation = false;
        }
        else
        {
            previousDialogueBlock = viewingDialogueBlock;
            ShowDialogue(viewingDialogueBlock.nextDialogueNode);
        }
    }

    public bool IsDialogueOver(string dialogueId)
    {
        return previousDialogueBlock != null && previousDialogueBlock.GetDialogueId().Equals(dialogueId) && !inConversation;
    }

    private DialogueBlock GetDialogueById(List<DialogueBlock> listToChooseFrom, string id)
    {
        foreach(DialogueBlock d in listToChooseFrom)
        {
            if (d.GetDialogueId().Equals(id))
            {
                return d;
            }
        }
        return null;
    }

    public DialogueBlock GetDialogueBlockFor(InteractableNpcs npc, string dialogueId)
    {
        switch (npc)
        {
            case InteractableNpcs.Empress:
                return GetDialogueById(allRagnaConversationStarters, dialogueId);
            case InteractableNpcs.Magician:
                return GetDialogueById(allAcridConversationStarters, dialogueId);
            case InteractableNpcs.HangedMan:
                return GetDialogueById(allBosneyConversationStarters, dialogueId);
            case InteractableNpcs.Fool:
                return GetDialogueById(allDorianConversationStarters, dialogueId);
            case InteractableNpcs.Priestess:
                return GetDialogueById(allLyraConversationStarters, dialogueId);
            case InteractableNpcs.Star:
                return GetDialogueById(allUleaConversationStarters, dialogueId);
            case InteractableNpcs.King:
                return GetDialogueById(allEdenConversationStarters, dialogueId);
            case InteractableNpcs.Academy:
                return GetDialogueById(allAcademyConversationStarters, dialogueId);
            case InteractableNpcs.Blacksmith:
                return GetDialogueById(allBlacksmithConversationStarters, dialogueId);
            case InteractableNpcs.Tower:
                return GetDialogueById(allTowerConversationStarters, dialogueId);
            case InteractableNpcs.Home:
                return GetDialogueById(allHomeConversationStarters, dialogueId);
            case InteractableNpcs.Church:
                return GetDialogueById(allChurchConversationStarters, dialogueId);
            case InteractableNpcs.Shop:
                return GetDialogueById(allShopConversationStarters, dialogueId); 
        }
        return null;
    }

    public List<DialogueBlock> GetConversationStarters(InteractableNpcs npc)
    {
        switch (npc)
        {
            case InteractableNpcs.Empress:
                return allRagnaConversationStarters;
            case InteractableNpcs.Magician:
                return allAcridConversationStarters;
            case InteractableNpcs.HangedMan:
                return allBosneyConversationStarters;
            case InteractableNpcs.Fool:
                return allDorianConversationStarters;
            case InteractableNpcs.Priestess:
                return allLyraConversationStarters;
            case InteractableNpcs.Star:
                return allUleaConversationStarters;
            case InteractableNpcs.King:
                return allEdenConversationStarters;
            case InteractableNpcs.Academy:
                return allAcademyConversationStarters;
            case InteractableNpcs.Blacksmith:
                return allBlacksmithConversationStarters;
            case InteractableNpcs.Church:
                return allChurchConversationStarters;
            case InteractableNpcs.Shop:
                return allShopConversationStarters;
            case InteractableNpcs.Tower:
                return allTowerConversationStarters;
            case InteractableNpcs.Home:
                return allHomeConversationStarters;
        }

        return new List<DialogueBlock>();
    }
    
}
