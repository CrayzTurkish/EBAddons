using EloBuddy;
using EloBuddy.SDK;

namespace CrayzLux
{
    internal class Damage
    {
        public static float QDamage(Obj_AI_Base target)
        {
            if (SpellManager.Q.IsReady() && SpellManager.Q.IsLearned)
                return Program._Player.GetSpellDamage(target, SpellSlot.Q);
            return 0f;
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return 0f;
        }

        public static float EDamage(Obj_AI_Base target)
        {
            if (SpellManager.E.IsReady() && SpellManager.E.IsLearned)
                return Program._Player.GetSpellDamage(target, SpellSlot.E);
            return 0f;
        }

        public static float RDamage(Obj_AI_Base target)
        {
            if (SpellManager.R.IsLearned && SpellManager.R.IsReady())
            {
                if (LuxPassive(target))
                {
                    return Program._Player.GetSpellDamage(target, SpellSlot.R) + LuxPassiveDamage();
                }
                else
                {
                    return Program._Player.GetSpellDamage(target, SpellSlot.R);
                }
            }
            return 0f;
        }

        public static float LuxPassiveDamage()
        {
            return 10 + (8 * Program._Player.Level) + (Player.Instance.TotalMagicalDamage / 100) * 25f;
        }

        public static float IgniteDamage(Obj_AI_Base target)
        {
            return Player.Instance.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
        }

        public static bool LuxPassive(Obj_AI_Base target)
        {
            if (target.HasBuff("LuxIlluminatingFraulein"))
            {
                return true;
            }
            return false;
        }

        public static double MaxDMG(Obj_AI_Base target)
        {
            double dmg = 0;
            if (Config.Modes.Combo.UseQ) dmg += QDamage(target);
            if (Config.Modes.Combo.UseE) dmg += EDamage(target);
            if (Config.Modes.Combo.UseR) dmg += RDamage(target);
            return dmg;
        }

        public static bool LuxE()
        {
            if (Player.HasBuff("LuxLightStrikeKugel"))
            {
                return true;
            }
            return false;
        }
    }
}