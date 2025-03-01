using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    private float time;
    private bool isRunning = false;
    private TextMeshProUGUI timerText;

    public void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if (isRunning)
        {
            time += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void SetTimer(TextMeshProUGUI timerText)
    {
        this.timerText = timerText;
        isRunning = true;
        UpdateTimerDisplay();
    }

    public void ClearTimer()
    {
        isRunning = false;
        time = 0;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(time/ 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
