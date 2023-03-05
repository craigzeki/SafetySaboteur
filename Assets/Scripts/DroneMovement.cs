using UnityEngine;
using PathCreation;
using System.Collections;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class DroneMovement : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    float distanceTravelled;

    [SerializeField] private bool _hoverAnimationEnabled = false;
    [SerializeField] private float _hoverPeriod = 1f; //in seconds
    [SerializeField] private float _hoverYOffset = 0.2f;
    [SerializeField] private bool _moving = false;
    private Coroutine _lerpCoroutine;
    private float _lerpTimeElapsed = 0f;
    private float _lerpT = 0f;
    private Vector3 _hoverPosition = Vector3.zero;
    private Vector3 _prevHoverPosition = Vector3.zero;

    public bool Moving { get => _moving; set => _moving = value; }

    void Start()
    {
        if (pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;
        }


        StartStopHover();

    }

    void Update()
    {
        if (pathCreator != null && _moving)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

        }

        transform.position += _hoverPosition;
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

    private void StartStopHover()
    {
        if(_lerpCoroutine != null)
        {
            StopCoroutine(_lerpCoroutine);
            _lerpCoroutine = null;
        }
        else
        {
            _lerpCoroutine = StartCoroutine(DoHover());
        }
    }

    IEnumerator DoHover()
    {


        while(_hoverAnimationEnabled)
        {
            _lerpTimeElapsed = 0;
            _prevHoverPosition = Vector3.zero;
            _prevHoverPosition.y = -_hoverYOffset;
            //do upward motion
            while (_lerpTimeElapsed < (_hoverPeriod / 2))
            {
                _lerpT = _lerpTimeElapsed / (_hoverPeriod / 2);
                _lerpT = _lerpT * _lerpT * (3f - (2f * _lerpT)); //smooth step
                _hoverPosition.y = Mathf.Lerp(-_hoverYOffset, _hoverYOffset, _lerpT) - _prevHoverPosition.y;
                _prevHoverPosition = _hoverPosition;
                _lerpTimeElapsed += Time.deltaTime;
                yield return null;
            }
            _hoverPosition.y = _hoverYOffset;
            //do downward motion
            _lerpTimeElapsed = 0;
            _prevHoverPosition = Vector3.zero;
            _prevHoverPosition.y = _hoverYOffset;
            while (_lerpTimeElapsed < (_hoverPeriod / 2))
            {
                _lerpT = _lerpTimeElapsed / (_hoverPeriod / 2);
                _lerpT = _lerpT * _lerpT * (3f - (2f * _lerpT)); //smooth step
                _hoverPosition.y = (-1 * Mathf.Lerp(-_hoverYOffset, _hoverYOffset, _lerpT)) - _prevHoverPosition.y;
                _prevHoverPosition = _hoverPosition;
                _lerpTimeElapsed += Time.deltaTime;
                yield return null;
            }
            _hoverPosition.y = -_hoverYOffset;
        }

        _hoverPosition.y = 0;
    }
}

