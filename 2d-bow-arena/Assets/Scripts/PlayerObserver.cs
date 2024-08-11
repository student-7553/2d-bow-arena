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
        yMargin = (playerCollider.size.y / 2f) - 0.05f;
        xMargin = (playerCollider.size.x / 2f) - 0.05f;

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
        Vector2 originLeftPosition = new Vector2(
            gameObject.transform.position.x - xMargin,
            gameObject.transform.position.y - yMargin
        );
        Vector2 originRightPosition = new Vector2(
            gameObject.transform.position.x + xMargin,
            gameObject.transform.position.y - yMargin
        );

        RaycastHit2D rayCastLeftResult = Physics2D.Raycast(
            originLeftPosition,
            Vector2.down,
            0.1f,
            layerMask
        );

        RaycastHit2D rayCastRightResult = Physics2D.Raycast(
            originRightPosition,
            Vector2.down,
            0.1f,
            layerMask
        );

        // Debug.DrawRay(originLeftPosition, Vector2.down * 0.1f, Color.red);
        // Debug.DrawRay(originRightPosition, Vector2.down * 0.1f, Color.red);

        return !!rayCastRightResult.collider || !!rayCastLeftResult.collider;
    }

    private bool computeIsSlidingRight()
    {
        Vector2 originBottomPosition = new Vector2(
            gameObject.transform.position.x + xMargin,
            gameObject.transform.position.y - yMargin
        );

        Vector2 originTopPosition = new Vector2(
            gameObject.transform.position.x + xMargin,
            gameObject.transform.position.y + yMargin
        );

        RaycastHit2D rayCastBottomResult = Physics2D.Raycast(
            originBottomPosition,
            Vector2.right,
            0.1f,
            layerMask
        );

        RaycastHit2D rayCastTopResult = Physics2D.Raycast(
            originTopPosition,
            Vector2.right,
            0.1f,
            layerMask
        );

        // Debug.DrawRay(originBottomPosition, Vector2.right * 0.1f, Color.red);
        // Debug.DrawRay(originTopPosition, Vector2.right * 0.1f, Color.red);

        return !!rayCastBottomResult.collider || !!rayCastTopResult.collider;
    }

    private bool computeIsSlidingLeft()
    {
        Vector2 originBottomPosition = new Vector2(
            gameObject.transform.position.x - xMargin,
            gameObject.transform.position.y - yMargin
        );

        Vector2 originTopPosition = new Vector2(
            gameObject.transform.position.x - xMargin,
            gameObject.transform.position.y + yMargin
        );

        RaycastHit2D rayCastBottomResult = Physics2D.Raycast(
            originBottomPosition,
            Vector2.left,
            0.1f,
            layerMask
        );

        RaycastHit2D rayCastTopResult = Physics2D.Raycast(
            originTopPosition,
            Vector2.left,
            0.1f,
            layerMask
        );

        // Debug.DrawRay(originBottomPosition, Vector2.left * 0.1f, Color.red);
        // Debug.DrawRay(originTopPosition, Vector2.left * 0.1f, Color.red);

        return !!rayCastBottomResult.collider || !!rayCastTopResult.collider;
    }
}
