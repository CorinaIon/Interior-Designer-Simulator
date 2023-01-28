using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private AppData appData;
    private List<InstantiatedObject> dataPersitenceObjects = new List<InstantiatedObject>();

    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {

        if(instance != null)
        {
            Debug.LogError("More than one Data Persistence Manager in scene");
        }
        instance = this;

    }

    public TMP_InputField fileNameText;

    private void Start()
    {
        fileName = SceneController.fileNameOnScene;
        LoadGame();
    }

    public void NewGame()
    {
        this.appData = new AppData();
    }

    public void LoadGame()
    {
        Debug.Log(fileName);
        
        if (fileName != null && fileName != "")
        {
            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName + ".txt");
            appData = dataHandler.Load();

            if (this.appData == null)
            {
                Debug.Log("No data was found. Initializing data to defaults.");
                NewGame();
            }

            //create objects
            foreach (ObjectData data in appData.objectData)
            {
                GameObject prefab = PrefabManager.instance.FindPrefab(data.prefabName);
                if(prefab != null)
                {
                    PlaceObject.instance.AddObject(prefab);
                    PlaceObject.instance.DeselectObject();
                }
            }

            //load data for objects
            foreach (InstantiatedObject obj in dataPersitenceObjects)
            {
                foreach (ObjectData data in appData.objectData)
                {
                    //put condition
                    if (obj.name == data.objectName)
                    {
                        obj.LoadData(data);
                        ARCloudAnchorManager.Instance.anchorIdToResolve = data.anchor;
                        ARCloudAnchorManager.Instance.Resolve();
                    }
                }
            }
        }
        else NewGame();
    }

    public void SaveGame()
    {
        appData = new AppData();
        fileName = fileNameText.text;
        
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName + ".txt");
        
        foreach (InstantiatedObject obj in dataPersitenceObjects)
        {
            PlaceObject.instance.SelectObject(obj.gameObject);
            ARCloudAnchorManager.Instance.HostAnchor();
            ARCloudAnchorManager.Instance.CheckHostingProgress();
            obj.anchorRef = ARCloudAnchorManager.Instance.anchorIdToResolve;
            obj.SaveData(ref appData);
        }

        dataHandler.Save(appData);
        
    }

    public void DeleteScene()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName + ".txt");
        try
        {
            if (File.Exists(fullPath)) File.Delete(fullPath);
            
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to delete scene: " + fullPath + " " + e);
        }

    }



    //to be called when creating or deleting an object
    public void AddDataPersistenceObject(InstantiatedObject obj)
    {
        dataPersitenceObjects.Add(obj);
    }

    public void RemoveDataPersistenceObject(InstantiatedObject obj)
    {
        dataPersitenceObjects.Remove(obj);
    }
}
