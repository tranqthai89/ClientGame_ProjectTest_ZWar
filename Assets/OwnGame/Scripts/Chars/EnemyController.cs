using UnityEngine;
using UnityEngine.AI;
using GlobalEnum;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : CharController
{
    public override CharType charType => CharType.EnemyChar;
    
    [Header("Components")]
    [SerializeField] Animator animator; // Animator để điều khiển hoạt ảnh
    [SerializeField] NavMeshAgent agent;

    [Header("HP Bar")]
    [SerializeField] CanvasGroup hpBarCanvasGroup; // CanvasGroup để điều chỉnh độ mờ của thanh máu
    [SerializeField] Canvas hpBarCanvas; 
    [SerializeField] Image hpBar_ImgFill; 

    public EnemyInfo MyCharInfo{get;set;} // Thông tin về kẻ địch
    public EnemyState CurrentState { get; set; } // Trạng thái hiện tại của kẻ địch
    Transform target; // Đối tượng nhân vật cần đuổi theo

    IEnumerator process_Die;

    public override void StopAllActionNow()
    {
        base.StopAllActionNow();
        
        process_Die = null;
    }
    public override void ResetData()
    {
        base.ResetData();
        MyCharInfo = null;
        CurrentState = EnemyState.Idle;
        hpBarCanvasGroup.alpha = 0f; // Đặt độ mờ của thanh máu về 100%
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

        RefreshHpBar();
        CanBeDamaged = true; // Cho phép nhận sát thương
        IsInstalled = true; // Đánh dấu là đã cài đặt
    }

    void Update() {
        if(!IsInstalled || CurrentState == EnemyState.Die){
            return;
        }
        target = FindTarget(); // Tìm kiếm mục tiêu
        if (target != null) {
            if(CurrentState != EnemyState.Move){
                animator.SetTrigger("Move");
                CurrentState = EnemyState.Move;
            }
            agent.SetDestination(target.position); // Đặt vị trí đích là nhân vật
        }else{
            if(CurrentState != EnemyState.Idle){
                animator.SetTrigger("Idle");
                CurrentState = EnemyState.Idle;
            }
        }
    }
    void LateUpdate()
    {
        hpBarCanvas.worldCamera = Camera.main; // Đặt camera cho canvas hiển thị thanh máu
        hpBarCanvas.transform.LookAt(Camera.main.transform);
        hpBarCanvas.transform.Rotate(0, 180, 0); // Đảo ngược hướng nếu cần
    }

    public Transform FindTarget()
    {
        if(GamePlayManager.Instance == null || GamePlayManager.Instance.MainChar == null || GamePlayManager.Instance.MainChar.CurrentState == MainCharState.Die){
            return null;
        }

        return GamePlayManager.Instance.MainChar.transform;
    }
    public void TakeDamage(int _damage)
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
    public void RefreshHpBar()
    {
        if (MyCharInfo.maxHp > 0)
        {
            hpBar_ImgFill.fillAmount = (float) CurrentHp / MyCharInfo.maxHp;
        }
    }
    public void SetUpDie()
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
        animator.SetTrigger("Die");
        hpBarCanvasGroup.alpha = 0f;

        yield return new WaitForSeconds(1f); // Thời gian chờ trước khi tự hủy

        SelfDestruction();
    }
    void OnTriggerEnter(Collider other) {
        if (CurrentState == EnemyState.Die) {
            return; // Không xử lý va chạm nếu đã chết
        }
        Debug.Log("OnTriggerEnter | Va chạm với: " + other.tag);
        if(other.tag.Equals("Bullet")){
            BulletController _bullet = other.transform.parent.GetComponent<BulletController>();
            if(_bullet != null){
                TakeDamage(_bullet.bulletValueDetail.damage);
                _bullet.SelfDestruction();
            }
        }
    }
    // void OnCollisionEnter(Collision collision) {
    //     Debug.Log("OnCollisionEnter | Va chạm với: " + collision.gameObject.name);
    // }
}
