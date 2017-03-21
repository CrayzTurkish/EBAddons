using EloBuddy;
using EloBuddy.SDK;

namespace CrayzTeemo
{

    internal class DamageLibrary
    {

        public static float CalculateDamage(Obj_AI_Base target, bool useQ, bool useW, bool useE, bool useR)
        {
            if (target == null)
            {
                return 0;
            }

            var totaldamage = 0f;

            if (useQ && Program.Q.IsReady())
            {
                totaldamage = totaldamage + QDamage(target);
            }

            if (useW && Program.W.IsReady())
            {
                totaldamage = totaldamage + WDamage(target);
            }

            if (useE && Program.E.IsReady())
            {
                totaldamage = totaldamage + EDamage(target);
            }

            if (useR && Program.R.IsReady())
            {
                totaldamage = totaldamage + RDamage(target);
            }

            return totaldamage;
        }

        private static float QDamage(Obj_AI_Base target)
        {
            return target.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 0, 80, 125, 170, 215, 260 }[Program.Q.Level] + (Player.Instance.TotalMagicalDamage * 0.8f));
        }

        private static float WDamage(Obj_AI_Base target)
        {
            return 0;
        }

        private static float EDamage(Obj_AI_Base target)
        {
            return target.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 0, 10, 20, 30, 40, 50 }[Program.E.Level] + (Player.Instance.TotalMagicalDamage * 0.3f));
        }

        private static float RDamage(Obj_AI_Base target)
        {
            return target.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 200, 325, 450 }[Program.R.Level] + (Player.Instance.TotalMagicalDamage * 0.125f)));
        }
    }
}
