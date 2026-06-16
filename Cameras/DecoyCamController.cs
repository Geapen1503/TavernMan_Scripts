using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class DecoyCamController : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public float minPitch = -30f;
    public float maxPitch = 30f;
    public float minYaw = -45f;
    public float maxYaw = 45f;

    public GameObject seaHintCanvas;
    public TextMeshProUGUI hintTextComponent;

    private float _pitch = 0f;
    private float _yaw = 0f;
    private Quaternion _initialRotation;

    private KeyCode nextDialogueKey = KeyCode.Mouse0;

    private bool _hasClickedOnce = false;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _initialRotation = transform.localRotation;
        _pitch = 0f;
        _yaw = 0f;

        if (seaHintCanvas != null) seaHintCanvas.SetActive(true);
        _hasClickedOnce = false;
    }

    private void Start()
    {
        UpdateHintText();
        if (vThirdPersonInput.Instance != null) nextDialogueKey = vThirdPersonInput.Instance.leftClickInput;
    }

    private void Update()
    {
        HandleCameraMovement();
        HandleDialogueClick();
    }

    private void UpdateHintText()
    {
        if (vThirdPersonInput.Instance != null) nextDialogueKey = vThirdPersonInput.Instance.leftClickInput;

        if (hintTextComponent != null)
        {
            string keyName = nextDialogueKey.ToString();

            if (nextDialogueKey == KeyCode.Mouse0) keyName = "Left Click";

            hintTextComponent.text = "Press " + keyName + " to display next dialogue";
        }
    }

    private void HandleCameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _yaw += mouseX;
        _pitch -= mouseY;

        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        _yaw = Mathf.Clamp(_yaw, minYaw, maxYaw);

        transform.localRotation = _initialRotation * Quaternion.Euler(_pitch, _yaw, 0f);
    }

    private void HandleDialogueClick()
    {
        if (Input.GetKeyDown(nextDialogueKey))
        {
            if (DialogueUIManager.Instance != null && DialogueUIManager.Instance.IsInDialogue)
            {
                if (DialogueUIManager.Instance.nextButton != null)
                {
                    DialogueUIManager.Instance.nextButton.onClick.Invoke();

                    if (!_hasClickedOnce)
                    {
                        _hasClickedOnce = true;
                        StartCoroutine(WaitAndHideHintRoutine());
                    }
                }
            }
        }
    }

    private IEnumerator WaitAndHideHintRoutine()
    {
        yield return new WaitForEndOfFrame();

        if (DialogueUIManager.Instance != null && DialogueUIManager.Instance.nextButton != null)
        {
            if (!DialogueUIManager.Instance.nextButton.gameObject.activeInHierarchy)
            {
                yield return new WaitUntil(() =>
                    !DialogueUIManager.Instance.IsInDialogue ||
                    (DialogueUIManager.Instance.nextButton != null && DialogueUIManager.Instance.nextButton.gameObject.activeInHierarchy)
                );
            }
        }

        if (seaHintCanvas != null) seaHintCanvas.SetActive(false);
    }
}