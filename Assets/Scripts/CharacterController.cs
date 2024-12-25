using System.Collections;
using System.Xml.XPath;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float lateralSpeed;

    [SerializeField] private float sprintSpeed;
    [SerializeField] private float timeToMaxSprint;

    [SerializeField] private float rotationPower;

    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;

    [SerializeField] private float minVerticalAngle;
    [SerializeField] private float maxVerticalAngle;

    [SerializeField] private Transform followTransform;

    [SerializeField] private float shootSpeed;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform emitterTransform;

    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip specialShootClip;
    [SerializeField] private AudioClip damageTakenClip;
    [SerializeField] private AudioClip pickupClip;


    [SerializeField] private Slider comboSlider;

    private Animator anim;
    private AudioSource audioSource;

    private UnityEngine.CharacterController controller;
    private Stats stats;

    private float dx, dz;
    private float currSpeed;

    private bool isShooting;
    private bool isRolling;
    private bool dying;

    private float verticalMovement;

    public int killed;

    public float killTimer;

    public int multiplier;

    void Start()
    {
        killed = 0;

        killTimer = 0;

        multiplier = 0;

        controller = GetComponent<UnityEngine.CharacterController>();

        audioSource = GetComponent<AudioSource>();

        stats = GetComponent<Stats>();

        anim = GetComponent<Animator>();
        anim.SetFloat("ShootSpeed", shootSpeed);

        isShooting = false;
        isRolling = false;
        dying = false;

        currSpeed = speed;
        verticalMovement = 0;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Update()
    {
        if (killed > 0)
            killTimer -= Time.deltaTime; 

        if (killTimer <= 0) 
        {
            killTimer = 4;
            killed = 0;
        }

        if (killTimer > 0 && killed >= 3)
        {
            comboSlider.gameObject.SetActive(true);
            multiplier = 1;
        }
        else
        {
            comboSlider.gameObject.SetActive(false);
            multiplier = 0; 
        }

        comboSlider.value = killTimer;

        if (dying)
            return;

        if (!dying && stats.currHealth <= 0)
        {
            dying = true;
            anim.Play("Die");
            Invoke("EndGame", 6);
            return;
        }

        if (isShooting)
            return;

        if (isRolling)
        {
            HandleMovement();
        }

        else
        {
            HandleInput();
            UpdateRotation();
            HandleMovement();
            UpdateAnimation();
        }


    }

    void HandleInput()
    {
        // Get input.
        dx = Input.GetAxisRaw("Horizontal");
        dz = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Roll();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            SpecialShoot();
            return;
        }
    }

    void UpdateRotation()
    {
        // Handle follow transform rotation based on mouse input.
        float mouseDx = Input.GetAxisRaw("Mouse X");
        float mouseDy = Input.GetAxisRaw("Mouse Y");

        followTransform.transform.rotation *= Quaternion.AngleAxis(mouseDx * rotationPower, Vector3.up);
        followTransform.transform.rotation *= Quaternion.AngleAxis(-mouseDy * rotationPower, Vector3.right);

        Vector3 rot = followTransform.transform.localEulerAngles;
        rot.z = 0;

        if (rot.x > 180 && rot.x < maxVerticalAngle)
            rot.x = maxVerticalAngle;
        else if (rot.x < 180 && rot.x > minVerticalAngle)
            rot.x = minVerticalAngle;

        followTransform.transform.localEulerAngles = rot;

        // Set the player rotation based on the look transform.
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);

        // Reset the y rotation of the look transform.
        followTransform.transform.localEulerAngles = new Vector3(rot.x, 0, 0);
    }

    void HandleMovement()
    {
        if (controller.isGrounded)
            verticalMovement = 0;
        else
            verticalMovement = -gravity * Time.deltaTime;

        if (isRolling)
        {
            controller.Move(transform.forward * rollSpeed * Time.deltaTime);
            controller.Move(new Vector3(0, verticalMovement, 0));
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            {
                verticalMovement += jumpForce;
            }

            if (Input.GetKey(KeyCode.LeftShift) && currSpeed < sprintSpeed && dz > 0)
            {
                float acceleration = (sprintSpeed - speed) / timeToMaxSprint;
                currSpeed += acceleration * Time.deltaTime;

                if (currSpeed > sprintSpeed)
                    currSpeed = sprintSpeed;
            }

            if (!Input.GetKey(KeyCode.LeftShift) && currSpeed > speed)
            {

                float deceleration = (speed - sprintSpeed) / timeToMaxSprint;
                currSpeed += deceleration * Time.deltaTime;

                if (currSpeed < speed)
                    currSpeed = speed;
            }

            // Apply movement.
            controller.Move(transform.forward * currSpeed * dz * Time.deltaTime);
            controller.Move(transform.right * lateralSpeed * dx * Time.deltaTime);
            controller.Move(new Vector3(0, verticalMovement, 0));
        }
    }

    void UpdateAnimation()
    {
        // Update animation.
        anim.SetFloat("SprintBlend", currSpeed / sprintSpeed);
        if (controller.velocity.y > 0.1f)
        {
            anim.Play("Jump");
        }
        else if (Mathf.Abs(controller.velocity.y) <= 0.1f)
        {
            if (dz == 0 && dx != 0)
            {
                if (dx < 0)
                    anim.Play("StrafeLeft");
                else
                    anim.Play("StrafeRight");
            }
            else if (dz != 0)
                anim.Play("SprintTree");
            else
                anim.Play("Idle");
        }
    }

    public void Shoot()
    {
        if (stats.currAmmo == 0) return;

        isShooting = true;
        stats.DecreaseAmmo(1);
        audioSource.PlayOneShot(shootClip);
        anim.Play("Shoot");
    }

    public void SpecialShoot()
    {
        if (stats.currCooldown < stats.skillCooldown)
            return;

        isShooting = true;
        stats.currCooldown = 0;
        audioSource.PlayOneShot(specialShootClip);
        anim.Play("Cast");

        float visionAngle = 70f;
        float visionDistance = 100f;

        Collider[] colliders = Physics.OverlapSphere(transform.position, visionDistance);

        foreach (Collider col in colliders)
        {
            Vector3 directionToTarget = (col.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                if (col.CompareTag("Enemy"))
                {
                    Eagle eagle = col.gameObject.GetComponent<Eagle>();
                    if (eagle != null)
                    {
                        eagle.Die();
                        stats.IncreaseXP(eagle.experience);
                    }
                    else
                    {
                        col.GetComponent<Wolf>().gameObject.SetActive(false);
                    }
                    killed++;
                    killTimer = 4;
                }
            }
        }
    }

    public void OnSpecialEnd()
    {
        isShooting = false;
    }

    public void Roll()
    {
        stats.invincible = true;
        isRolling = true;
        anim.Play("Roll");
    }

    public void OnRollEnd()
    {
        stats.invincible = false;
        isRolling = false;
    }

    public void OnAttackEnd()
    {
        isShooting = false;

        GameObject go = Instantiate(projectilePrefab, emitterTransform.position, Quaternion.identity);
        Arrow arrow = go.GetComponent<Arrow>();

        Vector3 direction = Camera.main.transform.forward;

        go.transform.rotation = Quaternion.LookRotation(direction);

        arrow.rb.AddForce(direction * arrow.speed, ForceMode.Impulse);
    }

    public void PlayPickupSound()
    {
        audioSource.PlayOneShot(pickupClip);
    }

    public void PlayDamageSound()
    {
        audioSource.PlayOneShot(damageTakenClip);
    }
}