using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace CrayzLux
{
    public static class Config
    {
        private const string MenuName = "Lux";

        private static readonly Menu Menu;

        static Config()
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("Crayz Lux!");
            Menu.AddLabel("Have Fun !");
            Modes.Initialize();
        }

        public static void Initialize()
        {
        }

        public static class Modes
        {
            private static readonly Menu Menu;

            static Modes()
            {
                Menu = Config.Menu.AddSubMenu("Config");
                Combo.Initialize();
                Menu.AddSeparator();
                Harass.Initialize();
                Menu.AddSeparator();
                Misc.Initialize();
                Menu.AddSeparator();
                Draw.Initialize();
                Menu.AddSeparator();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static readonly CheckBox _useQ;
                private static readonly CheckBox _useW;
                private static readonly CheckBox _useE;
                private static readonly CheckBox _useR;

                public static bool UseQ
                {
                    get { return _useQ.CurrentValue; }
                }

                public static bool UseW
                {
                    get { return _useW.CurrentValue; }
                }

                public static bool UseE
                {
                    get { return _useE.CurrentValue; }
                }

                public static bool UseR
                {
                    get { return _useR.CurrentValue; }
                }

                static Combo()
                {
                    Menu.AddGroupLabel("Combo");
                    _useQ = Menu.Add("comboUseQ", new CheckBox("Use Q", true));
                    _useW = Menu.Add("comboUseW", new CheckBox("Use W", true));
                    _useE = Menu.Add("comboUseE", new CheckBox("Use E", true));
                    _useR = Menu.Add("comboUseR", new CheckBox("Use R", true));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                public static bool UseQ
                {
                    get { return Menu["harassUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseE
                {
                    get { return Menu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }

                static Harass()
                {
                    Menu.AddGroupLabel("Harass");
                    Menu.Add("harassUseQ", new CheckBox("Use Q", true));
                    Menu.Add("harassUseE", new CheckBox("Use E", true));
                }

                public static void Initialize()
                {
                }
            }

            public static class Draw
            {
                public static bool ShowQ
                {
                    get { return Menu["drawQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool ShowW
                {
                    get { return Menu["drawW"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool ShowE
                {
                    get { return Menu["drawE"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool ShowR
                {
                    get { return Menu["drawR"].Cast<CheckBox>().CurrentValue; }
                }

                static Draw()
                {
                    Menu.AddGroupLabel("Drawing");
                    Menu.Add("drawQ", new CheckBox("Show Q Range", true));
                    Menu.Add("drawW", new CheckBox("Show W Range", false));
                    Menu.Add("drawE", new CheckBox("Show E Range", true));
                    Menu.Add("drawR", new CheckBox("Show R Range", false));
                }

                public static void Initialize()
                {
                }
            }

            public static class Misc
            {

                public static bool rSteal
                {
                    get { return Menu["rSteal"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool useQ
                {
                    get { return Menu["useQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool useE
                {
                    get { return Menu["useE"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool interruptQ
                {
                    get { return Menu["interruptQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool gapcloserQ
                {
                    get { return Menu["gapcloserQ"].Cast<CheckBox>().CurrentValue; }
                }

                static Misc()
                {
                    Menu.AddGroupLabel("Misc");
                    Menu.Add("rSteal", new CheckBox("Try steal drake or baron with R", true));
                    Menu.Add("useQ", new CheckBox("Use Q to LaneClear", true));
                    Menu.Add("useE", new CheckBox("Use E to LaneClear", true));
                    Menu.Add("interruptQ", new CheckBox("Use Q to interrupt spells ?"));
                    Menu.Add("gapcloserQ", new CheckBox("Use Q to antigapcloser spells ?"));
                }

                public static void Initialize()
                {
                }
            }
        }
    }
}