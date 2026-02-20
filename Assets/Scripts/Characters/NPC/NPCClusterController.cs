using System.Collections.Generic;
using UnityEngine;

public class NPCClusterController : MonoBehaviour
{
    List<NPCStateMachine> _npc = new();

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
        _npc.AddRange(GetComponentsInChildren<NPCStateMachine>());
    }

    void SignalControllers()
    {
        foreach (NPCStateMachine x in _npc)
        {
            if (x == null)
            {
                _npc.Remove(x);
                continue;
            }

            Vector2 _position = x.transform.position;
            NPCStateMachine _closestBat = null;
            float _closestDistance = Mathf.Infinity;

            foreach (NPCStateMachine y in _npc)
            {
                if (y == null)
                {
                    _npc.Remove(y);
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
