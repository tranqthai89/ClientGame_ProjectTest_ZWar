using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] Transform model;
    [SerializeField] Transform gunContainer;
    [SerializeField] SpriteRenderer radar;

    [Header("Guns")]
    [SerializeField] MachineGunController machineGun;

    [Header("Settings")]
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float speed = 5;
    [SerializeField] float rotSpeed = 5;
    [SerializeField] float gravity = -9.18f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float detectionRadius = 7.5f;
    [SerializeField] Color radarColor_Normal;
    [SerializeField] Color radarColor_Detecting;

    bool isGrounded;
    enum State{
        Idle, Walk
    }
    State state;
    Transform target;
    MainCharInfo mainCharInfo;
    GunController currentGun;

    void Start()
    {
        state = State.Idle;
        radar.transform.localScale = new Vector3(detectionRadius * 2, detectionRadius * 2, 1f);
        radar.color = radarColor_Normal;

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        Init(GameInformation.Instance.mainCharInfo);
        currentGun = machineGun;
    }

    void Init(MainCharInfo _mainCharInfo)
    {
        if (_mainCharInfo == null)
        {
            Debug.LogError("MainCharInfo is null!");
            return;
        }
        mainCharInfo = _mainCharInfo;
        speed = mainCharInfo.moveSpeed;
        rotSpeed = mainCharInfo.rotationSpeed;
        jumpHeight = mainCharInfo.jumpHeight;
        detectionRadius = mainCharInfo.detectionRadius;

        machineGun.Init(mainCharInfo.gunMachineInfo);
    }

    void Update()
    {
        target = FindNearestEnemy();

        // Lấy đầu vào từ bàn phím
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x == 0 && z == 0)
        {
            if (state != State.Idle)
            {
                animator.SetTrigger("Idle");
                state = State.Idle;
            }
        }
        else
        {
            if (state != State.Walk)
            {
                animator.SetTrigger("Walk");
                state = State.Walk;
            }
        }

        Vector3 _move = new Vector3(x, 0, z).normalized * speed;

        // Sử dụng Rigidbody để di chuyển nhân vật
        rb.velocity = new Vector3(_move.x, rb.velocity.y, _move.z);

        // Xoay nhân vật theo hướng di chuyển
        if(target != null)
        {
            FaceTarget();
            machineGun.Shoot();
            radar.color = radarColor_Detecting;
        }
        else if (_move != Vector3.zero)
        {
            Vector3 direction = _move.normalized;
            direction.y = 0; // Khóa trục X và Z, chỉ xoay theo trục Y
            model.forward = Vector3.Lerp(model.forward, direction, Time.deltaTime * rotSpeed);
            gunContainer.forward = model.forward; // Cập nhật hướng của spawner theo hướng của model
            radar.color = radarColor_Normal;
        }
        else
        {
            radar.color = radarColor_Normal;
        }

        // Xử lý nhảy
        isGrounded = IsGrounded();
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * gravity), ForceMode.Impulse);
        }
        // Debug.Log(move + " - " + speed + " - " + isGrounded + " - " + velocity);
    }
    public bool IsGrounded()
    {
        // Kiểm tra bằng Raycast trước (nhanh hơn và ít tốn tài nguyên)
        // if (Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer))
        // {
        //     return true;
        // }
        return Physics.CheckSphere(transform.position, 0.4f, obstacleLayer);

        // Nếu Raycast không phát hiện, kiểm tra bằng OverlapSphere nhưng chỉ với groundLayer
        // Collider[] colliders = Physics.OverlapSphere(transform.position, 0.4f, groundLayer);
        // return colliders.Length > 0;

        // return Physics.CheckSphere(transform.position + Vector3.down * 2f, 0.4f, groundLayer);
        // return Physics.CheckSphere(transform.position + Vector3.down * 1f, 0.4f);
        // return Physics.Raycast(transform.position, Vector3.down, 1.1f);
        // Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        // foreach (Collider collider in colliders)
        // {
        //     if (collider.gameObject.CompareTag("Ground") || collider.gameObject.CompareTag("Obstacle")) // Kiểm tra nếu là mặt đất
        //     {
        //         return true;
        //     }
        // }
        // return false;
    }
    void FaceTarget()
    {
        if (target != null)
        {
            Vector3 _direction = (target.position - transform.position).normalized;
            if(_direction != Vector3.zero)
            {
                _direction.y = 0; // Khóa trục X và Z, chỉ xoay theo trục Y
                Quaternion _lookRotation = Quaternion.LookRotation(_direction);
                model.rotation = Quaternion.Lerp(model.rotation, _lookRotation, Time.deltaTime * rotSpeed);
                gunContainer.rotation = model.rotation; // Cập nhật hướng của spawner theo hướng của model
            }
        }
    }
    public Transform FindNearestEnemy()
    {
        Collider[] _colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        Transform _nearestEnemy = null;
        float _minDistance = Mathf.Infinity;

        foreach (Collider _collider in _colliders)
        {
            // float distance = (_collider.transform.position - transform.position).sqrMagnitude;
            // if (distance < _minDistance)
            // {
            //     _minDistance = distance;
            //     _nearestEnemy = _collider.transform;
            // }
            Vector3 _directionToTarget = (_collider.transform.position - transform.position).normalized;
            float _distanceToTarget = Vector3.Distance(transform.position, _collider.transform.position);

            // Kiểm tra xem có vật cản giữa nhân vật và mục tiêu không
            if (!Physics.Raycast(transform.position, _directionToTarget, _distanceToTarget, obstacleLayer))
            {
                if (_distanceToTarget < _minDistance)
                {
                    _minDistance = _distanceToTarget;
                    _nearestEnemy = _collider.transform;
                }
            }
        }

        return _nearestEnemy;
    }
    // void OnTriggerEnter(Collider other) {
    //     Debug.Log("OnTriggerEnter | Va chạm với: " + other.gameObject.name);
    // }
    // void OnCollisionEnter(Collision collision) {
    //     Debug.Log("OnCollisionEnter | Va chạm với: " + collision.gameObject.name);
    // }
}