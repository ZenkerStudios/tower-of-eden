using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopHub : MonoBehaviour
{

    public GameObject hubWindow;
    public Animator windowAnimator;
    private PlayerManager player;

    [SerializeField]
    protected FriendlyToken friendlyToken;
    private DialogueManager dialogueManager;
    public Scrollbar scrollbar;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        dialogueManager = DialogueManager.instance;
        friendlyToken.urgentSymbol.SetActive(false);

        bool active = (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().GetTotalAttempts() > 10);
        gameObject.GetComponent<ButtonHover>().isInteractable = active;
        transform.GetChild(0).gameObject.GetComponent<Button>().interactable = active;

        if (!active && friendlyToken.nextDialogue != null && friendlyToken.nextDialogue.priority != DialoguePriority.Critical && Random.Range(0, 100) < 30)
        {
            friendlyToken.nextDialogue = null;
            friendlyToken.urgentSymbol.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenWindow()
    {
        scrollbar.value = 1;
        AudioManager.instance.PlaySfxSound("Button_Click");
        if (player.wentShopkeepThisRun)
        {
            ShowWindow();
        }
        else
        {
            player.wentShopkeepThisRun = true;
            if (friendlyToken.TriggerVisitDialogue())
            {
                StartCoroutine(GameUtilities.WaitForConversation(() => ShowWindow()));
            }
            else
            {
                ShowWindow();
            }

            player.numShopkeepInteraction++;
        }

    }

    public void ShowWindow()
    {

        //hubWindow.SetActive(true);
        //play open animation
        windowAnimator.Play("Shop Open");
        if (player.savedDialogueConditionsMet.Contains(DialogueConditions.UnlockShop) && Random.Range(0, 100) < friendlyToken.interactableNpc.barkChance)
        {
            DialogueBlock bark = friendlyToken.interactableNpc.GetRandomBark();
            if (bark != null)
            {
                dialogueManager.ShowBarkWithOwner(
               friendlyToken.interactableNpc, friendlyToken.characterImage, bark);
            }

        }
    }    

    public void CloseWindow()
    {
        //play close animation 
        windowAnimator.Play("Shop Close");

    }
}
