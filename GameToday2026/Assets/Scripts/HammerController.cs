using UnityEngine;

public class HammerController : MonoBehaviour
{
    [Header("References")]
    public Transform hammerPivot;
    public Transform hammer;
    public Transform hammerTip;
    public Rigidbody playerRb;
    public Camera cam;

    [Header("Hammer Distance")]
    public float minDistance = 0.5f;
    public float maxDistance = 3f;
    public float distanceMultiplier = 2f;

    [Header("Push")]
    public float pushForce = 25f;

    private bool hammerTouchingGround;
    private Vector3 previousHammerTipPosition;
    private float previousMouseDistance;

    [Header("Hammer Swing")]
    public float swingThreshold = 2f;

    private Vector3 hammerVelocity;

    void Start()
    {
        previousHammerTipPosition = hammerTip.position;
    }

    void Update()
    {
        UpdateHammer();
    }

    void FixedUpdate()
    {
        hammerVelocity = (hammerTip.position - previousHammerTipPosition)/ Time.fixedDeltaTime;
        if (hammerTouchingGround)
        {
            PushPlayer();
        }

        previousHammerTipPosition = hammerTip.position;
    }

    void UpdateHammer()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Plane playerPlane = new Plane(
            Vector3.forward,
            hammerPivot.position
        );

        if (!playerPlane.Raycast(ray, out float distance))
            return;

        Vector3 mouseWorld = ray.GetPoint(distance);

        Vector3 direction = mouseWorld - hammerPivot.position;
        direction.z = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        // Actual mouse distance from player
        float mouseDistance = Vector3.Distance(mouseWorld, hammerPivot.position);

        // How much the mouse moved outward this frame
        float mouseDelta = mouseDistance - previousMouseDistance;
        previousMouseDistance = mouseDistance;

        // Calculate hammer distance
        float hammerDistance = Mathf.Clamp(mouseDistance * distanceMultiplier, minDistance, maxDistance);

        // Rotate hammer toward mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        hammerPivot.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // Move hammer
        hammer.position = hammerPivot.position + direction * hammerDistance;
        hammer.position = new Vector3(hammer.position.x, hammer.position.y, hammerPivot.position.z);

        // -----------------------------------------
        // EXCESS MOUSE MOVEMENT
        // -----------------------------------------

        if (hammerDistance >= maxDistance && mouseDelta > 0f)
        {
            Vector3 launchDirection = -direction;

            launchDirection.z = 0f;
            playerRb.AddForce(launchDirection * mouseDelta * pushForce, ForceMode.Impulse);
        }
    }

    public bool IsHammerSwinging()
    {
        return hammerVelocity.sqrMagnitude >= swingThreshold * swingThreshold;
    }

    public void SetHammerTouchingGround(bool touching)
    {
        hammerTouchingGround = touching;
    }

    void PushPlayer()
    {
        Vector3 hammerMovement = hammerTip.position - previousHammerTipPosition;

        hammerMovement.z = 0f;

        if (hammerMovement.sqrMagnitude < 0.0001f)
            return;

        // Only push when the hammer is moving toward the ground/player
        Vector3 pushDirection = -hammerMovement.normalized;
        playerRb.AddForce(pushDirection * pushForce, ForceMode.Force);
    }
}