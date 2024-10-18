using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject coinPrefab;
    public int coinCount = 4;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // CreatePlayer();
        // CreateCoins();

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log("Client connected id = " + id);
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (reason) =>
        {
            Debug.Log("Client disconnect reason = " + reason);
        };

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            Debug.Log("Server started");
        };
    }

    private void CreateCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            Instantiate(coinPrefab, new Vector3(Random.Range(-10,10), 5f, Random.Range(-10,10)), Quaternion.identity);
        }
    }

    private void CreatePlayer()
    {
        Instantiate(playerPrefab, new Vector3(Random.Range(-5,5), 0.5f, Random.Range(-5,5)), Quaternion.identity);
    }

    public void OnStartServerButtonClicked()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Server started successfully");
        }
        else
        {
            Debug.Log("Server failed to start");
        }
    }

    public void OnStartClientButtonClicked()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started successfully");
        }
        else
        {
            Debug.Log("Client failed to start");
        }
    }

    public void OnStartHostButtonClicked()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully");
        }
        else
        {
            Debug.Log("Host failed to start");
        }
    }

    public void OnShutDownButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        Debug.Log("Server shut down successfully");
    }
}
