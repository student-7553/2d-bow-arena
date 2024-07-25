using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerState playerstate;

    void Start()
    {
        Singelton.AddPlayer(this);
    }

    public void handleRespawn()
    {
        Debug.Log("Handle restart.......");
    }
}
