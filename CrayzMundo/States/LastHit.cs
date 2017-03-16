using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace CrayzMundo.States
{
    class LastHit : StateTemplate
    {
        public override void Init()
        {

        }

        public override bool Startable()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Activate()
        {
            if (!SpellHandler.Q.IsReady() || !Program.koridor["useQLH"].Cast<CheckBox>().CurrentValue) return;
            
            if (EntityManager.MinionsAndMonsters.EnemyMinions.OrderByDescending(a => a.MaxHealth).Any(
                    a => a.Health <= Program.QDamage(a) && SpellHandler.Q.Cast(a)))
            {
                return;
            }
        }
    }
}
