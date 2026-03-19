using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(-46)]
public class AddArtifact : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerManager player;
    public RewardSystem rewardSystem;
    public Artifact artifact;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;
    public Image itemIcon;
    public Image imgFrame;
    public Image itemBg;
    public List<Sprite> bgRarity;
    public List<Sprite> frameRarity;
    public GameObject indicator;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        indicator.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Image>().material = GameUtilities.instance.GetColorForRarity(artifact.rarity);
        itemBg.sprite = bgRarity[(int)artifact.rarity - 2];
        imgFrame.sprite = frameRarity[(int)artifact.rarity - 2];
        itemDesc.text = artifact.GetDesc();
        itemName.text = artifact.GetName();
        itemIcon.sprite = artifact.GetSprite();
        GameUtilities.SetTextColorForRarity(itemName, artifact.rarity);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectThisArtifact()
    {
        player.AddArtifact(artifact);
        GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().selectedArtifacts.Remove(artifact);
        GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().ResetArtifactsNotSelected();
        rewardSystem.ReturnToEncounterSelect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        indicator.SetActive(true);

        if (!pulsating)
        {
            float timer = 0.0025f;
            StartCoroutine(GameUtilities.PulsateUiObject(indicator, timer, 0.02f, 1.1f, 1f, 1f));
            pulsating = true;
            StartCoroutine(PulseTime(0.3f + timer));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        indicator.SetActive(false);
    }

    private bool pulsating = false;

    IEnumerator PulseTime(float timer)
    {
        yield return new WaitForSeconds(timer);
        pulsating = false;

    }
}
