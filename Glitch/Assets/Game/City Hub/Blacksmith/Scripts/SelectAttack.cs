using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class SelectAttack : MonoBehaviour, IPointerEnterHandler
{
    public WeaponTypes weaponType;
    public Artifact weaponArtifact;
    private PlayerManager player;
    public Image img;

    public TextMeshProUGUI weaponDescText;

    public BlacksmithHub blacksmithHub;

    private string weaponDesc;
    private string bonus;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
      
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Desc()
    {
        int bn = 0;
        switch (weaponType)
        {
            case WeaponTypes.Melancolia:
                weaponDesc = "The Warrior weapon class balances attack and defense for well-rounded combat.\n\n";
                bn = player.melancoliaWeaponRank > 1 ? player.swordBonus * player.melancoliaWeaponRank : 0;
                bonus = "Bonus: +" + bn + " <sprite=49> Toughness";
                break;
            case WeaponTypes.Delta:
                weaponDesc = "The Marksman weapon class specializes on targeting multiple opponents.\n\n";
                bn = player.deltaWeaponRank > 1 ? player.gunBonus * player.deltaWeaponRank : 0;
                bonus = "Bonus: +" + bn + " <sprite=47> Crit Chance";
                break;
            case WeaponTypes.Firestorm:
                weaponDesc = "The Mage weapon class focuses on elemental attacks and mastery.\n\n";
                bn = player.firestormWeaponRank > 1 ? player.staffBonus * player.firestormWeaponRank : 0;
                bonus = "Bonus: +" + bn + " Status Chance for <sprite=37> <sprite=38> <sprite=40> <sprite=41> <sprite=42> ";
                break;
            case WeaponTypes.Renegade:
                weaponDesc = "The Brawler weapon class focuses on multi-hit attacks.\n\n";
                bn = player.renegadeWeaponRank > 1 ? player.fistBonus * player.renegadeWeaponRank : 0;
                bonus = "Bonus: +" + bn + " Status Resist for <sprite=37> <sprite=38> <sprite=40> <sprite=41> <sprite=42> ";
                break;
        }
        weaponDescText.text = weaponDesc + bonus;

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SelectThisAttack()
    {

        player.selectedWeaponType = weaponType;
        blacksmithHub.SelectedWeaponIndicator();

        blacksmithHub.CheckMaxRanks();

    }

    public void Rankup()
    {

        switch (weaponType)
        {
            case WeaponTypes.Melancolia:
                player.ChangeScrapsAmount(-blacksmithHub.GetRankCost(player.melancoliaWeaponRank));
                player.melancoliaWeaponRank = Mathf.Clamp(player.melancoliaWeaponRank + 1, player.melancoliaWeaponRank, 5);
                break;
            case WeaponTypes.Delta:
                player.ChangeScrapsAmount(-blacksmithHub.GetRankCost(player.deltaWeaponRank));
                player.deltaWeaponRank = Mathf.Clamp(player.deltaWeaponRank + 1, player.deltaWeaponRank, 5);
                break;
            case WeaponTypes.Firestorm:
                player.ChangeScrapsAmount(-blacksmithHub.GetRankCost(player.firestormWeaponRank));
                player.firestormWeaponRank = Mathf.Clamp(player.firestormWeaponRank + 1, player.firestormWeaponRank, 5);
                break;
            case WeaponTypes.Renegade:
                player.ChangeScrapsAmount(-blacksmithHub.GetRankCost(player.renegadeWeaponRank));
                player.renegadeWeaponRank = Mathf.Clamp(player.renegadeWeaponRank + 1, player.renegadeWeaponRank, 5);
                break;
        }
        blacksmithHub.CheckMaxRanks();
        Desc();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Desc();
    }
}
