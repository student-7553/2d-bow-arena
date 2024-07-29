using UnityEngine;

public class PlayerAimState : MonoBehaviour
{
    private PlayerState playerState;

    private bool isStateActive;
    public GameObject arrowPrefab;

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

        // Vector2 direction = playerState.playerMovementHandler.direction;
        // Debug.Log("Aiming/" + direction);
        // handle anim here
    }

    public void handleShoot()
    {
        Vector2 direction = playerState.playerMovementHandler.direction;
        if (!isAiming(direction))
        {
            return;
        }

        Debug.Log("Shot/" + direction);

        Vector3 location = transform.position + new Vector3(direction.x, direction.y, 0);
        GameObject spawnedArrowObject = Instantiate(arrowPrefab, location, Quaternion.identity);
        Arrow arrow = spawnedArrowObject.GetComponent<Arrow>();
        if (arrow == null)
        {
            throw new System.Exception("Arrow class missing from prefab object");
        }

        arrow.startShoot(direction);

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
