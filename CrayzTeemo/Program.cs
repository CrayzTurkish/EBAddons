using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

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
        public static Menu TeemoMenu, ComboMenu, HarassMenu, LaneClearMenu, FleeMenu, DrawMenu;

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
            ComboMenu.Add("useRCombo", new CheckBox("Use [R] in Combo"));
            ComboMenu.Add("useWCombo", new CheckBox("Use [W] in Combo (If the target is in range AA)"));

            HarassMenu = TeemoMenu.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("useQHarass", new CheckBox("Use [Q] Harass"));
            HarassMenu.Add("useWHarass", new CheckBox("Use [W] Harass"));

            LaneClearMenu = TeemoMenu.AddSubMenu("LaneClear", "LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("useQLH", new CheckBox("Use [Q] for LastHit"));
            LaneClearMenu.Add("useQWC", new CheckBox("Use [Q] for WaveClear"));

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
            

        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if(DrawMenu["Q.Draw"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Q.IsReady() ? Color.GreenYellow : Color.Red, Q.Range, Player.Instance.Position);
            }
            if (DrawMenu["R.Draw"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(R.IsReady() ? Color.GreenYellow : Color.Red, R.Range, Player.Instance.Position);
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
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateHandler.WaveClear();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                StateHandler.LastHit();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                StateHandler.Flee();
            }
        }
    }
}
