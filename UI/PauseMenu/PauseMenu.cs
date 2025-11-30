using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    public static PauseMenu Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PauseMenu instances detected. Keeping the first one.");
            return;
        }
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
    
    }

    public void TogglePauseGame()
    {
        var playerInput = Invector.vCharacterController.vThirdPersonInput.Instance;
        var playerCam = FPCam.Instance;

        if (pauseMenuUI.activeSelf)
        {
            playerInput.UnfreezeInputs();
            playerCam.LockOnPauseMenu = false;
            this.pauseMenuUI.SetActive(false);

            foreach (var feedback in pauseMenuUI.GetComponentsInChildren<ButtonFeedback>(true))
            {
                feedback.ResetVisualState();
            }

            Time.timeScale = 1f;
            AudioManager.Instance.ResumeEnvironmentSounds();
            //AudioListener.pause = false;
        }
        else
        {
            playerInput.FreezeInputs();
            playerCam.LockOnPauseMenu = true;
            pauseMenuUI.SetActive(true);

            Time.timeScale = 0f;
            AudioManager.Instance.PauseEnvironmentSounds();
            //AudioListener.pause = true;
        }
    }

    public void ResumeGame()
    {
        TogglePauseGame();
    }

    public void OpenOptions()
    {

    }

    public void CloseOptions()
    {

    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
