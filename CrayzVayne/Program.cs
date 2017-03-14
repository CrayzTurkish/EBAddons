using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace CrayzVayne
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool UltActive()
        {
            return Player.HasBuff("vaynetumblefade") && !UnderEnemyTower((Vector2)_Player.Position);
        }

        public static bool UnderEnemyTower(Vector2 pos)
        {
            return EntityManager.Turrets.Enemies.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(pos) < 1100);
        }

        public static Spell.Ranged Q;
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static Spell.Active Heal;

        public static Item HealthPotion;
        public static Item CorruptingPotion;
        public static Item RefillablePotion;
        public static Item TotalBiscuit;
        public static Item HuntersPotion;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);

        public static Menu Menu, ComboMenu, HarassMenu, FarmMenu, CondemnMenu, AutoPotHealMenu, ItemMenu, DrawMenu, InterruptorMenu, GapCloserMenu, CondemnPriorityMenu;

        public static string[] DangerSliderValues = { "Low", "Medium", "High" };
        public static string[] PriorityValues = { "Very Low", "Low", "Medium", "High", "Very High" };
        public static List<Vector2> Points = new List<Vector2>();

        private static void Game_OnStart(EventArgs args)
        {
            if (!_Player.ChampionName.ToLower().Contains("vayne")) return;

            Q = new Spell.Skillshot(SpellSlot.Q, int.MaxValue, SkillShotType.Linear);
            E = new Spell.Targeted(SpellSlot.E, 550);
            Condemn.ESpell = new Spell.Skillshot(SpellSlot.E, 550, SkillShotType.Linear, 250, 1200);
            R = new Spell.Active(SpellSlot.R);

            var slot = _Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }

            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);

            Chat.Print("<font color=\"#ef0101\" >Crayz Turkish Presents </font><font color=\"#ffffff\" > Crayz Vayne </font>");
            Chat.Print("Dont Feed!!", Color.GreenYellow);

            Menu = MainMenu.AddMenu("Crayz Vayne", "Crayz Turkish");

            Menu.AddGroupLabel("Crayz Vayne");
            Menu.AddLabel("Version: " + "7.5.0.0");
            Menu.AddSeparator();
            Menu.AddLabel("By Crayz Turkish ;)");
            Menu.AddSeparator();
            Menu.AddLabel("Have Fun!");

            ComboMenu = Menu.AddSubMenu("Combo", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQCombo", new CheckBox("Use [Q]"));
            ComboMenu.Add("useQKite", new CheckBox("Use [Q] to Kite Melee", false));
            ComboMenu.Add("useECombo", new CheckBox("Use [E] (Execute)"));
            ComboMenu.AddLabel("R Settings");
            ComboMenu.Add("useRCombo", new CheckBox("Use [R]", false));
            ComboMenu.Add("noRUnderTurret", new CheckBox("Disable [R] if Target is under enemy turret"));
            ComboMenu.Add("noaa", new CheckBox("No AA If active Ulty "));
            ComboMenu.Add("Noaaslider", new Slider("No AA when enemy in range ", 2, 1, 5));

            CondemnPriorityMenu = Menu.AddSubMenu("Auto Condemn", "Condemn Priority");
            CondemnPriorityMenu.AddGroupLabel("Condemn Priority");
            foreach (var enem in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy))
            {
                var champValue = CondemnPriorityMenu.Add(enem.ChampionName + "priority",
                    new Slider(enem.ChampionName + ": ", 1, 1, 5));
                var enem1 = enem;
                champValue.OnValueChange += delegate
                {
                    champValue.DisplayName = enem1.ChampionName + ": " + PriorityValues[champValue.CurrentValue];
                };
                champValue.DisplayName = enem1.ChampionName + ": " + PriorityValues[champValue.CurrentValue];
            }
            CondemnPriorityMenu.AddSeparator();
            var sliderValue = CondemnPriorityMenu.Add("minSliderAutoCondemn",
                new Slider("Min Priority for Auto Condemn: ", 2, 1, 5));
            sliderValue.OnValueChange += delegate
            {
                sliderValue.DisplayName = "Min Priority for Auto Condemn: " + PriorityValues[sliderValue.CurrentValue];
            };
            sliderValue.DisplayName = "Min Priority for Auto Condemn: " + PriorityValues[sliderValue.CurrentValue];
            CondemnPriorityMenu.Add("autoCondemnToggle",
                new KeyBind("Auto Condemn", false, KeyBind.BindTypes.PressToggle, 'H'));
            CondemnPriorityMenu.AddSeparator();

            CondemnMenu = Menu.AddSubMenu("Condemn", "Condemn");
            CondemnMenu.AddGroupLabel("Condemn Settings");
            CondemnMenu.AddSeparator();
            CondemnMenu.Add("pushDistance", new Slider("Push Distance", 410, 350, 420));
            CondemnMenu.Add("condemnPercent", new Slider("Condemn Percent", 33, 1));
            CondemnMenu.AddSeparator();
            CondemnMenu.AddLabel("Active Mode Settings");
            CondemnMenu.Add("smartVsCheap", new CheckBox("On(saves fps) OFF(360 degree check)", true));
            CondemnMenu.AddSeparator();
            CondemnMenu.Add("condemnCombo", new CheckBox("Condemn in Combo", true));
            CondemnMenu.Add("condemnComboTrinket", new CheckBox("Trinket Bush After [E]", true));
            CondemnMenu.Add("condemnHarass", new CheckBox("Condemn in Harass", true));

            HarassMenu = Menu.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQHarass", new CheckBox("Use [Q]", true));

            FarmMenu = Menu.AddSubMenu("LaneClear", "LaneClear");
            FarmMenu.AddGroupLabel("LaneClear Settings");
            FarmMenu.Add("onlyTumbleToCursor", new CheckBox("Only Tumble To Cursor", false));
            FarmMenu.AddLabel("Last Hit");
            FarmMenu.Add("useQLastHit", new CheckBox("Use [Q] LastHit", true));
            FarmMenu.AddLabel("WaveClear");
            FarmMenu.Add("useQWaveClear", new CheckBox("Use [Q] WaveClear", true));

            AutoPotHealMenu = Menu.AddSubMenu("Potion & Heal", "Potion & Heal");
            AutoPotHealMenu.AddGroupLabel("Auto pot usage");
            AutoPotHealMenu.Add("potion", new CheckBox("Use potions"));
            AutoPotHealMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            AutoPotHealMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));
            AutoPotHealMenu.AddGroupLabel("AUto Heal Usage");
            AutoPotHealMenu.Add("UseHeal", new CheckBox("Use Heal"));
            AutoPotHealMenu.Add("useHealHP", new Slider("Minimum Health % to use Heal", 20));

            ItemMenu = Menu.AddSubMenu("Item Settings", "ItemMenuettings");
            ItemMenu.Add("useBOTRK", new CheckBox("Use BOTRK"));
            ItemMenu.Add("useBotrkMyHP", new Slider("My Health < ", 60));
            ItemMenu.Add("useBotrkEnemyHP", new Slider("Enemy Health < ", 60));
            ItemMenu.Add("useYoumu", new CheckBox("Use Youmu"));
            ItemMenu.AddSeparator();
            ItemMenu.Add("useQSS", new CheckBox("Use QSS"));
            ItemMenu.Add("Qssmode", new ComboBox(" ", 0, "Auto", "Combo"));
            ItemMenu.Add("Stun", new CheckBox("Stun"));
            ItemMenu.Add("Blind", new CheckBox("Blind"));
            ItemMenu.Add("Charm", new CheckBox("Charm"));
            ItemMenu.Add("Suppression", new CheckBox("Suppression"));
            ItemMenu.Add("Polymorph", new CheckBox("Polymorph"));
            ItemMenu.Add("Fear", new CheckBox("Fear"));
            ItemMenu.Add("Taunt", new CheckBox("Taunt"));
            ItemMenu.Add("Silence", new CheckBox("Silence", false));
            ItemMenu.Add("QssDelay", new Slider("Use QSS Delay(ms)", 250, 0, 1000));

            DrawMenu = Menu.AddSubMenu("Misc Menu", "Misc Menu");
            DrawMenu.AddGroupLabel("Draw Settings");
            DrawMenu.Add("drawERange", new CheckBox("Draw [E] Range", false));
            DrawMenu.Add("condemnVisualiser", new CheckBox("Draw Condemn", false));
            DrawMenu.Add("drawStacks", new CheckBox("Draw [W] Stacks", false));
            DrawMenu.AddLabel("Misc");
            DrawMenu.Add("wallJumpKey", new KeyBind("Tumble Walls", false, KeyBind.BindTypes.HoldActive, 'Z'));
            DrawMenu.Add("condemnNextAA", new KeyBind("Condemn Next AA", false, KeyBind.BindTypes.PressToggle, 'E'));
            DrawMenu.AddLabel("Anti-Champions");
            DrawMenu.Add("antiKalista", new CheckBox("Anti-Kalista"));
            DrawMenu.Add("antiRengar", new CheckBox("Anti-Rengar"));

            InterruptorMenu = Menu.AddSubMenu("Interrupter", "Interrupter");
            InterruptorMenu.AddGroupLabel("Interrupter Menu");
            InterruptorMenu.Add("enableInterrupter", new CheckBox("Enable Interrupter"));
            InterruptorMenu.AddSeparator();
            var dangerSlider = InterruptorMenu.Add("dangerLevel", new Slider("Set Your Danger Level: ", 3, 1, 3));
            var dangerSliderDisplay = InterruptorMenu.Add("dangerLevelDisplay",
                new Label("Danger Level: " + DangerSliderValues[dangerSlider.Cast<Slider>().CurrentValue - 1]));
            dangerSlider.Cast<Slider>().OnValueChange += delegate
            {
                dangerSliderDisplay.Cast<Label>().DisplayName =
                    "Danger Level: " + DangerSliderValues[dangerSlider.Cast<Slider>().CurrentValue - 1];
            };

            GapCloserMenu = Menu.AddSubMenu("Anti-GapClosers", "Anti-GapClosers");
            GapCloserMenu.AddGroupLabel("Anti-GapCloser Menu");
            GapCloserMenu.Add("enableGapCloser", new CheckBox("Enable Anti-GapCloser"));

            Orbwalker.OnPreAttack += Events.Orbwalker_OnPreAttack;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Gapcloser.OnGapcloser += Events.Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Events.Interrupter_OnInterruptableSpell;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnBasicAttack += Events.ObjAiBaseOnOnBasicAttack;
            GameObject.OnCreate += Events.GameObject_OnCreate;
        }

        public static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && ComboMenu["noaa"].Cast<CheckBox>().CurrentValue
                && (args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AttackTo)
                &&
                (_Player.CountEnemiesInRange(1000f) >=
                 ComboMenu["Noaaslider"].Cast<Slider>().CurrentValue)
                && UltActive() || Player.HasBuffOfType(BuffType.Invisibility)
                && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
            }
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.IsAutoAttack())
            {
                var target = (Obj_AI_Base)args.Target;

                if (target is AIHeroClient)
                {
                    if (DrawMenu["condemnNextAA"].Cast<KeyBind>().CurrentValue && E.IsReady())
                    {
                        E.Cast(target);
                        DrawMenu["condemnNextAA"].Cast<KeyBind>().CurrentValue = false;
                    }
                    if (target.IsValidTarget() && Q.IsReady() &&
                        (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                        ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue ||
                         Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                         HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue))
                    {
                        var pos = (_Player.Position.Extend(Game.CursorPos, 300).Distance(target) <=
                                   _Player.GetAutoAttackRange(target) &&
                                   _Player.Position.Extend(Game.CursorPos, 300).Distance(target) > 100
                            ? Game.CursorPos
                            : (_Player.Position.Extend(target.Position, 300).Distance(target) < 100)
                                ? target.Position
                                : new Vector3());

                        if (pos.IsValid())
                        {
                            Player.CastSpell(SpellSlot.Q, pos);
                        }
                    }

                    if (ComboMenu["useQKite"].Cast<CheckBox>().CurrentValue &&
                        EntityManager.Heroes.Enemies.Any(
                            a => a.IsMelee && a.Distance(Player.Instance) < a.GetAutoAttackRange(Player.Instance)))
                    {
                        Player.CastSpell(SpellSlot.Q,
                            target.Position.Extend(Player.Instance.Position,
                                target.Position.Distance(Player.Instance) + 300).To3D());
                    }
                }

                if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && FarmMenu["useQLastHit"].Cast<CheckBox>().CurrentValue || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && FarmMenu["useQWaveClear"].Cast<CheckBox>().CurrentValue) && Q.IsReady())
                {
                    var source =
                        EntityManager.MinionsAndMonsters.EnemyMinions
                            .Where(
                                a => a.NetworkId != target.NetworkId && a.Distance(Player.Instance) < 300 + Player.Instance.GetAutoAttackRange(a) &&
                                    Prediction.Health.GetPrediction(a, (int)Player.Instance.AttackDelay) < Player.Instance.GetAutoAttackDamage(a, true) + Damages.QDamage(a))
                            .OrderBy(a => a.Health)
                            .FirstOrDefault();

                    if (source == null || Player.Instance.Position.Extend(Game.CursorPos, 300).Distance(source) >
                        Player.Instance.GetAutoAttackRange(source) &&
                        FarmMenu["onlyTumbleToCursor"].Cast<CheckBox>().CurrentValue) return;
                    Orbwalker.ForcedTarget = source;
                    Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 300).Distance(source) <= Player.Instance.GetAutoAttackRange(source) ? Game.CursorPos : source.Position);
                }
            }
        }

    private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name == Player.GetSpell(SpellSlot.R).Name)
            {
                Events.LastR = Environment.TickCount + new[] { 8000, 10000, 12000 }[R.Level - 1];
            }
            if (args.SData.Name.ToLower().Contains("vaynetumble"))
            {
                Orbwalker.ResetAutoAttack();
            }
            if (args.SData.Name == Player.GetSpell(SpellSlot.E).Name)
            {
                DrawMenu["condemnNextAA"].Cast<KeyBind>().CurrentValue = false;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            WallQ.Drawing_OnDraw();

            if (DrawMenu["drawStacks"].Cast<CheckBox>().CurrentValue && Events.AAedTarget != null)
            {
                var color = new[] { Color.White, Color.Aqua }[Events.AaStacks - 1];
                new Circle() { Color = color, Radius = 200 }.Draw(Events.AAedTarget.Position);
            }
            if (DrawMenu["drawERange"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.White, Radius = E.Range }.Draw(_Player.Position);
            }
            if (DrawMenu["condemnVisualiser"].Cast<CheckBox>().CurrentValue)
            {
                var t = TargetSelector.GetTarget(Program.E.Range + Program.Q.Range, DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var color = System.Drawing.Color.Red;
                    for (var i = 1; i < 8; i++)
                    {
                        var targetBehind = t.Position +
                                           Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position) * i * 50;

                        if (!targetBehind.IsWall())
                        {
                            color = System.Drawing.Color.Aqua;
                        }
                        else
                        {
                            color = System.Drawing.Color.Red;
                        }
                    }

                    var tt = t.Position + Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position) * 8 * 50;

                    var startpos = t.Position;
                    var endpos = tt;
                    var endpos1 = tt +
                                  (startpos - endpos).To2D().Normalized().Rotated(45 * (float)Math.PI / 180).To3D() *
                                  t.BoundingRadius;
                    var endpos2 = tt +
                                  (startpos - endpos).To2D().Normalized().Rotated(-45 * (float)Math.PI / 180).To3D() *
                                  t.BoundingRadius;

                    var width = 2;

                    var x = new Geometry.Polygon.Line(startpos, endpos);
                    {
                        x.Draw(color, width);
                    }

                    var y = new Geometry.Polygon.Line(endpos, endpos1);
                    {
                        y.Draw(color, width);
                    }

                    var z = new Geometry.Polygon.Line(endpos, endpos2);
                    {
                        z.Draw(color, width);
                    }
                }
            }
        }
            

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ForcedTarget == null || Orbwalker.ForcedTarget.Health <= 0 || Orbwalker.ForcedTarget.IsDead ||
                Orbwalker.ForcedTarget.Distance(Player.Instance) > (Player.Instance.IsDashing()
                ? Player.Instance.GetAutoAttackRange() + 300
                : Player.Instance.GetAutoAttackRange()))
            {
                Orbwalker.ForcedTarget = null;
            }

            if (Events.AAedTarget == null || Events.LastAa + 3500 + 400 <= Environment.TickCount || Events.AAedTarget.IsDead || !Events.AAedTarget.HasBuff("vaynesilvereddebuff") && (Events.LastAa + 1000 < Environment.TickCount))
            {
                Events.AAedTarget = null;
                Events.AaStacks = 0;
            }

            if (DrawMenu["wallJumpKey"].Cast<KeyBind>().CurrentValue)
            {
                WallQ.WallTumble();
            }
            else
            {
                Orbwalker.DisableMovement = false;
            }
            if (CondemnPriorityMenu["autoCondemnToggle"].Cast<KeyBind>().CurrentValue)
            {
                var condemnTarget = Condemn.CondemnTarget();
                if (condemnTarget != null)
                {
                    E.Cast(condemnTarget);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                States.Combo();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                States.Harass();
            }
        }
        public static
           void AutoPot()
        {
            if (AutoPotHealMenu["potion"].Cast<CheckBox>().CurrentValue && !EloBuddy.Player.Instance.IsInShopRange() &&
                EloBuddy.Player.Instance.HealthPercent <= AutoPotHealMenu["potionminHP"].Cast<Slider>().CurrentValue &&
                !(EloBuddy.Player.Instance.HasBuff("RegenerationPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlaskJungle") ||
                  EloBuddy.Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlask") ||
                  EloBuddy.Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                {
                    HealthPotion.Cast();
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    TotalBiscuit.Cast();
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    RefillablePotion.Cast();
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                    return;
                }
            }
            if (EloBuddy.Player.Instance.ManaPercent <= AutoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(EloBuddy.Player.Instance.HasBuff("RegenerationPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlask") ||
                  EloBuddy.Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                }
            }
        }

        public static
            void UseHeal()
        {
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                _Player.HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
                Chat.Print("<font color=\"#ffffff\" > USe Heal Noob </font>");
            }
        }

        public static
            void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical);


            if (ItemMenu["useYoumu"].Cast<CheckBox>().CurrentValue && Youmuu.IsOwned() && Youmuu.IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(1500) == 1)
                {
                    Youmuu.Cast();
                }
            }
            if (target != null)
            {
                if (ItemMenu["useBOTRK"].Cast<CheckBox>().CurrentValue && Item.HasItem(Cutlass.Id) &&
                    Item.CanUseItem(Cutlass.Id) &&
                    EloBuddy.Player.Instance.HealthPercent < ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ItemMenu["useBotrkEnemyHP"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem(Cutlass.Id, target);
                }
                if (ItemMenu["useBOTRK"].Cast<CheckBox>().CurrentValue && Item.HasItem(Botrk.Id) &&
                    Item.CanUseItem(Botrk.Id) &&
                    EloBuddy.Player.Instance.HealthPercent < ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ItemMenu["useBotrkEnemyHP"].Cast<Slider>().CurrentValue)
                {
                    Botrk.Cast(target);
                }
            }
        }

    }
}
