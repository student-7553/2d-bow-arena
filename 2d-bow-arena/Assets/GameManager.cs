using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[Serializable]
public struct PlayerInputs
{
    public InputActionReference jumpAction;
    public InputActionReference dashAction;
    public InputActionReference movement2dAction;
    public InputActionReference shootAction;
}

struct PlayerEntry
{
    public Player player;
    public int deathCount;

    public PlayerEntry(Player _player)
    {
        player = _player;
        deathCount = 0;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<PlayerInputs> playerInputs;

    private List<PlayerEntry> playerEntries = new List<PlayerEntry>();

    [SerializeField]
    private int currentPlayers = 1;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void respawnPlayer(Player player)
    {
        if (player.deathCount >= 3)
        {
            Debug.Log("Game over");
            return;
        }

        player.handleRespawn();
    }

    public int initPlayer(Player player)
    {
        if (playerInputs.Count <= currentPlayers)
        {
            throw new Exception("Requesting player overflow");
        }

        player.playerInputHandler.init(playerInputs[currentPlayers]);

        playerEntries.Add(new PlayerEntry(player));

        currentPlayers++;

        return currentPlayers;
    }
}
