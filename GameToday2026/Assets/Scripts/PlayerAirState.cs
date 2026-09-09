using UnityEngine;

public class PlayerAirState : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
    public bool IsGrounded { get; private set; }

    public bool IsAirborne
    {
        get { return !IsGrounded; }
    }

    void Update()
    {
        CheckGround();
    }

    void CheckGround()
    {
        if (groundCheck == null)
        {
            return;
        }

        IsGrounded = Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = IsGrounded ? Color.green : Color.red;

        Gizmos.DrawLine(
            groundCheck.position,
            groundCheck.position +
            Vector3.down * groundCheckDistance
        );
    }
}