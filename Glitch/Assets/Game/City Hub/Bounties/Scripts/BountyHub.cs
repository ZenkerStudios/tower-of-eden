using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BountyHub : MonoBehaviour
{

    public GameObject hubWindow;
    public Animator windowAnimator;

    public GameObject newBountyPanel;
    public GameObject pfBounty;

    private PlayerManager player;
    public GameObject equippedBountyPanel;

    private DialogueManager dialogueManager;

    private void Awake()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        dialogueManager = DialogueManager.instance;
        bool active = (player.GetSuccessfulAttempts() > 2 && player.GetTotalAttempts() > 14);
        gameObject.GetComponent<ButtonHover>().isInteractable = active;
        transform.GetChild(0).gameObject.GetComponent<Button>().interactable = active;

        transform.GetChild(0).GetChild(1).gameObject.SetActive((active && !player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockBounties)) || player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockBounties));


        GameUtilities.DeleteAllChildGameObject(newBountyPanel);

        for (int x = 0; x < 4; x++)
        {
            var newObj = Instantiate(pfBounty);
            BountyItem bounty = newObj.GetComponent<BountyItem>();
            bounty.bounty = new Bounty(player.savedDialogueConditionsMet);
            bounty.playerEquipped = false;
            bounty.tavern = this;
            newObj.transform.SetParent(newBountyPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfBounty.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfBounty.GetComponent<RectTransform>().localScale;
        }
        RenderBounties();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenWindow()
    {
        AudioManager.instance.PlaySfxSound("Button_Click");
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        if (player.wentBountyThisRun)
        {
            ShowWindow();
        }
        else
        {
            player.wentBountyThisRun = true;
            if (!player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockBounties))
            {
                dialogueManager.ShowDialogue(dialogueManager.GetDialogueBlockFor(InteractableNpcs.Home, "HOME_014_01"));
                StartCoroutine(GameUtilities.WaitForConversation(() => ShowWindow()));
            } 
            else if (Random.Range(0, 100) < 30)
            {
                dialogueManager.ShowDialogue(dialogueManager.GetDialogueBlockFor(InteractableNpcs.Home, "HOME_015_01"));
                StartCoroutine(GameUtilities.WaitForConversation(() => ShowWindow()));
            }
            else
            {
                ShowWindow();
            }

            player.numBountyInteraction++;
        }
    }

    public void ShowWindow()
    {

        //play open animation
        windowAnimator.Play("Bounty Open");
    }

    public void CloseWindow()
    {
        //play close animation 
        windowAnimator.Play("Bounty Close");

    }

    public void RenderBounties()
    {
        if(player.equippedBounties == null)
        {
            return;
        }
        GameUtilities.DeleteAllChildGameObject(equippedBountyPanel);
        foreach(Bounty b in player.equippedBounties)
        {
            var newObj = Instantiate(pfBounty);
            BountyItem bounty = newObj.GetComponent<BountyItem>();
            bounty.bounty = b;
            bounty.playerEquipped = true;
            bounty.tavern = this;
            newObj.transform.SetParent(equippedBountyPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfBounty.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfBounty.GetComponent<RectTransform>().localScale;
        }
    }
}


