using UnityEngine;

public class HammerTip : MonoBehaviour
{
    public HammerController hammerController;

    private Vector3 previousPosition;
    private float swingSpeed;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        // Calculate how fast the hammer tip is moving
        swingSpeed =
            Vector3.Distance(transform.position, previousPosition)
            / Time.deltaTime;

        previousPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            hammerController.SetHammerTouchingGround(true);
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            Debug.Log("HAMMER HIT ENEMY! Swing speed: " + swingSpeed);

            // Kill if hammer is moving fast enough
            if (swingSpeed >= hammerController.swingThreshold)
            {
                Debug.Log("ENEMY KILLED!");

                enemy.Die();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            hammerController.SetHammerTouchingGround(false);
        }
    }
}