using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InstantiatedObject : MonoBehaviour, IDataPersistence
{
    public bool isSelected = false;
    public string roomName;
    Rigidbody rb;
    public string prefabName = "";
    public string anchorRef = "";

    public void OnSelectItem()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
        SceneEditUIManager.instance.GoToObjectEditPanel();
        isSelected = true;

    }

    public void OnDeselectItem()
    {
        SceneEditUIManager.instance.GoToMainEditPanel();
        isSelected = false;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(string prefab)
    {
        gameObject.AddComponent<ColorChange>();
        gameObject.AddComponent<MaterialChange>();
        prefabName = prefab;
    }

    public void LoadData(ObjectData data)
    {
        //give properties to object
        gameObject.name = data.objectName;
        transform.position = data.position;
        transform.localScale = data.scale;
        transform.eulerAngles = data.rotation;
        prefabName = data.prefabName;
        
        MaterialChange mg = gameObject.GetComponent<MaterialChange>();
        if (mg != null)
        {
            Material mat = MaterialManager.instance.FindMaterial(data.materialName);
            mg.selectedMaterial = mat;
            mg.SetMaterials();
        }

        ColorChange cg = gameObject.GetComponent<ColorChange>();
        if (cg != null)
        {
            Color outVal;
            ColorUtility.TryParseHtmlString(data.color, out outVal);
            cg.actualColor = outVal;
            cg.SetColor();
        }

        anchorRef = data.anchor;
        
    }

    public void SaveData(ref AppData data)
    {
        ObjectData obj = new ObjectData();
        obj.objectName = gameObject.name;
        obj.position = transform.position;
        obj.scale = transform.localScale;
        obj.rotation = transform.eulerAngles;
        obj.prefabName = prefabName;
        ColorChange cg = gameObject.GetComponent<ColorChange>();
        if(cg != null)
        {
            obj.color = ColorUtility.ToHtmlStringRGB(cg.actualColor);
        }
        MaterialChange mg = gameObject.GetComponent<MaterialChange>();
        if (mg != null)
        {
            obj.materialName = mg.selectedMaterial.name;
        }
        obj.anchor = anchorRef;
        
        data.objectData.Add(obj);
    }
}
