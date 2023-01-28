using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class FileDataHandler 
{
    private string dirPath = "";
    private string fileName = "";

    public FileDataHandler(string val1, string val2)
    {
        dirPath = val1;
        fileName = val2;
    }

    public AppData Load()
    {
        string fullPath = Path.Combine(dirPath, fileName);
        AppData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<AppData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error occured when trying to load data: " + fullPath + " " + e);
            }
        }
        return loadedData;
    }

    public void Save(AppData data)
    {
        string fullPath = Path.Combine(dirPath,fileName);
        
        try
        {
            if(File.Exists(fullPath)) File.Delete(fullPath);
            //serialize game data
            string dataToStore = JsonUtility.ToJson(data, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        } catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data: " + fullPath + " " + e);
        }
    }
}
