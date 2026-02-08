using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public Day currentDay;
    public DaysMenu daysMenuUI;

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

}
