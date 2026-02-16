using UnityEngine;

public class SpikeBallSlotTrigger : MonoBehaviour
{
    [SerializeField] Animator animator;
    SpikeBallController _spikeBallController;
    bool _isLocked = false;
    const float FUSE_THRESHOLD = .25f;

    void Update()
    {
        if (_isLocked || _spikeBallController == null)
        {
            return;
        }
        float _dist = Vector2.Distance(transform.position, _spikeBallController.transform.position);

        if (_dist < FUSE_THRESHOLD)
        {
            Destroy(_spikeBallController.gameObject);
            animator.SetBool("IsFusing", true);
            _isLocked = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<SpikeBallController>(out var spikeBall))
        {
            _spikeBallController = spikeBall;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<SpikeBallController>(out var spikeBall))
        {
            _spikeBallController = null;
        }
    }
}
