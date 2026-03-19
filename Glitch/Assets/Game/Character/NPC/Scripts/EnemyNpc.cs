using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyNpc", menuName = "Enemies/EnemyNpc", order = 2)]
public class EnemyNpc : ScriptableObject
{
    [SerializeField]
    [TextArea(5, 10)]
    public string background;

    public bool isBoss;

    public bool isInteractable;

    public Sprite enemySprite;

    public Sprite enemyIcon;

    public EnemySystem enemySystem;

    public EnemyEnumValue _myType;

    public int block;

    public int maxMeterGain;

    public StatSystem maxHealth;

    public StatSystem accuracy;

    public StatSystem critChance;

    public StatSystem actions;

    public int startingSpecialMeter = 0;

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

    public List<DamageTypeEnumValue> resistances;

    public List<Special> attacks;

    public List<Special> bossSpecials;

    public Special specialAbility;

    public List<EnemyNpc> allyTokens;

    public int maxNumAllies;

    public bool abilityAfterDeath;

    public Special deathAbility;

    public bool dazedResAbility;

    public bool echoAbillity;

    public EnemyNpc echoTokens;

    public bool canRevive;

    public EnemyNpc reviveTokens;

    public bool isTransforming;

    public int transformationTurn;

    public bool abilityOnSelfDestruct;

    public bool canSelfDestruct;

    public int selfDestructTurn;

    public InteractableNpc interactableNpc;

    public List<Special> allAbilites = new List<Special>();
    public List<Trait> traits;

    private void Awake()
    {
       
    }

    private void OnDestroy()
    {
    }

    public string GetName()
    {
        string nameStr = name.Replace("(Clone)", "");
        nameStr = nameStr.Replace(" 1", "");
        nameStr = nameStr.Replace(" 2", "");
        return nameStr;
    }

    public string GetBackground()
    {
        return background;
    }
}
