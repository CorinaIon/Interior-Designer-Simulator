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

    [SerializeField]
    private float resolveAnchorPassedTimeout = 10.0f;

    public ARAnchorManager arAnchorManager = null;
    private ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchor = null;
    private string anchorIdToResolve;
    private bool anchorHostInProgress = false;
    private bool anchorResolveInProgress = false;
    private float safeToResolvePassed = 0;
    private AnchorCreatedEvent anchorCreatedEvent = null;
    public Button g1, g2, g3;

    public static ARCloudAnchorManager Instance { get; private set; }

    public TextMeshProUGUI m_debugTextPermanent;

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

    // #TO_Andreea
    // asta e ca sa asignez variabilei globale pendingHostAnchor o Ancora (a obiectului curent. stie pozitie si rotatie)
    public void QueueAnchor(ARAnchor arAnchor)
    {
        pendingHostAnchor = arAnchor;
    }

    // #TO_Andreea
    // functia asta e apelata prin butonul portocaliu 1 stanga
    public void HostAnchor()
    {
        g1.image.color = Color.red;
        g2.image.color = Color.red;

        Debug.Log("HostAnchor call in progress");
        m_debugTextPermanent.text += " HostAnchor call in progress: " + pendingHostAnchor.ToString();

        // Get FeatureMapQuality
        FeatureMapQuality quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        m_debugTextPermanent.text += " Feature Map Quality: " + quality.ToString();

        // Save in cloudAnchor variable the result of the hosting process
        // al doilea param e ttlDays: 1-365
        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor, 1);
        
        if (cloudAnchor == null)
        {
            m_debugTextPermanent.text += " Unable to host cloud anchor";
        }
        else
        {
            anchorHostInProgress = true;
        }
        g1.image.color = Color.green;
    }

    // #TO_Andreea
    // functia asta e apelata prin butonul portocaliu 2 dreapta
    public void Resolve()
    {
        g3.image.color = Color.red;
        m_debugTextPermanent.text = "Resolve call in progress.";

        // Save in cloudAnchor variable the result of the resolve process
        cloudAnchor = arAnchorManager.ResolveCloudAnchorId(anchorIdToResolve);

        if (cloudAnchor == null)
        {
            m_debugTextPermanent.text = "Unable to resolve cloud anchor " + anchorIdToResolve.ToString();
        }
        else
        { 
            safeToResolvePassed = resolveAnchorPassedTimeout;
            anchorResolveInProgress = true;
        }
    }

    private void CheckHostingProgress()
    {
        // Implement CheckHostingProgress logic
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            anchorHostInProgress = false;
            anchorIdToResolve = cloudAnchor.cloudAnchorId;
            g2.image.color = Color.green;
        }
        else
        {
            if (cloudAnchorState != CloudAnchorState.TaskInProgress)
            {
                m_debugTextPermanent.text = "Error while hosting cloud anchor: " + cloudAnchorState.ToString();
                anchorHostInProgress = false;
            }
        }
    }

    private void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;

        if (cloudAnchorState == CloudAnchorState.Success)
        {
            anchorResolveInProgress = false;
            // #TO_Andreea
            // aici invoc o functie pe care am scris-o in Awake, in cazul asta: RecreatePlacement dim=n PlaceObject.cs (care creaza un prefab cub)
            anchorCreatedEvent?.Invoke(cloudAnchor.transform);
            g3.image.color = Color.green;
        }
        else
        {
            if (cloudAnchorState != CloudAnchorState.TaskInProgress)
            {
                m_debugTextPermanent.text = "Error while resolving cloud anchor: " + cloudAnchorState.ToString();
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
                m_debugTextPermanent.text = "Resolving Anchor Id: " + anchorIdToResolve.ToString();
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }
    }

    // #TO_Andreea
    // ideal. Dai pe buton Save Scene. Are numele NewScene1 (dar userul il poate schimba in orice mai putin "" (nimic), caz in care ramane NewScene1 de fapt. 
    // se apeleaza o functie care ia toate obiectele instantiate din scena si creaza un doc "NewScene1.extensie" ce contine o lista (sau cum te pricepi tu de la licenta):
    // prefab1 idAncora1 
    // prefab2 idAncora2
    // prefab1 (se poate repeta) idAncora3
    //..
    //cand dai Load si alegi NewScene1, se duce si citeste din fisier si face Resolve pentru fiecare id. Eventual modifici tu functia sa fie Resolve(anchorIdToResolve, prefab)
    // si Resolve mai departe invoca functia RecreatePlacement(prefab)
    //ideal ar fi sa faca astea de Host/Resolve in paralel (mai multe de o data), dar merge si intr-un for pe rand..
}