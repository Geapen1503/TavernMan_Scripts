using System.Collections;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.Playables;

public class BoatCinematicTrigger : MonoBehaviour
{
    [Header("Mission Link")]
    public CinematicMoveObjectsMissionSO targetMissionSO;

    [Header("Cinematic & Targets")]
    public PlayableDirector timelineDirector;
    public GameObject boatObject;
    public Transform finalDestination;

    private BoxCollider boatTriggerCol;
    private bool _isPlayerInside;
    private bool _hasTriggered;

    private bool _lockPosition = false;

    public static BoatCinematicTrigger Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        _hasTriggered = false;
        _isPlayerInside = false;
        _lockPosition = false;
    }

    private void Start()
    {
        boatTriggerCol = this.gameObject.GetComponent<BoxCollider>();
        ToggleBoxTriggerActivation(false);
    }

    private void Update()
    {
        if (boatTriggerCol == null) return;

        if (_isPlayerInside && !_hasTriggered && Input.GetKeyDown(vThirdPersonInput.Instance.talkInput))
        {
            StartCinematic();
        }
    }

    private void LateUpdate()
    {
        if (_lockPosition && boatObject != null && finalDestination != null)
        {
            boatObject.transform.position = finalDestination.position;
            boatObject.transform.rotation = finalDestination.rotation;
        }
    }

    public void ToggleBoxTriggerActivation(bool enable)
    {
        if (boatTriggerCol != null)
        {
            if (enable) boatTriggerCol.enabled = true;
            if (!enable) boatTriggerCol.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;
            if (PlayerUI.Instance != null)
            {
                PlayerUI.Instance.ShowPressKey("Press " + vThirdPersonInput.Instance.talkInput + " to pull the boat");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = false;
            if (PlayerUI.Instance != null)
            {
                PlayerUI.Instance.HidePressKey();
            }
        }
    }

    private void StartCinematic()
    {
        _hasTriggered = true;

        if (PlayerUI.Instance != null)
        {
            PlayerUI.Instance.HidePressKey();
        }

        if (timelineDirector != null)
        {
            timelineDirector.Play();
            StartCoroutine(WaitForCinematicEndRoutine());
        }
    }

    private IEnumerator WaitForCinematicEndRoutine()
    {
        yield return new WaitUntil(() => timelineDirector.time >= timelineDirector.duration);

        _lockPosition = true;

        if (timelineDirector != null)
        {
            timelineDirector.Stop();
        }

        if (boatObject != null)
        {
            Animator animator = boatObject.GetComponent<Animator>();
            if (animator != null) Destroy(animator);

            Rigidbody rb = boatObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (targetMissionSO != null)
        {
            targetMissionSO.OnCinematicTriggerFinished(this);
        }

        if (boatTriggerCol != null) boatTriggerCol.enabled = false;
    }
}