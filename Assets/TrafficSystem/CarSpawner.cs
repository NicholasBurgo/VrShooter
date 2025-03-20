using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    [Tooltip("List of car prefabs to randomly choose from when spawning.")]
    public List<GameObject> carPrefabs;

    [Tooltip("List of waypoints (including spawn node, intersection nodes, and an exit node) for the car route.")]
    public List<Transform> routeWaypoints;

    [Tooltip("Minimum time interval (in seconds) between car spawns.")]
    public float minSpawnInterval = 2f;

    [Tooltip("Maximum time interval (in seconds) between car spawns.")]
    public float maxSpawnInterval = 5f;

    void Start()
    {
        StartCoroutine(SpawnCar());
    }

    IEnumerator SpawnCar()
    {
        while (true)
        {
            float randomSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomSpawnTime);

            if (carPrefabs.Count > 0)
            {
                int randomIndex = Random.Range(0, carPrefabs.Count);
                GameObject selectedCarPrefab = carPrefabs[randomIndex];

                GameObject car = Instantiate(selectedCarPrefab, transform.position, transform.rotation);
                CarController controller = car.GetComponent<CarController>();
                if (controller != null)
                {
                    controller.waypoints = routeWaypoints;
                }
            }
        }
    }

    // Draw a green cube to mark the spawn point, and draw the route.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);

        if (routeWaypoints == null || routeWaypoints.Count == 0)
            return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < routeWaypoints.Count; i++)
        {
            if (routeWaypoints[i] != null)
            {
                Gizmos.DrawSphere(routeWaypoints[i].position, 0.3f);
                if (i < routeWaypoints.Count - 1 && routeWaypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(routeWaypoints[i].position, routeWaypoints[i + 1].position);
                }
            }
        }
    }
}
