using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using Settings = CrayzLux.Config.Modes.Misc;

namespace CrayzLux
{
    internal static class EventsManager
    {
        public static void Initialize()
        {
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || !Settings.gapcloserQ) return;

            if (sender.IsValidTarget(CrayzLux.SpellManager.Q.Range))
            {
                CrayzLux.SpellManager.Q.Cast(sender);
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || !Settings.interruptQ || e.DangerLevel != DangerLevel.High) return;

            if (sender.IsValidTarget(CrayzLux.SpellManager.Q.Range) )
            {
                CrayzLux.SpellManager.Q.Cast(sender);
            }
        }
    }
}
