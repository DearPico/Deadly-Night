using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DashAbility))]
public class DashAbilityInput : MonoBehaviour
{
    private DashAbility dashAbility;

    [Header("Input")]
    public InputActionReference dashAction; // Bind to "Dash" action

    private void Awake()
    {
        dashAbility = GetComponent<DashAbility>();
    }

    private void OnEnable()
    {
        if (dashAction != null)
            dashAction.action.performed += OnDashPerformed;

        dashAction?.action.Enable();
    }

    private void OnDisable()
    {
        if (dashAction != null)
            dashAction.action.performed -= OnDashPerformed;

        dashAction?.action.Disable();
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        dashAbility.TriggerDash(); // Appelle une méthode dans DashAbility
    }
}

