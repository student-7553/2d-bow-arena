using UnityEngine;

public class PlayerAimState : MonoBehaviour
{
    private PlayerState playerState;

    private bool isStateActive;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
    }

    private void FixedUpdate()
    {
        if (!isStateActive)
        {
            return;
        }

        Vector2 direction = playerState.playerMovementHandler.direction;
        Debug.Log("Aiming/" + direction);
    }

    public void handleShoot()
    {
        Vector2 direction = playerState.playerMovementHandler.direction;
        if (!isAiming(direction))
        {
            return;
        }
        Debug.Log("Shot/" + direction);
        // is aiming
    }

    private bool isAiming(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return false;
        }
        return true;
    }

    public void stateEnd()
    {
        isStateActive = false;
    }

    public void stateStart()
    {
        isStateActive = true;
    }
}
