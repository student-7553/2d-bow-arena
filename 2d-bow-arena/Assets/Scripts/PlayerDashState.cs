using UnityEngine;

public class PlayerDashState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private bool isStateActive;
    private Vector2 currentDirection;
    private float cachedGravityScale;

    private int currentTickCount;

    public int totalTickCountForDash;
    public float positonAddPerTick;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector2 getCurrentDirectionVector()
    {
        Vector2 effectiveDirection = currentDirection;

        switch (playerState.player.playerObserver.observedState)
        {
            case ObservedState.NEAR_LEFT_WALL:
            case ObservedState.NEAR_RIGHT_WALL:

                if (effectiveDirection.x != 0 && effectiveDirection.y != 0)
                {
                    if (effectiveDirection.y > 0)
                    {
                        // upward
                        effectiveDirection = new Vector2(0, 1);
                    }
                    else
                    {
                        effectiveDirection = new Vector2(0, -1);
                    }
                }
                break;
            case ObservedState.GROUND:
                if (effectiveDirection.x != 0 && effectiveDirection.y < 0)
                {
                    if (effectiveDirection.x > 0)
                    {
                        // upward
                        effectiveDirection = new Vector2(1, 0);
                    }
                    else
                    {
                        effectiveDirection = new Vector2(-1, 0);
                    }
                }
                break;
        }

        Vector2 directionVector = Vector2.zero;

        if (effectiveDirection.x != 0)
        {
            directionVector.x = positonAddPerTick * effectiveDirection.x;
        }

        if (currentDirection.y != 0)
        {
            directionVector.y = positonAddPerTick * effectiveDirection.y;
        }

        return directionVector;
    }

    private void FixedUpdate()
    {
        if (!isStateActive)
        {
            return;
        }

        Vector2 directionTick = getCurrentDirectionVector();

        Vector2 newPosition = (Vector2)gameObject.transform.position + directionTick;

        playerRigidbody.MovePosition(newPosition);

        currentTickCount++;
        if (currentTickCount > totalTickCountForDash)
        {
            playerState.changeState(PlayerPossibleState.NONE);
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
        currentTickCount = 0;

        playerRigidbody.gravityScale = cachedGravityScale;

        playerRigidbody.velocity = Vector2.zero;
    }

    public void stateStart(Vector2 direction)
    {
        isStateActive = true;
        currentDirection = direction;

        cachedGravityScale = playerRigidbody.gravityScale;

        playerRigidbody.gravityScale = 0;

        playerRigidbody.velocity = Vector2.zero;
    }
}
