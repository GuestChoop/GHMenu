using UnityEngine;

public class Fly
{
    public static void PlayerFly()
    {
        float speedfly = InputsManager.Get().IsActionActive(InputsManager.InputAction.Sprint) ? 0.5f : 0.25f;
        var playerposition = Player.Get().transform;
        var playercam = CameraManager.Get().m_MainCamera.transform;
        if (InputsManager.s_Instance.IsActionActive(InputsManager.InputAction.Jump))
        {
            playerposition.position += playerposition.up + playercam.up * Time.deltaTime;
        }
        if (InputsManager.s_Instance.IsActionActive(InputsManager.InputAction.Forward))
        {
            //playerposition.position += playercam.forward + playerposition.forward * Time.deltaTime;
            playerposition.position = new Vector3(playerposition.position.x + playercam.forward.x * speedfly, playerposition.position.y + playercam.forward.y * speedfly,
                playerposition.position.z + playercam.forward.z * speedfly);
            //Player.Get().m_CharacterController.Move(moveFoward, true);
            //playerposition.position = moveFoward;
        }
        if (InputsManager.s_Instance.IsActionActive(InputsManager.InputAction.Backward))
        {
            //playerposition.position -= playercam.forward + playerposition.forward * Time.deltaTime;
            playerposition.position = new Vector3(playerposition.position.x - playercam.forward.x * speedfly, playerposition.position.y - playercam.forward.y * speedfly,
                playerposition.position.z - playercam.forward.z * speedfly);
            //Player.Get().m_CharacterController.Move(moveBack, true);
            //playerposition.position = moveBack;
        }
        if (InputsManager.s_Instance.IsActionActive(InputsManager.InputAction.Right))
        {
            //playerposition.position += playercam.right * speedfly * Time.unscaledDeltaTime + playerposition.right * InputsManager.Get().GetActionValue(InputsManager.InputAction.Right);
            playerposition.position = new Vector3(playerposition.position.x + playercam.right.x * speedfly, playerposition.position.y, playerposition.position.z + playercam.right.z * speedfly);
            //Player.Get().m_CharacterController.Move(moveRight, true);
            //playerposition.position = moveRight;
        }
        if (InputsManager.s_Instance.IsActionActive(InputsManager.InputAction.Left))
        {
            //playerposition.position -= playercam.right * speedfly * Time.unscaledDeltaTime + playerposition.right * InputsManager.Get().GetActionValue(InputsManager.InputAction.Left);
            playerposition.position = new Vector3(playerposition.position.x - playercam.right.x * speedfly, playerposition.position.y, playerposition.position.z - playercam.right.z * speedfly);
            //Player.Get().m_CharacterController.Move(moveLeft, true);
            //playerposition.position = moveLeft;
        }
    }
}
