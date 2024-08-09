using UnityEngine;
using System.Collections;

public class PlayerDeadState : MonoBehaviour
{
    private Player player;
    public int deathTimerSecond;

    void Start()
    {
        player = GetComponent<Player>();
    }

    public void stateEnd()
    {
        Debug.LogError("PlayerDeadState should never change");
        // player.playerMovementHandler.handleUnDisableMovement();
        // gameObject.layer = 6;
    }

    public void stateStart()
    {
        player.playerMovementHandler.handleDisableMovement();
        player.deathCount++;

        // Setting the player to dead player layer
        gameObject.layer = 8;

        StartCoroutine(deathTimer());
    }

    private IEnumerator deathTimer()
    {
        yield return new WaitForSeconds(deathTimerSecond);
        GameManager.instance.respawnPlayer(player.playerIndex);
    }
}
