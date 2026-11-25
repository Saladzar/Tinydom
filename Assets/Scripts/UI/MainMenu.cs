using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject creditsScreen;

    void Start()
    {
        creditsScreen.SetActive(false);
    }

    public void OnPlay() { SceneManager.LoadScene("Game"); }

    public void OnCredits() { creditsScreen.SetActive(true); }

    public void OnExitCredits() { creditsScreen.SetActive(false); }

    public void OnQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
