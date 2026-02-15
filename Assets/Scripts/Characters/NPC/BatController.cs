using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(NPC))]
public class BatController : MonoBehaviour
{
    [SerializeField] float playerDetectionRange = 5;
    public Vector2 ClusterSignal { get; set; }
    NPC _npc;
    Movement _movement;
    bool _isAwake = false;

    void Start()
    {
        Cache();
    }


    void Update()
    {
        if (!_isAwake)
        {
            if (!IsPlayerWithinDetection())
            {
                return;
            }

            _isAwake = true;
            _npc.Animator.SetBool("IsAwake", true);
        }

        FlyPlayerStraight();
    }

    void Cache()
    {
        _npc = GetComponent<NPC>();
        _movement = GetComponent<Movement>();
    }

    void FlyPlayerStraight()
    {
        Vector2 _playerToBat = Utils.PlayerToTransform(transform).normalized;
        _movement.Input = _playerToBat + ClusterSignal;
        _npc.SetFacing(_playerToBat);
    }

    bool IsPlayerWithinDetection()
    {
        Vector2 _playerToBat = Utils.PlayerToTransform(transform);
        return _playerToBat.sqrMagnitude <= Mathf.Pow(playerDetectionRange, 2);
    }
}
