using UnityEngine;

public class MovingBox : MonoBehaviour
{
    [SerializeField] private float waitingTime;

    private Vector3 _startPosition;
    private float _currentTime;

    private void Start()
    {
        _startPosition = transform.position;
        _currentTime = 0f;
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if ( _currentTime > waitingTime )
        {
            _currentTime = 0f;
        }
    }
}
