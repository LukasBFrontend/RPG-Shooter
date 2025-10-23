using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using URP = UnityEngine.Rendering.Universal;


public class CameraMove : Singleton<CameraMove>
{
    [SerializeField] GameObject player;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] Vector2 freeMovementBox = new(3f, 2f);
    [SerializeField] float followSpeed = 5f;

    Vector3 startPosition;
    Vector3 targetPosition;
    Vector2 cameraUnitSize;
    float transitionDuration = 1f;
    float elapsed = 0f;
    bool isTransitioning, isReversing, isFollowingPlayer, anchorToRoom, nudgeXEnabled, nudgeYEnabled, playerReached = false;
    URP.PixelPerfectCamera pixelCam;

    public struct RoomEdgeTracker
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

        public IEnumerable<KeyValuePair<string, bool>> GetSides()
        {
            yield return new KeyValuePair<string, bool>("Left", Left);
            yield return new KeyValuePair<string, bool>("Right", Right);
            yield return new KeyValuePair<string, bool>("Bottom", Bottom);
            yield return new KeyValuePair<string, bool>("Top", Top);
        }

        public override string ToString()
        {
            return $"{{ left: {Left}, right: {Right}, bottom: {Bottom}, top: {Top} }}";
        }
    }

    void Start()
    {
        targetPosition = transform.position;
        pixelCam = gameObject.GetComponent<URP.PixelPerfectCamera>();
        cameraUnitSize = new(pixelCam.refResolutionX / pixelCam.assetsPPU, pixelCam.refResolutionY / pixelCam.assetsPPU);

        AnchorToRoom();
    }

    void Update()
    {
        if (isFollowingPlayer)
            FollowPlayer();
        else if (isTransitioning)
            TransitionStep();
        else if (anchorToRoom)
        {
            FollowConstrained();
        }
    }

    public void AnchorToRoom()
    {
        anchorToRoom = true;
    }
    RoomEdgeTracker touchingRoomSide;
    void FollowConstrained()
    {


        Room room = RoomManager.Instance.GetActiveRoom();
        Vector2 roomPos = room.transform.position;
        Vector2 cameraPos = transform.position;


        float xMin = roomPos.x - room.size.x / 2 + cameraUnitSize.x / 2;
        float xMax = roomPos.x + room.size.x / 2 - cameraUnitSize.x / 2;
        float yMin = roomPos.y - room.size.y / 2 + cameraUnitSize.y / 2;
        float yMax = roomPos.y + room.size.y / 2 - cameraUnitSize.y / 2;

        touchingRoomSide = new RoomEdgeTracker(
            cameraPos.x <= xMin,
            cameraPos.x >= xMax,
            cameraPos.y <= yMin,
            cameraPos.y >= yMax
        );

        Vector2 newPos = TrackPlayerSmooth();
        if (playerReached) return;


        float clampedX = Mathf.Clamp(newPos.x, xMin, xMax);
        float clampedY = Mathf.Clamp(newPos.y, yMin, yMax);


        transform.position = new(clampedX, clampedY, transform.position.z);
    }

    Vector2 TrackPlayerSmooth()
    {
        float width = freeMovementBox.x;
        float height = freeMovementBox.y;
        const float threshold = .1f;

        Vector3 cameraPos = transform.position;
        Vector2 playerToCamera = player.transform.position - cameraPos;

        int sidesTouched = 0;

        foreach (var side in touchingRoomSide.GetSides())
        {
            sidesTouched += side.Value ? 1 : 0;
        }

        bool insideYStrip = Math.Abs(playerToCamera.y) < height / 2;
        bool insideXStrip = Math.Abs(playerToCamera.x) < width / 2;
        bool insideFreeZone;

        switch (sidesTouched)
        {
            case 0:
                insideFreeZone = insideXStrip && insideYStrip;

                if (playerToCamera.magnitude > threshold && !insideFreeZone) playerReached = false;
                if (playerToCamera.magnitude <= threshold) playerReached = true;

                break;
            case 1:
                if (touchingRoomSide.Left)
                {
                    insideFreeZone = insideYStrip && playerToCamera.x < width / 2;

                    if (playerToCamera.magnitude > threshold && !insideFreeZone) playerReached = false;
                    else if (Math.Abs(playerToCamera.y) <= threshold) playerReached = true;
                }
                else if (touchingRoomSide.Right)
                {

                    insideFreeZone = insideYStrip && playerToCamera.x > -width / 2;

                    if (playerToCamera.magnitude > threshold && !insideFreeZone) playerReached = false;
                    else if (Math.Abs(playerToCamera.y) <= threshold) playerReached = true;
                }
                else if (touchingRoomSide.Bottom)
                {
                    insideFreeZone = insideXStrip && playerToCamera.y < height / 2;

                    if (playerToCamera.magnitude > threshold && !insideFreeZone) playerReached = false;
                    else if (Math.Abs(playerToCamera.x) <= threshold) playerReached = true;
                }
                else if (touchingRoomSide.Top)
                {
                    insideFreeZone = insideXStrip && playerToCamera.y > -height / 2;

                    if (playerToCamera.magnitude > threshold && !insideFreeZone) playerReached = false;
                    else if (Math.Abs(playerToCamera.x) <= threshold) playerReached = true;
                }

                break;
            case 2:
                playerReached = false;
                break;
            default:
                break;
        }

        Vector2 smoothPos = Vector2.Lerp(
            transform.position,
            player.transform.position,
            followSpeed * Time.deltaTime
        );

        return smoothPos;
    }

    void FollowPlayer()
    {
        Vector3 playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }

    void TransitionStep()
    {

        float timeStep = Time.deltaTime;
        if (isReversing) timeStep *= -1f;
        elapsed += timeStep;
        float t = Mathf.Clamp01(elapsed / transitionDuration);


        float curveT = speedCurve.Evaluate(t);

        transform.position = Vector3.Lerp(startPosition, targetPosition, curveT);

        if (t >= 1f || t <= 0f)
        {
            isTransitioning = false;
            isReversing = false;
            AnchorToRoom();
        }
    }


    public void MoveToRoom(Room targetRoom)
    {
        Vector2 cameraPos = transform.position;
        Vector2 roomCenter = targetRoom.transform.position;
        Vector2 roomSize = targetRoom.size;

        float halfCamWidth = cameraUnitSize.x / 2;
        float halfCamHeight = cameraUnitSize.y / 2;

        // Compute room bounds
        float minX = roomCenter.x - roomSize.x / 2 + halfCamWidth;
        float maxX = roomCenter.x + roomSize.x / 2 - halfCamWidth;
        float minY = roomCenter.y - roomSize.y / 2 + halfCamHeight;
        float maxY = roomCenter.y + roomSize.y / 2 - halfCamHeight;

        // Clamp the camera’s *current* position to the closest valid point inside the room
        float clampedX = Mathf.Clamp(cameraPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(cameraPos.y, minY, maxY);

        Vector2 closestPoint = new Vector2(clampedX, clampedY);

        Debug.Log($"Closest available position: {closestPoint}");

        StartSmoothTransition(closestPoint, 1f);
    }




    public void StartSmoothTransition(Vector2 newTarget, float duration)
    {

        transitionDuration = duration;
        if (isTransitioning)
        {
            isReversing = !isReversing;
        }
        else
        {
            elapsed = 0f;
            startPosition = transform.position;
            targetPosition = new Vector3(newTarget.x, newTarget.y, transform.position.z);
        }
        isTransitioning = true;
        isFollowingPlayer = false;
    }

    public void SetFollowPlayer(bool follow)
    {
        isFollowingPlayer = follow;
        isTransitioning = false;
    }

    private void OnDrawGizmos()
    {
        float width = freeMovementBox.x;
        float height = freeMovementBox.y;

        if (width <= 0 || height <= 0) return;

        Vector2 pos = transform.position;

        Vector2 bottomLeft = new(-width / 2, -height / 2);
        Vector2 bottomRight = new(width / 2, -height / 2);
        Vector2 topLeft = new(-width / 2, height / 2);
        Vector2 topRight = new(width / 2, height / 2);

        /*         Debug.DrawLine(bottomLeft + pos, bottomRight + pos);
                Debug.DrawLine(bottomRight + pos, topRight + pos);
                Debug.DrawLine(topRight + pos, topLeft + pos);
                Debug.DrawLine(topLeft + pos, bottomLeft + pos);

                if (anchorToRoom && !playerReached) Debug.DrawLine(pos, player.transform.position); */
    }
}
