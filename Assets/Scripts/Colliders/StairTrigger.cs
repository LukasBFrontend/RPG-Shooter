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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*     [Range(0, 2)][SerializeField] int lowerZ;
        [Range(0, 2)][SerializeField] int upperZ;
        [SerializeField] Direction lowerDirection;
        [SerializeField] Direction upperDirection; */
    [SerializeField] StairLevel lowerLevel;
    [SerializeField] StairLevel higherLevel;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
        if (!other.CompareTag("Player Trigger")) return;
        if (FromDirection(other) == lowerLevel.Direction)
        {
            SetZLayer(other.CompareTag("Player Trigger") ? PlayerConfig.Instance.gameObject : other.gameObject, higherLevel.ZLvl);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player Trigger")) return;

        if (FromDirection(other) == lowerLevel.Direction)
        {
            SetZLayer(other.CompareTag("Player Trigger") ? PlayerConfig.Instance.gameObject : other.gameObject, lowerLevel.ZLvl);
        }
    }
}
