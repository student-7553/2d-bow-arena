using UnityEngine;

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

    public void handleHit()
    {
        if (playerstate.currentState == PlayerPossibleState.DEAD)
        {
            return;
        }

        playerstate.changeState(PlayerPossibleState.DEAD);
        gameObject.layer = 8;
        // playerMovementHandler.playerRigidbody.AddForce(arrowDirection * arrowForce);
    }

    public void dashMark()
    {
        isDashAvailable = false;
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
