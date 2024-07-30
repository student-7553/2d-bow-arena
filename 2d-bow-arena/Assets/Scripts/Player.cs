using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerState playerstate;
    public PlayerInputHandler playerInputHandler;
    public PlayerObserver playerObserver;

    public bool isDashAvailable;
    public int arrowCount;

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
