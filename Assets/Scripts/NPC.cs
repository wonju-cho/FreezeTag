using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject signifierGameObject;

    public InputActionReference colorReference = null;

    public bool detectPlayer;
    public bool isNPCFree;

    public Animation freeAnimation;

    public MeshRenderer bodyMeshRenderer;

    public Material freeMaterial;
    public Material originalMaterial;

    [SerializeField] private float freeTime = 2f;
    private float freeContainer;
    [SerializeField] private float destroyTime = 1f;

    public ParticleSystem particleSystem;

    public GameObject npcGameObject;
    public GameObject[] enemies;
    public GameObject player;

    public Image npcProgressBar;

    private AudioSource audioSource;
    public AudioClip releasingClip;
    public AudioClip releasingSuccessClip;

    private bool once;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        freeContainer = freeTime;
        isNPCFree = false;
        signifierGameObject.SetActive(false);
        particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (detectPlayer)
        {
            float value = colorReference.action.ReadValue<float>();
            UpdateColor(value);
            signifierGameObject.SetActive(true);
        }
        else if(!detectPlayer)
        {
            signifierGameObject.SetActive(false);
        }

        if (isNPCFree)
        {
            if (!particleSystem.isPlaying) particleSystem.Play();
            destroyTime -= Time.deltaTime;
            if (destroyTime < 0)
            {
                Destroy(npcGameObject);
                player.GetComponent<PlayerController>().UpdateNPCCount();
                
                foreach(GameObject enemy in enemies)
                {
                    enemy.GetComponent<EnemyAI>().PlayerReleasedNPC();
                }
            }
        }
    }

    private void UpdateColor(float value)
    {
        if (value == 1)
        {
            if (!once && !isNPCFree)
            {
                audioSource.PlayOneShot(releasingClip);
                once = true;
            }
            
            freeContainer -= Time.deltaTime;
            float fillAmount = freeTime - freeContainer - 0.5f;
            
            npcProgressBar.fillAmount = fillAmount;

            freeAnimation.Play();
            if (freeContainer < 0f)
            {
                if (!isNPCFree)
                {
                    audioSource.PlayOneShot(releasingSuccessClip);
                }

                isNPCFree = true;
                freeAnimation.Stop();
                bodyMeshRenderer.material = freeMaterial;
                
            }

        }
        else if (value == 0 && !isNPCFree) 
        {
            if(freeAnimation.isPlaying) freeAnimation.Stop();
            bodyMeshRenderer.material = originalMaterial;
            freeContainer = freeTime;
            once = false;
        }

    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            detectPlayer = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Destroy everything that leaves the trigger
        if (other.gameObject.CompareTag("Player"))
        {
            detectPlayer = false;
        }
    }
}
