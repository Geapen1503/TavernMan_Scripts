using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Move Objects")]
public class MoveObjectsMissionSO : MissionSO
{
    [Header("Player Feedbacks")]
    public string[] onTaskCompletedDialogues = {
    "That’s where it stays.",
    "Off my hands and into the shadows.",
    "Gravity does the rest.",
    "There. Let the dust settle on it.",
    "It’s right where it needs to be. For now.",
    "Done with this one. Let it rot in the corner.",
    "This heat makes everything feel twice as heavy.",
    "Moving things in the dark... just another way to kill time.",
    "One foot in front of the other. The island way.",
    "Keeping the weight moving. It’s the only way to stay ahead of the night.",
    "Let the salty air have it.",
    "One less thing to carry.",
    "Stay there. Don't move.",
    "Dropped. Just like my expectations for this night.",
    "It’s the ground’s problem now.",
    "Found its home for the night.",
    "Out of sight, out of mind.",
    "Off my back and onto the wood.",
    "Let the humidity eat it.",
    "One more for the pile.",
    "Landing felt harder than it should.",
    "Sit tight.",
    "Another piece of the puzzle on the floor."
};
    public string[] onMissionFinishedDialogues = {
    "One task down. The night doesn't feel any shorter.",
    "Done. Back to the grind.",
    "Another small job for a small man on a small island.",
    "The list gets shorter, but the air stays just as thick.",
    "Finished. Now for the next headache.",
    "Task complete. If only everything was this simple.",
    "Done. The island hasn't moved, but at least this did.",
    "Task finished. The heat remains.",
    "Moving stuff around... the story of my life.",
    "Everything in its place. For a while, anyway.",
    "Done. One small win against the chaos.",
    "Inventory's moved. My legs feel it.",
    "Wrapped up. Just me and the ocean breeze now.",
    "No more hauling. At least for the next ten minutes.",
    "The chore's over. The guilt lingers.",
    "Another day, another haul.",
    "Everything's where it should be. If only life worked like that."
};

    private int _completedTasks;

    protected override void MissionContentPlaying()
    {
        List<MoveObjectsMissionManager.MoveTaskRefs> refs = MoveObjectsMissionManager.Instance.GetRefsForMission(this);

        if (refs == null)
        {
            CompleteMission();
            return;
        }

        _completedTasks = 0;

        foreach (MoveObjectsMissionManager.MoveTaskRefs task in refs)
        {
            if (task.objectInScene != null) task.objectInScene.SetActive(true);
            if (task.targetZoneCollider != null) task.targetZoneCollider.gameObject.SetActive(true);

            MoveObjectsMissionManager.Instance.StartCoroutine(WatchObjectRoutine(task, refs));
        }

        if (FPCam.Instance != null)
        {
            FPCam.Instance.isGrabbingSystemActive = true;
            FPCam.Instance.CheckGrabbableObjectOutline();
        }

        if (vThirdPersonInput.Instance != null)
        {
            vThirdPersonInput.Instance.canGrab = true;
        }
    }

    private IEnumerator WatchObjectRoutine(MoveObjectsMissionManager.MoveTaskRefs currentTask, List<MoveObjectsMissionManager.MoveTaskRefs> allTasks)
    {
        bool isTaskDone = false;

        while (isTaskDone == false)
        {
            yield return new WaitForSeconds(0.1f);

            if (currentTask.objectInScene == null) break;

            if (currentTask.targetZoneCollider.bounds.Contains(currentTask.objectInScene.transform.position))
            {
                isTaskDone = true;
            }
        }

        FinalizeSingleTask(currentTask, allTasks);
    }

    private void FinalizeSingleTask(MoveObjectsMissionManager.MoveTaskRefs completedTask, List<MoveObjectsMissionManager.MoveTaskRefs> allTasks)
    {
        if (completedTask.objectInScene != null)
        {
            var playerController = vThirdPersonController.Instance;
            if (playerController != null) playerController.DetachObjectFromRightHand(completedTask.objectInScene); 

            Destroy(completedTask.objectInScene);
        }

        _completedTasks = _completedTasks + 1;

        bool colliderStillNeeded = false;

        foreach (MoveObjectsMissionManager.MoveTaskRefs task in allTasks)
        {
            if (task.targetZoneCollider == completedTask.targetZoneCollider && task.objectInScene != null)
            {
                colliderStillNeeded = true;
                break;
            }
        }

        if (colliderStillNeeded == false && completedTask.targetZoneCollider != null)
        {
            completedTask.targetZoneCollider.gameObject.SetActive(false);
        }

        if (_completedTasks < allTasks.Count)
        {
            string randomDialogue = onTaskCompletedDialogues[Random.Range(0, onTaskCompletedDialogues.Length)];
            PlayerUI.Instance.InjectDialogueToTavernMan(randomDialogue, 3.5f);
        }

        if (_completedTasks >= allTasks.Count)
        {
            if (FPCam.Instance != null) FPCam.Instance.ForceDisableAllOutlines();
            if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.canGrab = false;

            string randomEndingDialogue = onMissionFinishedDialogues[Random.Range(0, onMissionFinishedDialogues.Length)];
            PlayerUI.Instance.InjectDialogueToTavernMan(randomEndingDialogue, 3.0f);

            CompleteMission();
        }
    }
}