using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC : Character
{
    public Collider2D[] RaycastIgnore;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] Attack primaryAttack;
    [SerializeField] Attack secondaryAttack;
    Vector2 _previousFaceDir = new();
    public float DetectionRange { get { return detectionRange; } }
    public Attack PrimaryAttack { get { return primaryAttack; } }
    public Attack SecondaryAttack { get { return secondaryAttack; } }
    void Awake()
    {
        OnDeath = () => Destroy(gameObject);
        CollisionIgnoreTags = Utils.EnemyTags;
    }

    void Update()
    {
        if (Health < 0)
        {
            Die();
        }
        //TurnNPCSmooth();
    }

    /// <summary>
    /// Sets the NPC transform rotation equivalent to it's face direction, ignoring single frame changes
    /// </summary>
    void TurnNPCSmooth()
    {
        bool _isFaceDirSame = _previousFaceDir == FaceDir;
        _previousFaceDir = FaceDir;

        if (!_isFaceDirSame)
        {
            return;
        }

        float _angle = -Mathf.Round(Mathf.Atan2(FaceDir.y, FaceDir.x) * Mathf.Rad2Deg + 90);

        transform.rotation = Quaternion.Euler(new(0, 0, Mathf.Round(Mathf.Atan2(FaceDir.y, FaceDir.x) * Mathf.Rad2Deg + 90)));
        /* Pixelate pixelate = GetComponentInChildren<Pixelate>();
        if (pixelate)
        {
            pixelate.Rotation = Quaternion.Euler(new(0, 0, _angle));
        }
        else
        {
            Debug.LogError($"No pixelate script found on {gameObject.name}, could not render face direction.");
        } */
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Player>(out var player))
        {
            return;
        }
        primaryAttack.CharactersInRange.Add(player);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<Player>(out var player))
        {
            return;
        }
        primaryAttack.CharactersInRange.Remove(player);
    }
}
