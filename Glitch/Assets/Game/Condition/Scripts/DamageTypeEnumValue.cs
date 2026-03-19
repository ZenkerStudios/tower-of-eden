using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnumValue", menuName = "Variable/ConditionEnumValue", order = 2)]
public class DamageTypeEnumValue : ScriptableObject
{
    public Sprite sprite;
    public string spriteIndex;
    public string desc;
    public string condName;

    public string textColor;

    public GameObject token;
    public GameObject vfx;
    public string audio;

    
    public string GetConditionName()
    {
        return condName;
    }


    public string GetTypeName()
    {
        return name.Replace("(Clone)", "");
    }



}


