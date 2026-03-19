using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class VictorySplash : MonoBehaviour
{
    private PlayerManager player;
    public GameObject artifactDisplayPanel;
    public GameObject abilityDisplayPanel;
    public UiDisplayAbility baseAttackDisplay;

    public TextMeshProUGUI totalClear;
    public TextMeshProUGUI currentStreak;
    public TextMeshProUGUI bestStreak;

    public TextMeshProUGUI diff;
    public TextMeshProUGUI damageDealt;
    public TextMeshProUGUI damageTaken;

    public TextMeshProUGUI academy;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        player.inBattle = false;
        Attempts attempt = player.attempts.Last();

        totalClear.text = player.GetSuccessfulAttempts() + "";
        currentStreak.text = player.GetCurrentStreak() + 1 + "";
        bestStreak.text = player.bestStreak + "";

        diff.text = ((Difficulty)player.chosenDifficulty).ToString();
        damageDealt.text = Mathf.Abs(attempt.damageDealt) + "";
        damageTaken.text = Mathf.Abs(attempt.damageTaken) + "";

        foreach (Artifact a in player.h.GetArtifacts())
        {
            var newObj = Instantiate(player.pfDisplayArtifact);
            newObj.GetComponent<UIDisplayArtifact>().Initialize(a, player);
            newObj.transform.SetParent(artifactDisplayPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = player.pfDisplayArtifact.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = player.pfDisplayArtifact.GetComponent<RectTransform>().localScale;
        }

        baseAttackDisplay.Initialize(player.h.baseAttack, player);
        foreach (IAbility a in player.h.specials)
        {
            var newObj = Instantiate(player.pfDisplayAbility);
            newObj.GetComponent<UiDisplayAbility>().Initialize(a, player);
            newObj.transform.SetParent(abilityDisplayPanel.transform);
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = player.pfDisplayAbility.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = player.pfDisplayAbility.GetComponent<RectTransform>().localScale;
        }

        
        for (int x = 0; x < attempt.enchanments.Count; x++)
        {
            academy.text += "<sprite=67> " +  attempt.enchanments[x] + "\n";
        }


       

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
