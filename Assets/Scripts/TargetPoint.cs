using UnityEngine;

public class TargetPoint
{
    public Vector3 Position;
    public int UniqueID;
    public iTakesDamage damageInterface;
    public int InactiveCount = 0;
    public bool IsActive = true;

    public TargetPoint(Vector3 position, int uniqueID, iTakesDamage damageInterface)
    {
        Position = position;
        UniqueID = uniqueID;
        this.damageInterface = damageInterface;
    }
}
