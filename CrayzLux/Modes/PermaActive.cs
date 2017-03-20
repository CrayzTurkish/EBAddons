using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System.Linq;
using Color = System.Drawing.Color;

namespace CrayzLux.Modes
{
    public sealed class PermaActive : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return true;
        }

        public override void Execute()
        {

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(t => t.IsEnemy).Where(t => Program._Player.GetAutoAttackRange() >= t.Distance(Program._Player)).Where(t => t.IsValidTarget()))
            {
                if (Damage.LuxPassive(enemy))
                {
                    Orbwalker.ForcedTarget = enemy;
                }
                else
                {
                    Orbwalker.ForcedTarget = null;
                }
            }


            if (Config.Modes.Misc.rSteal && R.IsReady())
            {
                foreach (
                    var monster in
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(target => target.IsMonster)
                            .Where(target => R.IsInRange(target))
                            .Where(target => target.Health < Damage.RDamage(target) * 2))
                {
                    if (monster.Name.Contains("Baron") && R.IsInRange(monster))
                    {
                        R.Cast(monster);
                    }
                    else if (monster.Name.Contains("Dragon") && R.IsInRange(monster))
                    {
                        R.Cast(monster);
                    }
                }
            }

            //E2 cast
            /*TODO: FIX objects
            if (Program.EObject != null)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemy.IsValidTarget() && enemy.IsEnemy && Vector3.Distance(Program.EObject.Position, enemy.Position) <= E.Width + 15))
                {
                    Program.DrawLog("Wybucham E w " + enemy.Name + " bije za:" + Damage.EDamage(enemy), Color.MediumVioletRed);
                    E2.Cast();
                }
            }
            */
            //W cast
            foreach (var ally in ObjectManager.Get<Obj_AI_Base>().Where(ally => ally.IsValid() && ally.IsAlly && !ally.IsMinion && !ally.IsMonster && Vector3.Distance(Player.Instance.Position, ally.Position) <= W.Width))
            {
                // if (ally.IsStunned || !ally.IsCharmed || !ally.IsFeared || ally.HealthPercent < 60 && ally.CountEnemiesInRange(700) > 0) bugged!!
                if (ally.CountEnemiesInRange(600) > 0)
                {
                    W.Cast(ally);
                }
            }
        }
    }
}