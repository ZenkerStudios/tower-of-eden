using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactCodex : MonoBehaviour
{
    public Image artifactImg;
    public Artifact artifact;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    private PlayerManager player;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "???";
    }

    // Start is called before the first frame update
    void Start()
    {
        if(artifact != null)
        {
            if (player.HasUsedArtifact(artifact))
            {
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = artifact.GetName();
            }
        }
        else
        {
            GameUtilities.ShowMessage(MessageLevel.Info, "No artifact to show in codex");
        }
        GetComponent<Button>().onClick.AddListener(() => ShowInfo());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowInfo()
    {
        if (artifact != null)
        {
            if (player.HasUsedArtifact(artifact))
            {
                artifactImg.sprite = artifact.GetSprite();
                nameText.text = artifact.GetName();
                descText.text = artifact.GetDesc();
                GameObject.FindGameObjectWithTag("Home").GetComponent<HomeHub>().viewingArtifact = this;

            }
        }
    }
}
