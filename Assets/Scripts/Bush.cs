using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] private GameObject eagle;
    [SerializeField] private GameObject eagleElite;
    [SerializeField] private float eliteProbability;
    [SerializeField] private int minSpawn;
    [SerializeField] private int maxSpawn;

    private bool spawned;

    private void Start()
    {
        spawned = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawned)
            return;

        if(other.CompareTag("Player"))
        {
            spawned = true;
            int toSpawn = Random.Range(minSpawn, maxSpawn + 1);
            for (int i = 0; i < toSpawn; i++)
            {
                float rand = Random.Range(0f, 1f);
                GameObject prefabToSpawn = eagle;
                if (rand <= eliteProbability)
                    prefabToSpawn = eagleElite;
                Instantiate(prefabToSpawn, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            }
        }
    }
}
