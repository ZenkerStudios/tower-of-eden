using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class AilmentItem : MonoBehaviour
{
    public TextMeshProUGUI conDescText;
    public TextMeshProUGUI taskText;
    public TextMeshProUGUI taskTracker;

    [SerializeField]
    private Ailment thisAilment;
    private PlayerManager player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        taskTracker.text = thisAilment.TrackTask();
        if(thisAilment.VerifyTask())
        {
            if (thisAilment.conIndex == 6 && thisAilment.artifactToDisable != null)
            {
                thisAilment.artifactToDisable.thisArtifact.EnableArtifact();
                player.h.GetMaxHealth();
                player.h.ChangeHealth(0);
            }
            player.h.playerInventory.ailments.Remove(thisAilment);
            Destroy(gameObject);
        }
    }

    public void AssignAilment(Ailment ail)
    {
        thisAilment = ail;
        conDescText.text = thisAilment.con;
        taskText.text = thisAilment.task;
    }

    public Ailment GetAilment()
    {
        return thisAilment;
    }    
}

[Serializable]
public class Ailment
{
    public string con;
    public string task;
    public int conIndex;
    public int taskIndex;

    public PlayerManager player;
    public Artifact artifactToDisable;


    public static string[] conDescriptions = new string[]
    {
        "Ailment: Do not gain health per encounter.",
        "Ailment: Gain only half health from infusion.",
        "Ailment: Do not gain gold per encounter.",
        "Ailment: Cannot pick up new ability with grail of power.",
        "Ailment: Cannot upgrade abilities with mastery rune.",
        "Ailment: Attacks do 50% less damage at 30% health",
        "Ailment: Random artifact disabled.",
        "Ailment: Reduce max Skills in your loadout to current amount.",
        "Ailment: Cannot use consumables.",
        "Ailment: Shop items cost more."
    };

    public static string[] taskDescription = new string[]
    {
        "Task: Kill 10 enemies.",
        "Task: Buy a consumable.",
        "Task: Buy an ability.",
        "Task: Buy an artifact.",
        "Task: Win an elite enconter.",
        "Task: Win any encounter.",
        "Task: Collet 35 metal scraps.",
        "Task: Collect 45 gems.",
        "Task: Collect 2 vial.",
        "Task: Collect 1 Shard."
    };

    public int enemyTarget = 10;
    public int scrapTarget = 35;
    public int gemTarget = 45;
    public int vialTarget = 2;
    public int shardTarget = 1;
    public int maxAbility = 8;

    public Ailment(PlayerManager pm, bool difficult)
    {
        player = pm;
        conIndex = UnityEngine.Random.Range(0, conDescriptions.Length);
        taskIndex = UnityEngine.Random.Range(0, taskDescription.Length);

        con = conDescriptions[conIndex];
        task = taskDescription[taskIndex];

        if(difficult)
        {
            enemyTarget = 15;
            task = task.Replace("10", "15");
            scrapTarget = 45;
            task = task.Replace("35", "45");
            gemTarget = 65;
            task = task.Replace("45", "65");
            vialTarget = 3;
            task = task.Replace("2", "3");
            shardTarget = 2;
            task = task.Replace("1", "2");
        }

        if (conIndex == 6)
        {
            artifactToDisable = player.h.playerInventory.GetRandomArtifact();
            if(artifactToDisable != null)
            {
                artifactToDisable.thisArtifact.DisablePassiveStat();
                player.h.GetMaxHealth();
                player.h.ChangeHealth(0);
            }
        }

        if(conIndex == 7)
        {
            List<Ailment> existingAilments = pm.h.playerInventory.ailments.Where(a => a.conIndex == 7).ToList();

            maxAbility = pm.h.specials.Count;
            if(existingAilments.Count > 0)
            {
                maxAbility = existingAilments[0].maxAbility;
            }
        }
    }



    public int enemiesKilled;
    public bool consumableBought;
    public bool abilityBought;
    public bool artifactBought;
    public bool beatMiniBoss;
    public bool wonEncounter;
    public int scrapsCollected;
    public int gemsCollected;
    public int vialCollected;
    public int shardCollected;

    public bool VerifyTask()
    {
        switch(taskIndex)
        {
            case 0:
                return enemiesKilled >= enemyTarget;
            case 1:
                return consumableBought;
            case 2:
                return abilityBought;
            case 3:
                return artifactBought;
            case 4:
                return beatMiniBoss;
            case 5:
                return wonEncounter;
            case 6:
                return scrapsCollected >= scrapTarget;
            case 7:
                return gemsCollected >= gemTarget;
            case 8:
                return vialCollected >= vialTarget;
            case 9:
                return shardCollected >= shardTarget;
            default:
                return false;
        }
    }

    public string TrackTask()
    {
        switch (taskIndex)
        {
            case 0:
                return enemiesKilled + "/" + enemyTarget;
            case 1:
                return Convert.ToInt32(consumableBought)+ "/" + 1;
            case 2:
                return Convert.ToInt32(abilityBought) + "/" + 1;
            case 3:
                return Convert.ToInt32(artifactBought) + "/" + 1;
            case 4:
                return Convert.ToInt32(beatMiniBoss) + "/" + 1;
            case 5:
                return Convert.ToInt32(wonEncounter) + "/" + 1;
            case 6:
                return scrapsCollected + "/" + scrapTarget;
            case 7:
                return gemsCollected + "/" + gemTarget;
            case 8:
                return vialCollected + "/" + vialTarget;
            case 9:
                return shardCollected + "/" + shardTarget;
            default:
                return "*/*";
        }
    }
}