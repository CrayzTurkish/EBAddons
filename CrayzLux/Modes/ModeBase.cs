using EloBuddy.SDK;

namespace CrayzLux.Modes
{
    public abstract class ModeBase
    {
        protected Spell.Skillshot E
        {
            get { return SpellManager.E; }
        }

        protected Spell.Active E2
        {
            get { return SpellManager.E2; }
        }

        protected Spell.Skillshot Q
        {
            get { return SpellManager.Q; }
        }

        protected Spell.Skillshot R
        {
            get { return SpellManager.R; }
        }

        protected Spell.Skillshot W
        {
            get { return SpellManager.W; }
        }

        public abstract void Execute();

        public abstract bool ShouldBeExecuted();
    }
}