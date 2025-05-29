using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;
    [SerializeField] Transform model;

    [Header("Settings")]
    [SerializeField] float speed = 5;
    [SerializeField] float rotSpeed = 5;
    [SerializeField] float gravity = -9.18f;
    [SerializeField] float jumpHeight = 3f;

    Vector3 velocity;
    bool isGrounded;
    enum State{
        Idle, Walk
    }
    State state;

    void Start()
    {
        state = State.Idle;
    }

    void Update()
    {
        // Di chuyển nhân vật
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if(x == 0 && z == 0){
            if(state != State.Idle){
                animator.SetTrigger("Idle");
                state = State.Idle;
            }
        }else{
            if(state != State.Walk){
                animator.SetTrigger("Walk");
                state = State.Walk;
            }
        }
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Xoay nhân vật theo hướng di chuyển
        if (move != Vector3.zero)
        {
            model.forward = Vector3.Lerp(model.forward, move.normalized, Time.deltaTime * rotSpeed);
        }
        
        // Xử lý nhảy
        isGrounded = IsGrounded();
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Debug.Log(move + " - " + speed + " - " + isGrounded + " - " + velocity);
    }
    public bool IsGrounded()
    {
        // return Physics.CheckSphere(transform.position + Vector3.down * 1f, 0.4f);
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
        // Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        // foreach (Collider collider in colliders)
        // {
        //     if (collider.gameObject.CompareTag("Ground")) // Kiểm tra nếu là mặt đất
        //     {
        //         return true;
        //     }
        // }
        // return false;
    }
    void OnTriggerEnter(Collider other) {
        Debug.Log("OnTriggerEnter | Va chạm với: " + other.gameObject.name);
    }
    void OnCollisionEnter(Collision collision) {
        Debug.Log("OnCollisionEnter | Va chạm với: " + collision.gameObject.name);
    }
}