using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObjectData
{
    public string anchor;
    public string prefabName;
    public string objectName;
    public Vector3 position;
    public Vector3 scale;
    public Vector3 rotation;
    public string color;
    public string materialName;

}

[System.Serializable]
public class AppData 
{
    public List<ObjectData> objectData = new List<ObjectData>();
    public string sceneName;

    public AppData()
    {
        this.objectData = new List<ObjectData>();
    }
}
