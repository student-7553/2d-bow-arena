using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct PlayerInputs
{
    public InputActionReference jumpAction;
    public InputActionReference dashAction;
    public InputActionReference movement2dAction;
    public InputActionReference shootAction;
}

[Serializable]
public struct PlayerEntry
{
    public PlayerInputs playerInputs;
    public Vector3 spawnLocation;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject playerPrefab;

    public List<PlayerEntry> playerEntries;

    public Dictionary<int, int> playerDeathCounterMap = new Dictionary<int, int>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        spawnPlayers();
    }

    private void spawnPlayers()
    {
        for (int index = 0; index < playerEntries.Count; index++)
        {
            initPlayerIndex(index);
            playerDeathCounterMap.Add(index, 0);
        }
    }

    public void respawnPlayer(int playerIndex)
    {
        if (playerDeathCounterMap[playerIndex] >= 3)
        {
            Debug.Log("Game over");
            return;
        }

        initPlayerIndex(playerIndex);

        playerDeathCounterMap[playerIndex] = playerDeathCounterMap[playerIndex] + 1;
    }

    public void initPlayerIndex(int index)
    {
        if (playerEntries.Count <= index)
        {
            throw new Exception("Requesting player overflow");
        }

        GameObject playerGameObject = Instantiate(
            playerPrefab,
            playerEntries[index].spawnLocation,
            Quaternion.identity
        );

        Player player = playerGameObject.GetComponent<Player>();
        player.init(index, playerEntries[index].playerInputs);
    }
}
