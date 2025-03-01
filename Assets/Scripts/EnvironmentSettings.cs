using System;
using TMPro;
using UnityEngine;

public class EnvironmentSettings : MonoBehaviour
{
    public static EnvironmentSettings Instance { get; private set; }

    [SerializeField] private GameObject beforeLobby;
    [SerializeField] private GameObject onLobby;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject matchMaking;
    [SerializeField] private GameObject mainScenePlayer;
    [SerializeField] private TextMeshProUGUI matchMakingTimerText;
    [SerializeField] private TextMeshProUGUI lobbyTimerText;

    [SerializeField] private GameObject spotLight;
    [SerializeField] private Light _light;
    [SerializeField] private Material daySkyBoxMaterial;

    public EnvironmentState currentState;

    private EventHandler OnSetEnvironment;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (currentState == EnvironmentState.InLobby && NetworkTimer.Instance != null)
        {
            float timeRemaining = NetworkTimer.Instance.TimeRemaining;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            lobbyTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }


    public void ChangeEnvironment(EnvironmentState environmentState)
    {
        OnSetEnvironment = environmentState switch
        { 
            EnvironmentState.MatchMaking => MatchMakingEnvironment,
            EnvironmentState.InLobby => InLobbyEnvironment,
            EnvironmentState.InGame => InGameEnvironment,
            _=> DefaultEnvironment
        };

        OnSetEnvironment?.Invoke(this, EventArgs.Empty);
    }

    private void DefaultEnvironment(object sender, EventArgs e)
    {
        currentState = EnvironmentState.Default;
        Timer.Instance.ClearTimer();
        matchMaking.SetActive(false);
        playButton.SetActive(true);
        currentState = EnvironmentState.Default;
    }

    private void MatchMakingEnvironment(object sender, EventArgs e)
    {
        playButton.SetActive(false);
        matchMaking.SetActive(true);
        Timer.Instance.SetTimer(matchMakingTimerText);
        currentState = EnvironmentState.MatchMaking;
    }

    private void InLobbyEnvironment(object sender, EventArgs e)
    {
        onLobby.SetActive(true);
        mainScenePlayer.SetActive(false);
        Timer.Instance.ClearTimer();
        matchMaking.SetActive(false);
        beforeLobby.SetActive(false);

        _light.intensity = 1;
        RenderSettings.skybox = daySkyBoxMaterial; 
        DynamicGI.UpdateEnvironment();

        currentState = EnvironmentState.InLobby;
    }

    private void InGameEnvironment(object sender, EventArgs e)
    {
        currentState = EnvironmentState.InGame;
    }

}

public enum EnvironmentState { Default, MatchMaking, InLobby, InGame}
