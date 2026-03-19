using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIDisplayArtifact : MonoBehaviour, IPointerEnterHandler
{
    public Image artifactImg;
    public Artifact artifact;
    public ArtifactItem artifactItem;

    private PlayerManager player;


    public void Initialize(Artifact a, PlayerManager pm)
    {
        artifact = a;
        artifactItem = a.thisArtifact;
        player = pm;
        artifactImg.sprite = artifact.GetSprite();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!player.inBattle) return;
        player.abilityRank.gameObject.SetActive(false);
        player.statusChance.gameObject.SetActive(false);
        player.turns.gameObject.SetActive(false);
        player.description.text = "";
        player.turns.text = "";
        player.statusChance.text = "";
        player.numTarget.text = "";


        player.abilityname.text = artifact.GetName();
        player.description.text = artifact.GetDesc();

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (player.inBattle)
        {
            artifactImg.color = new Color32(255, 255, 255, 255);

            if (artifactItem.isDisabled)
            {
                artifactImg.color = new Color32(175, 175, 175, 255);

            }
        }

    }
}
