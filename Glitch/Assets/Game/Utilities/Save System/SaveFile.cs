using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveFile : MonoBehaviour
{
    [SerializeField]
    protected string fileName;

    public GameObject fileExists;
    public GameObject noFile;
    public TextMeshProUGUI lastPlayed;
    public TextMeshProUGUI attempts;
    public DeleteFile markForDelete;
    public GameObject deleteButton;

    private void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        markForDelete.gameObject.SetActive(false);
        CheckExistence();
    }

    public void CheckExistence()
    {
        fileExists.SetActive(false);
        noFile.SetActive(false);
        if (File.Exists(SaveSystem.GetPath(fileName)))
        {
            PlayerData data = SaveSystem.LoadPlayer(fileName);
            attempts.text = data.attemptCount + "";
            lastPlayed.text = data.lastPlayed;
            deleteButton.SetActive(true);
            fileExists.SetActive(true);
        }
        else
        {
            deleteButton.SetActive(false);
            noFile.SetActive(true);
        }

    }

    public void SelectSaveFile()
    {
        GameController gc = GameController.instance;
        GameController.currentSaveFileName = fileName;
        
        if (!File.Exists(SaveSystem.GetPath(fileName)))
        {
            gc.StartNewGame();
        }
        else
        {
            gc.StartSavedGame();
        }
    }


    public void MarkForDelete()
    {
        if(File.Exists(SaveSystem.GetPath(fileName)))
        {
            markForDelete.save = this;
            markForDelete.gameObject.SetActive(true);
        }
    }

    public string GetFileName()
    {
        return fileName;
    }
}
