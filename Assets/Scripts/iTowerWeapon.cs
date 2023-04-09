
public interface iTowerWeapon
{
    public void FireSingle(int damage, iTakesDamage damageReceiver);
    public void FireContinuous(int damage, float damageInterval, iTakesDamage damageReceiver);

    public void Stop();
}
