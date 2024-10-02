using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    
    public PlayerControl player;
    public GameObject shot;
    public NavMeshAgent agent;
    public GameObject Corpse;

    [Header("Enemy Stats")]
    public int health = 3;
    public int maxHealth = 3;
    public int damageRecieved = 1;
    public float pushBackForce = 10000;

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
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Find him
        if (collision.gameObject.tag == "Player")
        { agent.destination = player.transform.position; }
        else if (collision.gameObject.tag == "Shot")
        { agent.destination = shot.transform.position; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Ive been shot?
        if (collision.gameObject.tag == "Shot")
        {
            Destroy(collision.gameObject);
            health--;
        }
    }
}
