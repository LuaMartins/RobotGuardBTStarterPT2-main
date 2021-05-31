using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    //declaração de variaveis
    public Transform player;
    public Transform bulletSpawn;
    public Slider healthBar;
    public GameObject bulletPrefab;

    NavMeshAgent agent; // criação de NavMesh
    public Vector3 destination; // destino
    public Vector3 target;      // criação do alvo
    float health = 100.0f; // vida do agente
    float rotSpeed = 5.0f; // rotação do agente
    float visibleRange = 80.0f; //alcance de visão do agente
    float shotRange = 40.0f; // alcance do tiro do agente

    // ao inicio do projeto, atribuir varias coisas, como navmesh e tiro
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; 
        InvokeRepeating("UpdateHealth", 5, 0.5f); // vida
    }

    // criação e posicionamento da barra de vida
    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position); // fazer a barra aparecer
        healthBar.value = (int)health; // valor da barra
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0); // posição da barra de vida
    }

    // criação da regeneração da vida do agente
    void UpdateHealth()
    {
        if (health < 100)  // valor da vida 
            health++;
    }

    // dano e colisão do tiro do personagem
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "bullet") // tag bullet da bala
        {
            health -= 10; // se colidir tira 10 de vida
        }
    }

    [Task] // referencia para o plugin
    public void PickRandomDestination() // movimentação aleatoria do agente
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100)); 
        agent.SetDestination(dest); 
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination()  // declaração de tempo para a movimentação do personagem
    
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time); 
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) 
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void PickDestination(int x, int z) // metodo de destino
    {
        Vector3 dest = new Vector3(x, 0, z); // valores atribuidos ao agente
        agent.SetDestination(dest); // destino do agente
        Task.current.Succeed();
    }

    [Task]
    public void TargetPlayer()
    {
        target = player.transform.position;
        Task.current.Succeed();
    }

    [Task] //força e fisica do agente
    public bool Fire() // metodo da bala saindo
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, // instanciando o prefab da bala
            bulletSpawn.transform.position, bulletSpawn.transform.rotation); // spawna as balas 

        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000); // força da bala

        return true;
    }

    [Task] // metodo de direção do agente
    public void LookAtTarget() // olhar para onde aponta
    {
        Vector3 direction = target - this.transform.position; // posição do target
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, // rotação
            Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed); // velocidade

        if (Task.isInspected)
            Task.current.debugInfo = string.Format("angle={0}",
                Vector3.Angle(this.transform.forward, direction));
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool SeePlayer()
    {// see player é o metodo criado para olhar ao player
        Vector3 distance = player.transform.position - this.transform.position;
        //colisão de raycast nas paredes
        RaycastHit hit;
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);
        if (Physics.Raycast(this.transform.position, distance, out hit)) // inicio do raycast
        {
            if (hit.collider.gameObject.tag == "wall") // se colidir com a tag wall
            {
                seeWall = true; // ele olha
            }
        }
        if (Task.isInspected) Task.current.debugInfo = string.Format("wall={0}", seeWall);
        if (distance.magnitude < visibleRange && !seeWall) // distancia da magnitude
            return true;
        else
            return false; // retorna o falso
    }

    [Task] bool Turn(float angle) // gira o personagem para certo angulo
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) *
            this.transform.forward;
        target = p;
        return true; } // retorna o verdadeiro

    [Task]
    public bool IsHealthLessThan(float health) // nivel de vida do meu personagem
    {
        return this.health < health;
    }

    [Task]
    public bool Explode() // se meu personagem explodiu ou não
    {
        Destroy(healthBar.gameObject);
        Destroy(this.gameObject);
        return true;
    }
}

