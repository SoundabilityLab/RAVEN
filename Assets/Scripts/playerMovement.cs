using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Rigidbody rBody;
    [SerializeField] float speed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float jumpForce;
    GameObject head;
    Vector2 mouseLook = Vector2.zero;
    [SerializeField] private bool inAir;

    public bool manualTurning = false;
    public float rotationSpeed = 50f;


    [SerializeField] float gravity = 9.8f;
    // Start is called before the first frame update
    void Start()
    {
        rBody = gameObject.GetComponent<Rigidbody>();
        head = GameObject.FindGameObjectWithTag("MainCamera");
        LockMouse();
        inAir = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        //transform.Translate(move);
        Vector3 v = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        if (v.magnitude > 1) v.Normalize();
        v *= speed;
        // rBody.velocity = new Vector3(0, rBody.velocity.y, 0);
        rBody.velocity = v;

        //Jumping
        if (Input.GetButtonDown("Jump") && !inAir)
        {
            inAir = true;
            rBody.AddForce(Vector3.up * jumpForce);
        }

        //Looking
        if (!manualTurning) 
        {
            // float prevX = mouseLook.x;
            // mouseLook += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            // mouseLook = new Vector2(mouseLook.x, Mathf.Clamp(mouseLook.y, -90 / mouseSensitivity, 90 / mouseSensitivity));
            // if (Cursor.lockState == CursorLockMode.Locked)
            // {
            //     transform.localRotation = Quaternion.AngleAxis((mouseLook.x - prevX) * mouseSensitivity, Vector3.up) * transform.localRotation;
            //     head.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y * mouseSensitivity, Vector3.right);
            // }
            float horizontal = 0f;
            float vertical = 0f;

            // WASD input
            if (Input.GetKey(KeyCode.A)) horizontal = -1f;
            if (Input.GetKey(KeyCode.D)) horizontal = 1f;
            if (Input.GetKey(KeyCode.W)) vertical = -1f; // Inverted for natural look-up/down
            if (Input.GetKey(KeyCode.S)) vertical = 1f;

            // Rotate camera
            transform.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, vertical * rotationSpeed * Time.deltaTime, Space.Self);
        }

        //Mouse
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     if (Cursor.visible)
        //         LockMouse();
        //     else
        //         UnlockMouse();
        // }

        // // re-lock the cursor whenever it's unlocked
        // if (Cursor.lockState != CursorLockMode.Locked)
        // {
        //     Debug.LogWarning($"Cursor unlocked at {Time.time} by:\n{StackTraceUtility.ExtractStackTrace()}");
        //     LockMouse();
        // }
    }

    private void OnCollisionEnter(Collision collision)
    {
        inAir = false;
    }

    public void LockMouse()
    {
        // Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetManualTurning(bool isFacing)
    {
        manualTurning = isFacing;
    }

    public void SyncMouseLook()
    {

        Vector3 bodyEuler = transform.localEulerAngles;
        Vector3 headEuler = head.transform.localEulerAngles;

        if (headEuler.x > 180f) 
            headEuler.x -= 360f;

        float pitch = headEuler.x;   
        float yaw   = bodyEuler.y;   


        float newX = yaw   / mouseSensitivity; 
        float newY = -pitch / mouseSensitivity;

        mouseLook = new Vector2(newX, newY);
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f / mouseSensitivity, 90f / mouseSensitivity);
    }

}
