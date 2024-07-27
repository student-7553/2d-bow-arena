using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerState playerstate;
    public PlayerInputHandler playerInputHandler;

    void Start()
    {
        GameManager.instance.initPlayer(this);
    }

    public void handleRespawn()
    {
        Debug.Log("Handle restart.......");
    }
}
