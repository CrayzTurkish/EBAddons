using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System.Linq;

namespace CrayzTeemo
{
    internal class Program 
    {
        private static void GapCloser(AIHeroClient qAndr, Gapcloser.GapcloserEventArgs qAndrGap)
        {
            if (qAndr.IsEnemy && qAndr.IsValidTarget(Q.Range) && qAndrGap.End.Distance(StateHandler.Teemo) <= 250)
            {
                Q.Cast(qAndr);
            }
            if (qAndr.IsEnemy && qAndr.IsValidTarget(R.Range) && qAndrGap.End.Distance(StateHandler.Teemo) <= 250)
            {
                R.Cast(qAndrGap.End);
            }
        }

        private static void Main() 
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
            Gapcloser.OnGapcloser += GapCloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Chat.Print("<font color=\"#6909aa\" >Crayz Turkish Presents </font><font color=\"#fffffff\" > Crayz Teemo Loaded Version 7.5.179.44 </font>");
        }
        public static Spell.Targeted Q;
        public static Spell.Active W, E;
        public static Spell.Skillshot R;
        public static Menu TeemoMenu, ComboMenu, KillStealMenu, HarassMenu, JungleClearMenu, LaneClearMenu, FleeMenu, DrawMenu;
        public static int Delay = new Random().Next(1000, 5000);
        public static int LaneClearLastR;

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (StateHandler.Teemo.Hero != Champion.Teemo) return;

            Bootstrap.Init(null);
            Q = new Spell.Targeted(SpellSlot.Q, 680);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E, (uint)Player.Instance.AttackRange);
            R = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Circular, 1000, 1000, 135);

            TeemoMenu = MainMenu.AddMenu("CrayzTeemo", "CrayzTeemo");
            TeemoMenu.AddGroupLabel("CrayzTeemo");
            TeemoMenu.AddSeparator();
            TeemoMenu.AddLabel("Made By Crayz Turkish");
            TeemoMenu.AddLabel("Have Fun!");
            TeemoMenu.AddLabel("AntiGapcloser Q and R were activated automatically.");

            ComboMenu = TeemoMenu.AddSubMenu("Combo", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("useQCombo", new CheckBox("Use [Q] in Combo"));
            ComboMenu.Add("rcombo", new CheckBox("Use [R] in Combo"));
            ComboMenu.Add("useWCombo", new CheckBox("Use [W] in Combo (If the target is in range AA)"));
            ComboMenu.Add("rCharge", new Slider("Charges of R before using R: {0}", 2, 1, 3));

            KillStealMenu = TeemoMenu.AddSubMenu("KillSteal", "KillSteal");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("KSQ", new CheckBox("Use [Q] KS"));
            KillStealMenu.Add("KSR", new CheckBox("Use [R] KS"));


            HarassMenu = TeemoMenu.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("useQHarass", new CheckBox("Use [Q] Harass"));
            HarassMenu.Add("useWHarass", new CheckBox("Use [W] Harass"));

            LaneClearMenu = TeemoMenu.AddSubMenu("LaneClear", "LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("useQLH", new CheckBox("Use [Q] for LastHit"));
            LaneClearMenu.Add("qclear", new CheckBox("Use [Q] LaneClear", false));
            LaneClearMenu.Add("qManaManager", new Slider("[Q] Mana Manager", 50));
            LaneClearMenu.Add("rclear", new CheckBox("[R] LaneClear"));
            LaneClearMenu.Add("minionR", new Slider("Hit of minion to use [R]", 3, 1, 4));

            JungleClearMenu = TeemoMenu.AddSubMenu("JungleClear", "JungleClear");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("qclear", new CheckBox("Use [Q] Jungle"));
            JungleClearMenu.Add("rclear", new CheckBox("Use [R] Jungle"));
            JungleClearMenu.Add("qManaManager", new Slider("[Q] Mana setting", 25));


            FleeMenu = TeemoMenu.AddSubMenu("Flee", "Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.AddSeparator();
            FleeMenu.Add("useRFlee", new CheckBox("Use [R] Flee"));
            FleeMenu.Add("useWFlee", new CheckBox("Use [W] Flee"));

            DrawMenu = TeemoMenu.AddSubMenu("Draw", "Draw");
            DrawMenu.AddGroupLabel("Draw Settings");
            DrawMenu.AddSeparator();
            DrawMenu.Add("Q.Draw", new CheckBox("Draw [Q] Range", false));
            DrawMenu.Add("R.Draw", new CheckBox("Draw [R] Range", false));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["Q.Draw"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Q.IsReady() ? Color.GreenYellow : Color.Red, Q.Range, Player.Instance.Position);
            }
            var drawR = DrawMenu["R.Draw"].Cast<CheckBox>().CurrentValue;
            var colorBlind = DrawMenu["colorBlind"].Cast<CheckBox>().CurrentValue;
            var player = Player.Instance.Position;
            if (drawR && colorBlind)
            {
                Circle.Draw(R.IsReady() ? SharpDX.Color.YellowGreen : SharpDX.Color.Red, R.Range, player);
            }

            if (drawR && !colorBlind)
            {
                Circle.Draw(R.IsReady() ? SharpDX.Color.LightGreen : SharpDX.Color.Red, R.Range, player);
            }

        }

        private static void KillSteal()
        {
            var ksq = KillStealMenu["KSQ"].Cast<CheckBox>().CurrentValue;
            var ksr = KillStealMenu["KSR"].Cast<CheckBox>().CurrentValue;

            if (ksq)
            {
                var target =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            t.IsValidTarget() && Q.IsInRange(t) &&
                            DamageLibrary.CalculateDamage(t, true, false, false, false) >= t.Health)
                        .OrderBy(t => t.Health)
                        .FirstOrDefault();

                if (target != null && Q.IsReady())
                {
                    Q.Cast(target);
                }
            }

            if (!ksr)
            {
                return;
            }

            var rTarget =
                EntityManager.Heroes.Enemies.Where(
                    t =>
                        t.IsValidTarget() && R.IsInRange(t) &&
                        DamageLibrary.CalculateDamage(t, false, false, false, true) >= t.Health)
                    .OrderBy(t => t.Health)
                    .FirstOrDefault();

            if (rTarget == null || !R.IsReady())
            {
                return;
            }
            var pred = R.GetPrediction(rTarget);

            if (pred.HitChance >= HitChance.High)
            {
                R.Cast(pred.CastPosition);
            }
        }

        private static void JungleClear()
        {
            var useQ = JungleClearMenu["qclear"].Cast<CheckBox>().CurrentValue;
            var useR = JungleClearMenu["rclear"].Cast<CheckBox>().CurrentValue;
            var ammoR = Player.Instance.Spellbook.GetSpell(SpellSlot.R).Ammo;
            var qManaManager = JungleClearMenu["qManaManager"].Cast<Slider>().CurrentValue;
            var jungleMobQ =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, Q.Range)
                    .FirstOrDefault();
            var jungleMobR = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, R.Range);

            if (useQ && jungleMobQ != null)
            {
                if (Q.IsReady() && qManaManager <= (int)Player.Instance.ManaPercent)
                {
                    Q.Cast(jungleMobQ);
                }
            }

            var firstjunglemobR = jungleMobR.FirstOrDefault();

            if (!useR || firstjunglemobR == null)
            {
                return;
            }

            if (R.IsReady() && Environment.TickCount - LaneClearLastR >= Delay && ammoR >= 1)
            {
                R.Cast(firstjunglemobR.ServerPosition);
                LaneClearLastR = Environment.TickCount;
            }
        }

        private static void LaneClear()
        {
            var qClear = LaneClearMenu["qclear"].Cast<CheckBox>().CurrentValue;
            var qManaManager = LaneClearMenu["qManaManager"].Cast<Slider>().CurrentValue;
            var qMinion =
                EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                    t => Q.IsInRange(t) && t.IsValidTarget());

            foreach (var m in qMinion.Where(m => Q.IsReady()
                                                 && qClear
                                                 &&
                                                 m.Health <= DamageLibrary.CalculateDamage(m, true, false, false, false)
                                                 && qManaManager <= (int)Player.Instance.ManaPercent))
            {
                Q.Cast(m);
            }

            var useR = LaneClearMenu["rclear"].Cast<CheckBox>().CurrentValue;

            if (useR)
            {
                var allMinionsR =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => R.IsInRange(t) && t.IsValidTarget())
                        .OrderBy(t => t.Health);
                var rLocation = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(allMinionsR, R.Width,
                    (int)R.Range);
                var minionR = LaneClearMenu["minionR"].Cast<Slider>().CurrentValue;

                if (rLocation.HitNumber >= minionR
                    && Environment.TickCount - LaneClearLastR >= Delay)
                {
                    R.Cast(rLocation.CastPosition);
                    LaneClearLastR = Environment.TickCount;
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateHandler.Combo();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateHandler.Harass();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                StateHandler.LastHit();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                StateHandler.Flee();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (KillStealMenu["KSQ"].Cast<CheckBox>().CurrentValue
                || KillStealMenu["KSR"].Cast<CheckBox>().CurrentValue)
            {
                KillSteal();
            }
        }
    }
}
