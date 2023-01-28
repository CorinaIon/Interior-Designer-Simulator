using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager instance { get; private set; }

    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogError("More than one Material Manager in scene");
        }
        instance = this;

    }

    public List<Material> materials;

    public Material FindMaterial(string val)
    {
        foreach (Material mat in materials)
        {
            if (val == mat.name || val.Contains(mat.name) || mat.name.Contains(val)) return mat;
        }
        return null;
    }
}
