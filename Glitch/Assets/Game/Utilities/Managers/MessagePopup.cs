using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessagePopup : MonoBehaviour
{
    public Button closeWindow;
    public TextMeshProUGUI title;
    public TextMeshProUGUI messageTextBox;


    public IEnumerator Init(MessageLevel level, string message)
    {
        messageTextBox.text = message;
        switch (level)
        {
            case MessageLevel.Info:
                title.text = level.ToString();
                title.color = new Color32(255, 255, 255, 255);
                break;
            case MessageLevel.Warning:
                title.text = level.ToString();
                title.color = new Color32(255, 199, 88, 255);
                break;
            case MessageLevel.Error:
                title.text = level.ToString();
                title.color = new Color32(255, 0, 0, 255);
                break;
        }
        return CountdownToClose();
    }

    public IEnumerator CountdownToClose()
    {
        yield return new WaitForSeconds(10f);
        CloseWindow();
    }

    public void CloseWindow()
    {
        GameUtilities.CloseMessageWindow();
    }
}
