using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


[System.Serializable]
public class BossEncounter
{
    public Difficulty bossDiff;
    public List<EnemyNpc> boss;

}

public class FloorManager : MonoBehaviour
{
    public Floors floorCode;
    public Floors nextFloorCode;

    public Animator floorEventAnimator;
    public Animator transitionAnimator;
    public GameObject battleBg;
    public GameObject pfEncounter;
    public GameObject pfDoorBG;
    public GameObject encounterSelect;
    public GameObject encounterPanel;

    public Difficulty floorDifficulty;
    public int floorMaxEncounter;
    public int encountersWon = 0;
    public int encountersToBoss = 0;
    private bool bossDefeated = false;
    public int encountersToMiniBoss = 0;
    public List<EnemyNpc> floorEnemies;
    public List<BossEncounter> miniBosses;
    public List<BossEncounter> finalBosses;
    public List<BossEncounter> floorSpecialEncounter;
    public List<string> bossNames;
    public bool isUniqueRoom = false;
    public bool uniqueRoomEligible = true;
    public List<int> whenToOfferShop;
    
    private PlayerManager player;
    private AudioManager audioManager;

    public Sprite abilityRewardSprite;
    public Sprite artifactRewardSprite;
    public Sprite shopRewardSprite;
    public Sprite goldRewardSprite;
    public Sprite powerupRewardSprite;
    public Sprite maxHealthRewardSprite;
    public Sprite grimoireRewardSprite;
    public Sprite vialRewardSprite;
    public Sprite shardRewardSprite;
    public Sprite scrapsRewardSprite;
    public Sprite gemstonesRewardSprite;

    public Sprite abilityRewardSpriteH;
    public Sprite artifactRewardSpriteH;
    public Sprite shopRewardSpriteH;
    public Sprite goldRewardSpriteH;
    public Sprite powerupRewardSpriteH;
    public Sprite maxHealthRewardSpriteH;
    public Sprite grimoireRewardSpriteH;
    public Sprite vialRewardSpriteH;
    public Sprite shardRewardSpriteH;
    public Sprite scrapsRewardSpriteH;
    public Sprite gemstonesRewardSpriteH;

    public RewardType bossReward;
    public Sprite bossRewardSprite;

    List<KeyValuePair<bool, List<EnemyNpc>>> encounters = new List<KeyValuePair<bool, List<EnemyNpc>>>();

    private bool offeredPowerup = false;
    private bool offeredArtifact = false;
    private bool offeredGold = false;
    private bool offeredHp = false;
    private bool offeredAbility = false;
    private bool offeredGrimoire = false;
    private bool offeredScraps = false;
    private bool offeredGem = false;
    private bool offeredShop = false;
    private bool offeredVial = false;
    private bool offeredShard = false;

    public GameObject finalReward;

    public List<string> floorBgAudioSource;
    public List<string> floorBossAudioSource;
    public List<string> floorBg2AudioSource;

    private void Awake()
    {
        audioManager = AudioManager.instance;
        DialogueManager.instance.ResetBarks();
        if(floorCode != Floors.Floor_05)
        {
            finalReward.SetActive(false);
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        HideDorianCrowd();
        if (floorCode == Floors.Floor_03 
            && player.savedDialogueConditionsMet.Contains(DialogueConditions.DorianResolve) 
            && !player.savedDialogueConditionsMet.Contains(DialogueConditions.SoldierResolve))
        {
            encountersToBoss = 0;
            for(int x = 0; x < progressTrackerPanel.transform.childCount - 1; x++)
            {
                progressTrackerPanel.transform.GetChild(x).gameObject.SetActive(false);
            }
        }
        int diff = Mathf.Clamp(player.chosenDifficulty + (int)floorCode - 2, 1, 5);
        floorDifficulty = (Difficulty)diff;
        int halfHp = player.h.GetMaxHealth()/2;
        player.h.ChangeHealth(halfHp);
        player.ResetHubStats();

        foreach (BossEncounter be in miniBosses)
        {
            be.boss.ForEach(delegate (EnemyNpc npc)
            {
                bossNames.Add(npc.GetName());

            });
        }
        foreach (BossEncounter be in finalBosses)
        {
            be.boss.ForEach(delegate (EnemyNpc npc)
            {
                bossNames.Add(npc.GetName());

            });
        }

        bossNames.Distinct();
        PlayFloorMusic();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("SpecialDB").GetComponent<SpecialCollection>().GetSpecialsByWeaponType(player.selectedWeaponType);
        if (floorCode == Floors.Floor_01)
        {
            if(player.GetTotalAttempts() > 1)
            {
                player.GetStartingLoadout();
            }
            else
            {
                CutsceneManager.instance.Play(Cutscenes.One);
                SteamIntegration.UnlockThisAchievement(Achievements.GAME_START.ToString());
            }
        }
        NextRoom();
        player.playerManagerAnimator.Play("Floor_" + ((int)floorCode - 1) + "_Start");

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SetDoorBG()
    {
        for (int x = 0; x < encounterPanel.transform.childCount; x++)
        {
            Transform door = encounterPanel.transform.GetChild(x).GetComponent<Transform>();

            var newObj = GameObject.Instantiate(pfDoorBG);
            newObj.transform.SetParent(door.transform);
            newObj.transform.SetAsFirstSibling();
            newObj.transform.localPosition = new Vector2(0, 0);
            newObj.GetComponent<RectTransform>().sizeDelta = pfDoorBG.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = pfDoorBG.GetComponent<RectTransform>().localScale;
           
        }


    }

    public void Option1()
    {
        player.h.GetStrength().AddModifier(new StatModifier(5, StatModType.Flat, this));
        NextFloor();
    }

    public void Option2()
    {
        player.h.IncreaseMaxHealth(25);
        NextFloor();
    }

    public void Option3()
    {
        player.h.GetDominance().AddModifier(new StatModifier(5, StatModType.Flat, this));
        NextFloor();
    }

    public void Option4()
    {
        player.h.GetAccuracy().AddModifier(new StatModifier(10, StatModType.Flat, this));
        NextFloor();
    }

    public void Option5()
    {
        player.h.GetCritChance().AddModifier(new StatModifier(10, StatModType.Flat, this));
        NextFloor();
    }

    public void Option6()
    {
        player.h.GetAttackLifestrike().AddModifier(new StatModifier(25, StatModType.Flat, this));
        NextFloor();
    }

    public void Option7()
    {
        player.h.GetSpecialLifeSteal().AddModifier(new StatModifier(25, StatModType.Flat, this));
        NextFloor();
    }

    public void Option8()
    {
        player.h.GetToughness().AddModifier(new StatModifier(5, StatModType.Flat, this));
        NextFloor();
    }

    public void Option9()
    {
        player.h.GetActions().AddModifier(new StatModifier(1, StatModType.Flat, this));
        NextFloor();
    }

    public void NextFloor()
    {
        if(floorCode == Floors.Floor_05)
        {
            player.highestDifficultyWon = player.chosenDifficulty;
            player.PlayerReset(true);
            GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().DestroyOnRunFinish();
        }
        //Go to next floor
        GameController.instance.LoadGame(nextFloorCode);
    }

    public void NextRoom()
    {
        foreach (Consumable cons in player.GetConsumables())
        {
            cons.ActivateConsumable();
        }
        StartCoroutine(FloorEventTrigger());
        offeredPowerup = false;
        offeredArtifact = false;
        offeredGold = false;
        offeredHp = false;
        offeredAbility = false;

        //start bg
        battleBg.transform.GetChild(0).gameObject.SetActive(encountersWon < encountersToBoss/2);
        //mid point bg
        battleBg.transform.GetChild(1).gameObject.SetActive(encountersWon >= encountersToBoss / 2 && encountersWon < encountersToBoss);
        //boss bg
        battleBg.transform.GetChild(2).gameObject.SetActive(encountersWon >= encountersToBoss);

        if (bossDefeated)
        {
            finalReward.SetActive(true);
        }
        else
        {
            encounters = GetEncounters();


            //Getting weird behavior when it creates 1 extra encounter
            GameUtilities.DeleteAllChildGameObject(encounterPanel);

            foreach (KeyValuePair<bool, List<EnemyNpc>> enc in encounters)
            {
                Reward r = GetReward(Random.Range(1, 101));
                //Give floor boss specific reward
                if(encountersWon >= encountersToBoss)
                {
                    r = new Reward(GetRewardSprite(bossReward), bossReward);
                }
                var newObj = GameObject.Instantiate(pfEncounter);
                newObj.transform.SetParent(encounterPanel.transform);
                newObj.transform.localPosition = new Vector2(0, 0);
                newObj.GetComponent<RectTransform>().sizeDelta = pfEncounter.GetComponent<RectTransform>().sizeDelta;
                newObj.GetComponent<RectTransform>().localScale = pfEncounter.GetComponent<RectTransform>().localScale;
                SelectEncounter e = newObj.GetComponent<SelectEncounter>();
                e.difficulty = floorDifficulty;
                //If next room is shop
                if(GetShop())
                {
                    offeredShop = true;
                    e.rewardType = new Reward(GetRewardSprite(RewardType.Shop), RewardType.Shop); 
                    e.encounterKey = floorCode;
                    e.isDifficult = false;
                    e.rewardImg.sprite = GetRewardSprite(RewardType.Shop)[1];
                    if (encountersWon >= encountersToBoss-1)
                    {
                        SetDoorBG();
                        encounterSelect.SetActive(true);
                        return;
                    } 
                }
                else
                {
                    e.rewardType = r;
                    e.enc = enc.Value;
                    e.encounterKey = floorCode;
                    e.isDifficult = enc.Key;
                    e.rewardImg.sprite = r.highlightImg;

                }

            }
            SetDoorBG();
            encounterSelect.SetActive(true);

        }
    }

    public int floorTriggers = 3;
    private static readonly int triggerCooldown = 2;
    private int floorTriggerOnCooldown = 2;
    public bool floorEventTriggered = false;
    private IEnumerator FloorEventTrigger()
    {
        int hazardChance = 15 + player.h.playerInventory.GetHazardBonus();
        yield return new WaitForSeconds(0.5f);

        if (floorEventTriggered) floorEventTriggered = false;

        if (player.GetTotalAttempts() < 3) yield break;

        floorTriggerOnCooldown--;
           
        if (floorTriggers > 0 && floorTriggerOnCooldown <= 0)
        {
            switch (floorCode)
            {
                case Floors.Floor_01:
                case Floors.Floor_02:
                case Floors.Floor_04:
                    if (Random.Range(1, 101) <= hazardChance)
                    {
                        floorEventTriggered = true;
                    }
                    break;
                case Floors.Floor_03:
                    if (encountersWon < encountersToBoss && Random.Range(1, 101) <= hazardChance + 10)
                    {
                        floorEventTriggered = true;
                    }
                    break;
            }

            if(floorEventTriggered)
            {
                floorTriggers--;
                floorEventAnimator.Play("Floor_Event_Triggered");            
                floorTriggerOnCooldown = triggerCooldown;
            }

        }
    }

    public void ExitRoom()
    {
        UpdateProgressTracker();
        PlayFloorMusic();
        audioManager.IncreaseMusicVolume(0.5f);
        offeredShop = false;
        if (!player.h.playerInventory.ailments.Exists(a => a.conIndex == 0))
        {
            int val = player.healthOnExit;
            if(player.h.playerInventory.halfHealthEnc)
            {
                val /= 2;
            }
            player.h.ChangeHealth(val);
        }
        if (!player.h.playerInventory.ailments.Exists(a => a.conIndex == 2))
        {
            int val = player.goldOnExit;
            if (player.h.playerInventory.halfHealthEnc)
            {
                val /= 2;
            }
            if (player.h.playerInventory.noGoldFloor == floorCode)
            {
                val = 0;
            }
            player.h.playerInventory.ChangeGoldAmount(val);
        }

        foreach(Consumable cons in player.GetConsumables())
        {
            cons.DisableConsumable();
        }
        isUniqueRoom = false;
        player.h.ClearBattleModifiers();
        player.RenderLoadout();

    }

    public List<KeyValuePair<bool, List<EnemyNpc>>> GetEncounters()
    {
        int numEncounters = Random.Range(2, floorMaxEncounter+1);
        List<KeyValuePair<bool, List<EnemyNpc>>> allEncounters = new List<KeyValuePair<bool, List<EnemyNpc>>>();


        if (IsBossBattle())
        {
            int index = 0;
            //Only show ulea at diff 3 or higer
            if (floorCode == Floors.Floor_04 && player.chosenDifficulty > 2)
            {
                DialogueBlock nextLyraConvo = finalBosses[0].boss[0].interactableNpc.InitiateDialogue();
                bool uleaEnc =(nextLyraConvo != null && nextLyraConvo.GetDialogueId() == "LYRA_010_01")|| player.savedDialogueConditionsMet.Contains(DialogueConditions.StarRevealed);
                if (uleaEnc) index = 1;
            }
            allEncounters.Add(AddEnc(true, finalBosses[index].boss));
        }
        else if (IsMiniBossBattle())
        {
            //If this is mini-boss encounter
            allEncounters.Add(AddEnc(true, GetMiniBoss()));
            for (int x = 1; x < numEncounters; x++)
            {
                allEncounters.Add(AddEnc(false, GetRandomEnemiesForFloor()));

            }
        }
        else
        {
            //If this is normall encounter
            for (int x = 0; x < numEncounters; x++)
            {
                allEncounters.Add(AddEnc(false, GetRandomEnemiesForFloor()));
            }
        }

        return allEncounters;
    }

    private List<EnemyNpc> GetMiniBoss()
    {
        List<EnemyNpc> enc = new List<EnemyNpc>();
        enc.AddRange(miniBosses.OrderBy(a => System.Guid.NewGuid()).ToList()[0].boss);
        return enc;
    }

    private bool GetShop()
    {
        return whenToOfferShop.Contains(encountersWon) && !offeredShop;
    }

    public List<EnemyNpc> GetRandomEnemiesForFloor()
    {
        List<EnemyNpc> res = new List<EnemyNpc>();
        int numEnemies = UnityEngine.Random.Range(2, GameController.GetMaxEnemiesCount());
        //Keep number of enemies at a low number for first 3 enc
        if (floorCode == Floors.Floor_01 && player.chosenDifficulty < 2 && encountersWon < 5)
        {
            numEnemies = 2;
        }

        for (int x = 0; x < numEnemies; x++)
        {
            res.Add(floorEnemies[UnityEngine.Random.Range(0, floorEnemies.Count)]);
        }
        return res;
    }

    public KeyValuePair<bool, List<EnemyNpc>> AddEnc(bool key, List<EnemyNpc> value)
    {
        return new KeyValuePair<bool, List<EnemyNpc>>(key, value);
    }

    public bool IsBossBattle()
    {
        return encountersWon >= encountersToBoss;
    }

    public bool IsMiniBossBattle()
    {
        return encountersWon == encountersToMiniBoss;
    }

    public void BossDefeated()
    {
        bossDefeated = true;
    }

    private int floorInfusions = 2;

    public void AddInfusion(int val)
    {
        floorInfusions += val;
    }

    public Reward GetReward(int rand)
    {
        if (rand <= 12)
        {
            if(((encountersWon < encountersToBoss/2) && floorInfusions > 1)
                || ((encountersWon > encountersToBoss / 2) && floorInfusions > 0))
            {
                if (!offeredHp)
                {
                    offeredHp = true;
                    floorInfusions--;
                    return new Reward(GetRewardSprite(RewardType.MaxHealth), RewardType.MaxHealth);
                }
            }
            return GetReward(Random.Range(13, 101));
        }
        else if (rand > 12 && rand <= 20)
        {
            if (!offeredArtifact)
            {
                offeredArtifact = true;
                return new Reward(GetRewardSprite(RewardType.Artifact), RewardType.Artifact);
            }
            else
            {
                return GetReward(Random.Range(21, 101));
            }
        }
        else if (rand > 20 && rand <= 30)
        {
            if (!offeredGold)
            {
                offeredGold = true;
                return new Reward(GetRewardSprite(RewardType.Gold), RewardType.Gold);
            }
            else
            {
                return GetReward(Random.Range(31, 101));
            }
        }
        else if (rand > 30 && rand <= 35)
        {
            if (!offeredVial)
            {
                offeredVial = true;
                return new Reward(GetRewardSprite(RewardType.EtherVial), RewardType.EtherVial);
            }
            else
            {
                return GetReward(Random.Range(36, 101));
            }
        }
        else if (rand > 35 && rand <= 40)
        {
            if (!offeredShard)
            {
                offeredShard = true;
                return new Reward(GetRewardSprite(RewardType.EtherShard), RewardType.EtherShard);
            }
            else
            {
                return GetReward(Random.Range(41, 101));
            }
        }
        else if (rand > 40 && rand <= 50)
        {
            if (!offeredPowerup)
            {
                offeredPowerup = true;
                return new Reward(GetRewardSprite(RewardType.Powerup), RewardType.Powerup);
            }
            else
            {
                return GetReward(Random.Range(51, 101));
            }
        }
        else if (rand > 50 && rand <= 60)
        {
            if (!offeredGrimoire)
            {
                offeredGrimoire = true;
                return new Reward(GetRewardSprite(RewardType.GrimoirePage), RewardType.GrimoirePage);
            }
            else
            {
                return GetReward(Random.Range(61, 101));
            }
        }
        else if (rand > 60 && rand <= 70)
        {
            if (!offeredGem)
            {
                offeredGem = true;
                return new Reward(GetRewardSprite(RewardType.Gemstones), RewardType.Gemstones);
            }
            else
            {
                return GetReward(Random.Range(71, 101));
            }
        }
        else if (rand > 70 && rand <= 80)
        {
            if (!offeredScraps)
            {
                offeredScraps = true;
                return new Reward(GetRewardSprite(RewardType.MetalScraps), RewardType.MetalScraps);
            }
            else
            {
                return GetReward(Random.Range(81, 101));
            }
        }
        else
        {
            if (!offeredAbility)
            {
                offeredAbility = true;
                return new Reward(GetRewardSprite(RewardType.Ability), RewardType.Ability);
            }
            else
            {
                return GetReward(Random.Range(1, 101));
            }
        }
    }

    private Sprite[] GetRewardSprite(RewardType rewardType)
    {
        switch(rewardType)
        {
            case RewardType.Shop:
                return new Sprite[] { shopRewardSprite, shopRewardSpriteH };
            case RewardType.MaxHealth:
                return new Sprite[] { maxHealthRewardSprite, maxHealthRewardSpriteH };
            case RewardType.Artifact:
                return new Sprite[] { artifactRewardSprite, artifactRewardSpriteH };
            case RewardType.Gold:
                return new Sprite[] { goldRewardSprite, goldRewardSpriteH };
            case RewardType.Powerup:
                return new Sprite[] { powerupRewardSprite, powerupRewardSpriteH };
            case RewardType.GrimoirePage:
                return new Sprite[] { grimoireRewardSprite, grimoireRewardSpriteH };
            case RewardType.EtherShard:
                return new Sprite[] { shardRewardSprite, shardRewardSpriteH };
            case RewardType.EtherVial:
                return new Sprite[] { vialRewardSprite, vialRewardSpriteH };
            case RewardType.Gemstones:
                return new Sprite[] { gemstonesRewardSprite, gemstonesRewardSpriteH };
            case RewardType.MetalScraps:
                return new Sprite[] { scrapsRewardSprite, scrapsRewardSpriteH };
            case RewardType.Ability:
                return new Sprite[] { abilityRewardSprite, abilityRewardSpriteH };
        }
        return new Sprite[] { abilityRewardSprite, abilityRewardSpriteH };
    }

    public void Transition()
    {
        if(floorCode == Floors.Floor_05)
        {
            transitionAnimator.Play("Floor_5_entry");
        } else
        {
            transitionAnimator.Play("Close Gate");
        }
    }

    public void PlayFloorMusic()
    {
        if (encountersWon < (encountersToBoss / 2)-1)
        {
            PlayFloorOST1();
        }
        else
        {
            PlayFloorOST2();
        }
    }

    public void PlayFloorOST1()
    {
        int lastIndex = floorBgAudioSource.Count - 1;
        if (floorCode != Floors.Floor_05)
        {
            if (!audioManager.isPlaying(floorBgAudioSource[lastIndex]))
            {
                audioManager.PlayFloorMusic(floorBgAudioSource);
            }
        } else
        {
            audioManager.PlayFloorMusic(floorBgAudioSource);
        }
    }

    public void PlayFloorOST2()
    {
        int lastIndex = floorBg2AudioSource.Count - 1;
        if (floorCode != Floors.Floor_05)
        {
            if (!audioManager.isPlaying(floorBg2AudioSource[lastIndex]))
            {
                audioManager.PlayFloorMusic(floorBg2AudioSource);
            }
        }
        else
        {
            audioManager.PlayFloorMusic(floorBg2AudioSource);
        }
    }

    public void PlayBossMusic()
    {
        if(floorCode != Floors.Floor_05)
        {
            audioManager.PlayFloorMusic(floorBossAudioSource);
        }
    }

    public GameObject progressTrackerPanel;
    private Sprite currentRewardSprite;


    public void SetCurrentRewardSprite(RewardType r)
    {
        currentRewardSprite = GetRewardSprite(r)[1];
    }

    private void UpdateProgressTracker()
    {
        if (isUniqueRoom) return;
        GameObject currectRoom = progressTrackerPanel.transform.GetChild(encountersWon).gameObject;
        currectRoom.transform.GetChild(1).gameObject.SetActive(true);
        currectRoom.transform.GetChild(3).gameObject.GetComponentInChildren<UnityEngine.UI.Image>().sprite = currentRewardSprite;
    }

    [SerializeField]private GameObject bgCrowd;
    public void HideDorianCrowd()
    {
        if (floorCode == Floors.Floor_03
           && player.savedDialogueConditionsMet.Contains(DialogueConditions.DorianResolve))
        {
            bgCrowd.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            bgCrowd.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            bgCrowd.transform.GetChild(1).GetChild(3).gameObject.SetActive(false);
            bgCrowd.transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
            bgCrowd.transform.GetChild(1).GetChild(9).gameObject.SetActive(false);
            bgCrowd.transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
            bgCrowd.transform.parent.GetChild(0).gameObject.SetActive(false);
        }
    }
}