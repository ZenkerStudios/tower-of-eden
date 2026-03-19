using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using static UnityEngine.Rendering.HDROutputUtils;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    //Audio
    public AudioMixer masterMixer;

    private PlayerManager player; 
    private static int maxEnemyCount = 4;
    private static int startingAbilityCount = 5;

    //Prefabs
    public GameObject pfPlayerManager;
    public GameObject pfMessagePopup;
    public GameObject pfRewardPopup;
    public GameObject settingMenu;
    public GameObject tutorialMenu;
    public GameObject pfCombatSystem;
    public GameObject pfShop;
    public GameObject pfUniqueRoom;

    public Floors currentScene;
    private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public GameObject loadingCanvas;

    public static string currentSaveFileName = "ToEFile_1";

    public Animator gameControllerAnimator;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        Screen.SetResolution(
            (int)PlayerPrefs.GetInt("ResWidth", 1920),
            (int)PlayerPrefs.GetInt("ResHeight", 1080), PlayerPrefs.GetInt("fullscreen", 1) == 1);

        masterMixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", .9f)) * VolumeControl._multiplier);
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", .9f)) * VolumeControl._multiplier);
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", .9f)) * VolumeControl._multiplier);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void LoadGame(Floors scene)
    {
        currentScene = scene;
        player.PlayerCanvasDisplayActive(false);
        loadingCanvas.SetActive(true);

        if ((int)scene == 1)
        {
            SavePlayer();
        }
        StartCoroutine(GetSceneLoadingProgress(scene));
    }

    private IEnumerator GetSceneLoadingProgress(Floors scene)
    {
        yield return new WaitForSecondsRealtime(0.1f);

        AsyncOperation asyncOp  = SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Single);
        while (!asyncOp.isDone)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);
        loadingCanvas.SetActive(false);
        if (player != null) player.PlayerCanvasDisplayActive(true);
    }

    private void InitPlayerManager()
    {
        GameObject managerObj = GameObject.Find("Managers");
        Destroy(GameObject.Find("Player Manager(Clone)"));

        var newObj = GameObject.Instantiate(pfPlayerManager);
        newObj.transform.SetParent(managerObj.transform);
        newObj.transform.SetAsFirstSibling();
        newObj.transform.localPosition = new Vector2(0f, 0f);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    public void StartSavedGame()
    {
        InitPlayerManager();
        LoadPlayer();
        LoadGame(Floors.Hub);
    }

    public void StartNewGame()
    {
        InitPlayerManager();
        player.selectedWeaponType = WeaponTypes.Melancolia;
        player.attempts.Add(new Attempts());
        player.SetInTower();
        LoadGame(Floors.Floor_01);

    }

    public void StartEncounter(Floors f, List<EnemyNpc> enc, Reward r, Difficulty diff, bool elite)
    {

        GameObject canvas = GameObject.Find("Battle Container");

        var newObj = GameObject.Instantiate(pfCombatSystem);

        newObj.transform.SetParent(canvas.transform);
        newObj.transform.localPosition = new Vector2(0f, 0f);
        newObj.GetComponent<RectTransform>().sizeDelta = pfCombatSystem.GetComponent<RectTransform>().sizeDelta;
        newObj.GetComponent<RectTransform>().localScale = pfCombatSystem.GetComponent<RectTransform>().localScale;
        newObj.GetComponent<CombatSystem>().encounterKey = f;
        newObj.GetComponent<CombatSystem>().enemyList = enc;
        newObj.GetComponent<CombatSystem>().reward = r;
        newObj.GetComponent<CombatSystem>().difficulty = diff;
        newObj.GetComponent<CombatSystem>().elite = elite;

    }

    public static int GetMaxEnemiesCount()
    {
        return maxEnemyCount;
    }

    public static int GetStartingAbilitiesCount(PlayerManager pm)
    {
        if(pm.cityShopConsumables.Exists(a => a.consumableIntValue == 4))
        {
            return startingAbilityCount+1;
        }
        return startingAbilityCount;
    }

    public void OpenShop(Difficulty diff)
    {
        GameUtilities.CloseMessageWindow();

        GameObject canvas = GameObject.Find("Battle Container");

        var newObj = GameObject.Instantiate(pfShop);
        newObj.GetComponent<Shop>().floorDifficulty = diff;

        newObj.transform.SetParent(canvas.transform);
        newObj.transform.localPosition = new Vector2(0f, 0f);
        newObj.GetComponent<RectTransform>().sizeDelta = pfShop.GetComponent<RectTransform>().sizeDelta;
        newObj.GetComponent<RectTransform>().localScale = pfShop.GetComponent<RectTransform>().localScale;
        
    }

    public void UniqueRoom(Floors k, Difficulty diff, Reward r)
    {
        GameUtilities.CloseMessageWindow();

        GameObject canvas = GameObject.Find("Battle Container");

        var newObj = GameObject.Instantiate(pfUniqueRoom);

        newObj.transform.SetParent(canvas.transform);
        newObj.transform.localPosition = new Vector2(0f, 0f);
        newObj.GetComponent<RectTransform>().sizeDelta = pfUniqueRoom.GetComponent<RectTransform>().sizeDelta;
        newObj.GetComponent<RectTransform>().localScale = pfUniqueRoom.GetComponent<RectTransform>().localScale;
        newObj.GetComponent<UniqueRoom>().key = k;
        newObj.GetComponent<UniqueRoom>().difficulty = diff;
        newObj.GetComponent<UniqueRoom>().rewardType = r;
    }

    public void DisplayRewardNotif(string msg)
    {
        GameObject canvas = GameObject.Find("Popup window");

        var newObj = GameObject.Instantiate(pfRewardPopup);
        newObj.GetComponent<RewardNotif>().rewardMsg.text = msg;

        newObj.transform.SetParent(canvas.transform);
        newObj.transform.localPosition = new Vector2(0f, 0f);
        newObj.GetComponent<RectTransform>().sizeDelta = pfRewardPopup.GetComponent<RectTransform>().sizeDelta;
        newObj.GetComponent<RectTransform>().localScale = pfRewardPopup.GetComponent<RectTransform>().localScale;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public static Rarity GetAbilityRarity(Difficulty diff, int rarityBonus)
    {
        /*
                 * 45: Common
                 * 30: Uncommon
                 * 15: Rare
                 * 10: Legendary
                 */
        Dictionary<int, Rarity> rarityList = new Dictionary<int, Rarity>();
        rarityList.Add(45, Rarity.Common);
        rarityList.Add(30, Rarity.Uncommon);
        rarityList.Add(15, Rarity.Rare);
        rarityList.Add(10, Rarity.Legendary);
        int min = 0;
        if((int)diff > 2)
        {
            min = 19 * (int)diff;
        }
        int rand = UnityEngine.Random.Range(min + rarityBonus, 101);

        foreach (KeyValuePair<int, Rarity> weight in rarityList)
        {
            if (rand <= weight.Key)
            {
                return weight.Value;
            }
            else
            {
                rand -= weight.Key;
            }
        }
        return Rarity.Common;
    }


    public static int GetMinTier(Floors floor)
    {
        int maxTier = 0;
        switch (floor)
        {
            case Floors.Floor_02:
                maxTier = 50;
                break;
            case Floors.Floor_03:
                maxTier = 75;
                break;
            case Floors.Floor_04:
                maxTier = 85;
                break;

        }
        return maxTier;
    }


    public static List<Artifact> GetArtifacts(int bonus, int numArtifacts, Floors floor)
    {
       
        ArtifactCollection artifactCollection = GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>();

        List<Artifact> artTemp = new List<Artifact>();
        try
        {
            for (int x = 0; x < numArtifacts; x++)
            {
                int tier = UnityEngine.Random.Range(GetMinTier(floor),100);
                tier = Mathf.Clamp(tier + bonus, tier, 100);
                Artifact art = artifactCollection.GetLoot(tier);
                artTemp.Add(art);
            }
        }
        catch (Exception)
        {
            if (artTemp.Count <= 0)
            {
                return null;
            }

        }
        return artTemp;

    }

    public static Artifact GetArtifactByRarity(Rarity rarity)
    {
        ArtifactCollection artifactCollection = GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>();
        return artifactCollection.GetLoot(rarity);
    }

    public void OpenSetting()
    {
        GameUtilities.CloseMessageWindow();

        if (GameObject.Find("Settings Menu(Clone)"))
        {
            GameUtilities.ShowMessage(MessageLevel.Info, "Setting menu is already open");
        }
        else
        {
            GameObject canvas = GameObject.Find("Canvas");

            var newObj = GameObject.Instantiate(settingMenu);

            newObj.transform.SetParent(canvas.transform);
            newObj.transform.localPosition = new Vector2(0f, 0f);
            newObj.GetComponent<RectTransform>().sizeDelta = settingMenu.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = settingMenu.GetComponent<RectTransform>().localScale;
        }
    }

    public void OpenTutorial()
    {
        GameUtilities.CloseMessageWindow();

        if (GameObject.Find("Tutorial(Clone)"))
        {
            GameUtilities.ShowMessage(MessageLevel.Info, "Tutorial menu is already open");
        }
        else
        {
            GameObject canvas = GameObject.Find("Canvas");

            var newObj = GameObject.Instantiate(tutorialMenu);

            newObj.transform.SetParent(canvas.transform);
            newObj.transform.localPosition = new Vector2(0f, 0f);
            newObj.GetComponent<RectTransform>().sizeDelta = tutorialMenu.GetComponent<RectTransform>().sizeDelta;
            newObj.GetComponent<RectTransform>().localScale = tutorialMenu.GetComponent<RectTransform>().localScale;
        }
    }

    public void BackToStartScreen()
    {
        player.SetOutOfTower();
        DialogueManager dm = DialogueManager.instance;
        if (dm.inConversation)
        {
            dm.DialogueContainer.SetActive(false);
            dm.inConversation = false;
        }
        dm.ResetBarks();
        AudioManager.instance.StopAllMusic();
        PauseController.instance.TogglePause();
        LoadGame(Floors.Start);
        Destroy(GameObject.Find("Non-Destructible Collections"));
        Destroy(player.gameObject);

    }

    public void BackToHub()
    {
        //Surrender
        Floors floorCode = GameObject.Find("Floor Manager").GetComponent<FloorManager>().floorCode;
        player.attempts.Last().SurrenderFinalUpdate(player, false, true, floorCode);
        player.SetOutOfTower();
        AudioManager.instance.StopAllMusic();
        GameObject.FindGameObjectWithTag("ArtifactDB").GetComponent<ArtifactCollection>().DestroyOnRunFinish();
        player.PlayerReset(true);
        Resources.UnloadUnusedAssets();
        LoadGame(Floors.Hub);
        PauseController.instance.TogglePause();
        player.ailmentPanel.transform.parent.gameObject.SetActive(false);
        player.abilityDisplayPanel.transform.parent.gameObject.SetActive(false);
        player.abilityDisplayPanel.transform.parent.parent.GetChild(0).gameObject.SetActive(false);
    }

    public void SavePlayer()
    {
        if (!saving)
        {
            StartCoroutine(savingIcon());
        }
        SaveSystem.SavePlayer(player);
    }

    private bool saving = false;
    public GameObject saveIcon;
    private IEnumerator savingIcon()
    {
        //show icon
        saving = true;
        saveIcon.SetActive(saving);
        yield return new WaitForSecondsRealtime(5f);
        //hide icone
        saving = false;
        saveIcon.SetActive(saving);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer(currentSaveFileName);
        SaveUnpacker sU = new SaveUnpacker(data, player);
        player.SetOutOfTower();
    }
}

