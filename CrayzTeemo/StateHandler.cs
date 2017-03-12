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
        /*Bu kod ile yapılmak istenen kodu hangi şampiyona yazıyorsan onun özniteliklerini almak ve öznitelik eklemek. Örneğin: Teemo'nun AA menzilindeki düşmanları
        kontrol ettirmek gibi.*/
        /* " public static AIHeroClient _Player { get { return ObjectManager.Player; } } "
        Yukarıadki kod yerine bunu yazabilirsin*/
        public static AIHeroClient Teemo => Player.Instance; /* Bunu yazmak yerine, "Player.Instance" kodunu direk kodlardada kullanabilirsin şuanda "Teemo" yazacağız bu kodu yazmasaydık 
        "Player.Instance" yazarak aynı görevi yaptırabilirdik*/


        /*GetDynamicRange metodunu hiç bir yerde kullanmamışsın o nedenle kaldırım. Bu metodun görevini anlatayım sana Eğer Temonun Q yeteneği hazırsa 
        geriye Q'nun menzil değerini gönderiyor Hazır değilse Temonun AutoAttack menzilini gönderiyor (Bu nedenle dinamik) gönderdiği yer program.cs
        Bu menzili göstermek için "Drawing.OnDraw" kodunu kullanabilirsin. Tabi önce menzil çizimini ve rengini belirlemen lazım.*/
        /* public static float GetDynamicRange()
        {
            if (Program.Q.IsReady())
            {
                return Program.Q.Range;
            }
            return Teemo.GetAutoAttackRange();
        }*/
        public static void Combo()
        {
            /*Hedef seçici yok (Target) değişkeni kullanmışsın ama değişkenin belirlendği yer yoktu o nedenle kendim ekledim.*/
            
            /*Q*/
            var q = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Program.Q.Range));// Q için Tahmin eklenebilir.Ben sadece Q menzilindeki düşmanları seçtirdim.
            var targetQ = TargetSelector.GetTarget(q, DamageType.Mixed); // Q için tahmin düzeyi eklenmedi o nedenle direk Q yu bam diye atar. :D
            if (targetQ != null)
            {
                if (Program.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady())
                {
                    Program.Q.Cast(targetQ);
                }
            }
            /*Q*/

            /*W*/ // Burada eğer teemo'nun AA menzilinde düşman varsa W kullanır
            if (Program.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && (Teemo.CountEnemyChampionsInRange(Teemo.GetAutoAttackRange()) >= 1))
                {
                    Program.W.Cast();
                }
            /*W*/

            /*R*/
            var r = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Program.R.Range)); //R için Tahmin eklenebilir. Ben sadece R menzilindeki düşmanları seçtirdim.
            var targetR = TargetSelector.GetTarget(r, DamageType.Mixed);  // R için tahmin düzeyi eklenmedi o nedenle direk Q yu bam diye atar ard arda. :D
            if (targetR == null) return;
            if (Program.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && Program.R.IsReady())
            {
                Program.R.Cast(targetR.Position);
            }
            /*R*/


            /* // Gönderdiğin kod.
            if (target != null) 
            {
            if (Program.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
            {
                Program.Q.Cast(target);
            }
            else if (Program.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && target.Distance(Teemo) > Teemo.GetAutoAttackRange(target))
            {
                Program.W.Cast();
            }
            if (Program.ComboMenu["useRCombo"].Cast<CheckBox>().CurrentValue && Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
            {
                Program.R.Cast(target);
            }
            }
*/
        }

        public static void Harass()
        {
            /*Hedef seçici yok (Target) değişkeni kullanmışsın ama değişkenin belirlendği yer yoktu o nedenle kendim ekledim.*/
            /*Q 'C' tuşuna bastığında menzilde düşman varsa Q atar.*/
            var q = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Program.Q.Range));// Q için Tahmin eklenebilir.Ben sadece Q menzilindeki düşmanları seçtirdim.
            var targetQ = TargetSelector.GetTarget(q, DamageType.Mixed); // Q için tahmin düzeyi eklenmedi o nedenle direk Q yu bam diye atar. :D
            if (targetQ != null)
            {
                if (Program.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady())
                {
                    Program.Q.Cast(targetQ);
                }
            }
            /*Q*/

            /*W*/ // 'C' tuşuna bastığında Burada eğer teemo'nun AA menzilinde düşman varsa W kullanır
            if (Program.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && (Teemo.CountEnemyChampionsInRange(Teemo.GetAutoAttackRange()) >= 1))
            {
                Program.W.Cast();
            }
            /*W*/

            // Gönderdiğin kod.
            /*if (target != null)
            {
                if (Program.HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                {
                    Program.Q.Cast(target);
                }
                if (Program.HarassMenu["useWHarass"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && target.Distance(Teemo) > Teemo.GetAutoAttackRange(target))
                {
                    Program.W.Cast();
                }
            }     */
        }

        public static void LastHit()
        {
            if (!Program.LaneClearMenu["useQLH"].Cast<CheckBox>().CurrentValue || !Program.Q.IsReady()) return; // return;
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions().FirstOrDefault(x => x.IsValidTarget(Program.Q.Range));
            // Ekelendi, 'X' tuşuna bastığında Q menzili içinde Minyon varsa Q atar Tahmin eklenmedi örneğin kaç minyon olunca Q kullansın gibi, mana yönetimide yok.
            if (minion == null) return;
            Program.Q.Cast(minion);
        }

        public static void WaveClear()
        {
            if (!Program.LaneClearMenu["useQWC"].Cast<CheckBox>().CurrentValue || !Program.Q.IsReady()) return; // return;
            //var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(a => a.IsEnemy && a.Health <= QDamage(a));
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions().FirstOrDefault(x => x.IsValidTarget(Program.Q.Range));
            // Ekelendi, 'V' tuşuna bastığında Q menzili içinde Minyon varsa Q kullanarak minyonları keser. Tahmin eklenmedi örneğin kaç minyon olunca Q kullansın gibi, mana yönetimide yok.
            if (minion == null) return;
            Program.Q.Cast(minion);
        }

        public static void Flee() //Çalışıp çalışmadığını kontrol etmedim ama mantık aşağıyukarı aynı.
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


        /*Q'nun seviye başına artan hasarını hesaplatmış mesela ilk seviyede temel Q hasarı 80 olarak ermiş
        Minyonlara Q ile otomatik son vuruş yaptırabilmek için kullabilirsin. Hatta Q ile Auto killsteal bile yaptırabilirsin.
            */
        public static float QDamage(Obj_AI_Base target)
        {
            return Teemo.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 80, 125, 170, 215, 260 }[Program.Q.Level] + 0.8 * Teemo.FlatMagicDamageMod));
        }
    }
}
