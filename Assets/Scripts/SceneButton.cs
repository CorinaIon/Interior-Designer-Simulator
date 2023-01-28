using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public Text buttonName;
    public string fileName;

    public void Init(string val)
    {
        fileName = val;
        buttonName.text = val;
    }

    public void SendToNextScene()
    {
        SceneController.fileNameOnScene = fileName;
        SceneManager.LoadScene("NewSceneCreation", LoadSceneMode.Single);
    }
}
