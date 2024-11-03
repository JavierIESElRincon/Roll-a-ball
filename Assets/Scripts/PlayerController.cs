using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public float thrust = 5;
    bool m_isGrounded;
    public Transform groundPoint;
    public LayerMask groundLayer;
    [Range(0,1)] public float groundDistance = 0.2f;

    [SerializeField] private GameObject resetPanel;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundPoint.position, new Vector3 (groundPoint.position.x, groundPoint.position.y * -1 * groundDistance, groundPoint.position.z));
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
        resetPanel.SetActive(false);
    }

    private void OnMove(InputValue movementValue)
    {   
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // Update runs once per frame. FixedUpdate can run once, zero, or several times per frame, depending on how many physics frames per second 
    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);
    }

    private void Update()
    {
        m_isGrounded = Physics.Raycast(groundPoint.position, new Vector3(groundPoint.position.x, groundPoint.position.y * -1 * groundDistance, groundPoint.position.z), 5, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && m_isGrounded)
        {
            Jump();
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
            winTextObject.SetActive(true);
            resetPanel.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You lose!";
        }
    }
    
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        
        Scene currentScene = SceneManager.GetActiveScene();

        if ( currentScene.buildIndex == 0)
        {
            if (count == 4)
            {
                LoadScene(1);
                Destroy(GameObject.FindGameObjectWithTag("Enemy"));
            }

        } else if (currentScene.buildIndex == 1) 
        {
            if (count == 9)
            {
                winTextObject.SetActive(true);
                Destroy(GameObject.FindGameObjectWithTag("Enemy"));
                resetPanel.SetActive(true);
                resetPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Play again";
            }
        }
    }

    public void Jump()
    {
        rb.AddForce(0, thrust, 0, ForceMode.Impulse);
    }

    public void LoadScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
    }

}
