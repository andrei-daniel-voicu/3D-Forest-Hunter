using UnityEngine;
using UnityEngine.UI;

public class Eagle : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float flySpeed;
    [SerializeField] private float attackChance;
    [SerializeField] private int damage;
    [SerializeField] public int experience;

    [SerializeField] private Slider healthSlider;

    private Vector3 flyDirection;

    private Animator anim;

    private Rigidbody rb;

    private bool dead;

    void Start()
    {
        dead = false;

        anim = GetComponent<Animator>();
           
        rb = GetComponent<Rigidbody>();

        healthSlider.maxValue = health;
        healthSlider.value = health;

        flyDirection = Random.onUnitSphere;
        if (flyDirection.y < 0)
            flyDirection.y = -flyDirection.y;
        transform.LookAt(transform.position + flyDirection);

        anim.Play("Fly");
    }

    void Update()
    {
        if (dead)
            return;
    }

    private void FixedUpdate()
    {
        if (dead)
            return;

        rb.AddForce(flyDirection * flySpeed, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dead)
            return;

        if (other.CompareTag("Arrow"))
        {
            health -= other.GetComponent<Arrow>().damage + 1 * GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().multiplier;
            healthSlider.value = health;

            if (health <= 0)
            {
                Destroy(gameObject);
                GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().IncreaseXP(experience);
                GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().killed++;
                GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().killTimer = 4;
            }
        }
    }

    public void DestroyAfter()
    {
        Destroy(gameObject); 
    }

    public void Die()
    {
        dead = true;
        anim.Play("Die");
        Invoke("DestroyAfter", 10);
        healthSlider.gameObject.SetActive(false);
        rb.useGravity = true;
        GetComponent<AudioSource>().enabled = false;
    }
}
