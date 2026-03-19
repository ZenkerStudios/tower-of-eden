using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DeleteFile : MonoBehaviour
{

    public SaveFile save;

    public void DeleteSaveFile()
    {
        File.Delete(SaveSystem.GetPath(save.GetFileName()));
        save.CheckExistence();
        Unselect();
    }

    public void Unselect()
    {
        save = null;
        gameObject.SetActive(false);
    }
}
