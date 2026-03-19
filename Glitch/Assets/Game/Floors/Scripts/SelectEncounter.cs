using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectEncounter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerManager player;
    public GameObject difficultyImg;
    public Image rewardImg;
    public Image rewardDescImg;
    public TextMeshProUGUI desc;
    public Floors encounterKey;
    public Reward rewardType;
    public Difficulty difficulty;
    public List<EnemyNpc> enc = new List<EnemyNpc>();
    public bool isDifficult;

    private GameObject panel;
    private GameObject encSelect;
    [SerializeField]
    protected GameObject glow;
    private FloorManager fm;

    private void Awake()
    {
        glow.SetActive(false);
        fm = GameObject.Find("Floor Manager").GetComponent<FloorManager>();
        panel = GameObject.Find("Floor Manager").GetComponent<FloorManager>().encounterPanel;
        encSelect = GameObject.Find("Floor Manager").GetComponent<FloorManager>().encounterSelect;
    }

    // Start is called before the first frame update
    void Start()
    {
        rewardDescImg.transform.parent.gameObject.SetActive(false);

        difficultyImg.SetActive(false);
        if (isDifficult)
        {
            difficultyImg.SetActive(true);
            int newDiff = Mathf.Clamp((int)difficulty + 1, 1, 4);
            difficulty = (Difficulty)newDiff;
        }
        desc.text = rewardType.GetDesc(rewardType.reward)[0] + "\n\n" + rewardType.GetDesc(rewardType.reward)[1];
        rewardDescImg.sprite = rewardType.highlightImg;
    }

    private float descTimer;
    private float descWaitForTimer = 1.5f;
    private bool waitForDesc = false;
    // Update is called once per frame
    void Update()
    {
        if (waitForDesc && (descTimer + descWaitForTimer <= Time.time))
        {
            rewardDescImg.transform.parent.gameObject.SetActive(true);
        }
    }

    IEnumerator WaitForTransition()
    {
        yield return new WaitForSeconds(1f);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        fm.SetCurrentRewardSprite(rewardType.reward);
        bool uniqueCheck1 = (player.h.playerInventory.alwaysShowUniqueRoom) || (Random.Range(1, 101) <= 20);
        bool uniqueCheck3 = fm.uniqueRoomEligible;
        bool uniqueCheck2 = fm.encountersWon > 5;
        if (rewardType.reward == RewardType.Shop)
        {
            GameController.instance.OpenShop(difficulty);
        }
        else if (uniqueCheck1 && uniqueCheck2 && uniqueCheck3 && !isDifficult)
        {
            if (rewardType.reward == RewardType.MaxHealth)
            {
                fm.AddInfusion(1);
            }

            fm.uniqueRoomEligible = false;
            GameController.instance.UniqueRoom(encounterKey, difficulty, rewardType);
        }
        else
        {
            if(isDifficult && fm.IsBossBattle())
            {
                if(encounterKey == Floors.Floor_02)
                {
                    fm.GetComponent<FogController>().ToggleBossFog(fm.floorEventTriggered);
                } else if(encounterKey == Floors.Floor_04)
                {
                    fm.battleBg.transform.GetChild(2).GetComponent<Animator>().Play("Fog");
                }
                fm.PlayBossMusic();
            } 

            if (encounterKey == Floors.Floor_02)
            {
                fm.GetComponent<FogController>().ToggleHallwayFog(fm.floorEventTriggered, fm.encountersWon >= fm.encountersToMiniBoss);
            }
            GameController.instance.StartEncounter(encounterKey, enc, rewardType, difficulty, isDifficult);
        }
        encSelect.SetActive(false);
        GameUtilities.DeleteAllChildGameObject(panel);
    }

    public void SelectThisEncounter()
    {
        fm.Transition();

        StartCoroutine(WaitForTransition());

    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        glow.SetActive(true);
        descTimer = Time.time;
        waitForDesc = true;
        gameObject.GetComponent<Animator>().Play("Gate_Hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        glow.SetActive(false);
        waitForDesc = false;
        rewardDescImg.transform.parent.gameObject.SetActive(false);
        gameObject.GetComponent<Animator>().Play("Gate_Idle");
    }

}
