using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CinemachinePlayerFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;

    private bool foundPlayer;
    private void FixedUpdate()
    {
        if (foundPlayer || FindObjectOfType<PlayerController>() == null || EnvironmentSettings.Instance.currentState != EnvironmentState.InLobby)
        {
            return;
        }
        else
        {
            foundPlayer = true;

            vcam = GetComponent<CinemachineVirtualCamera>();

            GameObject player = FindObjectOfType<PlayerController>().gameObject;
            if (player != null)
            {
                vcam.Follow = player.transform;
                vcam.LookAt = player.transform;
            }
        }
    }
}
