using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class DialogueImporterWindow : EditorWindow
{
    private string rawText = "";
    private string savePath = "Assets/Missions";
    private string assetName = "NewMission";
    private NPCID targetNpcEnum;

    [MenuItem("Tools/Dialogue Importer")]
    public static void ShowWindow()
    {
        GetWindow<DialogueImporterWindow>("Dialogue Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Dialogue & Mission Importer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        savePath = EditorGUILayout.TextField("Save Path", savePath);
        assetName = EditorGUILayout.TextField("Asset Name", assetName);
        targetNpcEnum = (NPCID)EditorGUILayout.EnumPopup("Target NPC", targetNpcEnum);

        EditorGUILayout.Space();
        GUILayout.Label("Paste text from Word here:", EditorStyles.label);

        rawText = EditorGUILayout.TextArea(rawText, GUILayout.ExpandHeight(true), GUILayout.MinHeight(200));

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Scriptable Objects", GUILayout.Height(40)))
        {
            GenerateAssets();
        }
    }

    private void GenerateAssets()
    {
        if (string.IsNullOrEmpty(rawText))
        {
            EditorUtility.DisplayDialog("Error", "The text field is empty!", "OK");
            return;
        }

        if (string.IsNullOrEmpty(assetName))
        {
            EditorUtility.DisplayDialog("Error", "The asset name field is empty!", "OK");
            return;
        }

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
            AssetDatabase.Refresh();
        }

        string[] lines = rawText.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

        List<DialogueEntry> entries = new List<DialogueEntry>();
        string currentDialogueText = "";
        string currentButtonText = "Next";
        bool isPlayerSpeaking = false;
        bool hasStartedParsing = false;

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (trimmed.EndsWith(":"))
            {
                continue;
            }

            if (trimmed.Contains(" - "))
            {
                string[] parts = trimmed.Split(new[] { " - " }, 2, System.StringSplitOptions.None);
                string speaker = parts[0].Trim();
                string text = parts[1].Trim();

                if (speaker == "T")
                {
                    currentButtonText = text;
                    isPlayerSpeaking = true;
                }
                else
                {
                    if (hasStartedParsing)
                    {
                        entries.Add(new DialogueEntry(currentDialogueText, currentButtonText));
                    }

                    currentDialogueText = text;
                    currentButtonText = "Next";
                    isPlayerSpeaking = false;
                    hasStartedParsing = true;
                }
            }
            else
            {
                if (isPlayerSpeaking)
                {
                    currentButtonText += " " + trimmed;
                }
                else if (hasStartedParsing)
                {
                    currentDialogueText += " " + trimmed;
                }
            }
        }

        if (hasStartedParsing)
        {
            entries.Add(new DialogueEntry(currentDialogueText, currentButtonText));
        }

        if (entries.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No valid dialogue lines found.", "OK");
            return;
        }

        DialogueSO dialogueSO = ScriptableObject.CreateInstance<DialogueSO>();
        dialogueSO.npc = targetNpcEnum;
        dialogueSO.sequence = entries;

        string dialogueAssetPath = $"{savePath}/{assetName}_Dialogue.asset";
        AssetDatabase.CreateAsset(dialogueSO, dialogueAssetPath);

        TalkToNPCMissionSO missionSO = ScriptableObject.CreateInstance<TalkToNPCMissionSO>();
        missionSO.targetNpc = targetNpcEnum;
        missionSO.npcDialogue = dialogueSO;

        string missionAssetPath = $"{savePath}/{assetName}_TalkMission.asset";
        AssetDatabase.CreateAsset(missionSO, missionAssetPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = missionSO;

        EditorUtility.DisplayDialog("Success", $"Assets created successfully in {savePath} with name prefix '{assetName}'!", "OK");
    }
}