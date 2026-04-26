using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Missions/Going To")]
public class GoingToMissionSO : MissionSO
{
    // GOING WHERE? POWERING WHAT? I still don't know but we better get fifteen million merits

    protected override void MissionContentPlaying()
    {
        Collider targetCollider = GoingToMissionManager.Instance.GetColliderForMission(this);

        if (targetCollider == null)
        {
            Debug.LogError($"Mission {this.name} : No targetted zone in Manager!");
            CompleteMission();
            return;
        }

        targetCollider.gameObject.SetActive(true);
        GoingToMissionManager.Instance.StartCoroutine(WatchPlayerArrivalRoutine(targetCollider));
    }

    private IEnumerator WatchPlayerArrivalRoutine(Collider targetZone)
    {
        CapsuleCollider playerCollider = vThirdPersonInput.Instance.GetComponent<CapsuleCollider>();

        if (playerCollider == null) yield break;

        bool playerArrived = false;

        while (!playerArrived)
        {
            yield return new WaitForSeconds(0.1f);

            if (targetZone == null) break;
            if (targetZone.bounds.Intersects(playerCollider.bounds)) playerArrived = true;
        }

        FinalizeMission(targetZone);
    }

    private void FinalizeMission(Collider target)
    {
        if (target != null)
            target.gameObject.SetActive(false);

        CompleteMission();
    }
}
