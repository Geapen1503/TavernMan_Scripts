using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public List<DayData> daySequence;
    public DaysMenu daysMenuUI;

    public static bool HasTargetDayToLoad = false;
    public static DayID TargetDayToLoad;

    private int _currentDayIndex = 0;
    private static int _pendingDayIndex = -1;

    private Dictionary<NPCID, NPC> _npcRegistry = new Dictionary<NPCID, NPC>();

    public GameProgressData progressData { get; private set; }
    private string _savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _savePath = Path.Combine(Application.persistentDataPath, "user_progress.json");
        LoadProgress();
    }

    private void Start()
    {
        InitializeDaySequence();
    }

    private void InitializeDaySequence()
    {
        if (daySequence == null || daySequence.Count == 0) return;


        if (HasTargetDayToLoad)
        {
            int dayIndex = daySequence.FindIndex(d => d.dayID == TargetDayToLoad);
            if (dayIndex != -1) _currentDayIndex = dayIndex;

            HasTargetDayToLoad = false;
            _pendingDayIndex = -1;
        }
        else if (_pendingDayIndex >= 0 && _pendingDayIndex < daySequence.Count)
        {
            _currentDayIndex = _pendingDayIndex;
            _pendingDayIndex = -1;
        }
        else
        {
            _currentDayIndex = 0;
            for (int i = 0; i < daySequence.Count; i++)
            {
                if (!progressData.IsDayCompleted(daySequence[i].dayID))
                {
                    _currentDayIndex = i;
                    break;
                }
            }
        }

        StartDay(daySequence[_currentDayIndex]);
    }

    private void StartDay(DayData dayData)
    {
        if (dayData == null) return;

        Day.Instance.LoadDayData(dayData);

        var playerController = Invector.vCharacterController.vThirdPersonController.Instance;
        if (playerController != null) playerController.currentDay = Day.Instance;

        StartDayCanvasUI(dayData.dayID, daysMenuUI);
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

        Day.Instance.StartDay();
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

    public void CompleteCurrentDay()
    {
        DayID finishedDayId = Day.Instance.GetDayId();
        progressData.MarkDayAsCompleted(finishedDayId);
        SaveProgress();

        StartNextDay();
    }

    private void StartNextDay()
    {
        int nextIndex = _currentDayIndex + 1;

        if (nextIndex < daySequence.Count)
        {
            _pendingDayIndex = nextIndex;
            SceneManager.LoadScene(GameScenes.Game);
        }
        else
        {
            _pendingDayIndex = -1;
            SceneManager.LoadScene(GameScenes.MainMenu);
        }
    }

    public bool TryPlaySpecificDay(DayID dayID)
    {
        if (!progressData.IsDayCompleted(dayID)) return false;

        int dayIndex = daySequence.FindIndex(d => d.dayID == dayID);
        if (dayIndex != -1)
        {
            _pendingDayIndex = dayIndex;
            SceneManager.LoadScene(GameScenes.Game);
            return true;
        }

        return false;
    }

    private void SaveProgress()
    {
        string json = JsonUtility.ToJson(progressData, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"Progress saved at: {_savePath}");
    }

    private void LoadProgress()
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            progressData = JsonUtility.FromJson<GameProgressData>(json);
        }
        else
        {
            progressData = new GameProgressData();
        }
    }

    public static GameProgressData ReadSaveData()
    {
        string path = Path.Combine(Application.persistentDataPath, "user_progress.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameProgressData>(json);
        }
        return new GameProgressData();
    }
}
