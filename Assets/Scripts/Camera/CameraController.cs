using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }

    public Transform cameraTransform;

    public float normalSpeed;
    public float fastSpeed;

    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public float minZoom;
    public float maxZoom;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;

    private Grid grid;
    private float maxWidth;
    private float maxHeight;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;

        grid = GridCreator.grid;
        maxWidth = grid.GetWidth() * grid.GetCellSize();
        maxHeight = grid.GetHeight() * grid.GetCellSize();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
        HandleInput(); 
    }

    void HandleInput()
    {
        newPosition.x = Mathf.Clamp(newPosition.x, 0.0f, maxWidth);
        newPosition.z = Mathf.Clamp(newPosition.z, 0.0f, maxHeight);

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);

        newZoom.y = Mathf.Clamp(newZoom.y, minZoom, maxZoom);
        newZoom.z = Mathf.Clamp(newZoom.z, -maxZoom, -minZoom);

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

    void HandleMouseInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(2))
            {
                rotateStartPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                rotateCurrentPosition = Input.mousePosition;

                Vector3 _difference = rotateStartPosition - rotateCurrentPosition;

                rotateStartPosition = rotateCurrentPosition;

                newRotation *= Quaternion.Euler(Vector3.up * (-_difference.x / 5f));
            }
        }
    }

    void HandleMovementInput()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        if(Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if(Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if(Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }
    }
}
