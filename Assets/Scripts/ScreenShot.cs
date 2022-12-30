using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ScreenShot : MonoBehaviour
{
    private bool isProcessing = false;
    public float startX = 0;
    public float startY = 0;
    public int valueX = 10000;
    public int valueY = 10000;
    public float screenshotFactor = 0.5f;
    public float timeCloseScreenshotPreview = 1.5f;
    public GameObject screenshotImage;
    public Animator animator;
    public TextMeshProUGUI m_Text;

    public void ScreenshotPressed()
    {
        if (!isProcessing)
            StartCoroutine(captureScreenshot());
    }

    public IEnumerator captureScreenshot()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();

        var width = Screen.width * valueX / 10000;
        var height = Screen.height * valueY / 10000;
        Texture2D screenTexture = new Texture2D(width, height, TextureFormat.RGB24, true);

        // put buffer into texture
        //screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
        //create a Rect object as per your needs.
        screenTexture.ReadPixels(new Rect
                                 (Screen.width * startX, (Screen.height * startY), width, height), 0, 0);

        // apply
        screenTexture.Apply();

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO

        //byte[] dataToSave = Resources.Load<TextAsset>("everton").bytes;
        byte[] dataToSave = screenTexture.EncodeToPNG();

        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");

        File.WriteAllBytes(destination, dataToSave);
        Debug.Log("Screenshot captured at: " + destination);
        m_Text.text = "Screenshot captured at: " + destination;

        // Play sound here
        // TODO maybe?


        // UI stuff

        // Display screenshot on screen
        screenshotImage.GetComponent<RawImage>().texture = screenTexture;
        var rect = screenshotImage.GetComponent<RectTransform>().rect;
        //screenshotImage.GetComponent<RectTransform>().rect.Set(rect.x, rect.y, width * screenshotFactor, height * screenshotFactor);
        screenshotImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width * screenshotFactor, height * screenshotFactor);

        // Animation
        screenshotImage.SetActive(true);
        animator.SetBool("open", true);
        yield return new WaitForSeconds(timeCloseScreenshotPreview);
        animator.SetBool("open", false);
        yield return new WaitForSeconds(timeCloseScreenshotPreview);
        screenshotImage.SetActive(false);


        // facea share cica
        //if (!Application.isEditor)
        //{
        //    // block to open the file and share it ------------START
        //    AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //    AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //    intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        //    AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        //    AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
        //    intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
        //    intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Which Football Club does this Rebus represent?\n" +
        //                                         "Download the game on play store at " + "\nhttps://play.google.com/store/apps/details?id=com.TGC.RebusFC&pcampaignid=GPC_shareGame");
        //    intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Which club is this?");
        //    intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
        //    AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //    AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        //    // option one WITHOUT chooser:
        //    currentActivity.Call("startActivity", intentObject);

        //    // block to open the file and share it ------------END

        //}

        isProcessing = false;

    }
}