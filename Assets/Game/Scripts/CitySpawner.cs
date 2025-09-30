using UnityEngine;

public class CitySpawner : MonoBehaviour
{
    [Header("City Prefabs")]
    public GameObject[] cityPrefabs;   // assign 3 city prefabs in Inspector

    private void Start()
    {
        SpawnRandomCity();
    }

    private void SpawnRandomCity()
    {
        if (cityPrefabs == null || cityPrefabs.Length == 0)
        {
            Debug.LogWarning("No city prefabs assigned!");
            return;
        }

        int randomIndex = Random.Range(0, cityPrefabs.Length);

        // ✅ Instantiate at prefab’s original stored position & rotation
        GameObject city = Instantiate(cityPrefabs[randomIndex]);

        // ✅ Make it a child of this GameObject (keep its local transform intact)
        city.transform.SetParent(transform, false);

        city.name = $"City_{randomIndex + 1}";
    }
}
