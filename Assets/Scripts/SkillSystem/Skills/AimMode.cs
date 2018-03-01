public class AimMode : Skill
{
    public override void Shoot()
    {
        Character.AddBuff(BuffObject, 1, Character);
    }
}
