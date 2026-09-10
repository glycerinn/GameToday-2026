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

    [Header("Enemy Detection")]
    public float enemyHitRadius = 0.2f;

    [Header("Unlock")]
    public bool grappleUnlocked = false;

    private bool firing;
    public bool attached;

    private Vector3 hookPosition;
    private Vector3 hookDirection;
    private Vector3 anchorPoint;

    private AudioManager audioManager;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        rope.enabled = false;
    }

    void Update()
    {
        if (PlayerHealth.GameOver)
        {
            Detach();
            return;
        }
        
        if (UpgradeManager.UpgradeSelectionActive|| WaveManager.DialogueActive)
            return;

        if (!grappleUnlocked)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (!firing && !attached)
            {
                audioManager.playGrappleShootSFX();
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

        Collider[] hits = Physics.OverlapSphere(hookPosition, enemyHitRadius);

        foreach (Collider hit in hits)
        {
            IEnemy enemy = hit.GetComponentInParent<IEnemy>();

            if (enemy != null)
            {
                // Mainkan suara hit musuh (Bisa dipisah nanti kalau punya SFX khusus musuh)
                if (audioManager != null) audioManager.playGrappleWallSFX();

                // Tanya AI musuhnya! 
                bool isEnemyPulled = enemy.OnGrappled();

                if (isEnemyPulled)
                {
                    // Tali lepas karena musuh (Normal/Fast) terbang ke arah kita
                    CancelHook();
                }
                else
                {
                    // MUSUH HEAVY/SHOOTER TERTANGKAP!
                    // Aktifkan mekanik Player melesat terbang (sama seperti narik tembok)
                    AttachHook(hit.transform.position);
                }
                return;
            }

            if (hit.CompareTag("Ground"))
            {
                if (audioManager != null) audioManager.playGrappleWallSFX();
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
        audioManager.playGrapplingSFX();
        Vector3 direction = anchorPoint - playerRb.position;
        direction.z = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();
        playerRb.AddForce(direction * pullForce, ForceMode.Acceleration);
    }

    public void UnlockGrapple()
    {
        grappleUnlocked = true;

        Debug.Log("GRAPPLE UNLOCKED!");
    }
}