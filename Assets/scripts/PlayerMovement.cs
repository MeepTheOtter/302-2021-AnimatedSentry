using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private CharacterController pawn;
    public float walkSpeed = 5;

    public Transform leg1;
    public Transform leg2;

    private Vector3 inputDirection = new Vector3();

    private float verticalVelocity = 0;
    private float horizontalVelocity = 0;

    public float gravityMult = 10;
    public float jumpMult = 5;

    private float coyoteTime = 0;

    public bool isGrounded
    {
        get
        { // return true if pawn is on the ground OR "coyote-time" isn't zero
            return pawn.isGrounded || coyoteTime > 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (coyoteTime > 0) coyoteTime -= Time.deltaTime;

        movePlayer();
        if (isGrounded) wiggleLegs(); // idle + walk
        else airLegs(); // jump / falling
    }

    private void airLegs()
    {
        leg1.localRotation = AnimMath.Slide(leg1.localRotation, Quaternion.Euler(30,0,0), .001f);
        leg2.localRotation = AnimMath.Slide(leg2.localRotation, Quaternion.Euler(-30, 0, 0), .001f);
    }

    private void wiggleLegs()
    {
        float degrees = 45;
        float speed = 10;

        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDirection);
        Vector3 axis = Vector3.Cross(inputDirLocal, Vector3.up);

        // check the alignment of inputDirLocal against forward vector
        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        alignment = Mathf.Abs(alignment);

        degrees *= AnimMath.Lerp(.25f, 1, alignment); //decrease 'degrees' when strafing

        
        float wave = Mathf.Sin(Time.time * speed) * degrees;        

        leg1.localRotation = AnimMath.Slide(leg1.localRotation, Quaternion.AngleAxis(wave, axis), .001f);
        leg2.localRotation = AnimMath.Slide(leg2.localRotation, Quaternion.AngleAxis(-wave, axis), .001f);
    }

    private void movePlayer()
    {
        float h = Input.GetAxis("Horizontal"); // strafing?
        float v = Input.GetAxis("Vertical"); // forward / backward

        bool isJumpHeld = Input.GetButton("Jump");
        bool onJumpPress = Input.GetButtonDown("Jump");


        bool isTryingToMove = (h != 0 || v != 0);
        if (isTryingToMove)
        {
            // turn to face the correct direction...
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), .02f);
        }

        inputDirection = transform.forward * v + transform.right * h;
        if (inputDirection.sqrMagnitude > 1) inputDirection.Normalize();

        //apply gravity
        verticalVelocity += gravityMult * Time.deltaTime;

        //adds lateral movement to vertical
        Vector3 moveDelta = inputDirection * walkSpeed + verticalVelocity * Vector3.down;

        //move pawn
        CollisionFlags flags = pawn.Move(moveDelta * Time.deltaTime);
        if (pawn.isGrounded)
        {
            verticalVelocity = 0; // on ground, zero out vertical velocity
            coyoteTime = .2f;
        }


        if (isGrounded)
        {            
            if (isJumpHeld)
            {
                verticalVelocity = -jumpMult;
                coyoteTime = 0;
            }
        }
    }
}
