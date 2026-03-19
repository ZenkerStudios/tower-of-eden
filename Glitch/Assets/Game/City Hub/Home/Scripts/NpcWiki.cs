using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcWiki : MonoBehaviour
{
    public EnemyNpc enemyCharacter;
    public InteractableNpc friendlyCharacter;

    public NpcInfo npcInfoWindow;


    public bool useSpriteOverride;
    public Sprite spriteOverride;
    
    public Sprite headshot;
    public Image headshotContainer;
    public TextMeshProUGUI nameBox;
    public TextMeshProUGUI descBox;


    // Start is called before the first frame update
    void Start()
    {
        npcInfoWindow.gameObject.SetActive(false);
        npcInfoWindow.isHub = true;
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    public string GetName()
    {
        if (enemyCharacter != null)
        {
            return enemyCharacter.GetName();
        }
        else if (friendlyCharacter != null)
        {
            return friendlyCharacter.GetCharacterName();
        }
        return "???";
    }

    public void ShowInfo()
    {
        nameBox.text = GetName();
        descBox.text = enemyCharacter.background;
        headshotContainer.sprite = headshot;
        GameObject.FindGameObjectWithTag("Home").GetComponent<HomeHub>().viewingNpc = this;
        descBox.transform.parent.parent.GetChild(3).gameObject.SetActive(true);

    }

    public void ExpandInfo()
    {
        npcInfoWindow.useSpriteOverride = useSpriteOverride;
        npcInfoWindow.spriteOverride = spriteOverride;
        if(enemyCharacter != null)
        {
            //Show enemy info
            npcInfoWindow.Setup(enemyCharacter);
        } else if (friendlyCharacter != null)
        {
            //Show friendly info
            npcInfoWindow.Setup(friendlyCharacter);
        }
        npcInfoWindow.gameObject.SetActive(true);
    }
}
