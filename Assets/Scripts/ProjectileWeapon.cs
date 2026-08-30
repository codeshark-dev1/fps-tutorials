using UnityEngine;
using UnityEngine.InputSystem;
public class ProjectileWeapon : MonoBehaviour
{
    [SerializeField] private Transform FirePoint;
    [SerializeField] private float ShootForce;
    [SerializeField] private GameObject BulletPrefab;

    [SerializeField] private bool IsSemiAutomatic = false;

    [SerializeField] private float ShootDelay = 0.3f;
    private float nextShootTime;

    private InputAction shootAction;

    [SerializeField] private AudioClip AudioClip;
    private AudioSource audioSource;

    [SerializeField] private ParticleSystem MuzzleFlash;

    [SerializeField] private Transform ModelTransform;

    [SerializeField] private float RecoilKickBack = 0.08f;
    [SerializeField] private float RecoilKickUp = 6.0f;
    [SerializeField] private float RecoilReturnSpeed = 8.0f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        shootAction = InputSystem.actions.FindAction("Attack");
        shootAction.started += OnAttackButtonPressed;

        audioSource = GetComponent<AudioSource>();

        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    private void OnAttackButtonPressed(InputAction.CallbackContext _)
    {
        if (IsSemiAutomatic)
            Shoot();
    }

    private void Update()
    {
        if (!IsSemiAutomatic && shootAction.IsPressed())
            Shoot();

        transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, RecoilReturnSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, RecoilReturnSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + ShootDelay;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(AudioClip);
        MuzzleFlash.Play();

        GameObject newBullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        if (newBullet.TryGetComponent(out Rigidbody rb))
            rb.AddForce(ShootForce * FirePoint.forward * Time.fixedDeltaTime, ForceMode.Impulse);

        transform.localPosition -= new Vector3(0, 0, RecoilKickBack);
        transform.localRotation *= Quaternion.Euler(-RecoilKickUp, 0, 0);
    }

    private void OnDestroy()
    {
        shootAction.started -= OnAttackButtonPressed;
    }
}
