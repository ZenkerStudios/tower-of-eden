using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestArtifactNames : MonoBehaviour
{

    public List<Artifact> coArts = new List<Artifact>();
    public List<Artifact> unArts = new List<Artifact>();
    public List<Artifact> raArts = new List<Artifact>();
    public List<Artifact> leArts = new List<Artifact>();
    public List<Artifact> exArts = new List<Artifact>();

    public TextMeshProUGUI common;
    public TextMeshProUGUI uncommon;
    public TextMeshProUGUI rare;
    public TextMeshProUGUI legendary;
    public TextMeshProUGUI exalted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      

    }

    public void OnClick()
    {
        GameUtilities.SetTextColorForRarity(common, Rarity.Common);
        GameUtilities.SetTextColorForRarity(uncommon, Rarity.Uncommon);
        GameUtilities.SetTextColorForRarity(rare, Rarity.Rare);
        GameUtilities.SetTextColorForRarity(legendary, Rarity.Legendary);
        GameUtilities.SetTextColorForRarity(exalted, Rarity.Exalted);
        int index = 0;
        foreach (Artifact a in coArts)
        {
            common.text += index + ". " + a.GetName() + ": " + a.GetDesc() + ", " + a.rarity + "\n";
            index++;
        }
        foreach (Artifact a in unArts)
        {
            uncommon.text += index + ". " + a.GetName() + ": " + a.GetDesc() + ", " + a.rarity + "\n";
            index++;
        }
        foreach (Artifact a in raArts)
        {
            rare.text += index + ". " + a.GetName() + ": " + a.GetDesc() + ", " + a.rarity + "\n";
            index++;
        }
        foreach (Artifact a in leArts)
        {
            legendary.text += index + ". " + a.GetName() + ": " + a.GetDesc() + ", " + a.rarity + "\n";
            index++;
        }
        foreach (Artifact a in exArts)
        {
            exalted.text += index + ". " + a.GetName() + ": " + a.GetDesc() + ", " + a.rarity + "\n\n";
            index++;
        }
    }
}
