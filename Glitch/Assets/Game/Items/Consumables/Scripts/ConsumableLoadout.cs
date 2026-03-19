using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConsumableLoadout : MonoBehaviour, IPointerEnterHandler
{
    public Image consImg;
    public List<Sprite> consumableSprite;

    private TextMeshProUGUI descText;
    private PlayerManager player;
    private Consumable consumable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void Init(PlayerManager pm, Consumable cons)
    {
        consumable = cons;
        player = pm;

        descText = player.description;
        consImg.sprite = consumableSprite[consumable.consumableIntValue];

        if (consumable.consumableIntValue > 4 && consumable.totalTurns <= 0)
        {
            player.h.playerInventory.combatShopConsumables.Remove(consumable);
            consumable.DisableConsumable();
            Destroy(gameObject);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        player.statusChance.gameObject.SetActive(false);
        player.turns.gameObject.SetActive(false);
        player.description.text = "";
        player.abilityname.text = "";
        player.turns.text = "";
        player.statusChance.text = "";
        player.numTarget.text = "";

        descText.text = consumable.GetDesc(1);
        if(player.h.playerInventory.ailments.Exists(ail => ail.conIndex == 8))
        {
            descText.text = consumable.GetDesc(0);
        }
    }
}
