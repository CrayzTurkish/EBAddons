using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using CrayzMundo.States;
using SharpDX;

namespace CrayzMundo
{
    internal class Program
    {
        public static Menu Menu, kombo, durtme, koridor, cizimler;
        public static int ResetTime;

        public static bool HasW
        {
            get { return Player.HasBuff("BurningAgony") || ResetTime > Environment.TickCount; }
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }
       
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Chat.Print("<font color=\"#6909aa\" >Crayz Turkish Presents </font><font color=\"#fffffff\" > Crayz Mundo </font>");
            Chat.Print("Loaded Version 7.5.179.44", Color.Red);

            if (Player.Instance.Hero != Champion.DrMundo) return;

            Menu = MainMenu.AddMenu("CrayzMundo", "CrayzMundo");

            kombo = Menu.AddSubMenu("Combo", "Combo");
            kombo.Add("useQCombo", new CheckBox("Use [Q] Combo"));
            kombo.Add("useWCombo", new CheckBox("Use [W] Combo"));
            kombo.Add("useECombo", new CheckBox("Use [E] Combo"));
            kombo.Add("useRCombo", new CheckBox("Use [R] Combo"));
            kombo.Add("useRComboHPPercent", new Slider("Use [R] if HP %", 30));
            kombo.Add("useRComboEnemies", new Slider("Use [R] Min Enemies", 0, 0, 5));

            durtme = Menu.AddSubMenu("Harass", "Harass");
            durtme.Add("useQHarass", new CheckBox("Use [Q] Harass"));
            durtme.Add("useWHarass", new CheckBox("Use [W] Harass"));
            durtme.Add("useEHarass", new CheckBox("Use [E] Harass"));

            koridor = Menu.AddSubMenu("LaneClear", "LaneClear");
            koridor.AddLabel("Last Hit");
            koridor.Add("useQLH", new CheckBox("Use [Q] Last Hit"));
            koridor.AddLabel("Wave Clear");
            koridor.Add("useQWC", new CheckBox("Use [Q] Wave Clear"));
            koridor.AddLabel("Jungle");
            koridor.Add("useQJNG", new CheckBox("Use [Q] Jungle Clear"));


            cizimler = Menu.AddSubMenu("Drawing", "Drawing");
            cizimler.Add("drawQ", new CheckBox("Draw [Q] Range", false));
            cizimler.Add("drawW", new CheckBox("Draw [W] Range", false));

            StateHandler.Init();
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (cizimler["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.DarkOrange, SpellHandler.Q.Range, Player.Instance.Position);
            }
            if (cizimler["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.DarkRed, SpellHandler.W.Range, Player.Instance.Position);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name == Player.GetSpell(SpellSlot.E).Name)
            {
                Console.WriteLine(Player.GetSpell(SpellSlot.E).Name);
                Orbwalker.ResetAutoAttack();
            }
            if (args.SData.Name == Player.GetSpell(SpellSlot.W).Name && !Player.HasBuff("BurningAgony"))
            {
                ResetTime = Environment.TickCount + 500;
            }
        }

        public static float QDamage(Obj_AI_Base target)
        {
            var level = Player.GetSpell(SpellSlot.Q).Level;
            if (level < 1) return 0;
            var value = new[]
            {
                (new[] {80, 130, 180, 230, 280}[level - 1]),
                (int) (new[] {0.15, 0.175, 0.21, 0.225, 0.25}[level - 1]*target.Health)
            }.Max();
            if (EntityManager.Heroes.Enemies.Any(a => a.NetworkId == target.NetworkId))
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, value);

            var maxMonsters = new[] {300, 350, 400, 450, 500}[level - 1];
            if (maxMonsters < value)
            {
                value = maxMonsters;
            }
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, value);
        }
    }
}