using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialButton : MonoBehaviour
{
    public MaterialObject objectReference = null;
    public Image icon;
    public int index;
   
    public int bttnId;

    private void Awake()
    {
        icon = GetComponent<Image>();
    }
    public void ActionOnClick()
    {
        //TODO
        if (objectReference != null)
        {
            
            if(bttnId == 0)
                (MaterialUIMenu.instance).Invoke1();
            if (bttnId == 1)
                (MaterialUIMenu.instance).Invoke2();
            if (bttnId == 2)
                (MaterialUIMenu.instance).Invoke3();
            if (bttnId == 3)
                (MaterialUIMenu.instance).Invoke4();
            
            
        }
    }

    public void SetEvent(int e){
        bttnId = e;
    }

    public void SetReference(MaterialObject reff, int id)
    {
        objectReference = reff;
        icon.sprite = reff != null ? reff.icon : null;
        index = id;
        Color c = icon.color;
        c.a = reff != null ? 1 : 0;
        icon.color = c;
    }
}
