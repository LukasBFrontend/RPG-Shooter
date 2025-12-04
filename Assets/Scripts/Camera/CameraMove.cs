using System;
using System.Collections.Generic;
using UnityEngine;
using URP = UnityEngine.Rendering.Universal;


public class CameraMove : Singleton<CameraMove>
{
    struct RoomEdgeTracker
    {
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Bottom { get; set; }
        public bool Top { get; set; }

        public RoomEdgeTracker(bool left, bool right, bool bottom, bool top)
        {
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
        }

        public readonly IEnumerable<KeyValuePair<string, bool>> GetSides()
        {
            yield return new KeyValuePair<string, bool>("Left", Left);
            yield return new KeyValuePair<string, bool>("Right", Right);
            yield return new KeyValuePair<string, bool>("Bottom", Bottom);
            yield return new KeyValuePair<string, bool>("Top", Top);
        }

        public override readonly string ToString()
        {
            return $"{{ left: {Left}, right: {Right}, bottom: {Bottom}, top: {Top} }}";
        }
    }
    [SerializeField] GameObject player;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] Vector2 freeMovementBox = new(3f, 2f);
    [SerializeField] float followSpeed = 5f;
    Vector3 _startPosition;
    Vector3 _targetPosition;
    Vector2 _cameraUnitSize;
    float _transitionDuration = 1f;
    float _elapsed = 0f;
    bool _isTransitioning, _isReversing, _isFollowingPlayer, _anchorToRoom, _playerReached = false;
    URP.PixelPerfectCamera _pixelCam;
    RoomEdgeTracker _touchingRoomSide;

    void Start()
    {
        _targetPosition = transform.position;
        _pixelCam = gameObject.GetComponent<URP.PixelPerfectCamera>();
        _cameraUnitSize = new(_pixelCam.refResolutionX / _pixelCam.assetsPPU, _pixelCam.refResolutionY / _pixelCam.assetsPPU);

        AnchorToRoom();
    }

    void Update()
    {
        if (_isFollowingPlayer)
            FollowPlayer();
        else if (_isTransitioning)
            TransitionStep();
        else if (_anchorToRoom)
        {
            FollowConstrained();
        }
    }

    public void AnchorToRoom()
    {
        _anchorToRoom = true;
    }

    void FollowConstrained()
    {
        Room _room = RoomManager.ActiveRoom;
        Vector2 _roomPos = _room.transform.position;
        Vector2 _cameraPos = transform.position;

        float _xMin = _roomPos.x - _room.Size.x / 2 + _cameraUnitSize.x / 2;
        float _xMax = _roomPos.x + _room.Size.x / 2 - _cameraUnitSize.x / 2;
        float _yMin = _roomPos.y - _room.Size.y / 2 + _cameraUnitSize.y / 2;
        float _yMax = _roomPos.y + _room.Size.y / 2 - _cameraUnitSize.y / 2;

        _touchingRoomSide = new RoomEdgeTracker(
            _cameraPos.x <= _xMin,
            _cameraPos.x >= _xMax,
            _cameraPos.y <= _yMin,
            _cameraPos.y >= _yMax
        );

        Vector2 _newPos = TrackPlayerSmooth();
        if (_playerReached) return;

        float _clampedX = Mathf.Clamp(_newPos.x, _xMin, _xMax);
        float _clampedY = Mathf.Clamp(_newPos.y, _yMin, _yMax);

        transform.position = new(_clampedX, _clampedY, transform.position.z);
    }

    Vector2 TrackPlayerSmooth()
    {
        float _width = freeMovementBox.x;
        float _height = freeMovementBox.y;
        const float THRESHOLD = .1f;

        Vector3 _cameraPos = transform.position;
        Vector2 _playerToCamera = player.transform.position - _cameraPos;

        int _sidesTouched = 0;

        foreach (var side in _touchingRoomSide.GetSides())
        {
            _sidesTouched += side.Value ? 1 : 0;
        }

        bool _insideYStrip = Math.Abs(_playerToCamera.y) < _height / 2;
        bool _insideXStrip = Math.Abs(_playerToCamera.x) < _width / 2;
        bool _insideFreeZone;

        switch (_sidesTouched)
        {
            case 0:
                _insideFreeZone = _insideXStrip && _insideYStrip;

                if (_playerToCamera.magnitude > THRESHOLD && !_insideFreeZone) _playerReached = false;
                if (_playerToCamera.magnitude <= THRESHOLD) _playerReached = true;

                break;
            case 1:
                if (_touchingRoomSide.Left)
                {
                    _insideFreeZone = _insideYStrip && _playerToCamera.x < _width / 2;

                    if (_playerToCamera.magnitude > THRESHOLD && !_insideFreeZone) _playerReached = false;
                    else if (Math.Abs(_playerToCamera.y) <= THRESHOLD) _playerReached = true;
                }
                else if (_touchingRoomSide.Right)
                {

                    _insideFreeZone = _insideYStrip && _playerToCamera.x > -_width / 2;

                    if (_playerToCamera.magnitude > THRESHOLD && !_insideFreeZone) _playerReached = false;
                    else if (Math.Abs(_playerToCamera.y) <= THRESHOLD) _playerReached = true;
                }
                else if (_touchingRoomSide.Bottom)
                {
                    _insideFreeZone = _insideXStrip && _playerToCamera.y < _height / 2;

                    if (_playerToCamera.magnitude > THRESHOLD && !_insideFreeZone) _playerReached = false;
                    else if (Math.Abs(_playerToCamera.x) <= THRESHOLD) _playerReached = true;
                }
                else if (_touchingRoomSide.Top)
                {
                    _insideFreeZone = _insideXStrip && _playerToCamera.y > -_height / 2;

                    if (_playerToCamera.magnitude > THRESHOLD && !_insideFreeZone) _playerReached = false;
                    else if (Math.Abs(_playerToCamera.x) <= THRESHOLD) _playerReached = true;
                }

                break;
            case 2:
                _playerReached = false;
                break;
            default:
                break;
        }

        Vector2 _smoothPos = Vector2.Lerp(
            transform.position,
            player.transform.position,
            followSpeed * Time.deltaTime
        );

        return _smoothPos;
    }

    void FollowPlayer()
    {
        Vector3 _playerPos = player.transform.position;
        transform.position = new Vector3(_playerPos.x, _playerPos.y, transform.position.z);
    }

    void TransitionStep()
    {
        float _timeStep = Time.deltaTime;
        if (_isReversing)
        {
            _timeStep *= -1f;
        }
        _elapsed += _timeStep;

        float _t = Mathf.Clamp01(_elapsed / _transitionDuration);
        float _curveT = speedCurve.Evaluate(_t);

        transform.position = Vector3.Lerp(_startPosition, _targetPosition, _curveT);

        if (_t >= 1f || _t <= 0f)
        {
            _isTransitioning = false;
            _isReversing = false;
            AnchorToRoom();
        }
    }

    public void MoveToRoom(Room _targetRoom)
    {
        Vector2 cameraPos = transform.position;
        Vector2 roomCenter = _targetRoom.transform.position;
        Vector2 roomSize = _targetRoom.Size;

        float halfCamWidth = _cameraUnitSize.x / 2;
        float halfCamHeight = _cameraUnitSize.y / 2;

        float minX = roomCenter.x - roomSize.x / 2 + halfCamWidth;
        float maxX = roomCenter.x + roomSize.x / 2 - halfCamWidth;
        float minY = roomCenter.y - roomSize.y / 2 + halfCamHeight;
        float maxY = roomCenter.y + roomSize.y / 2 - halfCamHeight;

        float clampedX = Mathf.Clamp(cameraPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(cameraPos.y, minY, maxY);

        Vector2 closestPoint = new(clampedX, clampedY);

        StartSmoothTransition(closestPoint, 1f);
    }

    public void StartSmoothTransition(Vector2 newTarget, float duration)
    {
        _transitionDuration = duration;

        if (_isTransitioning)
        {
            _isReversing = !_isReversing;
        }
        else
        {
            _elapsed = 0f;
            _startPosition = transform.position;
            _targetPosition = new Vector3(newTarget.x, newTarget.y, transform.position.z);
        }
        _isTransitioning = true;
        _isFollowingPlayer = false;
    }

    public void SetFollowPlayer(bool follow)
    {
        _isFollowingPlayer = follow;
        _isTransitioning = false;
    }

    void OnDrawGizmos()
    {
        float _width = freeMovementBox.x;
        float _height = freeMovementBox.y;

        if (_width <= 0 || _height <= 0)
        {
            return;
        }

        Vector2 _pos = transform.position;

        Vector2 _bottomLeft = new(-_width / 2, -_height / 2);
        Vector2 _bottomRight = new(_width / 2, -_height / 2);
        Vector2 _topLeft = new(-_width / 2, _height / 2);
        Vector2 _topRight = new(_width / 2, _height / 2);

        Debug.DrawLine(_bottomLeft + _pos, _bottomRight + _pos);
        Debug.DrawLine(_bottomRight + _pos, _topRight + _pos);
        Debug.DrawLine(_topRight + _pos, _topLeft + _pos);
        Debug.DrawLine(_topLeft + _pos, _bottomLeft + _pos);

        if (_anchorToRoom && !_playerReached)
        {
            Debug.DrawLine(_pos, player.transform.position);
        }
    }
}
