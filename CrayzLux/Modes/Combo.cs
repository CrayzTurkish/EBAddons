using EloBuddy;
using EloBuddy.SDK;
using System.Drawing;

namespace CrayzLux.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            if (Config.Modes.Combo.UseQ)
            {
                if (Q.IsReady() && Q.IsLearned)
                {
                    var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical, Player.Instance.Position);
                    if (target.IsValidTarget() && !target.IsZombie && target.IsEnemy)
                    {
                        var qPred = Q.GetPrediction(target);
                        if (qPred.HitChancePercent >= Program.hitchanceCombo && qPred.Collision == false)
                        {
                            Q.Cast(qPred.CastPosition);
                        }
                    }
                }
            }
            if (Config.Modes.Combo.UseE)
            {
                if (E.IsReady() && E.IsLearned)
                {
                    var target = TargetSelector.GetTarget(E.Range, DamageType.Magical, Player.Instance.Position);
                    if (target.IsValidTarget() && !target.IsZombie && target.IsEnemy)
                    {
                        var ePred = E.GetPrediction(target);
                        if (ePred.HitChancePercent >= Program.hitchanceCombo)
                        {
                            E.Cast(ePred.CastPosition);
                        }
                    }
                }
            }

            if (Config.Modes.Combo.UseR)
            {
                if (R.IsReady() && R.IsLearned)
                {
                    var target = TargetSelector.GetTarget(R.Range, DamageType.Magical, Player.Instance.Position);
                    if (target.IsValidTarget() && !target.IsZombie && target.IsEnemy && target.Health < Damage.RDamage(target))
                    {
                        var rPred = R.GetPrediction(target);
                        if (rPred.HitChancePercent >= Program.hitchanceCombo && !target.IsDead)
                        {
                            R.Cast(rPred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}