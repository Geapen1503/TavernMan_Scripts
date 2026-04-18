using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPCam : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public GameObject dialogueNPCCanvas;

    private Camera thisCam;
    private float xRotation = 0f;
    private bool lockOnDialogue = false;
    private bool lockOnPauseMenu = false;

    [Header("Serving Settings")]
    private bool isServing = false;
    public float servingMinPitch = -20f;
    public float servingMaxPitch = 25f;

    [Header("Carrying Settings")]
    private bool isCarryingObject = false;
    public float carryMinPitch = -50f; 
    public float carryMaxPitch = 50f;

    [HideInInspector] public bool isGrabbingSystemActive = false;

    //private GameObject oldGrabbableObject = null;
    private GameObject[] grabbableGameObjects;

    public static FPCam Instance {  get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple FPCam instances detected. Keeping the first one.");
            return;
        }
        Instance = this;
    }

    void Start()
    {
        thisCam = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (lockOnDialogue == false && LockOnPauseMenu == false) CameraInputs();

        if (isGrabbingSystemActive) HighlightGameObject();
    }

    public virtual void CameraInputs()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        float min = -90f;
        float max = 90f;

        if (isServing)
        {
            min = servingMinPitch;
            max = servingMaxPitch;
        }
        else if (isCarryingObject)
        {
            min = carryMinPitch;
            max = carryMaxPitch;
        }

        xRotation = Mathf.Clamp(xRotation, min, max);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }


    public void EnterDialogueMode(float transitionTime = 1f)
    {
        if (lockOnDialogue) return;
        if (dialogueNPCCanvas == null) return;
        

        Canvas canvas = dialogueNPCCanvas.GetComponent<Canvas>();
        if (canvas == null) return;
        if (!canvas.enabled) return;

        lockOnDialogue = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(SmoothLookAt(dialogueNPCCanvas.transform, transitionTime));
    }

    public void ExitDialogueMode()
    {
        if (!lockOnDialogue) return;

        lockOnDialogue = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator SmoothLookAt(Transform target, float duration)
    {
        Vector3 dir = (target.position - transform.position).normalized;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        Vector3 targetEuler = targetRot.eulerAngles;

        float targetYaw = targetEuler.y;
        float targetPitch = targetEuler.x;

        float startYaw = playerBody.rotation.eulerAngles.y;
        float startPitch = xRotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!lockOnDialogue) yield break;

            float t = elapsed / duration;

            float newYaw = Mathf.LerpAngle(startYaw, targetYaw, t);
            float newPitch = Mathf.LerpAngle(startPitch, targetPitch, t);

            playerBody.rotation = Quaternion.Euler(0f, newYaw, 0f);
            xRotation = newPitch;
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerBody.rotation = Quaternion.Euler(0f, targetYaw, 0f);
        xRotation = targetPitch;
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void SetServingMode(bool active)
    {
        isServing = active;

        if (isServing) xRotation = Mathf.Clamp(xRotation, servingMinPitch, servingMaxPitch);
    }

    public void SetCarryingMode(bool active)
    {
        isCarryingObject = active;

        if (isCarryingObject) xRotation = Mathf.Clamp(xRotation, carryMinPitch, carryMaxPitch);
    }


    #region Grabbable bs

    public virtual void DisableOutlineFromGrabbable(GameObject[] pGrabbableGameObjects)
    {
        if (pGrabbableGameObjects == null) return;

        foreach (GameObject grabbable in pGrabbableGameObjects)
        {
            if (grabbable == null) continue;

            Outline grabbableOutline = grabbable.GetComponent<Outline>();
            if (grabbableOutline != null) grabbableOutline.enabled = false;
        }
    }

    public virtual void CheckGrabbableObjectOutline()
    {
        GameObject[] localGrabbableGameObjects = GameObject.FindGameObjectsWithTag("GrabbableObject");
        grabbableGameObjects = localGrabbableGameObjects;

        foreach (GameObject grabbable in localGrabbableGameObjects)
        {
            if (grabbable == null) continue;

            Outline outline = grabbable.GetComponent<Outline>();
            if (outline == null) outline = grabbable.AddComponent<Outline>();

            outline.enabled = false;
            outline.OutlineColor = new Color(1.0f, 0.5f, 0.0f, 1.0f);
            outline.OutlineWidth = 3.0f;
        }
    }

    public virtual void HighlightGameObject()
    {
        GameObject grabbableObjectTargeted = GetGrabbableWithRaycast();

        if (grabbableObjectTargeted != null)
        {
            Outline outlineTargeted = grabbableObjectTargeted.GetComponent<Outline>();

            if (outlineTargeted != null) outlineTargeted.enabled = true;
            //oldGrabbableObject = grabbableObjectTargeted;
        }
        else
        {
            DisableOutlineFromGrabbable(grabbableGameObjects);
        }

        //else
        //{
        //    if (oldGrabbableObject != null && oldGrabbableObject != grabbableObjectTargeted)
        //    {
        //        Outline oldOutline = oldGrabbableObject.GetComponent<Outline>();
        //        oldOutline.enabled = false;
        //    }
        //}
    }

    public virtual GameObject GetGrabbableWithRaycast() 
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = thisCam.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3.0f))
        {
            GameObject grabbableObject = hit.collider.gameObject;

            if (grabbableObject != null && grabbableObject.CompareTag("GrabbableObject")) return grabbableObject;
            else return null;
        }
        else
        {
            return null;
        }
    }

    public void ForceDisableAllOutlines()
    {
        isGrabbingSystemActive = false;
        DisableOutlineFromGrabbable(grabbableGameObjects);
    }

    #endregion


    public bool LockOnPauseMenu
    {
        get => lockOnPauseMenu;
        set
        {
            lockOnPauseMenu = value;

            if (lockOnPauseMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (!lockOnDialogue)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
