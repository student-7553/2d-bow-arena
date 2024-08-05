using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct PlayerInputs
{
    public InputActionReference jumpAction;
    public InputActionReference dashAction;
    public InputActionReference movement2dAction;
    public InputActionReference shootAction;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<PlayerInputs> playerInputs;
    private List<Player> players = new List<Player>();

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

    public void initPlayer(Player player)
    {
        if (playerInputs.Count <= currentPlayers)
        {
            throw new Exception("Requesting player overflow");
        }

        Debug.Log(currentPlayers);

        player.playerInputHandler.init(playerInputs[currentPlayers]);
        players.Add(player);
        currentPlayers++;
    }
}
