using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdmirationSystem 
{
    protected readonly int minRank = 0;
    protected readonly int maxRank = 20;

    public int currentRank = 0;

    public AdmirationSystem(int maxAdmirationRank)
    {
        maxRank = maxAdmirationRank;
    }

    public AdmirationSystem(AdmirationSystem a) : this(a.maxRank) { currentRank = a.currentRank; }

}
