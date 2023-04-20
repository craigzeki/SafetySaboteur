
using UnityEngine;

public interface iTowerWeapon
{
    public void FireSingle(int damage, iTakesDamage damageReceiver);
    public void FireSingle(int damage, Transform targetObject);
    public void FireContinuous(int damage, float damageInterval, iTakesDamage damageReceiver);

    public void Stop();

    public bool IsLoaded();
}
