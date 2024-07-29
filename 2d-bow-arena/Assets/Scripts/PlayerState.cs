using UnityEngine;
using System.Collections;
using System.Linq;

public enum PlayerPossibleState
{
    NONE,
    GROUND,
    FALLING,
    AIMING,
    DASHING,
    JUMPING,
    SLIDING,
    SLIDE_JUMPING
}

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    public PlayerPossibleState currentState;

    public PlayerObserver playerObserver;

    public PlayerMovementHandler playerMovementHandler;

    // ------------------STATES-------------------------------
    private PlayerJumpState playerJumpHandler;
    private PlayerDashState playerDashState;
    private PlayerSlideState playerSlideState;
    private PlayerSlideJumpState playerSlideJumpState;
    private PlayerAimState playerAimState;

    // -------------------------------------------------

    public bool isStateChangeOnCooldown;

    void Start()
    {
        playerObserver = GetComponent<PlayerObserver>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
        playerJumpHandler = GetComponent<PlayerJumpState>();
        playerDashState = GetComponent<PlayerDashState>();
        playerSlideState = GetComponent<PlayerSlideState>();
        playerSlideJumpState = GetComponent<PlayerSlideJumpState>();
        playerAimState = GetComponent<PlayerAimState>();

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
                    playerObserver.observedState != ObservedState.NEAR_LEFT_WALL
                    && playerObserver.observedState != ObservedState.NEAR_RIGHT_WALL
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
        if (!playerObserver.isDashAvailable)
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
                playerMovementHandler.handleUnDisableMovement();
                playerDashState.stateEnd();
                break;
            case PlayerPossibleState.AIMING:
                playerMovementHandler.handleUnDisableMovement();
                playerAimState.stateEnd();
                break;
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateEnd();
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                playerMovementHandler.handleUnDisableMovement();
                playerSlideJumpState.stateEnd();
                break;
        }

        switch (newState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateStart(playerMovementHandler.direction);
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                playerMovementHandler.handleDisableMovement();
                playerSlideJumpState.stateStart(
                    playerObserver.observedState == ObservedState.NEAR_RIGHT_WALL ? true : false
                );
                break;
            case PlayerPossibleState.DASHING:
                playerMovementHandler.handleDisableMovement();
                playerObserver.dashMark();

                Vector2 dashDirection = playerMovementHandler.direction;
                if (dashDirection == Vector2.zero)
                {
                    dashDirection =
                        playerMovementHandler.lookingDirection == LookingDirection.LEFT
                            ? Vector2.left
                            : Vector2.right;
                }

                playerDashState.stateStart(dashDirection);
                break;
            case PlayerPossibleState.AIMING:
                playerMovementHandler.handleDisableMovement();
                playerAimState.stateStart();
                break;
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateStart();
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
