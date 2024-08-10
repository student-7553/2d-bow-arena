using UnityEngine;

public enum ObservedWallState
{
    NEAR_LEFT_WALL,
    NEAR_RIGHT_WALL,
    NONE
}

public class PlayerObserver : MonoBehaviour
{
    private BoxCollider2D playerCollider;

    private float yMargin;
    private float xMargin;

    private Player player;

    public bool isOnGround;
    public ObservedWallState observedWallState = ObservedWallState.NONE;

    public LayerMask layerMask;

    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        yMargin = (playerCollider.size.y / 2f) - 0.1f;
        xMargin = (playerCollider.size.x / 2f) - 0.1f;

        player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        computeObservedState();
        handleSelfStates();
    }

    private void handleSelfStates()
    {
        if (
            player.playerstate.isStateChangeOnCooldown
            || (
                player.playerstate.currentState != PlayerPossibleState.NONE
                && player.playerstate.currentState != PlayerPossibleState.FALLING
                && player.playerstate.currentState != PlayerPossibleState.GROUND
                && player.playerstate.currentState != PlayerPossibleState.SLIDING
            )
        )
        {
            return;
        }

        if (isOnGround)
        {
            player.playerstate.changeState(PlayerPossibleState.GROUND);
            return;
        }
        if (
            observedWallState == ObservedWallState.NEAR_LEFT_WALL
            && player.playerMovementHandler.direction.x < 0
        )
        {
            player.playerstate.changeState(PlayerPossibleState.SLIDING);
            return;
        }
        if (
            observedWallState == ObservedWallState.NEAR_RIGHT_WALL
            && player.playerMovementHandler.direction.x > 0
        )
        {
            player.playerstate.changeState(PlayerPossibleState.SLIDING);
            return;
        }
        player.playerstate.changeState(PlayerPossibleState.FALLING);
    }

    private void computeObservedState()
    {
        isOnGround = computeIsOnGround();

        if (computeIsSlidingLeft())
        {
            observedWallState = ObservedWallState.NEAR_LEFT_WALL;
        }
        else if (computeIsSlidingRight())
        {
            observedWallState = ObservedWallState.NEAR_RIGHT_WALL;
        }
        else
        {
            observedWallState = ObservedWallState.NONE;
        }
    }

    private bool computeIsOnGround()
    {
        Vector2 originPosition = new Vector2(
            gameObject.transform.position.x,
            gameObject.transform.position.y - yMargin
        );

        RaycastHit2D rayCastResult = Physics2D.Raycast(
            originPosition,
            Vector2.down,
            0.15f,
            layerMask
        );

        // Debug.DrawRay(originPosition, Vector2.down * 0.25f, Color.red);

        return !!rayCastResult.collider;
    }

    private bool computeIsSlidingRight()
    {
        Vector2 originPosition = new Vector2(
            gameObject.transform.position.x + xMargin,
            gameObject.transform.position.y
        );

        RaycastHit2D rayCastResult = Physics2D.Raycast(
            originPosition,
            Vector2.right,
            0.15f,
            layerMask
        );

        // Debug.DrawRay(originPosition, Vector2.right * 0.15f, Color.red);

        return !!rayCastResult.collider;
    }

    private bool computeIsSlidingLeft()
    {
        Vector2 originPosition = new Vector2(
            gameObject.transform.position.x - xMargin,
            gameObject.transform.position.y
        );

        RaycastHit2D rayCastResult = Physics2D.Raycast(
            originPosition,
            Vector2.left,
            0.15f,
            layerMask
        );

        return !!rayCastResult.collider;
    }
}
