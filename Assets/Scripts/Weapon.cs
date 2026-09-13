using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [Header("Shooting")]
[SerializeField] private float Range = 100.0f;
[SerializeField] private LayerMask LayerMask;

[SerializeField] private bool IsSemiAutomatic = false;

[SerializeField] private float ShootDelay = 0.3f;
private float nextShootTime;

[SerializeField] private int Damage = 20;

private Camera mainCamera;
private InputAction shootAction;

[Header("Visuals and sound")]
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
    shootAction = InputSystem.actions.FindAction("Attack");
    audioSource = GetComponent<AudioSource>();

    startPosition = transform.localPosition;
    startRotation = transform.localRotation;

    shootAction.started += OnAttackButtonPressed;
}

private void Update()
{
    if (!IsSemiAutomatic && shootAction.IsPressed())
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
        shootAction.started -= OnAttackButtonPressed;
}

private void Shoot()
{
    if (Time.time < nextShootTime)
        return;

    PlayShootEffects();
    nextShootTime = Time.time + ShootDelay;

    RaycastHit hit;
    if (!Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Range, LayerMask))
        return;

    GameObject impact = Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
    Destroy(impact, 1.0f);

    if (hit.collider.TryGetComponent(out Target target))
        target.TakeDamage(Damage);
}

private void PlayShootEffects()
{
    audioSource.pitch = Random.Range(0.95f, 1.05f);
    audioSource.PlayOneShot(ShootSound);
    MuzzleFlash.Play();

    transform.localPosition -= new Vector3(0, 0, RecoilKickBack);
    transform.localRotation *= Quaternion.Euler(-RecoilKickUp, 0, 0);
}
}