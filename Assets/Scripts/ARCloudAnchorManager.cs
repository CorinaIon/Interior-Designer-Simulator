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

/* TODO 1. Enable ARCore Cloud Anchors API */
public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField]
    public Camera arCamera = null;

    [SerializeField]
    private float resolveAnchorPassedTimeout = 10.0f;

    public ARAnchorManager arAnchorManager = null;
    private ARCloudAnchorManager arCloudAnchorManager = null;
    private ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchor = null;
    private string anchorIdToResolve;
    private bool anchorHostInProgress = false;
    private bool anchorResolveInProgress = false;
    private float safeToResolvePassed = 0;
    private AnchorCreatedEvent anchorCreatedEvent = null;
    public Button g1, g2, g3, g4, g5, g6, g7, g8, g9, g10;

    public static ARCloudAnchorManager Instance { get; private set; }

    public TextMeshProUGUI m_debugTextPermanent;
    public TextMeshProUGUI m_debugTextPermanent3;

    private void Awake()
    {
        //arAnchorManager = GetComponent<ARAnchorManager>();
        //arCloudAnchorManager = GetComponent<ARCloudAnchorManager>();
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
        g1.image.color = Color.red;
        g2.image.color = Color.red;
        g3.image.color = Color.red;
        g4.image.color = Color.red;
        g5.image.color = Color.red;
        g6.image.color = Color.red;
        g7.image.color = Color.red;
        g8.image.color = Color.red;
        g9.image.color = Color.red;
        g10.image.color = Color.red;

        if (arAnchorManager == null)
            m_debugTextPermanent.text = "howww ";
        else
            m_debugTextPermanent.text = "Anchor: pressed ";
        g1.image.color = Color.green;

        Debug.Log("HostAnchor call in progress");
        m_debugTextPermanent.text += " HostAnchor call in progress: " + pendingHostAnchor.ToString();
        g2.image.color = Color.green;

        /* TODO 3.1 Get FeatureMapQuality */
        FeatureMapQuality quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());

        Debug.Log(string.Format("Feature Map Quality: {0}", quality));
        m_debugTextPermanent.text += " Feature Map Quality: " + quality.ToString();
        if (quality != FeatureMapQuality.Insufficient)
            m_debugTextPermanent3.text = "a fost bine";
        g3.image.color = Color.green;
        /* TODO 3.2 Save in cloudAnchor variable the result of the hosting process */
        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor, 1);
        g4.image.color = Color.green;
        if (cloudAnchor == null)
        {
            g5.image.color = Color.red;
            Debug.Log("Unable to host cloud anchor");
            m_debugTextPermanent.text += " Unable to host cloud anchor";
        }
        else
        {
            g5.image.color = Color.green;
            anchorHostInProgress = true;

        }
        g6.image.color = Color.green;
    }

    public void Resolve()
    {
        Debug.Log("Resolve call in progress");
        m_debugTextPermanent.text = "Resolve call in progress.";

        /* TODO 5 Save in cloudAnchor variable the result of the resolve process */
        cloudAnchor = arAnchorManager.ResolveCloudAnchorId(anchorIdToResolve);

        if (cloudAnchor == null)
        {
            Debug.Log(string.Format("Unable to resolve cloud anchor {0}", anchorIdToResolve));
            m_debugTextPermanent.text = "Unable to resolve cloud anchor " + anchorIdToResolve.ToString();
        }
        else
        {
            //safeToResolvePassed = resolveAnchorPassedTimeout;
            anchorResolveInProgress = true;
        }
    }

    private void CheckHostingProgress()
    {
        /* TODO 3.3 Implement CheckHostingProgress logic */
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;

        g7.image.color = Color.green;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            anchorHostInProgress = false;
            anchorIdToResolve = cloudAnchor.cloudAnchorId;
            g10.image.color = Color.green;
        }
        else
        {
            if (cloudAnchorState != CloudAnchorState.TaskInProgress)
            {
                g8.image.color = Color.green;
                Debug.Log(string.Format("Error while hosting cloud anchor: {0}", cloudAnchorState));
                m_debugTextPermanent.text = "Error while hosting cloud anchor: " + cloudAnchorState.ToString();
                anchorHostInProgress = false;
                g9.image.color = Color.green;
            }
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
                Debug.Log(string.Format("Error while resolving cloud anchor: {0}", cloudAnchorState));
                m_debugTextPermanent.text = "Error while resolving cloud anchor: " + cloudAnchorState.ToString();
                anchorResolveInProgress = false;
            }
        }
    }

    void Update()
    {
        
        
        /* Check host result */
        if (anchorHostInProgress)
        {
            CheckHostingProgress();
            return;
        }

        /* Check resolve result */
        if (anchorResolveInProgress && safeToResolvePassed <= 0)
        {
            safeToResolvePassed = resolveAnchorPassedTimeout;

            if (!string.IsNullOrEmpty(anchorIdToResolve))
            {
                Debug.Log(string.Format("Resolving Anchor Id: {0}", anchorIdToResolve));
                m_debugTextPermanent.text = "Resolving Anchor Id: " + anchorIdToResolve.ToString();
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }
}