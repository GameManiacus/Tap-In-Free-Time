using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    #region Variables
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject networkTimer;
    [SerializeField] private float minSpawnX, maxSpawnX, minSpawnY, maxSpawnY, minSpawnZ, maxSpawnZ;

    private int minPlayers = 2;
    private int maxPlayers = 20;
    private string roomName;

    private bool networkTimerSpawned;
    private NetworkRunner _runner;

    #endregion

    #region Custom Public Functions

    public void PlayButton()
    {
        AudioManager.Instance.PlaySfx(ClipName.ButtonClick);

        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;
            _runner.JoinSessionLobby(SessionLobby.Shared);
        }
        EnvironmentSettings.Instance.ChangeEnvironment(EnvironmentState.MatchMaking);
    }

    public void CancelMatchMaking()
    {
        AudioManager.Instance.PlaySfx(ClipName.ButtonClick);
        EnvironmentSettings.Instance.ChangeEnvironment(EnvironmentState.Default);
        _runner.Shutdown(false, ShutdownReason.ConnectionTimeout);
    }
    #endregion

    #region Custom Private Functions

    private void SpawnPlayer(NetworkRunner runner)
    {
        Vector3 spawnLocation = new Vector3(Random.Range(minSpawnX, maxSpawnX), Random.Range(minSpawnY, maxSpawnY), Random.Range(minSpawnZ, maxSpawnZ));
        NetworkObject _player = runner.Spawn(playerPrefab, spawnLocation,Quaternion.identity);
    }

    private void IntoLobby(NetworkRunner runner)
    {
        EnvironmentSettings.Instance.ChangeEnvironment(EnvironmentState.InLobby);

        if (runner.IsSharedModeMasterClient)
        {
            NetworkTimer.Instance.StartTimer(TimeState.InLobby);
        }
    }

    #endregion

    #region Photon

    private async void StartGame(GameMode mode)
    {
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            PlayerCount = maxPlayers,
            GameMode = mode,
            SessionName = roomName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });       
    }
    #endregion

    #region Used Photon Callbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        if (runner.IsSharedModeMasterClient && !networkTimerSpawned)
        {
            networkTimerSpawned = true;
            runner.Spawn(networkTimer);
        }

        if (player == runner.LocalPlayer) SpawnPlayer(runner);

        if (runner.SessionInfo.PlayerCount >= minPlayers) IntoLobby(runner);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        if (!string.IsNullOrEmpty(roomName)) return;

        foreach (var session in sessionList)
        {
            if (session.PlayerCount < maxPlayers && session.IsOpen && session.IsVisible)
            {
                roomName = session.Name;
                break;
            }
        }

        roomName = roomName ?? "SharedRoom_" + IDGenerator.GenerateID();
        StartGame(GameMode.Shared);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

    #endregion

    #region UnUsed Photon Callbacks
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    #endregion

}

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
}