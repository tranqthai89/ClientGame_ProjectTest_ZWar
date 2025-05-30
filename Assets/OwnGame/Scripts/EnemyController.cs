using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player; // Đối tượng nhân vật cần đuổi theo
    public Animator animator; // Animator để điều khiển hoạt ảnh
    private NavMeshAgent agent;

    enum State{
        Idle, Walk
    }
    State state;

    void Start() {
        state = State.Idle;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if (player != null) {
            if(state != State.Walk){
                animator.SetTrigger("Walk");
                state = State.Walk;
            }
            agent.SetDestination(player.position); // Đặt vị trí đích là nhân vật
        }
    }
    void OnTriggerEnter(Collider other) {
        Debug.Log("OnTriggerEnter | Va chạm với: " + other.tag);
        if(other.tag.Equals("Bullet")){
            BulletController _bullet = other.transform.parent.GetComponent<BulletController>();
            if(_bullet != null){
                _bullet.SelfDestruction();
            }
        }
        
    }
    // void OnCollisionEnter(Collision collision) {
    //     Debug.Log("OnCollisionEnter | Va chạm với: " + collision.gameObject.name);
    // }
}
