using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviour
{
    PhotonView pv;

    [SerializeField] float moveSpeed;
    [SerializeField] float sprintMultiplier = 2;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask jumpMask;
    public Transform camLoc;

    float currentSprintMultiplier = 1;
    bool isGrounded;

    Animator anim;
    Rigidbody rb;

    float inputX, inputZ;
    Vector3 movementVector;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        HudManager.Instance.SetUpMenu();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, .2f);
    }
    private void OnDisable()
    {
        if (!pv.IsMine)
            return;
        anim.SetFloat("X", 0);
        anim.SetFloat("Z", 0);
    }
    private void Update()
    {
        if (!pv.IsMine)
            return;

        Inputs();

        anim.SetFloat("X", inputX * currentSprintMultiplier);
        anim.SetFloat("Z", inputZ * currentSprintMultiplier);

        movementVector = new Vector3(inputX, 0, inputZ);
        Vector3.ClampMagnitude(movementVector, 1);
    }
    void Inputs()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        if (Input.GetButton("Sprint"))
        {
            currentSprintMultiplier = sprintMultiplier;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            currentSprintMultiplier = 1;
        }
        isGrounded = Physics.CheckSphere(transform.position, .2f, jumpMask);
        if (Input.GetButtonDown("Jump"))
        {
            if(isGrounded)
                rb.AddForce(new Vector3(0, jumpForce, 0));
        }
    }
    private void FixedUpdate()
    {
        if (!pv.IsMine)
            return;

        float calcMultiplier = moveSpeed * currentSprintMultiplier * Time.deltaTime;
        transform.Translate(movementVector.x * calcMultiplier, 0, movementVector.z * calcMultiplier);
    }
}