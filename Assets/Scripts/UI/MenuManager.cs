using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    private InputSystem_Actions inputActions;

    [SerializeField] private GameObject pauseScreen;

    private bool isPaused => pauseScreen.activeInHierarchy;

    void Awake()
    {
        Init();
    }

    public void OnBuild()
    {
        BuildingPlacement.Instance.BuildMenu();
        SFXManager.Instance.PlayConfirmSound();
    }

    public void OnStage()
    {
        Pathfinding.Instance.UpdatePathfinding();
        EnemySpawner.Instance.SpawnEnemies();
        SFXManager.Instance.PlayConfirmSound();
    }

    private void OnPause()
    {
        pauseScreen.SetActive(!isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        if (isPaused) SFXManager.Instance.PlayBackSound();
        else SFXManager.Instance.PlayEnterSound();
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void OnResume()
    {
        OnPause();
    }

    public void OnOptions()
    {
        //To be developed
    }

    public void OnQuit()
    {
        SFXManager.Instance.PlayBackSound();
        SceneManager.LoadScene("MainMenu");
    }

    private void Init()
    {
        Instance = this;

        pauseScreen.SetActive(false);
        Time.timeScale = 1f;

        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.Player.Pause.performed += ctx => OnPause();
    }
}
