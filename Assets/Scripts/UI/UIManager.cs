using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private string PlayScene;
    
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(PlayScene, LoadSceneMode.Single);
    }

    public void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("OnSettingsButtonClicked");
    }
}
