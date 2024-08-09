using System;
using UnityEngine;

public enum PlayerArrowHitResult
{
    HIT,
    CATCHED
}

public class Player : MonoBehaviour
{
    public PlayerState playerstate;
    public PlayerInputHandler playerInputHandler;
    public PlayerObserver playerObserver;
    public PlayerMovementHandler playerMovementHandler;

    [NonSerialized]
    public Arrow stuckArrow;

    public bool isDashAvailable;
    public int arrowCount;
    public int deathCount;

    [NonSerialized]
    public int playerIndex;

    public void init(int _playerIndex, PlayerInputs playerInputs)
    {
        playerIndex = _playerIndex;
        playerInputHandler.init(playerInputs);
    }

    private void FixedUpdate()
    {
        handleDashCooldown();
    }

    public PlayerArrowHitResult handleArrowHit()
    {
        if (playerstate.currentState == PlayerPossibleState.DASHING)
        {
            grabArrow();
            playerMovementHandler.playerRigidbody.velocity = Vector2.zero;
            return PlayerArrowHitResult.CATCHED;
        }

        playerstate.changeState(PlayerPossibleState.DEAD);
        return PlayerArrowHitResult.HIT;
    }

    public void startDash()
    {
        isDashAvailable = false;
    }

    public void grabArrow()
    {
        arrowCount++;
    }

    public void handleDashCooldown()
    {
        if (playerObserver.observedState != ObservedState.GROUND)
        {
            return;
        }

        if (playerstate.currentState == PlayerPossibleState.DASHING)
        {
            return;
        }

        isDashAvailable = true;
    }
}
