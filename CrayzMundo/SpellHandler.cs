using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace CrayzMundo
{
    class SpellHandler
    {
        public static Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, 975, SkillShotType.Linear, 500, 1500, 75) { AllowedCollisionCount = 0};

        public static Spell.Active W = new Spell.Active(SpellSlot.W, 325);

        public static Spell.Active E { get { return new Spell.Active(SpellSlot.E, (uint) Player.Instance.GetAutoAttackRange());} }

        public static Spell.Active R = new Spell.Active(SpellSlot.R);
    }
}
