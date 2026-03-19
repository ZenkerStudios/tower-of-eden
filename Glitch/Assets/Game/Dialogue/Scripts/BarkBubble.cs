using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarkBubble : MonoBehaviour
{
    public TextMeshProUGUI messageTextBox;
    public Image bubble;
    public List<Sprite> sprites;
    private int[] xPos = new int[] { -200, 0, 200};
    private int[] yPos = new int[] { 200, 300, 200};

    public void Init(InteractableNpc ownerNpc, DialogueBlock bark)
    {
        int index = Random.Range(0, 3);
        switch (ownerNpc.thisNpc)
        {
            case InteractableNpcs.Blacksmith:
            case InteractableNpcs.Church:
            case InteractableNpcs.Academy:
            case InteractableNpcs.Shop:
                index = 0;
                xPos = new int[] { -450 };
                yPos = new int[] { 600 };
                break;
            case InteractableNpcs.None:
            case InteractableNpcs.Empress:
            case InteractableNpcs.Magician:
            case InteractableNpcs.HangedMan:
            case InteractableNpcs.Fool:
            case InteractableNpcs.Priestess:
            case InteractableNpcs.Star:
            case InteractableNpcs.King:
                break;
            case InteractableNpcs.Izaak:
                xPos = new int[] { 300, 300, 300, 300, 300 };
                yPos = new int[] { 400, 400, 400, 400, 400 };
                index = 3;
                break;
            case InteractableNpcs.Eden:
                xPos = new int[] { 250, 250, 250, 250, 250 };
                yPos = new int[] { 200, 200, 200, 200, 200 };
                index = 4;
                break;


        }

        bubble.sprite = sprites[index];
        transform.localPosition = new Vector2(xPos[index], yPos[index]);

        messageTextBox.text = bark.dialogueBlock;
        StartCoroutine(CountdownToClose(ownerNpc, bark));
    }

    public IEnumerator CountdownToClose(InteractableNpc ownerNpc, DialogueBlock bark)
    {
        yield return new WaitForSeconds(bark.barkTimer);
        ownerNpc.isBarking = false;
        bark.BarkUsed();
        Destroy(gameObject);
    }

}
