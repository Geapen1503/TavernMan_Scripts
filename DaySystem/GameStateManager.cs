using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public Day currentDay;
    public DaysMenu daysMenuUI;

    private Dictionary<NPCID, NPC> _npcRegistry = new Dictionary<NPCID, NPC>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartDay(currentDay);
    }


    public void StartDay(Day day)
    {
        var playerController = Invector.vCharacterController.vThirdPersonController.Instance;
        playerController.currentDay = day;

        StartDayCanvasUI(day.GetDayId(), daysMenuUI);
    }

    public void StartDayCanvasUI(DayID dayID, DaysMenu daysMenu)
    {
        var playerInput = Invector.vCharacterController.vThirdPersonInput.Instance;
        StartCoroutine(ShowDayRoutine(dayID, daysMenu, playerInput));
    }

    private IEnumerator ShowDayRoutine(DayID dayID, DaysMenu daysMenu, Invector.vCharacterController.vThirdPersonInput playerInput)
    {
        playerInput.FreezeInputs();

        daysMenu.ShowDay(dayID);
        yield return new WaitForSeconds(daysMenu.displayTime + daysMenu.fadeDuration);

        playerInput.UnfreezeInputs();

        currentDay.StartDay();
    }

    public void RegisterNPC(NPCID id, NPC npc)
    {
        if (!_npcRegistry.ContainsKey(id)) _npcRegistry.Add(id, npc);
        else _npcRegistry[id] = npc;
    }

    public void UnregisterNPC(NPCID id)
    {
        if (_npcRegistry.ContainsKey(id)) _npcRegistry.Remove(id);
    }

    public NPC GetNPC(NPCID id)
    {
        _npcRegistry.TryGetValue(id, out NPC npc);
        return npc;
    }
}
