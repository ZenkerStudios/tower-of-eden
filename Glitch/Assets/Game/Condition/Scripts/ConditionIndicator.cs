using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConditionIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private DamageTypeEnumValue dt;
    private int turn;
    private int stacks;
    private int max;
    private float effect;

    public TextMeshProUGUI descTxt;

    public CombatSystem combatSystem;
    public TextMeshProUGUI condSprite;
    private ICharacter owner;
    private string description;
    public string condName;

    private void OnDestroy()
    {
        combatSystem.DisableDesc();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        descTxt.text = description;

        if (dt != null)
        {
            string s = dt.desc;
            descTxt.text = s.Replace("/max", max + "");
            Color c;
            ColorUtility.TryParseHtmlString("#" + dt.textColor, out c);
            descTxt.color = c;
            descTxt.text = descTxt.text.Replace("/trn", turn + "");

            if (stacks > 0)
            {
                descTxt.text = descTxt.text.Replace("/stk", stacks+"");
            }

            if (effect > 0)
            {
                descTxt.text = descTxt.text.Replace("/efx", effect + "");
          
            }
        }
        else if (condName.Equals("Regenerating"))
        {
            descTxt.color = new Color32(0, 255, 0, 255);
            descTxt.text = description.Replace("/trn", turn + "");
        }
        else if (condName.Equals("Verse"))
        {
            descTxt.color = new Color32(255, 255, 255, 255);
            descTxt.text = description.Replace("/efx", effect + "");
        }
        else if (condName.Equals("Seal"))
        {
            descTxt.color = new Color32(255, 255, 255, 255);
            descTxt.text = description.Replace("/efx", effect + "");
        }
        else if (condName.Equals("Apotheosis"))
        {
            descTxt.color = new Color32(255, 255, 255, 255);
            descTxt.text = description.Replace("/trn", turn + "");
        }

        combatSystem.SetConditionDesc();
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        combatSystem.DisableDesc();
    }

    // Start is called before the first frame update
    public void Init(DamageTypeEnumValue damageType, ICharacter target)
    {
        dt = damageType;
        condName = dt.GetConditionName();
        owner = target;
        condSprite.text = "<sprite=" + damageType.spriteIndex + ">";
    }

    // Start is called before the first frame update
    public void Init(string name, string desc, ICharacter target, string spIndex)
    {
        description = desc;
        condName = name;
        owner = target;
        condSprite.text = " <sprite=" + spIndex + ">";
    }

    private void Start()
    {
        combatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        descTxt = combatSystem.conditionDescription.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (condName.Equals("Regenerating"))
        {
            turn = owner.GetHealthRegen()[0];
            if (turn <= 0)
            {
                Destroy(this.gameObject);
            }
        }
        
        if (condName.Equals("Verse"))
        {
            effect = combatSystem.verse;
        }

        if (condName.Equals("Seal"))
        {
            effect = combatSystem.seal;
        }

        if (condName.Equals("Apotheosis"))
        {
            turn = combatSystem.totalApotheosisTurn;
            if (turn <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        if (dt != null)
        {
            if (dt.name.Equals("Ice"))
            {
                turn = owner.GetIsFrozen();
                stacks = 0;
                effect = 0;
            } else if (dt.name.Equals("Poison"))
            {
                turn = owner.GetIsPoisoned();
                stacks = 0;
                effect = 0;
            }
            else if (dt.name.Equals("Fire"))
            {
                turn = owner.GetIsBurning()[0];
                stacks = 0;
                effect = owner.GetIsBurning()[1];
            }
            else if (dt.name.Equals("Psychic"))
            {
                turn = (int)owner.GetIsVulnerable()[0];
                stacks = 0;
                effect = owner.GetIsVulnerable()[1];
            }
            else if (dt.name.Equals("Divine"))
            {
                turn = owner.GetDamnation()[0];
                stacks = owner.GetDamnation()[1];
                effect = owner.GetDamnation()[2];
                max = owner.GetDamnation()[3];
            }
            else if (dt.name.Equals("Lightning"))
            {
                turn = owner.GetIsShocked()[0];
                stacks = 0;
                effect = owner.GetIsShocked()[1];
            }

            if (turn <= 0)
            {
                Destroy(this.gameObject);
            }
        }

    }

}

