using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System;

namespace CrayzLux
{
    public static class Program
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public const float hitchanceCombo = 90f;
        public const float hitchanceHarras = 90f;
        public static GameObject EObject;
        public const string ChampName = "Lux";

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != ChampName)
            {
                Chat.Print("Wrong champion xDD");
                return;
            }
            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();
            Drawing.OnDraw += OnDraw;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
        }
        private static void OnCreateObject(GameObject obj, EventArgs args)
        {
            if (obj != null && obj.Name.Contains("Lux_Base_E_mis.troy"))
            {

                if (obj.IsMe)
                {
                    EObject = obj;
                }
            }
        }

        private static void OnDeleteObject(GameObject obj, EventArgs args)
        {

            if (obj != null && obj.Name.Contains("Lux_Base_E_mis.troy"))
            {
                if (obj.IsMe)
                {
                    EObject = null;
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (_Player.IsDead)
                return;
            if (Config.Modes.Draw.ShowQ)
            {
                Circle.Draw(Color.White, SpellManager.Q.Range, Player.Instance.Position);
            }
            if (Config.Modes.Draw.ShowE)
            {
                Circle.Draw(Color.Blue, SpellManager.E.Range, Player.Instance.Position);
            }

            if (Config.Modes.Draw.ShowR)
            {
                Circle.Draw(Color.Red, SpellManager.R.Range, Player.Instance.Position);
            }

            if (Config.Modes.Draw.ShowW)
            {
                Circle.Draw(Color.Green, SpellManager.W.Range, Player.Instance.Position);
            }
        }
    }
}