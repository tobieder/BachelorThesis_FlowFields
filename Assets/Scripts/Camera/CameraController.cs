using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }

    public Transform m_CameraTransform;

    public float m_NormalSpeed;
    public float m_FastSpeed;

    public float m_MovementSpeed;
    public float m_MovementTime;
    public float m_RotationAmount;
    public Vector3 m_ZoomAmount;

    public float m_MinZoom;
    public float m_MaxZoom;

    private Vector3 m_NewPosition;
    private Quaternion m_NewRotation;
    private Vector3 m_NewZoom;

    private Vector3 m_RotateStartPosition;
    private Vector3 m_RotateCurrentPosition;

    private float m_MaxWidth;
    private float m_MaxHeight;

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
        m_NewPosition = transform.position;
        m_NewRotation = transform.rotation;
        m_NewZoom = m_CameraTransform.localPosition;

        Grid grid = GridCreator.s_Grid;
        m_MaxWidth = grid.GetWidth() * grid.GetCellSize();
        m_MaxHeight = grid.GetHeight() * grid.GetCellSize();
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
        m_NewPosition.x = Mathf.Clamp(m_NewPosition.x, 0.0f, m_MaxWidth);
        m_NewPosition.z = Mathf.Clamp(m_NewPosition.z, 0.0f, m_MaxHeight);

        transform.position = Vector3.Lerp(transform.position, m_NewPosition, Time.deltaTime * m_MovementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_NewRotation, Time.deltaTime * m_MovementTime);

        m_NewZoom.y = Mathf.Clamp(m_NewZoom.y, m_MinZoom, m_MaxZoom);
        m_NewZoom.z = Mathf.Clamp(m_NewZoom.z, -m_MaxZoom, -m_MinZoom);

        m_CameraTransform.localPosition = Vector3.Lerp(m_CameraTransform.localPosition, m_NewZoom, Time.deltaTime * m_MovementTime);
    }

    void HandleMouseInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            m_NewZoom += Input.mouseScrollDelta.y * m_ZoomAmount;
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(2))
            {
                m_RotateStartPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                m_RotateCurrentPosition = Input.mousePosition;

                Vector3 _difference = m_RotateStartPosition - m_RotateCurrentPosition;

                m_RotateStartPosition = m_RotateCurrentPosition;

                m_NewRotation *= Quaternion.Euler(Vector3.up * (-_difference.x / 5f));
            }
        }
    }

    void HandleMovementInput()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            m_MovementSpeed = m_FastSpeed;
        }
        else
        {
            m_MovementSpeed = m_NormalSpeed;
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            m_NewPosition += (transform.forward * m_MovementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            m_NewPosition += (transform.forward * -m_MovementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            m_NewPosition += (transform.right * m_MovementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_NewPosition += (transform.right * -m_MovementSpeed);
        }

        if(Input.GetKey(KeyCode.Q))
        {
            m_NewRotation *= Quaternion.Euler(Vector3.up * m_RotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            m_NewRotation *= Quaternion.Euler(Vector3.up * -m_RotationAmount);
        }

        if(Input.GetKey(KeyCode.R))
        {
            m_NewZoom += m_ZoomAmount;
        }
        if(Input.GetKey(KeyCode.F))
        {
            m_NewZoom -= m_ZoomAmount;
        }
    }
}
