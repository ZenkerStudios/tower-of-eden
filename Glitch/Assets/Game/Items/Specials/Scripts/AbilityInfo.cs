using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AbilityInfo : MonoBehaviour
{
    private PlayerManager player;

    public WeaponTypes weaponType;
    public Attack attack;
    public Special special;

    public TextMeshProUGUI scrollSelectorTextBox;

    public TextMeshProUGUI typeTextBox;
    public TextMeshProUGUI nameTextBox;
    public TextMeshProUGUI descTextBox;
    public TextMeshProUGUI targetTextBox;
    public TextMeshProUGUI durationTextBox;
    public TextMeshProUGUI statusChanceTextBox;
    public Image abilityImage;

    private string abilityName;
    private string abilityDesc;
    private string abilityTarget;
    private int abilityDuration;
    private int abilityStatus;
    private Sprite abilitySprite;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

    }
    // Start is called before the first frame update
    void Start()
    {
        if(attack == null && special != null)
        {
            //Is special
            if(player.HasUsedAbility(special.GetName()))
            {
                abilityName = special.GetName();
                scrollSelectorTextBox.text = abilityName;
                abilityDesc = special.GetDesc();
                abilityTarget = "Targets: " + special.GetNumTarget().ToString();
                if (special.GetTargetTypes() == TargetTypes.OnSelf)
                {
                    abilityTarget = "Target: Self";
                }

                abilityStatus = special.GetStatusChance();

                abilityDuration = special.GetDuration();
            }
            else
            {
                scrollSelectorTextBox.text = "???";
                abilityName = "???";
                abilityDesc = "???";
                abilityTarget = "???";
                abilityDuration = 0;
                abilityStatus = 0;
            }
            abilitySprite = special.GetSprite();
        }
        else if (attack != null && special == null)
        {
            //Is attack
            if(player.HasUsedAbility(attack.GetName()))
            {
                abilityName = attack.GetName();
                scrollSelectorTextBox.text = abilityName;
                abilityDesc = attack.GetDesc();
                abilityTarget = "Targets: " + attack.GetNumTarget().ToString();
                if (attack.GetTargetTypes() == TargetTypes.OnSelf)
                {
                    abilityTarget = "Target: Self";
                }

                abilityStatus = attack.GetStatusChance();

                abilityDuration= attack.GetDuration();
            }
            else
            {
                scrollSelectorTextBox.text = "???";
                abilityName = "???";
                abilityDesc = "???";
                abilityTarget = "???";
                abilityDuration = 0;
                abilityStatus = 0;
            }
            abilitySprite = attack.GetSprite();
        }
        else
        {
            GameUtilities.ShowMessage(MessageLevel.Info, "No ability to show in codex");
        }

        GetComponent<Button>().onClick.AddListener(() => ShowInfo());

    }


    public void ShowInfo()
    {
        durationTextBox.gameObject.SetActive(abilityDuration > 0);
        statusChanceTextBox.gameObject.SetActive(abilityStatus > 0);

        typeTextBox.text = weaponType.ToString();
        nameTextBox.text = abilityName;
        descTextBox.text = abilityDesc;
        targetTextBox.text = abilityTarget;
        statusChanceTextBox.text = "Chance to apply condition: " + abilityStatus + "%";
        durationTextBox.text = "Duration: " + abilityDuration;
        abilityImage.sprite = abilitySprite;
        
        GameObject.FindGameObjectWithTag("Home").GetComponent<HomeHub>().viewingAbility = this;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
