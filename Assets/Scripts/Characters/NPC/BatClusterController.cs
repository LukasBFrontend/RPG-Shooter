using System.Collections.Generic;
using UnityEngine;

public class BatClusterController : MonoBehaviour
{
    List<BatController> _bats = new();

    void Start()
    {
        Cache();
    }


    void Update()
    {
        SignalControllers();
    }

    void Cache()
    {
        _bats.AddRange(GetComponentsInChildren<BatController>());
    }

    void SignalControllers()
    {
        foreach (BatController x in _bats)
        {
            if (x == null)
            {
                _bats.Remove(x);
                continue;
            }

            Vector2 _position = x.transform.position;
            BatController _closestBat = null;
            float _closestDistance = Mathf.Infinity;

            foreach (BatController y in _bats)
            {
                if (y == null)
                {
                    _bats.Remove(y);
                    continue;
                }
                if (x == y)
                {
                    continue;
                }

                Vector2 _otherPosition = y.transform.position;
                float _distance = Vector2.Distance(_position, _otherPosition);

                if (_distance < _closestDistance)
                {
                    _closestBat = y;
                    _closestDistance = _distance;
                }
            }

            if (_closestBat == null)
            {
                continue;
            }
            Vector2 _directionToClosest = (Vector2)_closestBat.transform.position - _position;

            x.ClusterSignal = _closestDistance <= 1f ? -_directionToClosest.normalized : Vector2.zero;
        }
    }
}
