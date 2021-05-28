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

    [Header("Weapon Related")]
    public WeaponManager weaponManager;
    public Transform rifleOnUseDummy, weaponRestPositionDummy;

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
    [SerializeField] private bool isUsingWeapon;
    [SerializeField] private bool isUsingRifle;
    public bool doFire;


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
            // CameraMovement();
            // slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

            hor = Input.GetAxis("Horizontal");
            ver = Input.GetAxis("Vertical");
            fire = Input.GetAxis("Fire1");

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                if (weaponManager!=null) {
                    if (!isUsingRifle) {
                        isUsingRifle = true;
                        isUsingWeapon = true;
                        animator.SetBool("isUsingRifle", true);
                        weaponManager.transform.SetParent(rifleOnUseDummy);
                        weaponManager.transform.localPosition = Vector3.zero;
                        weaponManager.transform.localRotation = Quaternion.identity;
                        
                    } else {
                        isUsingRifle = false;
                        isUsingWeapon = false;
                        animator.SetBool("isUsingRifle", false);
                        weaponManager.transform.SetParent(weaponRestPositionDummy);
                        weaponManager.transform.localPosition = Vector3.zero;
                        weaponManager.transform.localRotation = Quaternion.identity;

                    }
                } else {
                    Debug.Log("You don't have any rifle to use");
                }
            }

            moveMultiplier = 1;
            if (Input.GetKey(KeyCode.LeftShift)) {
                moveMultiplier = 2;
            }

            // transform.position+=(transform.forward * ver * moveSpeed + transform.right * hor * moveSpeed) * Time.deltaTime;
            animator.SetFloat("vertical", ver * moveMultiplier);
            animator.SetFloat("horizontal", hor * moveMultiplier);

            mouseX += Input.GetAxis("Mouse X") * sensitivity;
            transform.rotation = Quaternion.Euler(0f, mouseX, 0f);

            if (fire>0) {
                doFire=true;
            }

            if (doFire) {
                Fire();
            }
        } else {
            if (doFire) {
                Fire();
            }
        }
    }
    // private void FixedUpdate()
    // {
    //     Movement();
    // }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(isUsingRifle);
            stream.SendNext(isUsingWeapon);
            stream.SendNext(doFire);

        } else if (stream.IsReading) {
            isUsingRifle = (bool) stream.ReceiveNext();
            isUsingWeapon = (bool) stream.ReceiveNext();
            doFire = (bool) stream.ReceiveNext();
        }
    }

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
    // private void CameraMovement()
    // {
    //     if (playerCamera) {
    //         if (playerCamera.parent!=null) {
    //             playerCamera.SetParent(null);
    //         }
    //         playerCamera.position = -GetComponent<PlayerCameraAssigner>().cameraPositionDummy.forward * 3;
    //         mouseX += Input.GetAxis("Mouse X") * sensitivity;
    //         mouseY += Input.GetAxis("Mouse Y") * sensitivity;
    //         mouseY = Mathf.Clamp(mouseY, -90f, 90f);
    //         playerCamera.transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
    //         transform.rotation = Quaternion.Euler(0f, mouseX, 0f);
    //     }
    // }
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
        if (weaponManager!=null && isUsingWeapon) {
            weaponManager.Fire();
            doFire = false;
            photonView.RPC("FireOnClones", RpcTarget.Others);
        }
    }

    [PunRPC]
    private void FireOnClones() {
        weaponManager.Fire();
    }


}