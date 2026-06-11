namespace SBabchuk
{
    public class MeleeEnemyController : EnemyControllerBase
    {
        public override void Attacked()
        {
            WaitNextAttack(Properties.AttackSpeed);
        }
    }
}