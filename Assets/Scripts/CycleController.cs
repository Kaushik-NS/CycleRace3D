using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

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

    public float maxSpeed = 12f;
    public float acceleration = 8f;
    public float deceleration = 10f;
    public float turnSpeed = 120f;
    public Transform SeatMount;

    public TextMeshProUGUI WinnerText;
    public TextMeshProUGUI Winner;

    Rigidbody rb;
    float currentSpeed = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!GameManager.GameStarted)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null)
            return;

        float moveInput = 0f;
        float turnInput = 0f;

        switch (player)
        {
            case PlayerID.Player1:
                moveInput = Input.GetKey(KeyCode.W) ? 1 :
                            Input.GetKey(KeyCode.S) ? -1 : 0;

                turnInput = Input.GetKey(KeyCode.A) ? -1 :
                            Input.GetKey(KeyCode.D) ? 1 : 0;
                break;

            case PlayerID.Player2:
                moveInput = Input.GetKey(KeyCode.UpArrow) ? 1 :
                            Input.GetKey(KeyCode.DownArrow) ? -1 : 0;

                turnInput = Input.GetKey(KeyCode.LeftArrow) ? -1 :
                            Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
                break;

            case PlayerID.Player3:
                moveInput = Input.GetKey(KeyCode.I) ? 1 :
                            Input.GetKey(KeyCode.K) ? -1 : 0;

                turnInput = Input.GetKey(KeyCode.J) ? -1 :
                            Input.GetKey(KeyCode.L) ? 1 : 0;
                break;

            case PlayerID.Player4:
                moveInput = Input.GetKey(KeyCode.T) ? 1 :
                            Input.GetKey(KeyCode.G) ? -1 : 0;

                turnInput = Input.GetKey(KeyCode.F) ? -1 :
                            Input.GetKey(KeyCode.H) ? 1 : 0;
                break;
        }

        // Smooth acceleration / deceleration
        float targetSpeed = moveInput * maxSpeed;

        if (Mathf.Abs(targetSpeed) > 0.01f)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);

        // forward movement
        Vector3 velocity = transform.forward * currentSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        // Smooth turning 
        float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime * (Mathf.Abs(currentSpeed) / maxSpeed);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turnAmount, 0));
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(" Somethging hit..." + other.gameObject.tag);
        if (other.gameObject.tag == "FinishLine")
        {
            Winner.text = Winner.text + GetComponentInChildren<TMP_Text>(true).text;
            Debug.Log("The winner is " + GetComponentInChildren<TMP_Text>(true).text);
            GameManager.GameStarted = false;
            StartCoroutine(ShowWinner());
        }
    }

    IEnumerator ShowWinner()
    {
        Winner.gameObject.SetActive(true);
        WinnerText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        Winner.gameObject.SetActive(false);
        WinnerText.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
 