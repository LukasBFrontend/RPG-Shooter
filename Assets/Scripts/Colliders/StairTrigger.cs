using UnityEngine;

[System.Serializable]
public struct StairLevel
{
    public StairLevel(Direction direction, int zLvl)
    {
        Direction = direction;
        ZLvl = zLvl;
    }

    [Range(0, 2)] public int ZLvl;
    public Direction Direction;

}
public class StairTrigger : Trigger
{
    [SerializeField] StairLevel lowerLevel;
    [SerializeField] StairLevel higherLevel;

    void SetZLayer(GameObject obj, int zLvl)
    {
        int z0 = 16;
        int z1 = 17;
        int z2 = 18;
        switch (zLvl)
        {
            case 0:
                obj.layer = z0;
                break;
            case 1:
                obj.layer = z1;
                break;
            case 2:
                obj.layer = z2;
                break;
            default:
                Debug.LogError("Bad value '" + zLvl + "', zLvl must be 0, 1 or 2. Layer not assigned");
                break;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player Trigger") && !other.CompareTag("Enemy")) return;
        if (FromDirection(other) == lowerLevel.Direction)
        {
            SetZLayer(other.CompareTag("Player Trigger") ? PlayerConfig.Instance.gameObject : other.gameObject, higherLevel.ZLvl);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player Trigger") && !other.CompareTag("Enemy")) return;

        if (FromDirection(other) == lowerLevel.Direction)
        {
            SetZLayer(other.CompareTag("Player Trigger") ? PlayerConfig.Instance.gameObject : other.gameObject, lowerLevel.ZLvl);
        }
    }
}
