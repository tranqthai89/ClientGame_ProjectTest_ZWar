using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalEnum;
using System;

public class MainCharController : CharController
{
    public override CharType charType => CharType.MainChar;

    [Header("Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] MainCharAnimationController myAnimation;
    [SerializeField] Transform model;
    [SerializeField] Transform gunContainer;
    [SerializeField] SpriteRenderer radar;

    [Header("HP Bar")]
    [SerializeField] CanvasGroup hpBarCanvasGroup; // CanvasGroup để điều chỉnh độ mờ của thanh máu
    [SerializeField] Canvas hpBarCanvas; 
    [SerializeField] Image hpBar_ImgFill; 

    [Header("Guns")]
    [SerializeField] MachineGunController machineGun;
    [SerializeField] MachineGunController gunCreateExplosion;

    [Header("Settings")]
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float gravity = -9.18f;
    [SerializeField] Color radarColor_Normal;
    [SerializeField] Color radarColor_Detecting;

    float speed;
    float rotSpeed;
    float jumpHeight;
    float detectionRadius;

    bool isGrounded;
    
    public MainCharState CurrentState{get;set;}
    public int CurrentDamage{
        get{
            if(CurrentGun != null)
            {
                return CurrentGun.GunValueDetail.bulletValueDetail.damage;
            }
            return 0; // Trả về 0 nếu không có súng hiện tại
        }
    }
    public float CurrentAtkSpeed{
        get{
            if(CurrentGun != null)
            {
                return CurrentGun.GunValueDetail.atkSpeed;
            }
            return 0f; // Trả về 0 nếu không có súng hiện tại
        }
    }
    public override bool IsRunning
    {
        get{
            return process_RunBehavior != null;
        }
    }
    EnemyController target;
    public MainCharInfo MyCharInfo{get;set;} // Thông tin về nhân vật chính
    public GunController CurrentGun{get;set;}
    public int IndexGunSelected{get;set;} // Chỉ số súng hiện tại được chọn
    public DateTime timeToSwitchGun; // Thời gian để chuyển đổi súng

    int indexSfxStep = 0; // Biến để đếm số lần phát âm thanh bước chân
    int countSteps = 0; // Biến đếm số bước đi

    IEnumerator process_RunBehavior, process_Die, process_UpdateHPBar;

    void Awake()
    {
        ResetData();
    }
    public override void StopAllActionNow()
    {
        base.StopAllActionNow();
        process_RunBehavior = null;
        process_UpdateHPBar = null;
        process_Die = null;
    }
    public override void ResetData()
    {
        base.ResetData();
        CurrentState = MainCharState.Idle;
        MyCharInfo = null;
        target = null;
        isGrounded = false;
        hpBarCanvasGroup.alpha = 0f; // Đặt độ mờ của thanh máu về 100%

        myAnimation.SetAnimByState(MainChar_StateAnimation.IdleAndMove);
        myAnimation.animator.SetFloat("Blend_Speed", 0f);

        if (radar != null)
        {
            radar.color = radarColor_Normal;
            radar.transform.localScale = Vector3.one; // Đặt kích thước ban đầu
        }
        model.rotation = Quaternion.identity; // Đặt hướng ban đầu của mô hình

        countSteps = 0; // Đặt lại số bước đi
        timeToSwitchGun = DateTime.UtcNow; // Đặt thời gian chuyển đổi súng ban đầu
    }

    public void Init(MainCharInfo _mainCharInfo)
    {
        if (_mainCharInfo == null)
        {
            Debug.LogError("MainCharInfo is null!");
            return;
        }
        MyCharInfo = _mainCharInfo;

        CurrentHp = _mainCharInfo.maxHp;
        speed = MyCharInfo.moveSpeed;
        rotSpeed = MyCharInfo.rotationSpeed;
        jumpHeight = MyCharInfo.jumpHeight;
        detectionRadius = MyCharInfo.detectionRadius;

        machineGun.Init(MyCharInfo.gunMachineValueDetail);
        gunCreateExplosion.Init(MyCharInfo.gunCreateExplosionValueDetail);
        SwitchMachinGun(); // Mặc định chọn súng máy
        
        radar.transform.localScale = new Vector3(detectionRadius * 2, detectionRadius * 2, 1f);
        radar.color = radarColor_Normal;

        countSteps = 0; // Đặt lại số bước đi

        RefreshHpBar();

        IsInstalled = true;
    }
    
    #region Behaviors
    public override void Run(){
        if(!IsInstalled){
            return;
        }
        if(process_RunBehavior != null){
            return;
        }

        base.Run();

        process_RunBehavior = DoProcess_Behavior();
        StartCoroutine(process_RunBehavior);

        process_UpdateHPBar = DoProcess_UpdateHPBar();
        StartCoroutine(process_UpdateHPBar);
    }
    private IEnumerator DoProcess_Behavior()
    {
        while(true)
        {
            if(CurrentState == MainCharState.Die || GamePlayManagerInstance.currentGameControl.currentState != GamePlayState.PlayGame)
            {
                break;;
            }
            yield return null;

            target = FindNearestEnemy();
            float _x = 0f;
            float _z = 0f;
            #if UNITY_EDITOR
                _x = Input.GetAxis("Horizontal");
                _z = Input.GetAxis("Vertical");
            #endif

            if(_x == 0f)
            {
                _x = GamePlayManagerInstance.UIManager.variableJoystick.Horizontal;
            }
            if(_z == 0f)
            {
                _z = GamePlayManagerInstance.UIManager.variableJoystick.Vertical;
            }
            // Vì chỉ có 2 animation là Idle và Move nên set Blend_Speed là 0 hoặc 1
            if (_x == 0 && _z == 0)
            {
                if (CurrentState != MainCharState.Idle)
                {
                    myAnimation.SetAnimByState(MainChar_StateAnimation.IdleAndMove);
                    myAnimation.animator.SetFloat("Blend_Speed", 0f); // Đặt tốc độ về 0 khi không di chuyển
                    CurrentState = MainCharState.Idle;
                }
                countSteps = 0; // Đặt lại số bước đi khi đứng yên
                indexSfxStep = 0; // Đặt lại chỉ số âm thanh bước chân
            }
            else
            {
                if (CurrentState != MainCharState.Move)
                {
                    myAnimation.SetAnimByState(MainChar_StateAnimation.IdleAndMove);
                    myAnimation.animator.SetFloat("Blend_Speed", 1f);
                    CurrentState = MainCharState.Move;
                }
                countSteps++;
                if(countSteps % 60 == 0) 
                {
                    MyAudioManager.Instance.PlaySfx(GameInformation.Instance.sfxListMainCharStep[indexSfxStep]);
                    indexSfxStep++;
                    if(indexSfxStep >= GameInformation.Instance.sfxListMainCharStep.Count)
                    {
                        indexSfxStep = 0; // Reset lại chỉ số âm thanh bước chân nếu đã đến cuối danh sách
                    }
                    countSteps = 0; // Đặt lại số bước đi
                }
            }

            Vector3 _move = new Vector3(_x, 0, _z).normalized * speed;

            // Sử dụng Rigidbody để di chuyển nhân vật
            rb.velocity = new Vector3(_move.x, rb.velocity.y, _move.z);

            // Xoay nhân vật theo hướng di chuyển
            if(target != null)
            {
                FaceTarget();
                CurrentGun.Shoot();
                radar.color = radarColor_Detecting;
            }
            else
            {
                if (_move != Vector3.zero)
                {
                    Vector3 _direction = _move.normalized;
                    _direction.y = 0; // Khóa trục X và Z, chỉ xoay theo trục Y
                    if(_direction != Vector3.zero)
                    {
                        model.forward = Vector3.Lerp(model.forward, _direction, Time.deltaTime * rotSpeed);
                        gunContainer.forward = model.forward; // Cập nhật hướng của spawner theo hướng của model
                    }
                }
                radar.color = radarColor_Normal;

                #if UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0)) // Kiểm tra nếu chuột trái được nhấn
                    {
                        CurrentGun.Shoot();
                    }
                #endif
            }

            // Xử lý nhảy
            isGrounded = IsGrounded();
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * gravity), ForceMode.Impulse);
            }
            // Debug.Log(move + " - " + speed + " - " + isGrounded + " - " + velocity);
        }
        process_RunBehavior = null;
    }
    private IEnumerator DoProcess_UpdateHPBar()
    {
        while(true){
            if(CurrentState == MainCharState.Die || GamePlayManagerInstance.currentGameControl.currentState != GamePlayState.PlayGame)
            {
                break;;
            }
            yield return new WaitForSeconds(0.1f);
            hpBarCanvas.worldCamera = Camera.main; // Đặt camera cho canvas hiển thị thanh máu
            hpBarCanvas.transform.LookAt(Camera.main.transform);
            hpBarCanvas.transform.Rotate(0, 180, 0); // Đảo ngược hướng nếu cần
        }
        process_UpdateHPBar = null;
    }
    
    protected void TakeDamage(int _damage)
    {
        if(!CanBeDamaged)
        {
            return; // Nếu không thể nhận sát thương, không làm gì cả
        }
        if(GamePlayManagerInstance.currentGameControl.currentState != GamePlayState.PlayGame){
            return;
        }
        MyAudioManager.Instance.PlaySfx(GameInformation.Instance.sfxMainCharHit);
        CurrentHp -= _damage;
        hpBarCanvasGroup.alpha = 1f; // Hiển thị thanh máu khi bị tấn công
        RefreshHpBar();
        if(CurrentHp <= 0)
        {
            SetUpDie(); // Gọi hàm Die nếu máu <= 0
        }
    }
    protected void SetUpDie()
    {
        if (CurrentState == MainCharState.Die)
        {
            return; // Nếu đã chết, không làm gì cả
        }
        process_Die = DoProcess_Die();
        StartCoroutine(process_Die);
    }
    IEnumerator DoProcess_Die()
    {
        CurrentState = MainCharState.Die;
        hpBarCanvasGroup.alpha = 0f;
        myAnimation.animator.SetFloat("Blend_Speed", 0f); 
        MyAudioManager.Instance.PlaySfx(GameInformation.Instance.sfxMainCharDie);

        OnDie?.Invoke(this); // Gọi callback khi chết

        //TODO: Diễn animation chết ...

        yield break;
    }
    #endregion

    #region  Others
    public void SwitchMachinGun(){
        if(!timeToSwitchGun.CheckIfItsTime()){return;}
        CurrentGun = machineGun;
        IndexGunSelected = 0; // Đặt chỉ số súng hiện tại là 0 (súng máy)
        timeToSwitchGun = DateTime.UtcNow.AddSeconds(1); // Cập nhật thời gian chuyển đổi súng
    }
    public void SwitchMissile(){
        if(!timeToSwitchGun.CheckIfItsTime()){return;}
        CurrentGun = gunCreateExplosion;
        IndexGunSelected = 1; // Đặt chỉ số súng hiện tại là 1 (súng gây nổ)
        timeToSwitchGun = DateTime.UtcNow.AddSeconds(1); // Cập nhật thời gian chuyển đổi súng
    }
    protected bool IsGrounded()
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
    protected void FaceTarget()
    {
        if (target != null)
        {
            Vector3 _direction = (target.PosOfDetect - transform.position).normalized;
            _direction.y = 0; // Khóa trục X và Z, chỉ xoay theo trục Y

            if(_direction != Vector3.zero)
            {
                model.forward = Vector3.Lerp(model.forward, _direction, Time.deltaTime * rotSpeed);
                gunContainer.forward = model.forward;
                // Quaternion _lookRotation = Quaternion.LookRotation(_direction);
                // model.rotation = Quaternion.Lerp(model.rotation, _lookRotation, Time.deltaTime * rotSpeed);
                // gunContainer.rotation = model.rotation; // Cập nhật hướng của spawner theo hướng của model
            }
        }
    }
    protected EnemyController FindNearestEnemy()
    {
        Collider[] _colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        EnemyController _nearestEnemy = null;
        float _minDistance = Mathf.Infinity;
    
        foreach (Collider _collider in _colliders)
        {
            EnemyController _enemyController = _collider.transform.parent.GetComponent<EnemyController>();
            if(_enemyController == null || _enemyController.CurrentState == EnemyState.Die)
            {
                continue; // Bỏ qua nếu không phải là EnemyController hoặc đã chết
            }
            // float distance = (_collider.transform.position - transform.position).sqrMagnitude;
            // if (distance < _minDistance)
            // {
            //     _minDistance = distance;
            //     _nearestEnemy = _collider.transform;
            // }
            Vector3 _directionToTarget = (_enemyController.PosOfDetect - PosOfDetect).normalized;
            float _distanceToTarget = Vector3.Distance(PosOfDetect, _enemyController.PosOfDetect);

            float _heightDifference = Mathf.Abs(_enemyController.PosOfDetect.y - PosOfDetect.y);
            // float _angleToTarget = Math.Abs(Vector3.Angle(model.forward, _directionToTarget));

            // Kiểm tra xem có vật cản giữa nhân vật và mục tiêu không
            if (!Physics.Raycast(PosOfDetect, _directionToTarget, _distanceToTarget, obstacleLayer))
            {
                // Kiểm tra điều kiện ngang tầm và đối mặt trước khi chọn enemy gần nhất
                if (_distanceToTarget < _minDistance && _heightDifference <= 1f)
                {
                    _minDistance = _distanceToTarget;
                    _nearestEnemy = _enemyController;
                }
            }
        }

        return _nearestEnemy;
    }
    protected void RefreshHpBar()
    {
        if (MyCharInfo.maxHp > 0)
        {
            hpBar_ImgFill.fillAmount = (float) CurrentHp / MyCharInfo.maxHp;
        }
    }
    #endregion

    #region Event Trigger
    void OnTriggerEnter(Collider _other) {
        OnEventTriggerEnter2D(_other);
    }
    public override void OnEventTriggerEnter2D(Collider _other) {
        if (CurrentState == MainCharState.Die) {
            return; // Không xử lý va chạm nếu đã chết
        }
        // Debug.Log("OnTriggerEnter | Va chạm với: " + other.tag);
        if(_other.tag.Equals("Bullet")){
            BulletController _bullet = _other.transform.parent.GetComponent<BulletController>();
            if(_bullet != null){
                _bullet.CreateEffectHit();

                TakeDamage(_bullet.bulletValueDetail.damage);
                if(!_bullet.bulletValueDetail.canPenetrated){
                    // Nếu viên đạn không thể xuyên qua, tự hủy viên đạn
                    _bullet.SelfDestruction();
                }
            }
        }
    }
    // void OnCollisionEnter(Collision collision) {
    //     Debug.Log("OnCollisionEnter | Va chạm với: " + collision.gameObject.name);
    // }
    #endregion
}