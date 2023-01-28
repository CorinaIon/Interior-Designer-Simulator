using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SceneManagementUIManager : MonoBehaviour
{
    public GameObject prefab;
    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.txt*");
        foreach(FileInfo f in info)
        {
            GameObject sceneButton = Instantiate(prefab, parent.transform);
            SceneButton sb = sceneButton.GetComponent<SceneButton>();
            if (sceneButton != null)
                sb.Init(f.Name.Split(".")[0]);
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
   
}
