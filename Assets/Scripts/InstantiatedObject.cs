using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedObject : MonoBehaviour
{
    public bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
