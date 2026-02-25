using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class CycleController : MonoBehaviour
{
    public enum PlayerID
    {
        Player1,
        Player2,
        Player3,
        Player4
    }

    public PlayerID player;

    public float moveSpeed = 12f;
    public float turnSpeed = 120f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Do not move before race starts
        if (!GameManager.GameStarted)
            return;

        // Do not move while typing in UI InputField
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null)
            return;

        float move = 0f;
        float turn = 0f;

        switch (player)
        {
            case PlayerID.Player1:   // WASD
                move = Input.GetKey(KeyCode.W) ? 1 :
                       Input.GetKey(KeyCode.S) ? -1 : 0;

                turn = Input.GetKey(KeyCode.A) ? -1 :
                       Input.GetKey(KeyCode.D) ? 1 : 0;
                break;

            case PlayerID.Player2:   // Arrow Keys
                move = Input.GetKey(KeyCode.UpArrow) ? 1 :
                       Input.GetKey(KeyCode.DownArrow) ? -1 : 0;

                turn = Input.GetKey(KeyCode.LeftArrow) ? -1 :
                       Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
                break;

            case PlayerID.Player3:   // IJKL
                move = Input.GetKey(KeyCode.I) ? 1 :
                       Input.GetKey(KeyCode.K) ? -1 : 0;

                turn = Input.GetKey(KeyCode.J) ? -1 :
                       Input.GetKey(KeyCode.L) ? 1 : 0;
                break;

            case PlayerID.Player4:   // TFGH
                move = Input.GetKey(KeyCode.T) ? 1 :
                       Input.GetKey(KeyCode.G) ? -1 : 0;

                turn = Input.GetKey(KeyCode.F) ? -1 :
                       Input.GetKey(KeyCode.H) ? 1 : 0;
                break;
        }

        // Forward movement
        Vector3 velocity = transform.forward * move * moveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        // Turning
        rb.MoveRotation(rb.rotation *
            Quaternion.Euler(0, turn * turnSpeed * Time.fixedDeltaTime, 0));
    }
}