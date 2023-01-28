using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance { get; private set; }

    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogError("More than one Prefab Manager in scene");
        }
        instance = this;

    }

    public List<GameObject> prefabs;

    public GameObject FindPrefab(string val)
    {
        foreach(GameObject obj in prefabs)
        {
            if (val == obj.name) return obj;
        }
        return null;
    }
}
