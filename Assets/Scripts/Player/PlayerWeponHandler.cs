using System.Collections.Generic;
using UnityEngine;

public class PlayerWeponHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> weaponPrefabs;
    [SerializeField] private bool usingMobileControls = true;
    [SerializeField] FixedJoystick fireVirtualJoystick; // For mobile controls, if needed
    public Vector2 fireDirection { get; private set; }
    public float joystickMagnitude { get; private set; } // For mobile controls, if needed
    GameObject currentWeapon;
    SpriteRenderer weaponSpriteRenderer;

    private void Awake()
    {
        //TEMP, use the first weapon prefab in the list
        if (weaponPrefabs.Count > 0)
        {
            currentWeapon = Instantiate(weaponPrefabs[0], transform.position + new Vector3(0.1f, 0, 0), Quaternion.identity);
            currentWeapon.transform.SetParent(transform);
            weaponSpriteRenderer = currentWeapon.GetComponentInChildren<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("No weapon prefabs assigned to PlayerWeaponHandler.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        fireDirection = Vector2.zero;
        joystickMagnitude = 0f; // Reset joystick magnitude
        if (usingMobileControls)
        {
            // Use the virtual joystick for firing direction
            fireDirection = new Vector2(fireVirtualJoystick.Horizontal, fireVirtualJoystick.Vertical);
            joystickMagnitude = fireDirection.magnitude; // Update joystick magnitude, range [0, 1]
            Debug.Log($"Joystick Direction: {fireDirection}, Magnitude: {joystickMagnitude}");
            if (fireDirection.magnitude < 0.01f)
            {
                fireDirection = Vector2.zero; // Prevent small movements
            }
        }
        else
        {
            // Use mouse position for firing direction
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            fireDirection = (mousePosition - (Vector2)transform.position).normalized;
        }

        // Fire the weapon in the calculated direction
        if (fireDirection != Vector2.zero)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg);
            if(transform.rotation.eulerAngles.z >90f && transform.rotation.eulerAngles.z < 270f)
            {
                weaponSpriteRenderer.flipY = true; // Flip the weapon sprite if facing downwards
            }
            else
            {
                weaponSpriteRenderer.flipY = false; // Reset flip if facing upwards
            }
        }
    }
}
