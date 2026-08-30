using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject ImpactParticles;

    private void Start()
    {
        Invoke(nameof(Impact), 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
            return;

        if (other.TryGetComponent(out Target target))
            target.TakeDamage(20);

        Impact();
    }

    private void Impact()
    {
        GameObject impactParticles = Instantiate(ImpactParticles, transform.position, Quaternion.identity);
        Destroy(impactParticles, 1.0f);

        Destroy(gameObject);
    }
}
