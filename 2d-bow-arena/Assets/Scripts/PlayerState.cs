using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public enum PlayerPossibleState
{
    NONE,
    GROUND,
    FALLING,
    AIMING,
    DASHING,
    JUMPING,
    SLIDING,
    SLIDE_JUMPING,
    DEAD,
}

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    public PlayerPossibleState currentState;

    [NonSerialized]
    public Player player;

    // ------------------STATES-------------------------------
    private PlayerJumpState playerJumpHandler;
    private PlayerDashState playerDashState;
    private PlayerSlideState playerSlideState;
    private PlayerSlideJumpState playerSlideJumpState;
    private PlayerAimState playerAimState;
    private PlayerDeadState playerDeadState;

    // -------------------------------------------------

    public bool isStateChangeOnCooldown;

    void Start()
    {
        player = GetComponent<Player>();
        playerJumpHandler = GetComponent<PlayerJumpState>();
        playerDashState = GetComponent<PlayerDashState>();
        playerSlideState = GetComponent<PlayerSlideState>();
        playerSlideJumpState = GetComponent<PlayerSlideJumpState>();
        playerAimState = GetComponent<PlayerAimState>();
        playerDeadState = GetComponent<PlayerDeadState>();

        currentState = PlayerPossibleState.GROUND;
    }

    public void handleJumpAction()
    {
        switch (currentState)
        {
            case PlayerPossibleState.GROUND:
                changeState(PlayerPossibleState.JUMPING);
                break;
            case PlayerPossibleState.SLIDING:
                changeState(PlayerPossibleState.SLIDE_JUMPING);
                break;
            case PlayerPossibleState.FALLING:
                if (
                    player.playerObserver.observedState != ObservedState.NEAR_LEFT_WALL
                    && player.playerObserver.observedState != ObservedState.NEAR_RIGHT_WALL
                )
                {
                    return;
                }
                changeState(PlayerPossibleState.SLIDE_JUMPING);
                break;
        }
    }

    public void handleShootAction()
    {
        if (
            currentState != PlayerPossibleState.NONE
            && currentState != PlayerPossibleState.GROUND
            && currentState != PlayerPossibleState.FALLING
        )
        {
            return;
        }
        if (player.arrowCount == 0)
        {
            return;
        }
        changeState(PlayerPossibleState.AIMING);
    }

    public void handleShootActionEnd()
    {
        if (currentState != PlayerPossibleState.AIMING)
        {
            return;
        }
        playerAimState.handleShoot();

        changeState(PlayerPossibleState.NONE);
        // if not state do nothing
    }

    public void handleDashAction()
    {
        if (currentState == PlayerPossibleState.DEAD)
        {
            return;
        }

        if (!player.isDashAvailable)
        {
            return;
        }

        changeState(PlayerPossibleState.DASHING);
    }

    public void handleJumpActionEnd()
    {
        if (currentState != PlayerPossibleState.JUMPING)
        {
            return;
        }

        playerJumpHandler.handleJumpEnd();
    }

    private bool isAllowedToChangeStateTo(PlayerPossibleState newState)
    {
        if (newState == currentState)
        {
            return false;
        }

        return true;
    }

    public bool changeState(PlayerPossibleState newState)
    {
        bool isAllowed = isAllowedToChangeStateTo(newState);

        if (!isAllowed)
        {
            return false;
        }

        switch (currentState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateEnd();
                break;
            case PlayerPossibleState.DASHING:
                player.playerMovementHandler.handleUnDisableMovement();
                playerDashState.stateEnd();
                break;
            case PlayerPossibleState.AIMING:
                player.playerMovementHandler.handleUnDisableMovement();
                playerAimState.stateEnd();
                break;
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateEnd();
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                player.playerMovementHandler.handleUnDisableMovement();
                playerSlideJumpState.stateEnd();
                break;
            case PlayerPossibleState.DEAD:
                playerDeadState.stateEnd();
                break;
        }

        switch (newState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateStart(player.playerMovementHandler.direction);
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                player.playerMovementHandler.handleDisableMovement();
                playerSlideJumpState.stateStart(
                    player.playerObserver.observedState == ObservedState.NEAR_RIGHT_WALL
                        ? true
                        : false
                );
                break;
            case PlayerPossibleState.DASHING:
                player.playerMovementHandler.handleDisableMovement();
                player.startDash();

                Vector2 dashDirection = player.playerMovementHandler.direction;
                if (dashDirection == Vector2.zero)
                {
                    dashDirection =
                        player.playerMovementHandler.lookingDirection == LookingDirection.LEFT
                            ? Vector2.left
                            : Vector2.right;
                }

                playerDashState.stateStart(dashDirection);
                break;
            case PlayerPossibleState.AIMING:
                player.playerMovementHandler.handleDisableMovement();
                playerAimState.stateStart();
                break;
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateStart();
                break;
            case PlayerPossibleState.DEAD:
                playerDeadState.stateStart();
                break;
        }

        currentState = newState;

        PlayerPossibleState[] cooldownStates = new PlayerPossibleState[]
        {
            PlayerPossibleState.JUMPING
        };

        if (cooldownStates.Contains(newState))
        {
            StartCoroutine(handleIsStateChangeOnCooldown());
        }

        return true;
    }

    public IEnumerator handleIsStateChangeOnCooldown()
    {
        // This function should prob be in jump handler tbh
        isStateChangeOnCooldown = true;
        yield return new WaitForSeconds(0.05f);
        isStateChangeOnCooldown = false;
    }
}
