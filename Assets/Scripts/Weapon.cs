using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private float Range = 100.0f;
    [SerializeField] private LayerMask LayerMask;

    [SerializeField] private bool IsSemiAutomatic = false;

    [SerializeField] private float ShootDelay = 0.3f;
    private float nextShootTime;

    private Camera mainCamera;
    private InputAction inputAction;

    [SerializeField] private AudioClip ShootSound;
    private AudioSource audioSource;

    [SerializeField] private ParticleSystem MuzzleFlash;

    [SerializeField] private GameObject ImpactEffect;

    [SerializeField] private float RecoilKickBack = 0.08f;
    [SerializeField] private float RecoilKickUp = 6.0f;
    [SerializeField] private float RecoilReturnSpeed = 8.0f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        mainCamera = GetComponentInParent<Camera>();
        inputAction = InputSystem.actions.FindAction("Attack");
        audioSource = GetComponent<AudioSource>();

        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        inputAction.started += OnAttackButtonPressed;
    }

    private void Update()
    {
        if (!IsSemiAutomatic && inputAction.IsPressed())
            Shoot();

        transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, RecoilReturnSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, RecoilReturnSpeed * Time.deltaTime);
    }

    private void OnAttackButtonPressed(InputAction.CallbackContext _)
    {
        if (IsSemiAutomatic)
            Shoot();
    }

    private void OnDestroy()
    {
        inputAction.started -= OnAttackButtonPressed;
    }

    private void Shoot()
    {
        if (Time.time < nextShootTime)
            return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(ShootSound);
        MuzzleFlash.Play();

        transform.localPosition -= new Vector3(0, 0, RecoilKickBack);
        transform.localRotation *= Quaternion.Euler(-RecoilKickUp, 0, 0);

        nextShootTime = Time.time + ShootDelay;

        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Range, LayerMask))
        {
            GameObject impact = Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 1.0f);

            if (hit.collider.TryGetComponent(out Target target))
            {
                Debug.Log($"Hit target {target.name}");
                target.TakeDamage(20);
            }
        }
    }
}
