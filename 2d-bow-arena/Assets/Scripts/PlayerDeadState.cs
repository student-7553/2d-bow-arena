using UnityEngine;

public class PlayerDeadState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private Player player;

    private bool isStateActive;

    void Start()
    {
        player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        // if (!isStateActive)
        // {
        //     return;
        // }
        //     playerState.changeState(PlayerPossibleState.NONE);
    }

    public void stateEnd()
    {
        // should not end
        isStateActive = false;
    }

    public void stateStart()
    {
        isStateActive = true;
        player.playerMovementHandler.handleDisableMovement();
        player.playerMovementHandler.handleChangeToTrigger();
    }
}
