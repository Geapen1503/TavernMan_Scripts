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
        if (!GoingToMissionManager.Instance.TryGetMissionData(this, out Collider targetCollider, out GameObject indicator)) return;
        if (targetCollider == null) return;

        targetCollider.gameObject.SetActive(true);
        if (indicator != null) indicator.SetActive(true);

        GoingToMissionManager.Instance.StartCoroutine(WatchPlayerArrivalRoutine(targetCollider, indicator));
    }

    private IEnumerator WatchPlayerArrivalRoutine(Collider targetZone, GameObject indicator)
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

        FinalizeMission(targetZone, indicator);
    }

    private void FinalizeMission(Collider target, GameObject indicator)
    {
        if (target != null) target.gameObject.SetActive(false);
        if (indicator != null) indicator.SetActive(false);

        CompleteMission();
    }
}
