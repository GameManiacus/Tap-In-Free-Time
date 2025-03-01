using Fusion;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; set; }


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

}
