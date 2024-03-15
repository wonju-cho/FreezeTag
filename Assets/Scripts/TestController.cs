using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public float moveSpeed = 6;
    public GameManager gameManager;

    Rigidbody rb;
    Camera viewCam;
    Vector3 velocity;
    private AudioSource audioSource;
    public AudioClip pickUpAudioClip;
    public int healthCount;

    private int eaten_gem_count = 0;

    public Transform respawnPos;
    private CellPhone_UI cellphone_;

    private List<string> gems;
    
    // Start is called before the first frame update
    void Start()
    {
        gems = new List<string>();
        rb = GetComponent<Rigidbody>();
        viewCam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        healthCount = 5;

        cellphone_ = GetComponentInChildren<CellPhone_UI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = viewCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCam.transform.position.y));

        transform.LookAt(mousePos + Vector3.up * transform.position.y);
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
    }

    public int GetEatenGemCount()
    {
        return eaten_gem_count;
    }
    
    public int GetHealthCount()
    {
        return healthCount;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Time.fixedDeltaTime * velocity);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponentInChildren<Pickup>())
        {
            Destroy(collision.gameObject);//cannot destroy immediatly

            if (gems.Contains(collision.gameObject.name))
                return;

            eaten_gem_count++;
            gems.Add(collision.gameObject.name);
            audioSource.PlayOneShot(pickUpAudioClip);

            cellphone_.UpdateGemCountText(eaten_gem_count);
        }
    }


    public void UpdateHealthCount()
    {
        --healthCount;

        cellphone_.UpdateHealthCountText(healthCount);

        if (healthCount <= 0)
        {
            //game end
            gameManager.EndGame();
        }

        //respawn
        this.transform.position = respawnPos.position;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; ++i)
        {
            enemies[i].GetComponent<EnemyAI>().MoveOriginalPosition();
        }
    }
}
