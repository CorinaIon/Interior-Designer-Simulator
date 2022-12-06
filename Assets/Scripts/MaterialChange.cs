using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour
{
    public Material selectedMaterial;
    InstantiatedObject obj;
    MaterialUIMenu menu;

    [SerializeField]
    Material[] baseMaterials;
    MeshRenderer[] renderers;
    Material[] materials;
    public void Initialize()
    {
        menu = MaterialUIMenu.instance;
        obj = GetComponent<InstantiatedObject>();
        
        menu.OnClickEvent += OnMaterialSelected;
        
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        baseMaterials = new Material[renderers.Length];
        materials = new Material[renderers.Length];
        int i = 0;
        foreach (MeshRenderer rend in renderers)
        {
            baseMaterials[i] = rend.material;
            materials[i] = rend.material;
        }

    }

    private void OnMaterialSelected(object sender, EventArgs e)
    {
        if (obj.isSelected)
        {
            //Debug.Log(sender as MaterialButton);
            
            MaterialButton matObj = sender as MaterialButton;
            int i = 0;
            foreach (MeshRenderer rend in renderers)
            {
                rend.material = matObj.objectReference.objectMaterial;
                materials[i] = rend.material;
            }
            selectedMaterial = matObj.objectReference.objectMaterial;
            ColorChange cg = GetComponent<ColorChange>();

            if (cg != null) cg.SetMaterials(materials);
        }
    }
}
