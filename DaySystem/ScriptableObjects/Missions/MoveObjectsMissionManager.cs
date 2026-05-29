using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class MoveObjectsMissionManager : MonoBehaviour
{
    public static MoveObjectsMissionManager Instance { get; private set; }


    [System.Serializable]
    public class MoveTaskRefs
    {
        public GameObject objectInScene;
        public Collider targetZoneCollider;

        [HideInInspector] public Vector3 initialPosition;
        [HideInInspector] public Quaternion initialRotation;
    }

    [System.Serializable]
    public struct MissionMapping
    {
        public MoveObjectsMissionSO mission;
        public List<MoveTaskRefs> tasks;
    }

    public List<MissionMapping> mappings;

    private void Awake()
    {
        Instance = this;
    }

    public List<MoveTaskRefs> GetRefsForMission(MoveObjectsMissionSO mission)
    {
        foreach (MissionMapping mapping in mappings)
        {
            if (mapping.mission == mission) return mapping.tasks;
        }

        return null;
    }

    public void ResetObjectFromWater(GameObject fallenObject)
    {
        foreach (MissionMapping mapping in mappings)
        {
            foreach (MoveTaskRefs task in mapping.tasks)
            {
                if (task.objectInScene == fallenObject)
                {
                    if (task.targetZoneCollider != null && task.targetZoneCollider.GetComponent<SeaWaterTrigger>() != null) return;

                    if (vThirdPersonController.Instance != null) vThirdPersonController.Instance.DetachObjectFromRightHand(fallenObject);

                    Rigidbody rb = fallenObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }

                    fallenObject.transform.position = task.initialPosition;
                    fallenObject.transform.rotation = task.initialRotation;

                    if (PlayerUI.Instance != null)
                    {
                        PlayerUI.Instance.InjectDialogueToTavernMan("That thing didn’t belong in the water. The water knew it... Square one. Let's do it again", 6.8f);
                    }
                    return;
                }
            }
        }
    }
}
