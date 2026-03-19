using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUtilities : MonoBehaviour
{
    [SerializeField] private List<Material> rewardMaterials;
    static IEnumerator messageCoroutine;
    static IEnumerator popupCoroutine;
    public static GameUtilities instance;
    public static GameObject lastPopup;
    public static string[] difficultyNames = new string[]
    {
        "Novice",
        "Novice",
        "Expert",
        "Nightmare",
        "Gauntlet",
        "Accursed"
    };

 
    public Material GetColorForRarity(Rarity r)
    {
        switch (r)
        {
            case Rarity.Common:
                return rewardMaterials[0];
            case Rarity.Uncommon:
                return rewardMaterials[1];
            case Rarity.Rare:
                return rewardMaterials[2];
            case Rarity.Legendary:
                return rewardMaterials[3];
            case Rarity.Exalted:
                return rewardMaterials[4];
        }
        return rewardMaterials[0];
    }

  

    void Awake()
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

    public static GameObject GetChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.gameObject.name == withName)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public static void DeleteAllChildGameObject(GameObject fromGameObject)
    {
        for (int i = 0; i < fromGameObject.transform.childCount; i++)
        {
            GameObject child = fromGameObject.transform.GetChild(i).gameObject;
            //Do something with child
            MonoBehaviour.Destroy(child);
        }
    }

    public static void ToggleActiveAllChildGameObject(GameObject fromGameObject, bool toggle)
    {
        for (int i = 0; i < fromGameObject.transform.childCount; i++)
        {
            GameObject child = fromGameObject.transform.GetChild(i).gameObject;
            //Do something with child
            child.SetActive(toggle);
        }
    }

    public static AnimationClip FindAnimation(Animator animator, string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }

        Debug.LogWarning("Cutscene Animation by name: " + clipName + " not found.");
        return null;
    }

    public static IEnumerator TimeUntilEndOfAnimation(float len, Action action)
    {
        yield return new WaitForSeconds(len);
        action();
    }

    public static void ShowLevelUpPopup(MonoBehaviour parent, List<Action> popups)
    {
        ShowPopup(parent, popups);
    }

    public static void ShowPopup(MonoBehaviour parent, List<Action> popups)
    {
        parent.StartCoroutine(Popups(popups));
    }

    private static IEnumerator Popups(List<Action> popups)
    {
        foreach (Action act in popups)
        {
            act();

            yield return new WaitForSeconds(0.65f);
        }
        yield break;
    }

    public static void ShowMessage(MessageLevel level, string message)
    {
        CloseMessageWindow();
        GameObject clone = GameController.instance.pfMessagePopup;
        
        messageCoroutine = clone.GetComponent<MessagePopup>().Init(level, message);
        var newObj = Instantiate(clone);
        instance.StartCoroutine(messageCoroutine);
        newObj.transform.SetParent(GameObject.Find("Canvas").transform);
        newObj.transform.localPosition = new Vector2(0f, 800f);
        newObj.GetComponent<RectTransform>().sizeDelta = clone.GetComponent<RectTransform>().sizeDelta;
        newObj.GetComponent<RectTransform>().localScale = clone.GetComponent<RectTransform>().localScale;
        lastPopup = newObj;
    }

    public static void CloseMessageWindow()
    {
        if (messageCoroutine != null) { instance.StopCoroutine(messageCoroutine); }
        if (lastPopup != null) { Destroy(lastPopup); }
    }


    public static void SetTextColorForRarity(TextMeshProUGUI text, Rarity r)
    {
        switch (r)
        {
            case Rarity.Default:
                text.color = new Color32(255, 255, 255, 255);
                break;
            case Rarity.Common:
                text.color = new Color32(255, 255, 255, 255);
                break;
            case Rarity.Uncommon:
                text.color = new Color32(35, 195, 15, 255);
                break;
            case Rarity.Rare:
                text.color = new Color32(65, 145, 255, 255);
                break;
            case Rarity.Legendary:
                text.color = new Color32(135, 65, 195, 255);
                break;
            case Rarity.Exalted:
                text.color = new Color32(255, 155, 45, 255);
                break;
        }
    }

    public static IEnumerator WaitForCutscene(Action act)
    {
        yield return new WaitForSeconds(0.3f);
        while (CutsceneManager.instance.inCutscene)
        {
            yield return new WaitForSeconds(0.3f);
        }
        act();
    }

    public static IEnumerator WaitForConversation(Action act)
    {
        yield return new WaitForSeconds(0.3f);
        while (DialogueManager.instance.inConversation)
        {
            yield return new WaitForSeconds(0.3f);
        }
        act();
    }

    public static IEnumerator WaitForConversation(float time, Action act)
    {
        yield return new WaitForSeconds(time);
        while (DialogueManager.instance.inConversation)
        {
            yield return new WaitForSeconds(0.3f);
        }
        act();
    }

    public static IEnumerator WaitForBark(InteractableNpc npc, Action act)
    {
        yield return new WaitForSeconds(0.3f);
        while (npc.isBarking)
        {
            yield return new WaitForSeconds(0.3f);
        }
        act();
    }

    public static IEnumerator PositionShakeUiObject(GameObject obj, Vector3 originalPos)
    {
        int shakeAmount = 25;
        float elapsed = 0.0f;

        while (elapsed < .25)
        {

            elapsed += Time.unscaledDeltaTime;
            float posDelta = UnityEngine.Random.value * shakeAmount - (shakeAmount / 2);
            obj.transform.position = new Vector3(originalPos.x + posDelta, originalPos.y + posDelta, originalPos.z);
            yield return null;
        }

        obj.transform.position = originalPos;
    }

    public static IEnumerator MoveUiObject(GameObject obj, Vector3 originalPos, Vector3 TargetPos, float rate)
    {
        float elapsed = 0.0f;
        float moveSpeed = 1.0f / rate;

        while (elapsed < 1)
        {
            elapsed += Time.unscaledDeltaTime * moveSpeed;
            obj.transform.position = Vector3.Lerp(originalPos, TargetPos, elapsed);
            yield return null;
        }

        obj.transform.position = TargetPos;
    }

    public static IEnumerator PingPongUiObject(GameObject obj, Vector3 originalPos, Vector3 TargetPos, float rate)
    {
        float elapsed = 0.0f;
        float moveSpeed = 1.0f/rate;

        while (elapsed < 1)
        {
            elapsed += Time.unscaledDeltaTime * moveSpeed;
            obj.transform.position = Vector3.Lerp(originalPos, TargetPos, elapsed);
            yield return null;
        }

        elapsed = 0.0f;

        yield return new WaitForSeconds(5f);

        while (elapsed < 1)
        {
            elapsed += Time.unscaledDeltaTime * moveSpeed;
            obj.transform.position = Vector3.Lerp(TargetPos, originalPos, elapsed);
            yield return null;
        }

        obj.transform.position = originalPos;
    }


    public static IEnumerator RotationShakeUiObject(GameObject obj, Quaternion originalRotation)
    {
        int shakeAmount = 2;
        float elapsed = 0.0f;

        while (elapsed < .25)
        {

            elapsed += Time.unscaledDeltaTime;
            float rotDelta = UnityEngine.Random.value * shakeAmount - (shakeAmount / 2);
            obj.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, originalRotation.z + rotDelta);
            yield return null;
        }
        obj.transform.rotation = originalRotation;

    }

    public static IEnumerator PulsateUiObject(GameObject obj, float timer, float growthSpeed, float maxSize, float minSize, float currentSize)
    {
        Vector3 originalScale = obj.transform.localScale;
        bool tokenAvailable = true;

        while(timer > 0)
        {
            while (currentSize != maxSize)
            {

                try
                {
                    currentSize = Mathf.MoveTowards(currentSize, maxSize, growthSpeed);

                    obj.transform.localScale = Vector3.one * currentSize;

                }
                catch (MissingReferenceException)
                {
                    tokenAvailable = false;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
            while (currentSize != minSize)
            {
                try
                {
                    currentSize = Mathf.MoveTowards(currentSize, minSize, growthSpeed);

                    obj.transform.localScale = Vector3.one * currentSize;

                }
                catch (MissingReferenceException)
                {
                    tokenAvailable = false;
                    break;
                }

                yield return new WaitForEndOfFrame();
           
            }

            timer -= Time.unscaledDeltaTime;
        }

        if (tokenAvailable)
        {
            obj.transform.localScale = originalScale;
        }
    }

    private void Compare(float last, float current, TextMeshProUGUI text)
    {
        if (last < current)
        {
            text.color = new Color32(0, 255, 0, 255);
            text.text = "(+" + (int)(current - last) + ")";
        }
        else if (last > current)
        {
            text.color = new Color32(255, 0, 0, 255);
            text.text = "(" + (int)(current - last) + ")";
        }
        else
        {
            text.text = "";
        }
    }


    public static IEnumerator Grow(Transform transform)
    {
        float currentSize = 1f;
        float maxSize = 1.1f;
        float growthSpeed = 0.02f;
        while (currentSize != maxSize)
        {

            try
            {
                currentSize = Mathf.MoveTowards(currentSize, maxSize, growthSpeed);

                transform.localScale = Vector3.one * currentSize;

            }
            catch (MissingReferenceException)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator Shrink(Transform transform)
    {
        float currentSize = 1.1f;
        float minSize = 1f;
        float growthSpeed = 0.02f;
        while (currentSize != minSize)
        {
            try
            {
                currentSize = Mathf.MoveTowards(currentSize, minSize, growthSpeed);

                transform.localScale = Vector3.one * currentSize;

            }
            catch (MissingReferenceException)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public static string GetEnchantmentName(string enchantmentName)
    {
        switch (enchantmentName)
        {
            case "burnBonus":
                return "Pyrotechnics";
            case "shockedBonus":
                return "Supercharged";
            case "damnedBonus":
                return "Guidance";
            case "vulnerableBonus":
                return "Mind's Eye";
            case "reviveBonus":
                return "Resurrection";
            case "maxHealthBonus":
                return "False Life";
            case "accuracyBonus":
                return "True Sight";
            case "critChanceBonus":
                return "Guided Strike";

            case "fireResistBonus":
                return "Burning Soul";
            case "iceResistBonus":
                return "Endothermic";
            case "lightningResistBonus":
                return "Negation";
            case "psychicResistBonus":
                return "Bullheaded";
            case "poisonResistBonus":
                return "Sanctified";
            case "actionBonus":
                return "Haste";
            case "entryHealthBonus":
                return "Rapid Healing";
            case "goldStart":
                return "Gold Rush";

            case "fireChanceBonus":
                return "Arsonist";
            case "iceChanceBonus":
                return "Subzero";
            case "lightningChanceBonus":
                return "Conductor";
            case "psychicChanceBonus":
                return "Psionics";
            case "poisonChanceBonus":
                return "Chemist";
            case "entrygoldBonus":
                return "Scavenger";
            case "attackLifestealBonus":
                return "Life Leech";
            case "specialLifestealBonus":
                return "Arcane Restoration";
        }
        return "";
    }
    
    public static string GetEnchantmentDesc(string enchantmentName)
    {
        switch (enchantmentName)
        {
            case "burnBonus":
                return "Increase <sprite=37> duration by +1 Turn per rank.";
            case "shockedBonus":
                return "Increase <sprite=41> duration by +1 Turn per rank.";
            case "damnedBonus":
                return "Increase <sprite=39> Stack applied by +1 Stack per rank.";
            case "vulnerableBonus":
                return "Increase <sprite=40> duration by +1 Turn per rank.";
            case "reviveBonus":
                return "Upon death, regain half of your Max Health<sprite=45>. Gain +1 Resurrection <sprite=32> per rank.";
            case "maxHealthBonus":
                return "Increase Max Health<sprite=45> by +5 HP per rank.";
            case "accuracyBonus":
                return "Increase Accuracy <sprite=46> by +2% per rank.";
            case "critChanceBonus":
                return "Increase Critical Chance <sprite=47> by +2% per rank.";

            case "fireResistBonus":
                return "Increase <sprite=37> resistance by +5% per rank.";
            case "iceResistBonus":
                return "Increase <sprite=42> resistance by +5% per rank.";
            case "lightningResistBonus":
                return "Increase <sprite=41> resistance by +5% per rank.";
            case "psychicResistBonus":
                return "Increase <sprite=40> resistance by +5% per rank.";
            case "poisonResistBonus":
                return "Increase <sprite=38> resistance by +5% per rank.";
            case "actionBonus":
                return "Increase number of Action <sprite=48> by +1 per rank.";
            case "entryHealthBonus":
                return "Increase Health regained after encounters <sprite=56> by +5 HP per rank.";
            case "goldStart":
                return "Start every attempt witth +10 Gold <sprite=11> per rank.";

            case "fireChanceBonus":
                return "Increase <sprite=37> Status Chance by +5% per rank.";
            case "iceChanceBonus":
                return "Increase <sprite=42> Status Chance by +5% per rank.";
            case "lightningChanceBonus":
                return "Increase <sprite=41> Status Chance by +15 per rank.";
            case "psychicChanceBonus":
                return "Increase <sprite=40> Status Chance by +5% per rank.";
            case "poisonChanceBonus":
                return "Increase <sprite=38> Status Chance by +5% per rank.";
            case "entrygoldBonus":
                return "Increase Gold reward after encounters <sprite=57> by +5 Gold per rank.";
            case "attackLifestealBonus":
                return "Increase Lifestrike <sprite=50> by +5% per rank.";
            case "specialLifestealBonus":
                return "Increase Lifesteal <sprite=55> by +5% per rank.";
        }
        return "";
    }

}
