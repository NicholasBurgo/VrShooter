using UnityEngine;
using System.Collections.Generic;

public class CarSpawnerManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Nodes where cars can spawn")]
    public TrafficNode[] spawnNodes;

    [Tooltip("Car prefab to spawn")]
    public GameObject carPrefab;

    [Range(0.1f, 60f), Tooltip("Cars per minute")]
    public float spawnRate = 30f;

    [Tooltip("Maximum simultaneous active cars")]
    public int maxCars = 20;

    [Tooltip("Offset distance to spawn the car to the right of the node")]
    public float spawnOffset = 2f;

    [Header("Debug")]
    [SerializeField] private int _currentActiveCars;
    [SerializeField] private float _spawnTimer;

    private List<GameObject> _activeCars = new List<GameObject>();

    void Update()
    {
        HandleSpawning();
        CleanupDestroyedCars();
    }

    void HandleSpawning()
    {
        if (ShouldSpawnCar())
        {
            SpawnCar();
            ResetSpawnTimer();
        }
        else
        {
            _spawnTimer -= Time.deltaTime;
        }
    }

    bool ShouldSpawnCar()
    {
        return _spawnTimer <= 0f &&
               _activeCars.Count < maxCars &&
               spawnNodes.Length > 0 &&
               carPrefab != null;
    }
    void SpawnCar()
    {
        TrafficNode spawnNode = spawnNodes[Random.Range(0, spawnNodes.Length)];
        TrafficNode nextNode = spawnNode.GetNextNode();

        // If there's no next node, just spawn at the node without offset
        if (nextNode == null)
        {
            // Fallback: no next node means we can’t compute direction
            GameObject fallbackCar = Instantiate(carPrefab, spawnNode.transform.position, spawnNode.transform.rotation);
            _activeCars.Add(fallbackCar);
            _currentActiveCars = _activeCars.Count;
            return;
        }

        // 1. Compute direction from this node to the next node
        Vector3 roadDirection = (nextNode.transform.position - spawnNode.transform.position).normalized;

        // 2. Compute a “right” vector by crossing with global up (assuming roads are flat on XZ plane)
        Vector3 rightSide = Vector3.Cross(Vector3.up, roadDirection).normalized;

        // 3. Offset the spawn position
        Vector3 spawnPosition = spawnNode.transform.position + rightSide * spawnOffset;

        // 4. Spawn the car
        GameObject newCar = Instantiate(carPrefab, spawnPosition, Quaternion.LookRotation(roadDirection, Vector3.up));

        // 5. Initialize the AI if needed
        if (newCar.TryGetComponent<CarAI>(out var carAI))
        {
            carAI.Initialize(spawnNode);
        }

        _activeCars.Add(newCar);
        _currentActiveCars = _activeCars.Count;
    }



    void ResetSpawnTimer()
    {
        _spawnTimer = 60f / spawnRate;
    }

    void CleanupDestroyedCars()
    {
        _activeCars.RemoveAll(car => car == null);
        _currentActiveCars = _activeCars.Count;
    }

    private void OnValidate()
    {
        _spawnTimer = 60f / spawnRate;
    }
}
