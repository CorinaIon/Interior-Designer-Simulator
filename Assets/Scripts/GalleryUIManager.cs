using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class GalleryUIManager : MonoBehaviour
{
    #region Singleton Gallery
    public static GalleryUIManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject prefab;
    public GameObject parent;
    public RawImage img;
    public GameObject panel1;
    public GameObject panel2;

    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.png*");
        foreach(FileInfo f in info)
        {
            GameObject galleryButton = Instantiate(prefab, parent.transform);
            GalleryButton gb = galleryButton.GetComponent<GalleryButton>();
            if (galleryButton != null) {
                byte[] fileData = File.ReadAllBytes(f.FullName);
                Texture2D tex = new Texture2D(500, 300);
                tex.LoadImage(fileData);
                gb.img.texture = tex;
                gb.file = f.FullName;
            }
        }
        FileInfo[] info2 = dir.GetFiles("*.jpg*");
        foreach (FileInfo f in info2)
        {
            GameObject galleryButton = Instantiate(prefab, parent.transform);
            GalleryButton gb = galleryButton.GetComponent<GalleryButton>();
            if (galleryButton != null)
            {
                byte[] fileData = File.ReadAllBytes(f.FullName);
                Texture2D tex = new Texture2D(500, 300);
                tex.LoadImage(fileData);
                gb.img.texture = tex;
                gb.file = f.FullName;
            }
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
   
    public void LoadButton(string f)
    {
        panel1.SetActive(true);
        byte[] fileData = File.ReadAllBytes(f);
        Texture2D tex = new Texture2D(500, 300);
        tex.LoadImage(fileData);
        img.texture = tex;
        //panel2.SetActive(false);
    }
}
