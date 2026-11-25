using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    private InputSystem_Actions inputActions;

    [SerializeField] private GameObject pauseScreen, lowerHUD, gameOverScreen;
    [SerializeField] private TextMeshProUGUI gameOverStats, highScoreText;

    private bool isPaused => pauseScreen.activeInHierarchy || gameOverScreen.activeInHierarchy;

    private GameState game;

    void Awake()
    {
        Init();
    }

    void Start()
    {
        game = GameState.Instance;
    }

    void Update()
    {
        lowerHUD.SetActive(!game.inCombat);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnBuild()
    {
        BuildingPlacement.Instance.BuildMenu();
        SFXManager.Instance.PlayConfirmSound();
    }

    public void OnStage()
    {
        game.BeginStage();
        SFXManager.Instance.PlayConfirmSound();
    }

    private void OnPause()
    {
        pauseScreen.SetActive(!isPaused);
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

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        lowerHUD.SetActive(false);
        gameOverStats.text = "STAGE REACHED: " + game.stage;
        if (game.stage > game.GetHighScore())
        {
            highScoreText.text = "NEW HIGH SCORE!";
            game.SetHighScore(game.stage);
        }
        else highScoreText.text = "HIGH SCORE: " + game.GetHighScore();
    }

    public void OnTryAgain() { SceneManager.LoadScene("Game"); }

    private void Init()
    {
        Instance = this;

        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        lowerHUD.SetActive(true);
        Time.timeScale = 1f;

        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.Player.Pause.performed += ctx => OnPause();
    }
}
