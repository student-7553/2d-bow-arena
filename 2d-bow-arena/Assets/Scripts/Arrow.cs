using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D arrowRigidBody;
    private BoxCollider2D arrowCollider;

    private Vector2 direction;

    private bool isAlive;

    private int fixedCounter;

    private float cachedGravityScale;

    public int maxThrustTickCount;
    public int travelVelocity;

    private void Start()
    {
        arrowRigidBody = GetComponent<Rigidbody2D>();
        if (arrowRigidBody == null)
        {
            throw new System.Exception("Rigidbody2D is empty in arrow prefab");
        }

        arrowCollider = GetComponent<BoxCollider2D>();
        if (arrowRigidBody == null)
        {
            throw new System.Exception("BoxCollider2D is empty in arrow prefab");
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }
        fixedCounter++;
        if (fixedCounter <= maxThrustTickCount)
        {
            if (arrowRigidBody.gravityScale != 0)
            {
                cachedGravityScale = arrowRigidBody.gravityScale;
                arrowRigidBody.gravityScale = 0;
            }
            // pre tick
            handleThrust();
        }
        else
        {
            if (arrowRigidBody.gravityScale == 0)
            {
                arrowRigidBody.gravityScale = cachedGravityScale;
            }
            // pre tick
        }
    }

    private void setNotActive()
    {
        isAlive = false;
        if (arrowRigidBody.gravityScale == 0)
        {
            arrowRigidBody.gravityScale = cachedGravityScale;
        }
        arrowRigidBody.bodyType = RigidbodyType2D.Static;
        arrowCollider.isTrigger = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        setNotActive();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // collected by the player
    }

    private void handleThrust()
    {
        Vector2 tickForce = direction * travelVelocity;
        arrowRigidBody.velocity = tickForce;
    }

    public void startShoot(Vector2 _direction)
    {
        direction = _direction;
        fixedCounter = 0;
        isAlive = true;
    }
}
