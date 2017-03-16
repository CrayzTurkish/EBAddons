using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace CrayzMundo.States
{
    class Harass : StateTemplate
    {
        public override void Init()
        {
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target != null && SpellHandler.E.IsReady() && Program.durtme["useEHarass"].Cast<CheckBox>().CurrentValue && EntityManager.Heroes.Enemies.Any(a => a.Name == target.Name && Player.Instance.IsInAutoAttackRange(a)))
            {
                SpellHandler.E.Cast();
            }
        }

        public override bool Startable()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Activate()
        {
            var target = TargetSelector.GetTarget(SpellHandler.Q.Range + 100, DamageType.Physical);

            if (target == null) return;

            if (SpellHandler.Q.IsReady() && Program.durtme["useQHarass"].Cast<CheckBox>().CurrentValue)
            {
                SpellHandler.Q.Cast(target);
            }

            if (SpellHandler.W.IsReady() && !Program.HasW && Program.durtme["useWHarass"].Cast<CheckBox>().CurrentValue)
            {
                SpellHandler.W.Cast();
            }
        }
    }
}

