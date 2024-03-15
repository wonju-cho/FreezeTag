using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float dashUpwardForce;
    [SerializeField] private float dashTime = 2f;
    [SerializeField] private float dashDuration = 2f;
    [SerializeField] private float dashSpeed = 2f;

    public GameManager gameManager;

    private bool isDash;
    private float timer;
    private float dashTimer;

    private Quaternion lockedRotation;
    public Camera mainCam;

    private XROrigin xrOrigin;
    private Vector2 inputAxis;
    private float currentSpeed;
    [SerializeField] private float originalSpeed = 1f;
    public XRNode inputSource;
    private CharacterController characterController;
    private FieldOfView fov;

    [Header("Dash debugging UIs")]
    public GameObject testDashUI;
    public TextMeshProUGUI currentSpeedText;
    public TextMeshProUGUI dashEnabledText;
    
    public bool enableDash;
    
    private int gemCount;

    public int healthCount;
    public Transform respawnPos;

    private AudioSource audioSource;
    public AudioClip pickupAudioClip;
    bool once = false;

    private bool does_collision_with_enemy = false;

    public ParticleSystem playerParticleSystem;

    private CellPhone_UI cellphone_;
    private int npc_count;
    private List<string> gem_names;

    // Start is called before the first frame update
    void Start()
    {
        //mainModule = playerParticleSystem.main;
        mainCam = Camera.main;
        lockedRotation = mainCam.transform.rotation;

        testDashUI.SetActive(false);

        healthCount = 5;
        gemCount = 0;
        timer = dashTime;
        dashTimer = dashDuration;
        currentSpeed = originalSpeed;
        xrOrigin = GetComponent<XROrigin>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        cellphone_ = GetComponentInChildren<CellPhone_UI>();
        gem_names = new List<string>();
        fov = GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        //lock y axis
        Vector3 newRotation = mainCam.transform.rotation.eulerAngles;
        mainCam.transform.rotation = Quaternion.Euler(newRotation.x, lockedRotation.y, newRotation.z);

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        currentSpeedText.text = currentSpeed.ToString();

        if (enableDash)
        {
            DoDash();
            dashEnabledText.text = "true";
        }
        else
        {
            dashEnabledText.text = "false";
        }
    }

    public int GetEatenGemCount()
    {
        return gemCount;
    }

    void DoDash()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isDash = true;
        }
        else if (timer >= 0)
        {
            playerParticleSystem.Stop();
        }

        if (isDash)
        {
            if (!playerParticleSystem.isPlaying)
            {
                playerParticleSystem.Play();
            }

            testDashUI.SetActive(true);
            currentSpeed = dashSpeed;
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDash = false;
                currentSpeed = originalSpeed;
                timer = dashTime;
                dashTimer = dashDuration;
                testDashUI.SetActive(false);

                once = false;
            }
        }

        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

    }

    private void FixedUpdate()
    {
        Quaternion headYaw = Quaternion.Euler(0, xrOrigin.Camera.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        characterController.Move(direction * Time.fixedDeltaTime * currentSpeed);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Gem"))
        {

            //Debug.Log("Collide with gem");

            gemCount++;

            audioSource.PlayOneShot(pickupAudioClip, 0.8f);
            hit.gameObject.GetComponent<Pickup>().DestoryGem();

            if(gem_names.Contains(hit.transform.parent.gameObject.name))
            {
                return;
            }

            gem_names.Add(hit.gameObject.name);
            gemCount++;
            cellphone_.UpdateGemCountText(gemCount);
        }

        
        /*
        if(hit.gameObject.CompareTag("Enemy"))
        {


            Debug.Log("collision with:" + hit.gameObject.name);
            Debug.Log("Collide with enemy");

            if (does_collision_with_enemy == false)
            {
                //UpdateHealthCount();
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                for (int i = 0; i < enemies.Length; ++i)
                {
                    enemies[i].GetComponent<EnemyAI>().MoveOriginalPosition();
                }

                //respawn
                transform.position = respawnPos.position;
                does_collision_with_enemy = true;

            }
            
        }

        */

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
            
    }

    public void RespawnPosition()
    {
        Debug.Log("respawn");


        characterController.enabled = false;
        //characterController.gameObject.SetActive(false);
        characterController.transform.position = respawnPos.position;
        xrOrigin.transform.position = respawnPos.position;

        characterController.enabled = true;
        //characterController.gameObject.SetActive(true);

    }

    public int GetHealthCount()
    {
        return healthCount;
    }

    public void UpdateNPCCount()
    {
        ++npc_count;
        cellphone_.UpdateNPCCountText(npc_count);
    }
}
