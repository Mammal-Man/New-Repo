using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    
    public PlayerControl player;
    public NavMeshAgent agent;
    public GameObject Corpse;
    public GameObject healBox;
    public GameObject bulletBox;
    public float corpseLifespan = 30;

    [Header("Enemy Stats")]
    public int health = 3;
    public int maxHealth = 3;
    public int damageRecieved = 1;
    public float pushBackForce = 10000;
    public float corpseForce = 100;

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
        {
            Destroy(gameObject);
            GameObject corpse = Instantiate(Corpse, transform.position, transform.rotation);

            if (!GameObject.Find("Ammo Pickup(Clone)") && player.CurrentAmmo < player.MaxAmmo)
            {
                GameObject shootingBox = Instantiate(bulletBox, transform.position, transform.rotation);
            }

            if (!GameObject.Find("Heal pickup(Clone)") && player.CurrentHealth < player.MaxHealth)
            {
                GameObject healingBox = Instantiate(healBox, transform.position, transform.rotation);
            }

            corpse.GetComponent<Rigidbody>().AddForce(-transform.forward * corpseForce);
            Destroy(corpse, corpseLifespan);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Find him
        if (other.gameObject.tag == "Player")
        { agent.destination = player.transform.position; }

        if (other.gameObject.tag == "Shot")
        { agent.destination = player.transform.position; }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        { agent.destination = transform.position; }
    }

    private void OnCollisionEnter(Collision other)
    {
        //Ive been shot?
        if (other.gameObject.tag == "Shot")
        {
            Destroy(other.gameObject);
            health--;
        }
    }
}
