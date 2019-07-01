using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    //Stats
    public int HP;
    public int MaxHP = 100;
    public int STR = 3;
    public int DEF = 2;
    public int STM = 2;
    public int Throwables = 0;
    
    //Movement
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;
    [Range(0, 1)]
    public float airControlPercent;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    Animator animator;
    Transform cameraT;
    CharacterController controller;

    void Start()
    {
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        HP = MaxHP;
    }

    void Update()
    {
        // input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        bool running = Input.GetKey(KeyCode.LeftShift);

        Move(inputDir, running);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.LogWarning("Light attack!");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.LogWarning("Heavy attack!");
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Debug.LogWarning("Bottlecap throw!");
        }
    }

    void Move(Vector2 inputDir, bool running)
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velocityY += Time.deltaTime * gravity;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
        {
            velocityY = 0;
        }

    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
        }
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Sword")
        {
            if (Input.GetButtonDown("Interact"))
            {
                other.transform.SetParent(this.transform);
                other.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z+3);
                Debug.LogWarning("Picked Sword up");
                Sword firstSword = GameObject.Find("SwordPH").GetComponent<Sword>();
                STR += firstSword.STRMod;
                firstSword.isEquipped = true;
                other.gameObject.SetActive(false);
            }
        }
    }
}
