using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Lean.Touch;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObject : MonoBehaviour
{
    #region Singleton PlaceObject
    public static PlaceObject instance;
    //private void Awake()
    //{
    //    instance = this;
    //}
    #endregion

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    public ARRaycastManager m_raycastManager;
    // The first-person camera being used to render the passthrough camera image (i.e. AR background)
    public Camera m_firstPersonCamera;
    // The object instantiated as a result of a successful raycast intersection with a plane
    public GameObject m_currentSelection = null;
    bool m_doubleTap = false;
    float m_lastTapTime;
    List<GameObject> m_addedObjects;
    [SerializeField]
    public Dictionary<string, GameObject> m_nameToPrefab;
    public GameObject prefabCube;
    public Vector2 m_touchPosition;
    public Canvas m_canvas;
    public GameObject m_inventoryParent;
    public GameObject m_newUI;
    public GameObject m_movingObject;
    public List<GameObject> m_menuButtons;
    private bool m_isDeleteHovered = false;
    public bool snap = false;
    public Button trashButton;

    public TextMeshProUGUI m_debugText;
    public GameObject parentForCreation;
    void Awake()
    {
        instance = this;

        m_raycastManager = GetComponent<ARRaycastManager>();
        m_addedObjects = new List<GameObject>();
        m_nameToPrefab = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        m_nameToPrefab = ObjectsUIMenu.instance.GetItemDictionary();
        SceneEditUIManager.instance.ChangeColorSnapButton(Color.red);
    }

    // Only the selected object should be able to be scaled, rotated or translated
    public void SelectObject(GameObject selected)
    {
        DeselectObject();

        m_currentSelection = selected;

        // Add the translation and scaling scripts to the current objects
        m_currentSelection.AddComponent<DragObject>();
        m_currentSelection.AddComponent<LeanPinchScale>();

        // Pinch and twist gestures require two fingers on screen
        m_currentSelection.GetComponent<LeanPinchScale>().Use.RequiredFingerCount = 2;

        selected.GetComponent<InstantiatedObject>().OnSelectItem();
        selected.GetComponent<ColorChange>().colorPicker = SceneEditUIManager.instance.pickerInstance;
        selected.GetComponent<ColorChange>().Initialize();
        selected.GetComponent<MaterialChange>().Initialize();
        string str = selected.GetComponent<InstantiatedObject>().roomName;
        MaterialUIMenu.instance.FilterList(str);

        ARCloudAnchorManager.Instance.QueueAnchor(selected.GetComponent<ARAnchor>());
    }

    // Remove translation, rotation and scaling scripts for previously selected object
    public void DeselectObject()
    {
        if (m_currentSelection != null)
        {
            // Destroy DragObject and LeanPinchScale components for previously selected object
            if (m_currentSelection.GetComponent<DragObject>())
            {
                Destroy(m_currentSelection.GetComponent<DragObject>());
            }
            if (m_currentSelection.GetComponent<LeanPinchScale>())
            {
                Destroy(m_currentSelection.GetComponent<LeanPinchScale>());
            }
            m_currentSelection.GetComponent<InstantiatedObject>().OnDeselectItem();

            m_currentSelection = null;
        }
    }

    bool CheckDoubleTap(out Vector2 touchPosition)
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            touchPosition = default;
            return false;
        }

        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;

            Ray ray = m_firstPersonCamera.ScreenPointToRay(touchPosition);
            RaycastHit hitObject = new RaycastHit();

            // Check if 3D object was tapped
            if (Physics.Raycast(ray, out hitObject))
            {
                if (hitObject.transform.tag == "Manipulated")
                {
                    // Check if a small amount of time has passed since the last tap
                    if (Time.time < m_lastTapTime + 0.3f)
                    {
                        m_doubleTap = true;
                    }

                    // If double tap event occured
                    if (m_doubleTap)
                    {
                        // Call function which adds the scripts for object manipulation
                        SelectObject(hitObject.transform.gameObject);

                        m_doubleTap = false;
                    }

                    m_lastTapTime = Time.time;
                    touchPosition = default;
                    return false;
                }
            }
            return true;
        }

        touchPosition = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        m_touchPosition = default;
        if (Input.touchCount < 1)
        {
            m_touchPosition = default;
        }
        else
        {
            m_touchPosition = Input.GetTouch(0).position;
        }

        if (m_currentSelection == null)
        {
            if (m_touchPosition != default && m_movingObject != null)
            {
                DragMenuButton();
            }
        }

        if (m_currentSelection == null)
        {
            CheckDoubleTap(out Vector2 useless_touchPosition);
        }
        
        DrawBoundingBox();
    }

    // Delete the currently selected object
    public void Delete()
    {
        DataPersistenceManager.instance.RemoveDataPersistenceObject(m_currentSelection.GetComponent<InstantiatedObject>());
        Destroy(m_currentSelection);
        m_addedObjects.Remove(m_currentSelection);
        m_currentSelection = null;
        m_debugText.text = "Deleted";
        SceneEditUIManager.instance.GoToMainEditPanel();
        m_isDeleteHovered = false;
    }

    public void DeleteAll()
    {
        foreach (var obj in m_addedObjects)
        {
            DataPersistenceManager.instance.RemoveDataPersistenceObject(obj.GetComponent<InstantiatedObject>());
            Destroy(obj);
        }
        m_addedObjects.Clear();
        m_movingObject = null;
        m_currentSelection = null;
        m_isDeleteHovered = false;
    }

    public void StartAddingObject(BaseEventData data)
    {
        if (m_currentSelection != null)
            return;

        m_debugText.color = Color.red;
        GameObject pressedButton = data.selectedObject;

        // Here we add the preview prefab (not the actual object, just a preview)
        m_movingObject = Instantiate(m_newUI, pressedButton.transform.position, pressedButton.transform.rotation);
        m_movingObject.transform.parent = m_inventoryParent.transform;

        m_movingObject.AddComponent<LeanDragTranslate>();
    }
    public void StopAddingObject(BaseEventData data)
    {
        if (m_currentSelection != null)
            return;

        m_debugText.color = Color.green;
        if (m_movingObject.GetComponent<LeanDragTranslate>())
        {
            Destroy(m_movingObject.GetComponent<LeanDragTranslate>());
        }

        if(!m_isDeleteHovered)
        {
            m_debugText.color = Color.cyan;
            var auxName = data.selectedObject.GetComponent<ObjectButton>().objectReference.name; //Debug.Log(auxName);
            var prefabToAdd = m_nameToPrefab[auxName]; //data.selectedObject.name
            AddObject(prefabToAdd);
        }
        // else currentSelection remains null, nothing is added.
        else
        {
            m_debugText.color = Color.magenta;
        }
        Destroy(m_movingObject);
        m_movingObject = null;
    }

    public void AddObject(GameObject prefabToAdd)
    {
        Vector3 position = parentForCreation.transform.position;
        
        if (m_movingObject != null)
        {
            m_debugText.text = "Addding new";
            m_debugText.text += "\ntp " + m_touchPosition;
            m_debugText.text += "\nmo.p " + m_movingObject.transform.position;

            if (m_raycastManager.Raycast(m_touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                position = s_Hits[0].pose.position;
            }
            else
            {
                //If the destination (last position of drag & drop) is not valid (on an AR plane) we will not instantiate an object
                // position = m_movingObject.transform.position;
            }

            m_debugText.text += "\nfinal p " + position;
        }
        // Add a new object in scene
        var spawnedObject = Instantiate(prefabToAdd, position, prefabToAdd.transform.rotation);
        spawnedObject.transform.localScale /= 10.0f;

        ////obsolete
        ///Attach an anchor to the prefab (Hint! Use AddAnchor method from arAnchorManager object)
        //ARAnchor anchor = arAnchorManager.AddAnchor(new Pose(spawnedObject.transform.position, spawnedObject.transform.rotation));
        //spawnedObject.transform.parent = anchor.transform;
        ARAnchor anchor = spawnedObject.AddComponent<ARAnchor>();
        // Send anchor to ARCloudAnchorManager
        ARCloudAnchorManager.Instance.QueueAnchor(anchor);
        //spawnedObject.AddComponent<InstantiatedObject>();
        
        m_addedObjects.Add(spawnedObject);
        spawnedObject.GetComponent<InstantiatedObject>().Initialize(prefabToAdd.name);
        
        InstantiatedObject iO = spawnedObject.GetComponent<InstantiatedObject>();
        
        DataPersistenceManager.instance.AddDataPersistenceObject(iO);
        if (parentForCreation != null) spawnedObject.transform.parent = parentForCreation.transform;
        SelectObject(spawnedObject);
    }

    void DrawBoundingBox()
    {
        if (m_currentSelection != null)
            ; // TODO
    }

    public void ConfirmChanges()
    {
        DeselectObject();
    }

    public void DragMenuButton()
    {
        m_debugText.color = Color.yellow;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)m_canvas.transform,
            m_touchPosition,
            m_canvas.worldCamera,
            out position
            );

        m_movingObject.transform.position = m_canvas.transform.TransformPoint(position);
        m_debugText.text = "pos pentru " + m_movingObject.transform.name + " este " + m_movingObject.transform.position;
    }

    public void DeleteEnterPointer()
    {
        m_isDeleteHovered = true;
        if(m_currentSelection == null)
            trashButton.image.color = Color.green;
    }

    public void DeleteExitPointer()
    {
        m_isDeleteHovered = false;
        trashButton.image.color = Color.red;
    }

    public void Rotate(int direction)
    {
        float amount = 15.0f;
        m_currentSelection.transform.Rotate(0, amount * direction, 0, Space.World);
    }

    public void SwitchSnapMode()
    {
        snap = !snap;
        if (snap) 
            SceneEditUIManager.instance.ChangeColorSnapButton(Color.green);
        else SceneEditUIManager.instance.ChangeColorSnapButton(Color.red);
    }

    public void RecreatePlacement(Transform transform)
    {
        var spawnedObject = Instantiate(prefabCube, transform.position, transform.rotation);
        spawnedObject.transform.parent = transform;
        m_debugText.text = "ancora repusa";
    }
}
