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

    [Header("Grip / Launch")]
    public float launchForce = 25f;
    public float gripRadius = 0.2f;
    public float minimumHammerMovement = 0.01f;

    [Header("Hammer Swing")]
    public float swingThreshold = 2f;

    private bool hammerTouchingGround;

    [Header("Player Speed Limit")]
    public float maxPlayerSpeed = 25f;

    private Vector3 gripPoint;

    private Vector3 previousHammerTipPosition;
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
        hammerVelocity = (hammerTip.position - previousHammerTipPosition) / Time.fixedDeltaTime;

        if (hammerTouchingGround)
        {
            GripAndLaunch();
        }

        previousHammerTipPosition = hammerTip.position;
    }

    void UpdateHammer()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Plane playerPlane = new Plane(Vector3.forward, hammerPivot.position);

        if (!playerPlane.Raycast(ray, out float distance))
            return;

        Vector3 mouseWorld = ray.GetPoint(distance);

        Vector3 direction = mouseWorld - hammerPivot.position;

        direction.z = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        float mouseDistance = Vector3.Distance(mouseWorld, hammerPivot.position);

        float hammerDistance = Mathf.Clamp(mouseDistance * distanceMultiplier, minDistance, maxDistance);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        hammerPivot.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        Vector3 hammerPosition = hammerPivot.position + direction * hammerDistance;

        hammerPosition.z = hammerPivot.position.z;

        hammer.position = hammerPosition;
    }

    void GripAndLaunch()
    {
        Vector3 hammerMovement = hammerTip.position - previousHammerTipPosition;

        hammerMovement.z = 0f;

        if (hammerMovement.sqrMagnitude < minimumHammerMovement * minimumHammerMovement)
        {
            return;
        }

        Vector3 direction = playerRb.position - gripPoint;

        direction.z = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        float hammerSpeed = hammerVelocity.magnitude;
        float force = hammerSpeed * launchForce;
        playerRb.AddForce(direction * force, ForceMode.Acceleration);

        Vector3 velocity = playerRb.linearVelocity;
        velocity.z = 0f;

        if (velocity.magnitude > maxPlayerSpeed)
        {
            velocity = velocity.normalized * maxPlayerSpeed;
        }

        playerRb.linearVelocity = velocity;
    }

    public bool IsHammerSwinging()
    {
        return hammerVelocity.sqrMagnitude >= swingThreshold * swingThreshold;
    }

    public void SetHammerTouchingGround(bool touching)
    {
        hammerTouchingGround = touching;

        if (touching)
        {
            gripPoint = hammerTip.position;

            Debug.Log("HAMMER GRIPPED AT: " + gripPoint);
        }
    }

    public void SetGripPoint(Vector3 point)
    {
        gripPoint = point;
        hammerTouchingGround = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (hammerPivot == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(hammerPivot.position, maxDistance);

        if (hammerTouchingGround)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(gripPoint, gripRadius);
        }
    }
}