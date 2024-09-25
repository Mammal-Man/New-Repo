using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    
    public PlayerControl player;
    public NavMeshAgent agent;

    [Header("Enemy Stats")]
    public int health = 3;
    public int maxHealth = 3;
    public int damageGiven = 5;
    public int damageRecieved = 1;
    public float pushBackForce = 10000;
    public float shootingRange = 10;
    public float shotSpeed = 10000;
    public float bulletLifespan = 1;
    public GameObject shot;
    public bool canFire = true;
    public float fireRate = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControl>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Kill me
        if (health <= 0)
        { Destroy(gameObject); }

        //Find him
        else
            agent.destination = player.transform.position;

        //Shoot Him
        if (Physics.Raycast(transform.position, transform.forward, shootingRange))
        {
            GameObject s = Instantiate(shot, transform.position, transform.rotation);
            s.GetComponent<Rigidbody>().AddForce(transform.forward * shotSpeed);
            Destroy(s, bulletLifespan);

            canFire = false;
            StartCoroutine("cooldownFire");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Ive been shot?
        if (collision.gameObject.tag == "Shot")
        {
            Destroy(collision.gameObject);
            health--;
        }
    }

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

}
