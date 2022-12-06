using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public Color actualColor;
    public ColorPicker colorPicker;
    InstantiatedObject obj;

    [SerializeField]
    Material[] materials;
    [SerializeField]
    Color[] baseColors;
    MeshRenderer[] renderers;

    // Start is called before the first frame update
    public void Initialize()
    {
        obj = GetComponent<InstantiatedObject>();
        colorPicker.OnColorSelect += OnColorSelected;

        renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        materials = new Material[renderers.Length];
        int i = 0;
        foreach (MeshRenderer rend in renderers)
        {
            materials[i] = rend.material;
            i++;
        }

        baseColors = new Color[materials.Length];
        i = 0;
        foreach (Material mat in materials)
        {
            baseColors[i] = mat.color;
            i++;
        }
    }

    private void OnColorSelected(object sender, EventArgs e)
    {
        if (obj.isSelected)
        {
            Color color = (sender as ColorPicker).color;
            foreach (Material mat in materials)
            {
                mat.color = color;
            }
            actualColor = color;
        }
    }

    public void SetMaterials(Material[] mat)
    {
        materials = mat;
    }
}
