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
            Destroy(gameObject);

        //Find him
        agent.destination = player.transform.position;
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
