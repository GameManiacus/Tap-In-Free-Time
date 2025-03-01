using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkTimer : NetworkBehaviour
{
    public static NetworkTimer Instance { get; private set; }
    [Networked] private float startTime { get; set; }
    [Networked] private bool isRunning { get; set; }
    [Networked] public float TimeRemaining { get; set; }

    private float duration;

    private TimeState currentTimerState;

    private float timeRemaining => Mathf.Max(0, duration - (Runner.SimulationTime - startTime));

    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Runner.Despawn(Object);
            return;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!isRunning)
        {
            return;
        }
        else
        {
            TimeRemaining = timeRemaining;
        }

        if (isRunning && TimeRemaining <= 0)
        {
            isRunning = false;
            OnTimerEnd();
        }
    }

    public void StartTimer(TimeState timerState)
    {
        if (Object.HasStateAuthority)
        {
            currentTimerState = timerState;
            duration = timerState == TimeState.InLobby ? 60f : 120f;
            startTime = Runner.SimulationTime; 
            isRunning = true;
        }
    }
 

    private void OnTimerEnd()
    {
        if(currentTimerState == TimeState.InLobby)
        {
            currentTimerState = TimeState.Default;
            var sceneRef = SceneRef.FromIndex(1);
            Runner.LoadScene(sceneRef, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Single});
        }
    }
}

public enum TimeState { Default, MatchMaking, InLobby, InGame }