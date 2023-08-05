using UnityEngine;

public class ControlProjectile : MonoBehaviour
{
    //public variables
    public GameObject contactPoint; //where the shot struck
    public GameObject sfxShotImpact;  //sfx for shot impact

    //private variables
    PlayerControl playerControl;    //references the class controlling the player
    float maxRange = 100;
    float rangeTravelled = 0.001f;  //tracks distance traveled
    RaycastHit shotHit;             //RaycastHit shoots people.

    void Awake()
    {
        playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        //this following conditional 0
        if (Physics.Raycast(transform.position, transform.forward, out shotHit, maxRange)){
            //instantiates hit shot in a game object
            GameObject objShotMark = Instantiate(contactPoint, shotHit.point + (shotHit.normal * rangeTravelled), Quaternion.LookRotation(shotHit.normal));
            playerControl.fltContactTime = Time.time + 0.2f;
        }
        else{
            //destroys prefab object
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
