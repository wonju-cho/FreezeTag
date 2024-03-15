using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Animation gemAnimation;
    //public bool destroy;
    public GameObject gem;

    public CapsuleCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        //destroy = false;
        gemAnimation = GetComponent<Animation>();

        collider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gemAnimation.isPlaying)
        {
            gemAnimation.Play();
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        //collision.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            collider.isTrigger = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
    }

    public void DestoryGem()
    {
        Destroy(gem);
    }
}
