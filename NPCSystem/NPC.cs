using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    [Header("Core Components")]
    //public string id;
    public NPCID npc;
    public Rigidbody rigidBody;
    public Animator animator;
    public NPCAnchor defaultAnchor;
    public string defaultAnimation = "TPose";
    public CapsuleCollider npcCollider;
    public AudioSource npcAudioSource;

    [Header("Interaction Settings")]
    public bool isTalkable = true;
    public BoxCollider dialogDetectorCol;
    public DialogueAnchor dialogueAnchor;
    public PlayerDialAnchor playerDialAnchor;

    private bool isPlayerInRange = false;

    private vThirdPersonController playerController;
    private vThirdPersonInput playerInput;

    void Start()
    {
        CheckIfPlayerValid();

        playerController = vThirdPersonController.Instance;
        playerInput = vThirdPersonInput.Instance;
        if (playerController == null || playerInput == null) Debug.LogError($"{name}: vThirdPersonController.Instance vThirdPersonInput.Instance is NULL.");

        InitializeNPC(defaultAnchor);
    }

    public virtual void InitializeNPC(NPCAnchor anchor)
    {
        if (anchor != null)
        {
            MoveToAnchor(anchor);

            string anim = !string.IsNullOrEmpty(anchor.defaultAnchorAnimation)
                ? anchor.defaultAnchorAnimation
                : defaultAnimation;

            PlayAnimation(anim);
        }
    }

    public void MoveToAnchor(NPCAnchor anchor)
    {
        if (anchor != null)
        {
            transform.position = anchor.transform.position;
            transform.rotation = anchor.transform.rotation;
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (animator != null && !string.IsNullOrEmpty(animationName)) animator.SetTrigger(animationName);
    }

    public void ResizeColliderForAnimation()
    {

    }

    public void TriggerDialog(DialogueSO dialogue) // don't forget to remove null once done
    {
        /*if (!isTalkable) return;
        StartCoroutine(TriggerDialogRoutine());

        List<DialogueEntry> sequence = new List<DialogueEntry>()
        {
            new DialogueEntry("Heeeeeeeey Niggaaaaaa !", "What..?"),
            new DialogueEntry("Bring me a beer boyo or I'll bring the bloody cavalry", "Yeah whatever"),
            new DialogueEntry("You better be fast boyo", "Okay")
        };

        DialogueUIManager.Instance.StartDialogueSequence(sequence);*/

        if (!isTalkable || dialogue == null) return;
        StartCoroutine(TriggerDialogRoutine());

        DialogueUIManager.Instance.StartDialogueSequence(dialogue.sequence, npc);

        if (PlayerUI.Instance != null) PlayerUI.Instance.HidePressKey();
    }

    private IEnumerator TriggerDialogRoutine()
    {
        Debug.Log($"{name}: NPC's Talking!");

        DialogueUIManager.Instance.MoveCanvasToNPC(dialogueAnchor);

        if (playerInput != null) yield return StartCoroutine(playerInput.FreezeInputsAndMoveToAnchor(playerDialAnchor));

        var playerCam = FPCam.Instance;
        if (playerCam != null) playerCam.EnterDialogueMode();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isTalkable) return;
        if (!other.CompareTag("Player")) return;

        IsPlayerInRange = true;

        if (PlayerUI.Instance != null && DialogueUIManager.Instance.IsInDialogue == false)
        {
            PlayerUI.Instance.ShowPressKey("Press " + playerInput.talkInput);
        }

        // Notify the player controller via the singleton
        if (playerController != null)
        {
            playerController.SetCurrentNPC(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isTalkable) return;
        if (!other.CompareTag("Player")) return;

        IsPlayerInRange = false;

        if (PlayerUI.Instance != null) PlayerUI.Instance.HidePressKey();

        if (playerController != null)
        {
            playerController.ClearCurrentNPC(this);
        }
    }

    public void CheckIfPlayerValid()
    {
        if (!animator || !npcCollider || !rigidBody || !npcAudioSource)
        {
            Debug.LogError($"{name}: Missing required NPC component(s).");
        }

        if (isTalkable && !dialogDetectorCol) Debug.LogWarning($"{name}: Talkable NPC without a dialogDetectorCol assigned.");
        if (isTalkable && !dialogueAnchor) Debug.LogWarning($"{name}: Talkable NPC without a dialogueAnchor assigned.");
        if (isTalkable && !playerDialAnchor) Debug.LogWarning($"{name}: Talkable NPC without a playerDialAnchor assigned.");
    }

    public bool IsPlayerInRange { get => isPlayerInRange; set => isPlayerInRange = value; }
}
