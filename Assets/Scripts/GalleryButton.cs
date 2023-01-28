using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GalleryButton : MonoBehaviour
{
    public RawImage img;
    
    public string file;
   
    public void Click()
    {
        GalleryUIManager.instance.LoadButton(file);
    }
}
