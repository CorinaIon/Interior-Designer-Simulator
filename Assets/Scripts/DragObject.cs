using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class DragObject : MonoBehaviour
{
    private Touch touch;

    public TextMeshProUGUI m_debugText;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Start()
    {
        var o = GameObject.Find("Locatia");
        m_debugText = o != null ? o.GetComponent<TextMeshProUGUI>() : null;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            // Screen position of the transform
            var screenPoint = Camera.current.WorldToScreenPoint(transform.position);
            Vector2 screenDelta = touch.deltaPosition;
            float sensitivity = 0.8f;

            // Add the deltaPosition
            screenPoint += (Vector3)screenDelta * sensitivity;

            Vector3 pos = touch.position;

            Ray ray = PlaceObject.instance.m_firstPersonCamera.ScreenPointToRay(pos);
            RaycastHit hitObject = new RaycastHit();

            // Check if a 3D object was hit
            // Check if 3D object was tapped

            if (Physics.Raycast(ray, out hitObject))
            {
                if (hitObject.transform.tag == "Manipulated" && hitObject.transform.gameObject == PlaceObject.instance.m_currentSelection)
                //m_debugText.text = "yep. Hit Tag " + hitObject.transform.tag + ", name: " + hitObject.transform.name + "Hit Coll Tag " + hitObject.collider.tag;
                {
                    bool snap = PlaceObject.instance.snap;
                    if (snap && PlaceObject.instance.m_raycastManager.Raycast(pos, s_Hits, TrackableType.PlaneWithinPolygon))
                    {
                        transform.position = s_Hits[0].pose.position;
                    }
                    else
                    {
                        // Convert back to world space
                        transform.position = Camera.current.ScreenToWorldPoint(screenPoint);
                    }
                    
                }
            }
        }
    }
}
