using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("Death Settings")]
    public float timeBeforeDeath = 300f; // 5 minutes

    [Header("References")]
    public Animator animator;
    public Rigidbody playerRigidbody;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private float timer;
    private bool isDead;

    void Awake()
    {
        if (!animator)
            animator = GetComponentInChildren<Animator>();

        if (!playerRigidbody)
            playerRigidbody = GetComponent<Rigidbody>();

        ragdollBodies = animator.GetComponentsInChildren<Rigidbody>();
        ragdollColliders = animator.GetComponentsInChildren<Collider>();

        SetRagdoll(false);
    }

    void Update()
    {
        if (isDead) return;

        timer += Time.deltaTime;

        if (timer >= timeBeforeDeath)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Disable movement & animation
        if (TryGetComponent(out PlayerMovement movement))
            movement.enabled = false;

        animator.enabled = false;

        // Disable player root physics
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.isKinematic = true;

        // Activate ragdoll
        SetRagdoll(true);
    }

    void SetRagdoll(bool active)
    {
        foreach (var rb in ragdollBodies)
        {
            if (rb == playerRigidbody) continue;
            rb.isKinematic = !active;
        }

        foreach (var col in ragdollColliders)
        {
            if (col.transform == transform) continue;
            col.enabled = active;
        }
    }
}
