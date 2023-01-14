using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedObject : MonoBehaviour
{
    public bool isSelected = false;
    public string roomName;
    Rigidbody rb;
    public void OnSelectItem()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
        SceneEditUIManager.instance.GoToObjectEditPanel();
        isSelected = true;
        //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        //rb.constraints = RigidbodyConstraints.None;
        //rb.useGravity = true;

    }

    public void OnDeselectItem()
    {
        SceneEditUIManager.instance.GoToMainEditPanel();
        isSelected = false;
        //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        //rb.constraints = RigidbodyConstraints.FreezeAll;
        //rb.useGravity = false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
