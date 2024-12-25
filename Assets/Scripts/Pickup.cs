using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Pickup : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int type;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == 0)
                other.GetComponent<Stats>().IncreaseAmmo(1);
            else
                other.GetComponent<Stats>().IncreaseHealth(1);

            other.GetComponent<CharacterController>().PlayPickupSound();
            Destroy(gameObject);
        }
    }
}
