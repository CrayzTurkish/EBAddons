using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace CrayzMundo.States
{
    class Combo : StateTemplate
    {
        public override void Init()
        {
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target != null && SpellHandler.E.IsReady() && Program.kombo["useECombo"].Cast<CheckBox>().CurrentValue && EntityManager.Heroes.Enemies.Any(a => a.Name == target.Name && Player.Instance.IsInAutoAttackRange(a)))
            {
                SpellHandler.E.Cast();
            }
            if (target != null && SpellHandler.W.IsReady() && Program.kombo["useWCombo"].Cast<CheckBox>().CurrentValue && EntityManager.Heroes.Enemies.Any(a => a.Name == target.Name && Player.Instance.IsInAutoAttackRange(a)))
            {
                SpellHandler.W.Cast();
            }
}

public override bool Startable()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Activate()
        {
            var target = TargetSelector.GetTarget(SpellHandler.Q.Range + 100, DamageType.Physical);

            if (target == null) return;

            if (SpellHandler.Q.IsReady() && Program.kombo["useQCombo"].Cast<CheckBox>().CurrentValue)
            {
                SpellHandler.Q.Cast(target);
            }
            if (SpellHandler.R.IsReady() && Program.kombo["useRCombo"].Cast<CheckBox>().CurrentValue && Player.Instance.HealthPercent <= Program.kombo["useRComboHPPercent"].Cast<Slider>().CurrentValue && Player.Instance.CountEnemiesInRange(1000) >= Program.kombo["useRComboEnemies"].Cast<Slider>().CurrentValue)
            {
                SpellHandler.R.Cast();
            }
        }
    }
}
