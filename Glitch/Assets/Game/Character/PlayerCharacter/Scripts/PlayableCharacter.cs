using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newPlayerHero", menuName = "Heroes/PlayableCharacter", order = 4)]
public class PlayableCharacter : ScriptableObject
{

    public Sprite characterSprite;

    public StatSystem maxHealth;

    public StatSystem accuracy;

    public StatSystem critChance;

    public StatSystem actions;

    public Attack baseAttack;

    public List<DamageTypeEnumValue> resistances;

    public int freezeFor = 1;
    public int poisonFor = 2;
    public int[] burningInfo = new int[2];
    public float[] vulnerableInfo = new float[2];
    public int[] shockedInfo = new int[2];
    public int[] damnationInfo = new int[4];

    public int iceResist;
    public int fireResist;
    public int poisonResist;
    public int psychicResist;
    public int lightningResist;

    public int iceStatus;
    public int fireStatus;
    public int poisonStatus;
    public int psychicStatus;
    public int lightningStatus;

    public StatSystem lifesteal;

    public StatSystem healChance;

    public StatSystem toughness;
    public InteractableNpc heroNpc;

    private void Awake()
    {
    }
}
