using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0,360)]
    public float viewAngle;
    public float viewRadius;

    public LayerMask enemyMask;
    public LayerMask wallMask;

    public Transform[] enemies;

    private void Start()
    {
        StartCoroutine("FindEnemiesWithDelay", 0.2f);
    }

    IEnumerator FindEnemiesWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleEnemy();
        }
    }

    void FindVisibleEnemy()
    {
        Transform enemy;
        Vector3 dir;
        float distance;

        for (int i = 0; i < enemies.Length; ++i)
        {
            enemy = enemies[i];
            dir = (enemy.position - transform.position).normalized;

            if(EnemyInRadius(enemy.position))
            {
                if (Vector3.Angle(transform.forward, dir) < viewAngle / 2)
                {
                    distance = Vector3.Distance(transform.position, enemy.position);

                    if (!Physics.Raycast(transform.position, dir, distance, wallMask))
                    {
                        //visible
                        enemy.GetComponent<EnemyAI>().isVisibleFromPlayer = true;
                        continue;
                    }
                }
            }

            enemy.GetComponent<EnemyAI>().isVisibleFromPlayer = false;
        }
    }

    private bool EnemyInRadius(Vector3 enemyPos)
    {
        float distance;
        Vector3 playerPos = transform.position;
        float x = Mathf.Pow((enemyPos.x - playerPos.x), 2);
        float y = Mathf.Pow((enemyPos.z - playerPos.z), 2);

        distance = Mathf.Pow(x + y, 0.5f);

        if(distance <= viewRadius)
        {
            return true;
        }
        return false;
    }


    public Vector3 DirFromAngle(float angle, bool isGlobal)
    {
        if(!isGlobal)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
