using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Settings")]
    public float normalSpeed = 8f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 5f;

    private TrafficNode _currentNode;
    private TrafficNode _targetNode;
    private bool _isStopped;
    private float _currentSpeed;

    public void Initialize(TrafficNode startNode)
    {
        _currentNode = startNode;
        SetNextTarget();
        _currentSpeed = normalSpeed;
    }

    void Update()
    {
        if (_targetNode == null) return;

        HandleMovement();
        HandleTrafficLightCheck();
    }

    void HandleMovement()
    {
        if (_isStopped) return;

        // Basic movement
        Vector3 direction = _targetNode.transform.position - transform.position;
        transform.position = Vector3.MoveTowards(transform.position,
            _targetNode.transform.position,
            _currentSpeed * Time.deltaTime);

        // Rotation
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        // Check arrival
        if (Vector3.Distance(transform.position, _targetNode.transform.position) < 0.5f)
        {
            _currentNode = _targetNode;
            SetNextTarget();
        }
    }

    void HandleTrafficLightCheck()
    {
        if (_targetNode.nodeType != TrafficNode.NodeType.Intersection) return;

        float distanceToNode = Vector3.Distance(transform.position, _targetNode.transform.position);
        bool isWithinStoppingDistance = distanceToNode < _targetNode.stoppingDistance;
        bool shouldStop = isWithinStoppingDistance && !_targetNode.IsLaneGreen(_currentNode.transform);

        // If the car is slightly past the stopping distance but still stopped, allow it to move
        bool isPastStoppingPoint = distanceToNode < _targetNode.stoppingDistance * 0.8f;

        if (!shouldStop || isPastStoppingPoint)
        {
            _isStopped = false;
            _currentSpeed = normalSpeed;
        }
        else
        {
            _isStopped = true;
            _currentSpeed = 0;
        }
    }



    void SetNextTarget()
    {
        if (_currentNode.outgoingLanes.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        // Get first available lane
        _targetNode = _currentNode.outgoingLanes[0].GetComponent<TrafficNode>();

        // Immediate check for intersection nodes
        if (_targetNode.nodeType == TrafficNode.NodeType.Intersection)
        {
            _isStopped = !_targetNode.IsLaneGreen(_currentNode.transform);
        }
    }
}