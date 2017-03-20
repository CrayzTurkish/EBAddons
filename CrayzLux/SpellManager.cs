using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System;

namespace CrayzLux
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q { get; set; }
        public static Spell.Skillshot W { get; set; }
        public static Spell.Skillshot E { get; set; }
        public static Spell.Active E2 { get; set; }
        public static Spell.Skillshot R { get; set; }

        static SpellManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear, 250, 1200, 180); //80);
            W = new Spell.Skillshot(SpellSlot.W, 1075, SkillShotType.Linear, 250, 1200, 150);
            E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Circular, 150, 1300, 350); //275
            E2 = new Spell.Active(SpellSlot.E, 350);
            R = new Spell.Skillshot(SpellSlot.R, 3340, SkillShotType.Linear, 1135, Int32.MaxValue, 190);
        }

        public static void Initialize()
        {
            E.AllowedCollisionCount = Int32.MaxValue;
            W.AllowedCollisionCount = Int32.MaxValue;
            R.AllowedCollisionCount = Int32.MaxValue;
            Chat.Print("CrayzLux Loaded.By Crayz Turkish.Have Fun ^.^ ");
        }
    }
}