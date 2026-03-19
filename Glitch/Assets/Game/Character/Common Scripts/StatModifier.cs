using System.Collections.Generic;


[System.Serializable]
public class StatModifier 
{
    public float value;
    public readonly StatModType statModType;
    public readonly int calcOrder;
    public readonly object modSource;
    public int duration;
    public bool toRemove = false;
    public string srcName;
    //Constructors
    public StatModifier(float val, StatModType type, int order, int dur, object src)
    {
        this.value = val;
        this.statModType = type;
        this.calcOrder = order;
        this.modSource = src;
        this.duration = dur;
        this.srcName = src.ToString();
    }

    public StatModifier(float val, StatModType type, object src) : this(val, type, (int)type, -1, src) { }
    
    public StatModifier(float val, StatModType type, int dur, object src) : this(val, type, (int)type, dur, src) { }


}
