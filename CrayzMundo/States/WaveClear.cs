using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace CrayzMundo.States
{
    class WaveClear : StateTemplate
    {
        public override void Init()
        {

        }

        public override bool Startable()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Activate()
        {
            if (!SpellHandler.Q.IsReady() || !Program.koridor["useQWC"].Cast<CheckBox>().CurrentValue) return;

            if (EntityManager.MinionsAndMonsters.EnemyMinions.OrderByDescending(a => a.MaxHealth).Any(
                    a => a.Health <= Program.QDamage(a) && SpellHandler.Q.Cast(a)))
            {
                return;
            }
        }
    }
}
