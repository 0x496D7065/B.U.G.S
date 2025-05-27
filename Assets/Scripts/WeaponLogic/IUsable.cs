using UnityEngine;
using UnityEngine.InputSystem;

public interface IUsable
{
    void Action1(InputAction.CallbackContext context);
    void Action2();
    void Action3(InputAction.CallbackContext context);
}
