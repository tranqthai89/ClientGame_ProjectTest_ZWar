using UnityEngine;
using UnityEngine.AI;
using GlobalEnum;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : CharController
{
    public override CharType charType => CharType.EnemyChar;
    
    [Header("Components")]
    [SerializeField] EnemyAnimationController myAnimation;
    [SerializeField] Transform model; 
    [SerializeField] NavMeshAgent agent;

    [Header("HP Bar")]
    [SerializeField] CanvasGroup hpBarCanvasGroup; // CanvasGroup để điều chỉnh độ mờ của thanh máu
    [SerializeField] Canvas hpBarCanvas; 
    [SerializeField] Image hpBar_ImgFill; 

    [Header("Guns")]
    [SerializeField] GunController myGun; // Súng của kẻ địch

    public EnemyInfo MyCharInfo{get;set;} // Thông tin về kẻ địch
    public EnemyState CurrentState { get; set; } // Trạng thái hiện tại của kẻ địch
    MainCharController target; // Đối tượng nhân vật cần đuổi theo

    IEnumerator process_Die;

    public override void StopAllActionNow()
    {
        base.StopAllActionNow();
        
        process_Die = null;
    }
    public override void ResetData()
    {
        base.ResetData();
        agent.isStopped = true;

        MyCharInfo = null;
        CurrentState = EnemyState.Idle;
        hpBarCanvasGroup.alpha = 0f; // Đặt độ mờ của thanh máu về 100%

        model.transform.localRotation = Quaternion.identity;
        model.transform.localPosition = Vector3.zero;
    }

    void Awake() {
        ResetData();
    }

    public void Init(EnemyInfo _enemyInfo) {
        if (_enemyInfo == null) {
            Debug.LogError("EnemyInfo is null!");
            return;
        }
        MyCharInfo = _enemyInfo;
        CurrentHp = MyCharInfo.maxHp;

        agent.speed = MyCharInfo.moveSpeed;
        agent.angularSpeed = MyCharInfo.rotationSpeed;

        myGun.Init(MyCharInfo.gunValueDetail); // Khởi tạo súng của kẻ địch
        myAnimation.onCreateDmg = ()=>{
            if(target == null || target.CurrentState == MainCharState.Die){
                return;
            }
            myGun.Shoot(target.PosOfDetect);
        };

        OnDie += (_enemy) => {
            // Xử lý khi kẻ địch chết
            GamePlayManager.Instance.currentGameControl.OnEnemyDie((EnemyController) _enemy);
        };
    
        RefreshHpBar();
        IsInstalled = true; // Đánh dấu là đã cài đặt
    }

    #region Behaviors
    void Update() {
        if(!IsRunning || !IsInstalled || CurrentState == EnemyState.Die || GamePlayManagerInstance.currentGameControl.currentState != GamePlayState.PlayGame){
            return;
        }
        if(CurrentState == EnemyState.Attack){return;}

        target = FindTarget(); // Tìm kiếm mục tiêu
        if (target != null) {
            if(myGun.CheckIfInRangeAttack(target.PosOfDetect)){
                if(CurrentState != EnemyState.Attack){
                    myAnimation.SetAnimByState(Enemy_StateAnimation.Attack);
                    CurrentState = EnemyState.Attack;
                }
                agent.isStopped = true; 
                myGun.Shoot();
            }else{
                if(CurrentState != EnemyState.Move){
                    myAnimation.SetAnimByState(Enemy_StateAnimation.Move);
                    CurrentState = EnemyState.Move;
                }
                agent.isStopped = false; 
                agent.SetDestination(target.transform.position); // Đặt vị trí đích là nhân vật
            }
        }else{
            if(CurrentState != EnemyState.Idle){
                myAnimation.SetAnimByState(Enemy_StateAnimation.Idle);
                CurrentState = EnemyState.Idle;
            }
            agent.isStopped = true; // Dừng di chuyển
        }
        model.transform.localPosition = Vector3.zero; // vì model tìm được đang lỗi animation nên buộc phải reset lại vị trí của model
    }
    void LateUpdate()
    {
        if(!IsRunning || !IsInstalled)
        {
            return;
        }
        hpBarCanvas.worldCamera = Camera.main; // Đặt camera cho canvas hiển thị thanh máu
        hpBarCanvas.transform.LookAt(Camera.main.transform);
        hpBarCanvas.transform.Rotate(0, 180, 0); // Đảo ngược hướng nếu cần
    }
    protected void TakeDamage(int _damage)
    {
        if(!CanBeDamaged)
        {
            return; // Nếu không thể nhận sát thương, không làm gì cả
        }
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
        if (CurrentState == EnemyState.Die)
        {
            return; // Nếu đã chết, không làm gì cả
        }
        process_Die = DoProcess_Die();
        StartCoroutine(process_Die);
    }
    IEnumerator DoProcess_Die()
    {
        CurrentState = EnemyState.Die;
        myAnimation.SetAnimByState(Enemy_StateAnimation.Die);
        hpBarCanvasGroup.alpha = 0f;
        agent.isStopped = true; // Dừng di chuyển khi chết
        MyAudioManager.Instance.PlaySfx(GameInformation.Instance.sfxEnemyDie);

        OnDie?.Invoke(this); // Gọi callback khi chết

        yield return new WaitForSeconds(1f); // Thời gian chờ trước khi tự hủy

        SelfDestruction();
    }
    #endregion

    #region Others
    protected void RefreshHpBar()
    {
        if (MyCharInfo.maxHp > 0)
        {
            hpBar_ImgFill.fillAmount = (float) CurrentHp / MyCharInfo.maxHp;
        }
    }
    protected MainCharController FindTarget()
    {
        if(GamePlayManager.Instance == null || GamePlayManager.Instance.currentGameControl.mainChar == null || GamePlayManager.Instance.currentGameControl.mainChar.CurrentState == MainCharState.Die){
            return null;
        }
        return GamePlayManager.Instance.currentGameControl.mainChar;
    }
    #endregion

    #region Event Trigger
    void OnTriggerEnter(Collider _other) {
        OnEventTriggerEnter2D(_other);
    }
    public override void OnEventTriggerEnter2D(Collider _other) {
        if (CurrentState == EnemyState.Die) {
            return; // Không xử lý va chạm nếu đã chết
        }
        // Debug.Log("OnTriggerEnter | Va chạm với: " + other.tag);
        if(_other.tag.Equals("Bullet")){
            BulletController _bullet = _other.transform.parent.GetComponent<BulletController>();
            if(_bullet != null){
                _bullet.CreateEffectHit();

                if(_bullet.bulletValueDetail == null){
                    Debug.LogError("BulletController: bulletValueDetail is null!");
                    _bullet.SelfDestruction();
                    return;
                }else{
                    if(_bullet.bulletValueDetail.canExplosion){
                        _bullet.CreateExplosion();
                        _bullet.SelfDestruction();
                        return;
                    }

                    TakeDamage(_bullet.bulletValueDetail.damage);
                    if(!_bullet.bulletValueDetail.canPenetrated){
                        // Nếu viên đạn không thể xuyên qua, tự hủy viên đạn
                        _bullet.SelfDestruction();
                    }
                }
            }
        }
    }
    // void OnCollisionEnter(Collision collision) {
    //     Debug.Log("OnCollisionEnter | Va chạm với: " + collision.gameObject.name);
    // }
    #endregion
}
