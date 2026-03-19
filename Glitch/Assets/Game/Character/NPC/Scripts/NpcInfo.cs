using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NpcInfo : MonoBehaviour, IPointerClickHandler
{
    public Transform parent;

    public ICharacter character;

    public GameObject skillPanel;

    public TextMeshProUGUI physical;
    public TextMeshProUGUI fire;
    public TextMeshProUGUI lightning;
    public TextMeshProUGUI poison;
    public TextMeshProUGUI ice;
    public TextMeshProUGUI psychic;
    public TextMeshProUGUI divine;

    public TextMeshProUGUI traitTextbox;

    public Image characterImage;
    public TextMeshProUGUI npcName;
    public TextMeshProUGUI bgDesc;

    public DamageTypeEnumValue ph;
    public DamageTypeEnumValue di;

    public bool isHub;

    public bool useSpriteOverride;
    public Sprite spriteOverride;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(EnemyToken npc)
    {
        character = npc;
        npcName.text = npc.GetName();
        characterImage.sprite = npc.enemyNpc.enemySprite;
        bgDesc.text = npc.enemyNpc.GetBackground();
     
        physical.text = "0";
        if(npc.HasResistance("Physical")) physical.text = "100";
        fire.text = npc.GetBurningResistance() + "";
        lightning.text = npc.GetShockedesistance() + "";
        poison.text = npc.GetPoisonResistance() + "";
        ice.text = npc.GetFrozenResistance() + "";
        psychic.text = npc.GetVulnerableResistance() + "";
        divine.text = "0";
        if(npc.HasResistance("Divine")) divine.text = "100";
        ;

        ShowSkills(npc.enemyNpc.allAbilites.Distinct().ToList().OrderBy(s => s.GetName()).ToList());
        ShowTraits(npc.traits.Distinct().ToList().OrderBy(s => s.GetName()).ToList());
    }

    public void Setup(EnemyNpc npc)
    {
        npc.enemySystem.SetResistance(npc);
        npcName.text = npc.GetName();
        characterImage.sprite = npc.enemySprite;
        if (useSpriteOverride) characterImage.sprite = spriteOverride;
      
        if (npc.GetName().Equals("Dorian Gauntlet") 
              || npc.GetName().Equals("Dorian Guard")
              || npc.GetName().Equals("Dorian Gun")
              || npc.GetName().Equals("Dorian Spear")
              || npc.GetName().Equals("Dorian Sword"))
        {
            npcName.text = "Sir Dorian Gold, The Fool";
        }


        bgDesc.text = npc.background;

        physical.text = "0";
        if (npc.resistances.Contains(ph)) physical.text = "100";
        fire.text = npc.fireResist + "";
        lightning.text = npc.lightningResist + "";
        poison.text = npc.poisonResist + "";
        ice.text = npc.iceResist + "";
        psychic.text = npc.psychicResist + "";
        divine.text = "0";
        if (npc.resistances.Contains(di)) divine.text = "100";

        List<Special> allAbilites = new List<Special>();

        allAbilites.AddRange(npc.bossSpecials);
        allAbilites.AddRange(npc.attacks);
        allAbilites.Add(npc.specialAbility);
        allAbilites.Add(npc.deathAbility);
        allAbilites.RemoveAll(item => item == null);
        allAbilites = allAbilites.Distinct().ToList();
        allAbilites = allAbilites.OrderBy(s => s.GetName()).ToList();
        npc.traits.Distinct().ToList().OrderBy(s => s.GetName()).ToList();

        ShowSkills(allAbilites);
        ShowTraits(npc.traits);
    }

    public void Setup(InteractableNpc npc)
    {
        npcName.text = npc.name.Replace("(Clone)", "");
        characterImage.sprite = npc.npcSprite;

        bgDesc.text = npc.GetBackground();

        skillPanel.SetActive(false);

        physical.transform.parent.parent.parent.gameObject.SetActive(false);
      
    }

    private void ShowTraits(List<Trait> traits)
    {
        string traitString = "";
        foreach(Trait t in traits)
        {
            traitString += t.GetTraitDesc() + "\n";
        }
        traitTextbox.text = traitString;
    }

    private void ShowSkills(List<Special> abilities)
    {
        for(int x = 0; x < abilities.Count; x++)
        {
            skillPanel.transform.GetChild(x).gameObject.SetActive(true);
            string icon = "<sprite=" + GetAbilityTypeIcon(abilities[x].GetAbilityType()) + ">   ";
            string name = abilities[x].GetName().Replace("_", "");
            skillPanel.transform.GetChild(x).GetChild(0).GetComponent<TextMeshProUGUI>().text = icon + name;
            
        }
    }

    private int GetAbilityTypeIcon(AbilityTypes at)
    {
        switch(at)
        {
            case AbilityTypes.Offensive:
                return 26; 
            case AbilityTypes.Recovery:
                return 31;
            case AbilityTypes.Affliction:
                return 29;
            case AbilityTypes.Enchantment:
                return 27;
            case AbilityTypes.Summon:
                return 30;
        }
        return 26;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isHub)
        {
            gameObject.SetActive(false);
        } else
        {
            Destroy(gameObject);
        }
    }
}
