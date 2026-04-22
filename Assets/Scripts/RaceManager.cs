using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    [SerializeField] TrackCheckpoints trackCheckpoints;
    
    [Header("Countdown UI")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Common UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private TextMeshProUGUI messageText;
    
    [Header("Settings")]
    [SerializeField] private float countdownSeconds = 3f;

    [Header("Car Setup")]
    [SerializeField] private CarDriverAgent[] carDriverAgent;
    
    public bool RaceStarted { get; private set; }
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        instance = this;
        RaceStarted = true;
        IsPaused = false;
        Time.timeScale = 1f;
    }

    public void Start()
    {
        StartRace();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void RestartRace()
    {
        trackCheckpoints.EndCarEpisodes();
        StartRace();
    }
    
    public void StartRace()
    {
        foreach (CarDriverAgent agent in carDriverAgent)
        {
            agent.ResetPositions();   
        }
        
        restartPanel.SetActive(false);
        IsPaused = true;
        SetAllCarsFrozen(true);
        
        StartCoroutine(CountdownThenStart());
    }
    
    IEnumerator CountdownThenStart()
    {
        if (countdownPanel)
        {
            countdownPanel.SetActive(true);
        }

        float t = countdownSeconds;
        
        while (t > 0)
        {
            if (countdownText)
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = Mathf.CeilToInt(t).ToString();
            }
            
            yield return new WaitForSecondsRealtime(1f);
            t--;
        }

        if (countdownText)
        {
            countdownText.text = "GO!";
        }
        
        yield return new WaitForSecondsRealtime(0.5f);

        if (countdownPanel)
        {
            countdownPanel.SetActive(false);
        }
        
        TogglePause(false);
        RaceStarted = true;
        SetAllCarsFrozen(false);
    }

    public void TogglePause(bool shouldShowPanel = true)
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        pausePanel.SetActive(IsPaused && shouldShowPanel);
        SetAllCarsFrozen(IsPaused);
    }

    public void ShowMesseage(bool enable, bool hasWon)
    {
        messagePanel.SetActive(enable);
        messageText.text = hasWon ? "You won!" : "You lost!";
    }
    
    public void BackToMainMenu()
    {
        restartPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        trackCheckpoints.OnGameEndEvent();
    }

    public void EnableRestartMenu()
    {
        restartPanel.SetActive(true);
    }
    
    private void SetAllCarsFrozen(bool frozen)
    {
        var frozenCar = FindObjectsByType<CarFreezeControl>(FindObjectsSortMode.None);
        
        foreach (var f in frozenCar)
        {
            f.SetFrozen(frozen);
        }
    }
}
