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

            /*W*/ // Burada eğer teemo'nun AA menzilinde düşman varsa W kullanır
            if (Program.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && (Teemo.CountEnemyChampionsInRange(Teemo.GetAutoAttackRange()) >= 1))
                {
                    Program.W.Cast();
                }

            /*R*/
            var r = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Program.R.Range)); //R için Tahmin eklenebilir. R menzilindeki düşmanları seçtirdim.
            var targetR = TargetSelector.GetTarget(r, DamageType.Mixed);  // R için tahmin düzeyi eklenmedi 
            if (targetR == null) return;
            if (Program.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && Program.R.IsReady())
            {
                Program.R.Cast(targetR.Position);
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

        public static void WaveClear()
        {
            if (!Program.LaneClearMenu["useQWC"].Cast<CheckBox>().CurrentValue || !Program.Q.IsReady()) return; 
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
    }
}
