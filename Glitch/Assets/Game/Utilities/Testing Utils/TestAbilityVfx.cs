using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestAbilityVfx : MonoBehaviour
{
    public List<Special> specials = new List<Special>();
    public List<Attack> attacks = new List<Attack>();

    public List<Special> f1specials = new List<Special>();
    public List<Special> f2specials = new List<Special>();
    public List<Special> f3specials = new List<Special>();
    public List<Special> f4specials = new List<Special>();
    public List<Special> f5specials = new List<Special>();

    public GameObject view;
    public TextMeshProUGUI abilityName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunTests()
    {
        StartCoroutine(PlayAllAbilityVfx());
    }

    IEnumerator PlayAllAbilityVfx()
    {

        Debug.Log("Showing attacks");
        foreach (Attack a in attacks)
        {
            //abilityName.text += a.name + ": " + a.GetDesc() + '\n';
            abilityName.text += a.name + ": " + a.GetNumTarget() + '\n';
            try
            {
                //AudioManager.instance.PlaySfxSound(a.GetAudio());
                CreateVFX(a.GetVfx(), view.transform);
            } catch (System.Exception)
            {
                Debug.LogWarning(a.GetName() + " failed to play vfx");
            }
            yield return new WaitForSeconds(.5f);
        }

        GameUtilities.DeleteAllChildGameObject(view);
        Debug.Log("Showing player specials");
        foreach (Special s in specials)
        {
            try
            {
               abilityName.text += s.name + ": " + s.GetNumTarget() + '\n';
               //abilityName.text += s.name + ": " + s.GetDesc() + '\n';
                //AudioManager.instance.PlaySfxSound(s.GetAudio());
                CreateVFX(s.GetVfx(), view.transform);
            }
            catch (System.Exception)
            {
                Debug.LogWarning(s.GetName() + " failed to play vfx");
            }
            yield return new WaitForSeconds(.5f);
        }

        //GameUtilities.DeleteAllChildGameObject(view);
        //Debug.Log("Showing f1 specials");
        //foreach (Special s in f1specials)
        //{
        //    abilityName.text = s.name;
        //    try
        //    {
        //        AudioManager.instance.PlaySfxSound(s.GetAudio());
        //        CreateVFX(s.GetVfx(), view.transform);
        //    }
        //    catch (System.Exception)
        //    {
        //        Debug.LogWarning(s.GetName() + " failed to play vfx");
        //    }
        //    yield return new WaitForSeconds(.5f);
        //}

        //GameUtilities.DeleteAllChildGameObject(view);
        //Debug.Log("Showing f2 specials");
        //foreach (Special s in f2specials)
        //{
        //    abilityName.text = s.name;
        //    try
        //    {
        //        AudioManager.instance.PlaySfxSound(s.GetAudio());
        //        CreateVFX(s.GetVfx(), view.transform);
        //    }
        //    catch (System.Exception)
        //    {
        //        Debug.LogWarning(s.GetName() + " failed to play vfx");
        //    }
        //    yield return new WaitForSeconds(.5f);
        //}

        //GameUtilities.DeleteAllChildGameObject(view);
        //Debug.Log("Showing f3 specials");
        //foreach (Special s in f3specials)
        //{
        //    abilityName.text = s.name;
        //    try
        //    {
        //        AudioManager.instance.PlaySfxSound(s.GetAudio());
        //        CreateVFX(s.GetVfx(), view.transform);
        //    }
        //    catch (System.Exception)
        //    {
        //        Debug.LogWarning(s.GetName() + " failed to play vfx");
        //    }
        //    yield return new WaitForSeconds(.5f);
        //}

        //GameUtilities.DeleteAllChildGameObject(view);
        //Debug.Log("Showing f4 specials");
        //foreach (Special s in f4specials)
        //{
        //    abilityName.text = s.name;
        //    try
        //    {
        //        AudioManager.instance.PlaySfxSound(s.GetAudio());
        //        CreateVFX(s.GetVfx(), view.transform);
        //    }
        //    catch (System.Exception)
        //    {
        //        Debug.LogWarning(s.GetName() + " failed to play vfx");
        //    }
        //    yield return new WaitForSeconds(.5f);
        //}

        //GameUtilities.DeleteAllChildGameObject(view);
        //Debug.Log("Showing f5 specials");
        //foreach (Special s in f5specials)
        //{
        //    abilityName.text = s.name;
        //    try
        //    {
        //        AudioManager.instance.PlaySfxSound(s.GetAudio());
        //        CreateVFX(s.GetVfx(), view.transform);
        //    }
        //    catch (System.Exception)
        //    {
        //        Debug.LogWarning(s.GetName() + " failed to play vfx");
        //    }
        //    yield return new WaitForSeconds(.5f);
        //}
        
        //GameUtilities.DeleteAllChildGameObject(view);
    }

    private void CreateVFX(GameObject fx, Transform parent)
    {
        var newVfx = GameObject.Instantiate(fx);
        newVfx.transform.SetParent(parent);
        newVfx.transform.localPosition = new Vector2(0, 0);
        newVfx.GetComponent<RectTransform>().sizeDelta = fx.GetComponent<RectTransform>().sizeDelta;
        newVfx.GetComponent<RectTransform>().localScale = fx.GetComponent<RectTransform>().localScale;
    }
}
