namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public class MeleeEnemyController : EnemyControllerBase
    {
        public override void Attacked()
        {
            WaitNextAttack(Properties.AttackSpeed);
        }
    }
}
