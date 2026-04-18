using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Move Objects")]
public class MoveObjectsMissionSO : MissionSO
{
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
        if (completedTask.objectInScene != null) Destroy(completedTask.objectInScene);

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

        if (_completedTasks >= allTasks.Count)
        {
            if (FPCam.Instance != null) FPCam.Instance.ForceDisableAllOutlines();
            if (vThirdPersonInput.Instance != null) vThirdPersonInput.Instance.canGrab = false;

            CompleteMission();
        }
    }
}