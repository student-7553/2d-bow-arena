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

    public bool isDashAvailable;
    public int arrowCount;

    public float arrowForce;

    void Start()
    {
        GameManager.instance.initPlayer(this);
    }

    private void FixedUpdate()
    {
        handleDashCooldown();
    }

    public void handleRespawn()
    {
        Debug.Log("Handle restart.......");
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

        // Setting the player to dead player layer
        gameObject.layer = 8;

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
