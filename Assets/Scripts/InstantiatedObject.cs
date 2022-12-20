using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedObject : MonoBehaviour
{
    public bool isSelected = false;
    public string roomName;
    public void OnSelectItem()
    {
        SceneEditUIManager.instance.GoToObjectEditPanel();
        isSelected = true;
    }

    public void OnDeselectItem()
    {
        SceneEditUIManager.instance.GoToMainEditPanel();
        isSelected = false;
    }
}
