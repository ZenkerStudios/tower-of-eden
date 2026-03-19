using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttemptUI : MonoBehaviour
{
    private int attemptIndex;
    private Attempts thisAttempt;
    public TextMeshProUGUI attemptText;
    public HomeHub homeHub;
    public Animator logAnim;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ViewAttempt()
    {
        logAnim.Play("UpdateLog");
        homeHub.GetAttempt(thisAttempt, attemptIndex);
    }

    public void Initialize(int index, Attempts att)
    {
        thisAttempt = att;
        attemptIndex = index + 1;
        attemptText.text = "Attempt: #" + attemptIndex;
        if (thisAttempt.reachedTop && !thisAttempt.surrender)
        {
            attemptText.text += "   <sprite=33>";
        }
    }


}


[System.Serializable]
public class Attempts
{
    public int damageDealt;
    public int damageTaken;
    public int numberOfEncounters;
    public bool reachedTop;
    public bool surrender;
    public string killedByName;
    public Floors floor;
    public WeaponTypes weapon;
    public Difficulty difficulty;

    public List<string> abilitiesName = new List<string>();
    public List<string> abilitiesRank = new List<string>();
    public List<int> abilitiesSpriteIndex = new List<int>();

    public List<string> artifactsNames = new List<string>();
    public List<int> artifactsSpriteIndex = new List<int>();

    public string mod = "N/A";
    public List<string> enchanments = new List<string>();
    public List<string> defeatedEnemies = new List<string>();

    public Attempts()
    {
        numberOfEncounters = 0;
    }

    public void PartialUpdateAttempt(CombatSystem combatSystem)
    {
        damageDealt += combatSystem.totalDamageDealt;
        damageTaken += combatSystem.totalDamageTaken;
        defeatedEnemies.AddRange(combatSystem.defeatedEnemies);
        if(combatSystem.defeatedEnemies.Contains("Scraps"))
        {
            defeatedEnemies.Add("Scrap Fenix");
        }
        if(combatSystem.defeatedEnemies.Contains("Sir Dorian Gold, The Fool"))
        {
            defeatedEnemies.AddRange(combatSystem.dorianModesUsed);
        }

        floor = combatSystem.fm.floorCode;
        PlayerUpdateAttempt(combatSystem.player);

        numberOfEncounters++;
    }

    private void PlayerUpdateAttempt(PlayerManager player)
    {

        if ((int)floor > (int)player.highestFloorReached)
        {
            player.highestFloorReached = floor;
        }
        weapon = player.selectedWeaponType;
        difficulty = (Difficulty)player.chosenDifficulty;

        abilitiesName = new List<string>();
        abilitiesRank = new List<string>();
        abilitiesSpriteIndex = new List<int>();
        artifactsNames = new List<string>();
        artifactsSpriteIndex = new List<int>();

        abilitiesName.Add(player.h.baseAttack.GetName());
        abilitiesRank.Add(player.h.baseAttack.GetMasteryRank());
        abilitiesSpriteIndex.Add(player.h.baseAttack.abilitySpriteIndex);

        foreach (Special s in player.h.specials)
        {
            abilitiesName.Add(s.GetName());
            abilitiesRank.Add(s.GetMasteryRank());
            abilitiesSpriteIndex.Add(s.abilitySpriteIndex);
        }

        foreach (Artifact a in player.h.GetArtifacts())
        {
            artifactsNames.Add(a.GetName());
            artifactsSpriteIndex.Add(a.GetSpriteIndex());
        }

        if (player.UsingMod(player.selectedWeaponType))
        {
            mod = player.h.playerInventory.weaponMod.GetDesc();
        }
        enchanments = player.equippedBonuses;

    }

    public void FinalUpdate(CombatSystem combatSystem, bool won, bool quit)
    {

        killedByName = combatSystem.lastKilledBy;
        reachedTop = won;
        surrender = quit;
        PartialUpdateAttempt(combatSystem);
    }

    public void SurrenderFinalUpdate(PlayerManager player, bool won, bool quit, Floors currentFloor)
    {
        reachedTop = won;
        surrender = quit;
        floor = currentFloor;
        PlayerUpdateAttempt(player);
    }

    public string GetOutcome()
    {
        string outcome = "N/A";
        if (reachedTop && !surrender)
        {
            outcome = "Conquered Tower";
        }
        else if (!reachedTop && !surrender)
        {
            outcome = "Failed";
            switch (floor)
            {
                case Floors.Floor_01:
                    outcome = "Slain by " + killedByName;
                    break;
                case Floors.Floor_02:
                    outcome = "Disposed of by " + killedByName;
                    break;
                case Floors.Floor_03:
                    outcome = "Defeated by " + killedByName;
                    break;
                case Floors.Floor_04:
                    outcome = "Succumbed to " + killedByName;
                    break;
                case Floors.Floor_05:
                    outcome = "Slain by " + killedByName;
                    break;
            }
        }
        else if (!reachedTop && surrender)
        {
            return "Abandoned Tower";
        }

        return outcome;
    }
}
