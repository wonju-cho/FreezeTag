using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyAI : MonoBehaviour
{
    [Range(0, 3)]
    public int enemy_type;

    public bool GizmosMode;
    public Transform player;
    public float speed = 5f;
    public float stun_time;

    Vector3[] path;
    Vector3 nextStepPath;
    int targetIndex;

    public bool isVisibleFromPlayer;
    public Material originalMaterial;
    public Material stopMaterial;
    public Material stunMaterial;

    public Transform originalTransform;
    private Vector3 original_position;

    //private Pathfinding pathManager;
    private MeshRenderer meshRender;
    private bool isProcessingWalk;
    private float timer_for_stunning;
    private bool does_player_release_npc = false;
    private bool is_trigger_walk = false;


    Rigidbody rb;

    private enum State
    {
        Chasing,
        StopMoving,
        Stunning,
    }

    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        //pathManager = GetComponent<Pathfinding>();
        meshRender = GetComponent<MeshRenderer>();
        currentState = State.Chasing;
        isProcessingWalk = false;
        //PathFindingManager.RequestPath(transform.position, player.position, OnFound);
        timer_for_stunning = stun_time;
        if(enemy_type==3)
        {
            speed = 4f;
        }

        original_position = originalTransform.position;
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case State.Chasing:
                if (isVisibleFromPlayer)
                {

                    currentState = State.StopMoving;
                    meshRender.material = stopMaterial;
                    break;
                }

                if(does_player_release_npc == true)
                {
                    currentState = State.Stunning;
                    meshRender.material = stunMaterial;
                    break;
                }

                if(isProcessingWalk== false)
                {
                    is_trigger_walk = true;
                    isProcessingWalk = true;
                    if(enemy_type == 0)//original enemy type
                    {
                        //from player position to enemy position
                        nextStepPath = PathFindingManager.RequestFindPath(player.position, transform.position);
                    }
                    else if(enemy_type == 1)
                    {
                        //from enemy position to position of (player's lookat vector - enemy)*2
                        //but if the player is near by enemy, just catch the player
                        float distance = Vector3.Distance(player.position, transform.position);
                        if(distance < 5.0f)
                        {
                            nextStepPath = PathFindingManager.RequestFindPath(player.position, transform.position);
                        }
                        else
                        {
                            Vector3 player_new_position = PathFindingManager.NearLookatVectorGrid(player.transform, transform.position);
                            
                            nextStepPath = PathFindingManager.RequestFindPath(player_new_position, transform.position);
                        }
                        
                    }
                    else if (enemy_type == 2)
                    {
                        //from enemy position to position of (player's lookat vector * 4)
                        //but if the plaeyr is near by enemy, just catch the player.
                        float distance = Vector3.Distance(player.position, transform.position);
                        if(distance<5.0f)
                        {
                            nextStepPath = PathFindingManager.RequestFindPath(player.position, transform.position);
                        }
                        else
                        {
                            Vector3 player_new_position = PathFindingManager.FindFrontVectorGrid(player.transform);
                            nextStepPath = PathFindingManager.RequestFindPath(player_new_position, transform.position);
                        }
                    }
                    else if(enemy_type == 3)
                    {
                        nextStepPath = PathFindingManager.RequestFindPath(player.position, transform.position);
                    }
                }

                Vector3 moveDirection = nextStepPath - transform.position;
                moveDirection.y = 0;
                moveDirection.Normalize();
                Quaternion rotate = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotate, Time.deltaTime * 500);

                
                if (Vector3.Distance(nextStepPath, transform.position) < 0.1f)
                {
                    isProcessingWalk = false;
                }
                break;

            case State.StopMoving:
                if (!isVisibleFromPlayer)
                {

                    currentState = State.Chasing;
                    meshRender.material = originalMaterial;

                    break;
                }

                rb.position = transform.position;
                rb.velocity = Vector3.zero;

                Vector3 look_vector = player.position - transform.position;
                look_vector.y = 0;
                look_vector.Normalize();
                Quaternion rotation_ = Quaternion.LookRotation(look_vector, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation_, Time.deltaTime * 500);

                if(does_player_release_npc == true)
                {
                    currentState = State.Stunning;
                    meshRender.material = stunMaterial;
                    break;
                }

                break;

            case State.Stunning:
                timer_for_stunning -= Time.deltaTime;
                if(timer_for_stunning < 0)
                {
                    currentState = State.Chasing;
                    meshRender.material = originalMaterial;
                    timer_for_stunning = stun_time;
                    does_player_release_npc = false;
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if(currentState == State.Chasing)
        {
            Vector3 delta_position = nextStepPath - transform.position;

            delta_position.Normalize();
            rb.MovePosition(transform.position + delta_position * Time.deltaTime * speed);
        }
    }

    public void OnFound(bool isSuccess, Vector3[] newPath)
    {
        if(isSuccess)
        {
            path = newPath;
            StopCoroutine("FollowPathForChasing");
            StartCoroutine("FollowPathForChasing");
        }
    }

    IEnumerator FollowPathForChasing()
    {
        Vector3 currentWayPoint = path[0];

        while(true)
        {
            if(transform.position == currentWayPoint)
            {
                ++targetIndex;
                if(targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWayPoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, Time.deltaTime * speed);

            Vector3 moveDirection = currentWayPoint - transform.position;
            moveDirection.Normalize();
            Quaternion rotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotate, Time.deltaTime * speed);
            yield return null; 

        }
    }

    public void OnDrawGizmos()
    {
        if(GizmosMode == true && path != null)
        {
            Gizmos.color = Color.black;

            for (int i = targetIndex; i<path.Length;++i)
            {
                Gizmos.DrawCube(path[i], Vector3.one*0.2f);
                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == player.name)
        {
            //collision with player
            if(player.GetComponent<TestController>())
            {
                player.GetComponent<TestController>().UpdateHealthCount();
            }
            else if(player.GetComponent<PlayerController>())
            {
                if (collision.gameObject.name == player.name)
                {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                    for (int i = 0; i < enemies.Length; ++i)
                    {
                        enemies[i].GetComponent<EnemyAI>().MoveOriginalPosition();

                    }
                    
                    player.GetComponent<PlayerController>().UpdateHealthCount();
                    player.GetComponent<PlayerController>().RespawnPosition();
                }

            }
        }

        if(collision.gameObject.GetComponent<EnemyAI>())
        {
            isProcessingWalk = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == player.name)
        {
            if (player.GetComponent<PlayerController>())
            {
                //player.GetComponent<PlayerController>().RespawnPosition();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.name == player.name)
        {
            //collision with player
            if (player.GetComponent<TestController>())
            {
                player.GetComponent<TestController>().UpdateHealthCount();
            }
            else if (player.GetComponent<PlayerController>())
            {

                if (other.gameObject.name == player.name)
                {

                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                    for (int i = 0; i < enemies.Length; ++i)
                    {
                        enemies[i].GetComponent<EnemyAI>().MoveOriginalPosition();

                    }


                    player.GetComponent<PlayerController>().UpdateHealthCount();

                }


            }
        }

        if(other.gameObject.GetComponent<EnemyAI>())
        {
            //isProcessingWalk = false;
        }
    }

    public void PlayerReleasedNPC()
    {
        does_player_release_npc = true;
    }

    public void MoveOriginalPosition()
    {
        this.transform.position = original_position;
        currentState = State.Chasing;
        isProcessingWalk = false;
    }
}


