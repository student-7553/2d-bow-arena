using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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

public struct PlayerInputHistoryEntry
{
    public PlayerInput input;
    public DateTime entryDateTime;

    public PlayerInputHistoryEntry(PlayerInput _playerInput, DateTime _entryDateTime)
    {
        input = _playerInput;
        entryDateTime = _entryDateTime;
    }
}

public struct PlayerStateHistoryEntry
{
    public PlayerPossibleState state;
    public DateTime entryDateTime;

    public PlayerStateHistoryEntry(PlayerPossibleState _playerState, DateTime _entryDateTime)
    {
        state = _playerState;
        entryDateTime = _entryDateTime;
    }
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

    public float kayoteMilisec;

    // todo move this to player inputs
    private PlayerInputHistoryEntry playerInputHistoryEntry = new PlayerInputHistoryEntry(
        PlayerInput.JUMP,
        DateTime.MinValue
    );

    private List<PlayerStateHistoryEntry> playerStateHistoryEntries =
        new List<PlayerStateHistoryEntry>();

    // new PlayerStateHistoryEntry(
    //     PlayerPossibleState.NONE,
    //     DateTime.MinValue
    // );

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
        playerJumpHandler.isHolding = true;
        playerInputHistoryEntry = new PlayerInputHistoryEntry(PlayerInput.JUMP, DateTime.Now);

        if (
            playerStateHistoryEntries.Count != 0
            && playerStateHistoryEntries[playerStateHistoryEntries.Count - 2].state
                == PlayerPossibleState.GROUND
            && playerStateHistoryEntries[
                playerStateHistoryEntries.Count - 1
            ].entryDateTime.AddMilliseconds(kayoteMilisec) > DateTime.Now
        )
        {
            changeState(PlayerPossibleState.JUMPING);
            return;
        }

        switch (currentState)
        {
            case PlayerPossibleState.GROUND:
                changeState(PlayerPossibleState.JUMPING);
                break;
            case PlayerPossibleState.SLIDING:
                changeState(PlayerPossibleState.SLIDE_JUMPING);
                break;
            case PlayerPossibleState.FALLING:
                if (player.playerObserver.observedWallState == ObservedWallState.NONE)
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
        playerJumpHandler.isHolding = false;

        if (currentState != PlayerPossibleState.JUMPING)
        {
            return;
        }

        changeState(PlayerPossibleState.NONE);
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

        if (
            newState == PlayerPossibleState.GROUND
            && playerInputHistoryEntry.entryDateTime.AddMilliseconds(kayoteMilisec) > DateTime.Now
        )
        {
            changeState(PlayerPossibleState.JUMPING);
            return true;
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
                    player.playerObserver.observedWallState == ObservedWallState.NEAR_RIGHT_WALL
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
        playerStateHistoryEntries.Add(new PlayerStateHistoryEntry(currentState, DateTime.Now));

        if (newState == PlayerPossibleState.JUMPING)
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
