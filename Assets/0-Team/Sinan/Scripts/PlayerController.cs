using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviour
{
    [Header("Movement stuff")]
    public float moveSpeed = 15f;
    public float moveMultiplier = 10f;
    public float jumpHeight = 5;
    public float runSpeed = 1;
    public float groundDrag;
    public float airDrag;

    [Header("Boolien checks")]
    public bool isGrounded = false;
    public bool isJumping = false;
    public bool isCrouching = false;

    [Header("Collision stuff")]
    public float sphereRadius = 0.4f;
    public float sphereMaxDistance;
    public LayerMask groundMask;

    [Header("Weapon Manager")]
    public WeaponManager weaponManager;

    [Header("Misc")]
    public Transform playerCamera;
    public float sensitivity = 1f;
    public CapsuleCollider playerCollider;//Called from the health script.
    public bool isPlayerInstance;


    private Animator animator;
    private Rigidbody rb;
    private float mouseX, mouseY;
    private float hor, ver, fire;
    private bool isMoving;
    private RaycastHit slopeHit;
    private Vector3 slopeMoveDirection;
    private Vector3 moveDirection;
    private int indexSelector;
    private PhotonView photonView;


    private void Awake() {
        photonView = GetComponent<PhotonView>();
        isPlayerInstance = photonView.IsMine;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        weaponManager = GetComponentInChildren<WeaponManager>();
    }
    private void Update()
    {
        if (isPlayerInstance) {
            // SprintFunction();
            // CrouchFunction();
            // MovementInput();
            // JumpFunction();
            // ControlDrag();
            // GroundDetectionSphereCast();
            CameraMovement();
            // slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

            hor = Input.GetAxis("Horizontal");
            ver = Input.GetAxis("Vertical");
            transform.position+=(transform.forward * ver * moveSpeed + transform.right * hor * moveSpeed) * Time.deltaTime;

            Fire();

        }
    }
    // private void FixedUpdate()
    // {
    //     Movement();
    // }
    private void GroundDetectionSphereCast()
    {
        //This checks if the player is grounded, if the player is grounded they can jump again
        if (!isGrounded)
        {
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, sphereRadius, Vector3.down, out hit, sphereMaxDistance))
            {
                if (hit.collider.tag == "Ground" || hit.collider.tag == "Phys" || hit.collider.tag == "HeavyPhys" || hit.collider.tag == "Slope")
                {
                    Debug.Log(hit.collider.tag);
                    isGrounded = true;
                    isJumping = false;
                }
            }
        }
    }

    private bool onSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down,out slopeHit, playerCollider.height / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void MovementInput() 
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        //This calculates move direction
        moveDirection = transform.forward * ver + transform.right * hor;

    }
    private void Movement()
    {
        //This is used for WASD movement
        if (!isJumping && !onSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * runSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        if (!isJumping && onSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * runSpeed*moveMultiplier,ForceMode.Acceleration);
        }
    }

    private void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        if (!isGrounded)
        {
            rb.drag = airDrag;
        }
    }
    private void SprintFunction()
    {
        //Sprint function
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && !isJumping)
        {
            runSpeed = 2;
            if (isJumping)
            {
                runSpeed = 2;
            }
        }

        else
        {
            runSpeed = 1;
        }
    }

    private void CrouchFunction()
    {
        if (Input.GetKey(KeyCode.LeftControl) && !isJumping)
        {
            playerCollider.height = 0.5f;
            moveSpeed = 5;
            isCrouching = true;
            isJumping = false;
        }
        else
        {
            playerCollider.height = 2;
            isCrouching = false;
        }
    }

    private void JumpFunction()
    {
        //Jump function
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping &&!isCrouching)
        {
            isJumping = true;
            isGrounded = false;
            rb.velocity += jumpHeight * Vector3.up;
        }
    }
    private void CameraMovement()
    {
        if (playerCamera) {
            mouseX += Input.GetAxis("Mouse X") * sensitivity;
            mouseY += Input.GetAxis("Mouse Y") * sensitivity;
            mouseY = Mathf.Clamp(mouseY, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, mouseX, 0f);
        }
    }
    //We use this to check if the player is falling
    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.tag == "Ground")
    //     {
    //         isGrounded = false;
    //         isJumping = true;
    //     }
    // }

    public void Fire() {
        fire = Input.GetAxis("Fire1");
        if (fire>0) {
            weaponManager.Fire();
            photonView.RPC("FireOnClones", RpcTarget.Others);
        }
    }

    [PunRPC]
    private void FireOnClones() {
        weaponManager.Fire();
    }


}