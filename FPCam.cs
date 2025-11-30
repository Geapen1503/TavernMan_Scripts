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
        //CheckGrabbableObjectOutline();
    }

    void Update()
    {
        if (lockOnDialogue == false && LockOnPauseMenu == false) CameraInputs();

        // HighlightGameObject();
    }

    public virtual void CameraInputs()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

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


    #region Grabbable bs

    public virtual void DisableOutlineFromGrabbable(GameObject[] pGrabbableGameObjects)
    {
        foreach (GameObject grabbable in pGrabbableGameObjects)
        {
            if (grabbable.GetComponent<Outline>() != null)
            {
                Outline grabbableOutline = grabbable.GetComponent<Outline>();
                grabbableOutline.enabled = false;
            }
        }
    }

    public virtual void CheckGrabbableObjectOutline()
    {
        GameObject[] localGrabbableGameObjects = GameObject.FindGameObjectsWithTag("GrabbableObject");
        grabbableGameObjects = localGrabbableGameObjects;

        foreach (GameObject grabbable in localGrabbableGameObjects)
        {
            if (grabbable.GetComponent<Outline>() == null) grabbable.AddComponent<Outline>();
            Outline grabbableOutline = grabbable.GetComponent<Outline>();

            grabbableOutline.enabled = false;
            grabbableOutline.OutlineColor = new Color(1.0f, 0.5f, 0.0f, 1.0f);
            grabbableOutline.OutlineWidth = 3.0f;
        }
    }

    public virtual void HighlightGameObject()
    {
        GameObject grabbableObjectTargeted = GetGrabbableWithRaycast();

        if (grabbableObjectTargeted != null)
        {
            Outline outlineTargeted = grabbableObjectTargeted.GetComponent<Outline>();

            outlineTargeted.enabled = true;
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
