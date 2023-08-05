using UnityEngine;
using UnityEngine.AI;

public class ZombieControl : MonoBehaviour
{
    public float zombieHP = 100f;
    public float zombieDmg = 4.5f;  //the amount of damage the zombie causes
    public bool isActive = true;    //whether the zombie is alive or not...?

    //Manages attack rate. I *personally* think it should only be One variable but okay???
    float atkRate = 0f;
    float waitAttack = 1f;

    //Manages their respective classes:
    NavMeshAgent navMeshZombie;
    Animation animZombie;
    PlayerControl playerControl;    //references the script PlayerControl

    // Start is called before the first frame update
    void Start()
    {
        navMeshZombie = GetComponent<NavMeshAgent>();
        animZombie = transform.GetComponentInChildren<Animation>(); //gets component from children under this object
        playerControl = GameObject.Find("fpscameraPlayer").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive == true){
            float _atkRange = Vector3.Distance(transform.position, playerControl.transform.position);
            if ((_atkRange <= navMeshZombie.stoppingDistance + 0.6f) && atkRate <= Time.time){
                animZombie.CrossFade("Z_Attack");
                Invoke("ArmSwing", 0.1f);   //calls method for attacking.
                Invoke("ArmSwing", 0.25f);
                atkRate = Time.time + waitAttack;
            }
            else if (navMeshZombie.remainingDistance > navMeshZombie.stoppingDistance){
                if(animZombie.isPlaying == false){
                    animZombie.CrossFade("Z_Walk1_InPlace");
                }
            }
            navMeshZombie.SetDestination(playerControl.transform.position);

            if(zombieHP <= 0){
                navMeshZombie.isStopped = true;
                animZombie.CrossFade("Z_FallingBack");
                isActive = false;
                ZombieDeath();  //I feel like this is unnecessary...?
            }
        }
    }

    void ArmSwing()
    {
        if (navMeshZombie.remainingDistance <= navMeshZombie.stoppingDistance + 0.6f){
            playerControl.playerHP -= zombieDmg;
            playerControl.regenTime = 300;
        }
    }

    public void ZombieDeath(){
        Destroy(gameObject, 3);  //*dies of cringe*
    }

    public void TakeDamage(float dmgTaken)
    {
        zombieHP = -dmgTaken;
    }
}
