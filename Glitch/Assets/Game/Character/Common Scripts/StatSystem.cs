using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[Serializable]
public class StatSystem
{
    //Stat info
    public float baseValue;
    public float maxValue;
    public float minValue;

    //Calculates and returns value for this stat
    public virtual float Value
    {
        get
        {
            if(modChanged || baseValue != lastBaseVal)
            {
                lastBaseVal = baseValue;
                oldVal = CalculateFinalValue();
                modChanged = false;
            }
            return Mathf.Clamp(oldVal, minValue, maxValue);
        }
    }

    protected float lastBaseVal = float.MinValue;
    protected bool modChanged = true;
    protected float oldVal;

    //Modifiers for stats
    public List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> publicStatModList;
    protected int maxModifiers = 1;

    //Constructors
    public StatSystem()
    {
        this.statModifiers = new List<StatModifier>();
        this.publicStatModList = statModifiers.AsReadOnly();
        this.maxValue = 100;
        this.minValue = 0;
    }

    public StatSystem(StatSystem n)
    {
        this.baseValue = n.baseValue;
        this.statModifiers = new List<StatModifier>(n.statModifiers);
        this.publicStatModList = statModifiers.AsReadOnly();
        this.maxValue = n.maxValue;
        this.minValue = n.minValue;
    }

    public StatSystem(float baseVal, float min, float max) : this() 
    {
        this.minValue = min;
        this.maxValue = max;
        this.baseValue = baseVal;
    }

    //Add aa modifier to the list of modifiers
    public virtual void AddModifier(StatModifier mod)
    {
        statModifiers.RemoveAll(s => s.modSource == mod.modSource);

        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
        modChanged = true;
    }

    //Remove modifier from list of modifiers
    public virtual bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            modChanged = true;
            return true;
        }
        return false;
    }

    //Remove all modifiers with the same source
    public virtual bool RemoveAllModFromSource(object src)
    {
        bool removed = false;
        for(int x = statModifiers.Count - 1; x >= 0; x--)
        {
            if(statModifiers[x].modSource == src)
            {
                modChanged = true;
                removed = true;
                statModifiers.RemoveAt(x);
            }
        }
        return removed;
    }

    public void SetModChangeTrue()
    {
        modChanged = true;
    }

    //Compares modifiers for ordering
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.calcOrder < b.calcOrder)
        {
            return -1;
        } else if (a.calcOrder > b.calcOrder)
        {
            return 1;
        }
        return 0; //Same order a == b
    }

    //Calculate final value for this stat with all modifiers applied
    public virtual float CalculateFinalValue()
    {
        float finalVal = baseValue;
        
        foreach (StatModifier statMod in statModifiers)
        {
            switch (statMod.statModType)
            {
                case StatModType.Flat:
                    //Adds x to flat value
                    finalVal += statMod.value;
                    break;
                case StatModType.PercentageIncrease:
                    //Increase flat value by x percent
                    finalVal *= 1 + statMod.value/100;
                    break;
                case StatModType.PercentageMult:
                    //Multiplies percentage value by x
                    finalVal *= 1 + statMod.value;
                    break;
            }
        }

        return (float)Math.Round(finalVal, 2);
    }
}
