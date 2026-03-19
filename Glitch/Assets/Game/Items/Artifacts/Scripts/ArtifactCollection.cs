using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[DefaultExecutionOrder(-45)]
public class ArtifactCollection : MonoBehaviour
{
    private List<Artifact> allArtifacts = new List<Artifact>();
    public List<Artifact> selectedArtifacts = new List<Artifact>();
    public List<Artifact> tier1Artifacts;
    public List<Artifact> tier2Artifacts;
    public List<Artifact> tier3Artifacts;
    public List<Artifact> tier4Artifacts;
    public List<Artifact> tier5Artifacts;

    public List<Artifact> tier1ArtifactsReadonly;
    public List<Artifact> tier2ArtifactsReadonly;
    public List<Artifact> tier3ArtifactsReadonly;
    public List<Artifact> tier4ArtifactsReadonly;
    public List<Artifact> tier5ArtifactsReadonly;


    private void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
        EmptyArtifacts();
        allArtifacts.AddRange(tier1Artifacts);
        allArtifacts.AddRange(tier2Artifacts);
        allArtifacts.AddRange(tier3Artifacts);
        allArtifacts.AddRange(tier4Artifacts);
        allArtifacts.AddRange(tier5Artifacts);
    }
    // Start is called before the first frame update
    void Start()
    {
        tier1ArtifactsReadonly = new List<Artifact>(tier1Artifacts);
        tier2ArtifactsReadonly = new List<Artifact>(tier2Artifacts);
        tier3ArtifactsReadonly = new List<Artifact>(tier3Artifacts);
        tier4ArtifactsReadonly = new List<Artifact>(tier4Artifacts);
        tier5ArtifactsReadonly = new List<Artifact>(tier5Artifacts);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EmptyArtifacts()
    {
        allArtifacts = new List<Artifact>();
    }

    public void ResetArtifacts()
    {
        tier1Artifacts = new List<Artifact>(tier1ArtifactsReadonly);
        tier2Artifacts = new List<Artifact>(tier2ArtifactsReadonly);
        tier3Artifacts = new List<Artifact>(tier3ArtifactsReadonly);
        tier4Artifacts = new List<Artifact>(tier4ArtifactsReadonly);
        tier5Artifacts = new List<Artifact>(tier5ArtifactsReadonly);
        Awake();

    }

    private List<Artifact> GetNextAvailableTier()
    {
        if(tier1Artifacts.Count > 0)
        {
            return tier1Artifacts;
        }
        else if (tier2Artifacts.Count > 0)
        {
            return tier2Artifacts;
        }
        else if (tier3Artifacts.Count > 0)
        {
            return tier3Artifacts;
        }
        else if (tier4Artifacts.Count > 0)
        {
            return tier4Artifacts;
        } 
        throw new System.Exception();
    }

    private Artifact GetItem(List<Artifact> list)
    {
        try
        {
            if(list.Count <= 0)
            {
                list = GetNextAvailableTier();
            }
            int randomIndex = Random.Range(0, list.Count);
            Artifact a = list[randomIndex];
            list.Remove(a);
            selectedArtifacts.Add(a);
            return a;
        } catch (System.Exception)
        {
            throw new System.Exception();
        }

    }


    public Artifact GetLoot(int rand)
    {
        /*
                 * 51: Common
                 * 31: Uncommon
                 * 11: Rare
                 * 7: Legendary
                 */
        //rand = 100;
        if(rand <= 51)
        {
            return GetItem(tier1Artifacts);
        }
        else if (rand > 51 && rand <= 82)
        {
            return GetItem(tier2Artifacts);
        }
        else if (rand > 82 && rand <= 93)
        {
            return GetItem(tier3Artifacts);
        }
        else if (rand > 93)
        {
            return GetItem(tier4Artifacts);
        }
        else
        {
            throw new System.Exception();
        }
    }

    public Artifact GetLoot(Rarity rarity)
    {
        /*
                 * 51: Common
                 * 31: Uncommon
                 * 11: Rare
                 * 7: Legendary
                 */

        if (rarity == Rarity.Common)
        {
            return GetItem(tier1Artifacts);
        }
        else if (rarity == Rarity.Uncommon)
        {
            return GetItem(tier2Artifacts);
        }
        else if (rarity == Rarity.Rare)
        {
            return GetItem(tier3Artifacts);
        }
        else if (rarity == Rarity.Legendary)
        {
            return GetItem(tier4Artifacts);
        }
        else
        {
            throw new System.Exception();
        }
    }

    public void ResetArtifactsNotSelected()
    {
        foreach(Artifact a in selectedArtifacts)
        {
            switch (a.rarity)
            {
                case Rarity.Common:
                    tier1Artifacts.Add(a);
                    break;
                case Rarity.Uncommon:
                    tier2Artifacts.Add(a);
                    break;
                case Rarity.Rare:
                    tier3Artifacts.Add(a);
                    break;
                case Rarity.Legendary:
                    tier4Artifacts.Add(a);
                    break;
                case Rarity.Exalted:
                    tier5Artifacts.Add(a);
                    break;
            }
        }
        selectedArtifacts = new List<Artifact>();
    }

    public void DestroyOnRunFinish()
    {
        Destroy(transform.parent.gameObject);
    }
}
