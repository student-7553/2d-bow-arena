using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private InputActionReference jumpAction;
    private InputActionReference dashAction;
    private InputActionReference movement2dAction;
    private InputActionReference shootAction;
    public Player player;

    public void init(PlayerInputs playerInputs)
    {
        jumpAction = playerInputs.jumpAction;
        dashAction = playerInputs.dashAction;
        movement2dAction = playerInputs.movement2dAction;
        shootAction = playerInputs.shootAction;

        jumpAction.action.performed += OnJumpAction;
        dashAction.action.performed += OnDashAction;
        movement2dAction.action.performed += OnMovementActionPerformed;
        movement2dAction.action.canceled += OnMovementActionCanceled;

        shootAction.action.performed += OnShootActionPerformed;
        shootAction.action.canceled += OnShootActionCanceled;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJumpAction;
        dashAction.action.performed -= OnDashAction;
        movement2dAction.action.performed -= OnMovementActionPerformed;
        movement2dAction.action.canceled -= OnMovementActionCanceled;

        shootAction.action.performed -= OnShootActionPerformed;
        shootAction.action.canceled -= OnShootActionCanceled;
    }

    public void OnJumpAction(InputAction.CallbackContext context)
    {
        bool isButtonDown = context.ReadValueAsButton();
        if (isButtonDown)
        {
            player.playerstate.handleJumpAction();
        }
        else
        {
            player.playerstate.handleJumpActionEnd();
        }
    }

    public void OnDashAction(InputAction.CallbackContext context)
    {
        player.playerstate.handleDashAction();
    }

    public void OnMovementActionPerformed(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        player.playerMovementHandler.handlePlayerDirectionInput(value);
    }

    public void OnMovementActionCanceled(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        player.playerMovementHandler.handlePlayerDirectionInput(value);
    }

    public void OnShootActionPerformed(InputAction.CallbackContext context)
    {
        player.playerstate.handleShootAction();
    }

    public void OnShootActionCanceled(InputAction.CallbackContext context)
    {
        player.playerstate.handleShootActionEnd();
    }
}
