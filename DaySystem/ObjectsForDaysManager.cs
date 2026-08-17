using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectsForDaysManager : MonoBehaviour
{
    public List<DayObjects> objectsByDay;

    public static ObjectsForDaysManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ActivateObjectsForDay(DayID dayID)
    {
        foreach (var entry in objectsByDay)
        {
            bool shouldBeActive = (entry.day == dayID);
            foreach (var obj in entry.objectsToActivate)
            {
                if (obj != null) obj.SetActive(shouldBeActive);
            }
        }
    }

    public List<GameObject> GetObjectsForDay(DayID dayID)
    {
        var entry = objectsByDay.Find(x => x.day == dayID);
        return entry != null ? entry.objectsToActivate : null;
    }
}

[System.Serializable]
public class DayObjects
{
    public DayID day; 
    public List<GameObject> objectsToActivate; 
}