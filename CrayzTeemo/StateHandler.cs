using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace CrayzTeemo
{
    internal class StateHandler
    {

        public static AIHeroClient Teemo => Player.Instance;

        private static bool IsShroomed(Vector3 position)
        {
            return
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(obj => obj.Name == "Noxious Trap")
                    .Any(obj => position.Distance(obj.Position) <= 250);
        }

        public static void Combo()
        {
            var q = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Program.Q.Range));// Q için Tahmin eklenebilir.Q menzilindeki düşmanları seçtirdim.
            var targetQ = TargetSelector.GetTarget(q, DamageType.Mixed); // Q için tahmin düzeyi eklenmedi
            if (targetQ != null)
            {
                if (Program.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady())
                {
                    Program.Q.Cast(targetQ);
                }
            }

            if (Program.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && (Teemo.CountEnemyChampionsInRange(Teemo.GetAutoAttackRange()) >= 1))
            {
                Program.W.Cast();
            }

            var enemies = EntityManager.Heroes.Enemies.FirstOrDefault(t => t.IsValidTarget() && Player.Instance.IsInAutoAttackRange(t));
            var rtarget = TargetSelector.GetTarget(Program.R.Range, DamageType.Magical);
            var useR = Program.ComboMenu["rcombo"].Cast<CheckBox>().CurrentValue;
            var rCount = Player.Instance.Spellbook.GetSpell(SpellSlot.R).Ammo;
            var rCharge = Program.ComboMenu["rCharge"].Cast<Slider>().CurrentValue;

            if (rtarget == null)
            {
                return;
            }
            var predictionR = Program.R.GetPrediction(rtarget);

            if (!Program.R.IsReady() || !useR || !Program.R.IsInRange(rtarget) || rCharge > rCount || !rtarget.IsValidTarget()
                || IsShroomed(predictionR.CastPosition))
            {
                return;
            }
            if (predictionR.HitChance >= HitChance.High)
            {
                Program.R.Cast(predictionR.CastPosition);
            }
        }

        public static void Harass()
        {
            var q = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Program.Q.Range));// Q için Tahmin eklenebilir.Q menzilindeki düşmanları seçtirdim.
            var targetQ = TargetSelector.GetTarget(q, DamageType.Mixed); // Q için tahmin düzeyi eklenmedi
            if (targetQ != null)
            {
                if (Program.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady())
                {
                    Program.Q.Cast(targetQ);
                }
            }

            if (Program.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && (Teemo.CountEnemyChampionsInRange(Teemo.GetAutoAttackRange()) >= 1))
            {
                Program.W.Cast();
            }
             
        }

        public static void LastHit()
        {
            if (!Program.LaneClearMenu["useQLH"].Cast<CheckBox>().CurrentValue || !Program.Q.IsReady()) return; 
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions().FirstOrDefault(x => x.IsValidTarget(Program.Q.Range));
           
            if (minion == null) return;
            Program.Q.Cast(minion);
        }

        public static void Flee() 
        {
            if (Program.FleeMenu["useRFlee"].Cast<CheckBox>().CurrentValue && Program.R.IsReady())
            {
                Program.R.Cast(Teemo.Position);
            }
            if (Program.FleeMenu["useWFlee"].Cast<CheckBox>().CurrentValue && Program.W.IsReady())
            {
                Program.W.Cast();
            }
        }


        public static float QDamage(Obj_AI_Base target)
        {
            return Teemo.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 80, 125, 170, 215, 260 }[Program.Q.Level] + 0.8 * Teemo.FlatMagicDamageMod));
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
