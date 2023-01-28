using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using TMPro;

public class AnchorCreatedEvent : UnityEvent<Transform> { }

public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField]
    public Camera arCamera = null;

    public ARAnchorManager arAnchorManager = null;
    private ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchor = null;
    private AnchorCreatedEvent anchorCreatedEvent = null;
    public string anchorIdToResolve;
    private bool anchorHostInProgress = false;
    private bool anchorResolveInProgress = false;
    private float resolveAnchorPassedTimeout = 10.0f;
    private float safeToResolvePassed = 0;
    //public TextMeshProUGUI m_debugText;

    public static ARCloudAnchorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        anchorCreatedEvent = new AnchorCreatedEvent();
        anchorCreatedEvent.AddListener((t) => PlaceObject.instance.RecreatePlacement(t));
    }

    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position, arCamera.transform.rotation);
    }

    public void QueueAnchor(ARAnchor arAnchor)
    {
        pendingHostAnchor = arAnchor;
    }

    public void HostAnchor()
    {
        // m_debugText.text += " HostAnchor call in progress: " + pendingHostAnchor.ToString();

        // Get FeatureMapQuality
        FeatureMapQuality quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        // m_debugText.text += " Feature Map Quality: " + quality.ToString();

        // Save in cloudAnchor variable the result of the hosting process
        // ttl for anchors 31 days. Only premium users can have them for 365 days ;)
        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor, 31);

        if (cloudAnchor == null)
        {
            //m_debugText.text += " Unable to host cloud anchor";
        }
        else
        {
            anchorHostInProgress = true;
        }
    }

    // #TO_Andreea
    // functia asta e apelata prin butonul portocaliu 2 dreapta
    public void Resolve()
    {
        // m_debugText.text = "Resolve call in progress.";

        // Save in cloudAnchor variable the result of the resolve process
        cloudAnchor = arAnchorManager.ResolveCloudAnchorId(anchorIdToResolve);

        if (cloudAnchor == null)
        {
            // m_debugText.text = "Unable to resolve cloud anchor " + anchorIdToResolve.ToString();
        }
        else
        {
            safeToResolvePassed = resolveAnchorPassedTimeout;
            anchorResolveInProgress = true;
        }
    }

    public bool CheckHostingProgress()
    {
        // Implement CheckHostingProgress logic
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            anchorHostInProgress = false;
            anchorIdToResolve = cloudAnchor.cloudAnchorId;
            return true;
        }
        else
        {
            if (cloudAnchorState != CloudAnchorState.TaskInProgress)
            {
                // m_debugText.text = "Error while hosting cloud anchor: " + cloudAnchorState.ToString();
                anchorHostInProgress = false;
            }
            return false;
        }
    }

    private void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;

        if (cloudAnchorState == CloudAnchorState.Success)
        {
            anchorResolveInProgress = false;
            anchorCreatedEvent?.Invoke(cloudAnchor.transform);
        }
        else
        {
            if (cloudAnchorState != CloudAnchorState.TaskInProgress)
            {
                // m_debugText.text = "Error while resolving cloud anchor: " + cloudAnchorState.ToString();
                anchorResolveInProgress = false;
            }
        }
    }

    void Update()
    {
        // Check host result
        if (anchorHostInProgress)
        {
            CheckHostingProgress();
            return;
        }

        // Check resolve result
        if (anchorResolveInProgress && safeToResolvePassed <= 0)
        {
            safeToResolvePassed = resolveAnchorPassedTimeout;

            if (!string.IsNullOrEmpty(anchorIdToResolve))
            {
                // m_debugText.text = "Resolving Anchor Id: " + anchorIdToResolve.ToString();
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }
}