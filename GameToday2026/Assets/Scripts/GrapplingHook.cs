using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("References")]
    public Transform hammerTip;
    public Camera cam;
    public LineRenderer rope;
    public Rigidbody playerRb;

    [Header("Hook")]
    public float hookSpeed = 20f;
    public float maxDistance = 20f;
    public float pullForce = 20f;

    private bool firing;
    private bool attached;

    private Vector3 hookPosition;
    private Vector3 hookDirection;
    private Vector3 anchorPoint;

    void Start()
    {
        rope.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !firing && !attached)
        {
            FireHook();
        }

        if (firing)
        {
            MoveHook();
        }

        if (attached)
        {
            UpdateRope();
            PullPlayer();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Detach();
        }
    }

    void FireHook()
    {
        firing = true;

        hookPosition = hammerTip.position;

        Vector3 mouseScreen = Input.mousePosition;

        // Distance from camera to hammer
        mouseScreen.z = Mathf.Abs(cam.transform.position.z - hammerTip.position.z);

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        // Get direction from hammer tip toward mouse
        hookDirection = (mouseWorld - hammerTip.position).normalized;

        rope.enabled = true;
    }

    void MoveHook()
    {
        hookPosition += hookDirection * hookSpeed * Time.deltaTime;

        // Move hook visually
        UpdateRope();

        // Check collision
        Collider[] hits = Physics.OverlapSphere(hookPosition, 0.1f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Ground"))
            {
                AttachHook(hookPosition);
                return;
            }
        }

        // Too far
        if (Vector3.Distance(hammerTip.position, hookPosition) > maxDistance)
        {
            CancelHook();
        }
    }

    void AttachHook(Vector3 position)
    {
        firing = false;
        attached = true;

        anchorPoint = position;

        UpdateRope();
    }

    void CancelHook()
    {
        firing = false;
        rope.enabled = false;
    }

    void UpdateRope()
    {
        rope.positionCount = 2;

        rope.SetPosition(0, hammerTip.position);
        rope.SetPosition(1, hookPosition);
    }

    public void Detach()
    {
        firing = false;
        attached = false;

        rope.enabled = false;
    }

    void PullPlayer()
    {
        Vector3 direction = anchorPoint - playerRb.position;
        direction.z = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        playerRb.AddForce(direction * pullForce, ForceMode.Force);
    }
}