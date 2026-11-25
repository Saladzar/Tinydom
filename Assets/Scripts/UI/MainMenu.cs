using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject creditsScreen;

    void Start()
    {
        creditsScreen.SetActive(false);
        SFXManager.Instance.PlayMainMenuMusic();
    }

    public void OnPlay()
    {
        SFXManager.Instance.PlayStartSound();
        SceneManager.LoadScene("Game");
    }

    public void OnCredits()
    {
        SFXManager.Instance.PlayEnterSound();
        creditsScreen.SetActive(true);
    } 

    public void OnExitCredits() 
    {
        SFXManager.Instance.PlayBackSound();
        creditsScreen.SetActive(false);
    }

    public void OnQuit()
    {
        SFXManager.Instance.PlayBackSound();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
