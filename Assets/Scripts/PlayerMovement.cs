using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float sidewaySpeed = 10f;
    [SerializeField] private float horizontalLimit = 3f;

    [Header("Input Settings")]
    [SerializeField] private float swipeSensitivity = 1f;
    [SerializeField] private float mouseSensitivity = 3f;
    [Range(0.1f, 2.0f)]
    [SerializeField] private float maxRawInputMagnitude = 1.0f;
    [Range(0.0f, 0.05f)]
    [SerializeField] private float inputDeadZone = 0.005f;

    [Header("Game Over Settings")]
    [SerializeField] private string obstacleTag = "Obstacle";

    [Header("Coin Settings")] 
    [SerializeField] private string coinTag = "Coin"; 

    private Rigidbody _rb;
    private float _currentHorizontalInput;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
        }
        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded())
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            return; 
        }

        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        GetRawInput();
        Vector3 newPosition = transform.position;
        newPosition.x += _currentHorizontalInput * sidewaySpeed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -horizontalLimit, horizontalLimit);
        transform.position = newPosition;
    }

    private void GetRawInput()
    {
        float rawInputThisFrame = 0f;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    rawInputThisFrame = touch.deltaPosition.x * swipeSensitivity;
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    rawInputThisFrame = Input.GetAxis("Mouse X") * mouseSensitivity;
                }
            }

        if (Mathf.Abs(rawInputThisFrame) < inputDeadZone)
        {
            _currentHorizontalInput = 0f;
        }
        else
        {
            _currentHorizontalInput = Mathf.Clamp(rawInputThisFrame, -maxRawInputMagnitude, maxRawInputMagnitude);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(obstacleTag))
        {
            Debug.Log("Game Over! Triggered with an obstacle: " + other.gameObject.name);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseGame();
            }
        }
        else if (other.gameObject.CompareTag(coinTag)) 
        {
            Debug.Log("Coin collected: " + other.gameObject.name);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectCoin();
            }
            Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-horizontalLimit, transform.position.y, transform.position.z),
                        new Vector3(horizontalLimit, transform.position.y, transform.position.z));
        Gizmos.DrawWireCube(transform.position, new Vector3(horizontalLimit * 2, 0.1f, 0.1f));
    }
}