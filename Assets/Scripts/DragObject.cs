using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class DragObject : MonoBehaviour
{
    private Touch touch;
    private float speedModifier;

    public TextMeshProUGUI m_debugText;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        speedModifier = 0.001f;
        var o = GameObject.Find("Locatia");
        m_debugText = o.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            //ar an 1 sem 1
            // This line moves the object up-down, left-right and front-back 
            //transform.position += new Vector3(0, touch.deltaPosition.y * speedModifier, 0);
            //transform.position += new Vector3(touch.deltaPosition.x * speedModifier, 0,  0);

            //net1
            //// get the touch position from the screen touch to world point
            //Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
            //// lerp and set the position of the current object to that of the touch, but smoothly over time.
            //transform.position = Vector3.Lerp(transform.position, touchedPos, Time.deltaTime);

            //leandragtranslate
            // Screen position of the transform
            var screenPoint = Camera.current.WorldToScreenPoint(transform.position);
            Vector2 screenDelta = touch.deltaPosition;
            float sensitivity = 0.8f;

            // Add the deltaPosition
            screenPoint += (Vector3)screenDelta * sensitivity;

            string tagName = " - ";
            string nameN = " - ";
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
                        m_debugText.text = "a mers";
                    }
                    else
                    {
                        // Convert back to world space
                        transform.position = Camera.current.ScreenToWorldPoint(screenPoint);
                        m_debugText.text = "nu a mers";
                    }
                    
                }
                //else m_debugText.text = "hit. dar nu e current";
            }
            //else m_debugText.text = "nimic";

            //m_debugText.text = "Tag: " + tagName + ", Name: " + nameN + ", Locatia: " + transform.position.ToString();

           

            
        }
    }
}
