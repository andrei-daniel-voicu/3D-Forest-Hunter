using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wolf : MonoBehaviour
{
    [SerializeField] private int health;

    [SerializeField] private Slider healthSlider;

    private bool dead;

    private void Start()
    {
        dead = false;

        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dead)
            return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Stats>().DecreaseHealth(1);
        }
         if (other.CompareTag("Arrow"))
        {
            health -= other.GetComponent<Arrow>().damage + 1 * GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().multiplier;
            healthSlider.value = health;

            if (health <= 0)
            {
                gameObject.SetActive(false);
                GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().IncreaseXP(20);
                GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().killed++;
                GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().killTimer = 4;
            }
        }
    }
}
