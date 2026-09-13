using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float Smooth;
    [SerializeField] private float SwayMultiplier;

    private void Update()
    {
        float mouseX = Mouse.current.delta.x.ReadValue() * SwayMultiplier;
        float mouseY = Mouse.current.delta.y.ReadValue() * SwayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Smooth * Time.deltaTime);
    }
}
