using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    int guideIndex = 0;
    public GameObject guideObject;
    public Text guideText;
    List<string> listGuideText;
    public Text nextButtonText;
    public Button previousButton;
    public GameObject inventory;
    public GameObject inventoryArea;
    public GameObject drag;
    public GameObject deleteDrag;
    public GameObject snap;
    public GameObject rotate;
    public GameObject texture;
    public GameObject color;
    public GameObject delete1;
    public GameObject confirm;
    public GameObject deleteAll;
    public GameObject screenshot;
    public GameObject save;
    public GameObject settings;
    
    void Start()
    {
        listGuideText = new List<string> {
             "",
            "In order to add new objects press the Inventory Button from the upper-left corner",
            "In this area you can see all the available objects, grouped by their room type, which can be switched from the tabs below",
            "To place an object, simply drag-and-drop it from the inventory to a visible plane",
            "If you no longer want to finalize the drag-and-drop operation, drop the object above the recycle bin, which will turn green",
            "To move an object, tap it with your finger and slide it slowly across the screen, to the desired position",
            "Press the snap Button if you want the object to be automatically snapped to an existing plane",
            "To increase or decrease the size of the object, place two fingers on the screen and move them closer or further apart",
            "To rotate and object, press the arrow buttons. The object will be rotated with 15 degrees to the left/right",
            "To change the texture of the object, press the Texture button and choose from the available list",
            "To change the color of the object, press the Color button and chose a color from the Color Wheel",
            "To delete the selected object, press the Recycle Bin button from the bottom-left corner",
            "When you want to exit the Edit Object Mode and enter the Scene Edit Mode again, press the green Confirm Button",
            "If you want to reselect an object double-tap it",
            "To delete all the objects in the scene, press the Recycle Bin Button while you are in Scene Edit Mode",
            "To take a screenshot of the current scene, press the Camera button. All your screenshots can be viewed in the gallery, or in your Android/data folder",
            "To save the scene, press the Save Scene button from the bottom-right corner. You can see all your saved scenes in the Load Scene view from the Main Menu",
            "If you want to disable the default yellow planes, start the guide or go back to the Main Menu, press the Settings button",
            "This is the end of the guide! You can start it again any time from the Settings Button!",
            ""
            };
    }

    public void StartGuide()
    {
        SceneEditUIManager.instance.HideSettings();
        guideObject.SetActive(true);
        guideIndex = 0;

        HideEverythhing();

        nextButtonText.text = "Next";
        Next();
    }

    public void Next()
    {
        ++guideIndex;
        UpdateGuideUI();
    }

    public void Previous()
    {
        --guideIndex;
        UpdateGuideUI();
    }

    private void UpdateGuideUI()
    {   
        guideText.text = listGuideText[guideIndex];
        switch (guideIndex)
        {
            case 1:
                {
                    SceneEditUIManager.instance.GoToMainEditPanel();

                    // close inventory if opened
                    inventoryArea.SetActive(false);
                    if (ObjectsUIMenu.instance.clicked)
                        ObjectsUIMenu.instance.DisplayMenu();

                    previousButton.interactable = false;
                    inventory.SetActive(true);
                    break;
                }
            case 2:
                {
                    drag.SetActive(false);

                    previousButton.interactable = true;
                    inventory.SetActive(false);

                    // open inventory
                    if (!ObjectsUIMenu.instance.clicked)
                        ObjectsUIMenu.instance.DisplayMenu();

                    inventoryArea.SetActive(true);

                    break;
                }
            case 3:
                {
                    deleteDrag.SetActive(false);

                    inventoryArea.SetActive(false);

                    drag.SetActive(true);
                    break;
                }
            case 4:
                {
                    // open inventory
                    if (!ObjectsUIMenu.instance.clicked)
                        ObjectsUIMenu.instance.DisplayMenu();

                    drag.SetActive(false);

                    deleteDrag.SetActive(true);
                    break;
                }
            case 5:
                {
                    snap.SetActive(false);
                    SceneEditUIManager.instance.GoToMainEditPanel();

                    deleteDrag.SetActive(false);
                    //close inventory
                    if (ObjectsUIMenu.instance.clicked)
                        ObjectsUIMenu.instance.DisplayMenu();

                    // move object
                    break;
                }
            case 6:
                {
                    //

                    snap.SetActive(true);
                    SceneEditUIManager.instance.GoToObjectEditPanel();
                    break;
                }
            case 7:
                {
                    rotate.SetActive(false);

                    snap.SetActive(false);
                    
                    // increase/decrease size
                    break;
                }
            case 8:
                {
                    // close texture
                    texture.SetActive(false);
                    if (MaterialUIMenu.instance.clicked)
                        MaterialUIMenu.instance.DisplayMenu();

                    rotate.SetActive(true);
                    break;
                }
            case 9:
                {
                    color.SetActive(false);

                    // close color menu
                    if (SceneEditUIManager.instance.isColorWheelActive)
                        SceneEditUIManager.instance.DisplayColorWheel();

                    rotate.SetActive(false);

                    //open texture menu
                    texture.SetActive(true);
                    if (!MaterialUIMenu.instance.clicked)
                        MaterialUIMenu.instance.DisplayMenu();
                    break;
                }
            case 10:
                {
                    delete1.SetActive(false);

                    // close texture menu
                    texture.SetActive(false);
                    if (MaterialUIMenu.instance.clicked)
                        MaterialUIMenu.instance.DisplayMenu();

                    // open color menu
                    color.SetActive(true);
                    if (!SceneEditUIManager.instance.isColorWheelActive)
                        SceneEditUIManager.instance.DisplayColorWheel();
                    break;
                }
            case 11:
                {
                    confirm.SetActive(false);

                    // close color menu 
                    if (SceneEditUIManager.instance.isColorWheelActive)
                        SceneEditUIManager.instance.DisplayColorWheel();
                    color.SetActive(false);

                    delete1.SetActive(true);
                    break;
                }
            case 12:
                {
                    SceneEditUIManager.instance.GoToObjectEditPanel();

                    delete1.SetActive(false);

                    confirm.SetActive(true);
                    break;
                }
            case 13:
                {
                    deleteAll.SetActive(false);

                    confirm.SetActive(false);

                    // double tap
                    SceneEditUIManager.instance.GoToMainEditPanel();
                    break;
                }
            case 14:
                {
                    screenshot.SetActive(false);

                    //

                    deleteAll.SetActive(true);
                    break;
                }
            case 15:
                {
                    save.SetActive(false);

                    deleteAll.SetActive(false);

                    screenshot.SetActive(true);
                    break;
                }
            case 16:
                {
                    settings.SetActive(false);

                    screenshot.SetActive(false);

                    save.SetActive(true);
                    break;
                }
            case 17:
                {
                    nextButtonText.text = "Next";

                    save.SetActive(false);

                    settings.SetActive(true);

                    break;
                }
            case 18:
                {
                    settings.SetActive(false);

                    nextButtonText.text = "Finish";
                    break;
                }
            case 19:
                {
                    Cancel();
                    break;
                }
        }
    }

    public void Cancel()
    {
        guideObject.SetActive(false);
        HideEverythhing();
        SceneEditUIManager.instance.GoToMainEditPanel();
    }

    private void HideEverythhing()
    {
        inventory.SetActive(false);
        inventoryArea.SetActive(false);
        drag.SetActive(false);
        deleteDrag.SetActive(false);
        snap.SetActive(false);
        rotate.SetActive(false);
        texture.SetActive(false);
        color.SetActive(false);
        delete1.SetActive(false);
        confirm.SetActive(false);
        deleteAll.SetActive(false);
        screenshot.SetActive(false);
        save.SetActive(false);
        settings.SetActive(false);
    }
}
