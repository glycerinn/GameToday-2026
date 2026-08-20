using UnityEngine;

public class HammerTip : MonoBehaviour
{
    public HammerController hammerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            hammerController.SetHammerTouchingGround(true);
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