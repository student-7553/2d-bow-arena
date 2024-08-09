using UnityEngine;
using System.Collections;

public class PlayerDeadState : MonoBehaviour
{
    private Player player;
    public int deathTimerSecond;

    private bool isStateActive;

    void Start()
    {
        player = GetComponent<Player>();
    }

    // private void FixedUpdate()
    // {
    //     // if (!isStateActive)
    //     // {
    //     //     return;
    //     // }
    //     //     playerState.changeState(PlayerPossibleState.NONE);
    // }

    public void stateEnd()
    {
        // should not end
        isStateActive = false;
        player.playerMovementHandler.handleUnDisableMovement();
        gameObject.layer = 6;
    }

    public void stateStart()
    {
        isStateActive = true;
        player.playerMovementHandler.handleDisableMovement();
        player.deathCount++;

        // Setting the player to dead player layer
        gameObject.layer = 8;

        StartCoroutine(deathTimer());
    }

    private IEnumerator deathTimer()
    {
        yield return new WaitForSeconds(deathTimerSecond);
        GameManager.instance.respawnPlayer(player);
    }
}
