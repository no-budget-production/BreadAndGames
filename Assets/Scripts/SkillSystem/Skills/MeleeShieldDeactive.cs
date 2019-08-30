public class MeleeShieldDeactive : Skill
{
    public BuffObject BuffObjectToRemove;
    private bool active = false;

    public override void Shoot()
    {
        if (active)
            return;
        active = true;
        var handler = Character.GetComponentInChildren<MeleeShieldHandler>();
        if (handler == null)
            return;
        handler.Deactive();

        Character.RemoveBuff(BuffObjectToRemove);
    }
    public override void StopShoot()
    {
        active = false;
    }
}
