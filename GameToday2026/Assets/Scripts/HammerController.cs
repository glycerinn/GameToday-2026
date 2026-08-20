using UnityEngine;

public class HammerController : MonoBehaviour
{
    [Header("References")]
    public Transform hammerPivot;
    public Transform hammerTip;
    public Camera cam;

    [Header("Movement")]
    public float pushForce = 12f;

    private Rigidbody rb;

    private bool hammerTouchingGround;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        RotateHammer();
    }

    void RotateHammer()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3 worldMouse = cam.ScreenToWorldPoint(
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                Mathf.Abs(cam.transform.position.z)
            )
        );
    
        worldMouse.z = transform.position.z;

        Vector3 direction = worldMouse;

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        hammerPivot.rotation = Quaternion.Euler(
            0f,
            0f,
            angle
        );
    }

    public void SetHammerTouchingGround(bool touching)
    {
        hammerTouchingGround = touching;
    }
}