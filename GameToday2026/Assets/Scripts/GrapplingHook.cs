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
        if (Input.GetMouseButtonDown(1))
        {
            if (!firing && !attached)
            {
                FireHook();
            }
            else if (attached)
            {
                Detach();
            }
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
    }

    void FireHook()
    {
        firing = true;

        hookPosition = hammerTip.position;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.forward, hammerTip.position);
        if (playerPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorld = ray.GetPoint(distance);

            // Direction from hammer tip to mouse
            hookDirection = (mouseWorld - hammerTip.position).normalized;
            hookDirection.z = 0f;
            hookDirection.Normalize();
        }

        rope.enabled = true;
    }

    void MoveHook()
    {
        hookPosition += hookDirection * hookSpeed * Time.deltaTime;
        UpdateRope();
        Collider[] hits = Physics.OverlapSphere(hookPosition, 0.02f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Ground"))
            {
                AttachHook(hookPosition);
                return;
            }
        }

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

        playerRb.AddForce(direction * pullForce, ForceMode.Acceleration);
    }
}