using EloBuddy.SDK;

namespace CrayzLux.Modes
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            foreach (var minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy))
            {
                if (minion.IsValidTarget())
                {
                    if (Config.Modes.Misc.useQ)
                        Q.Cast(minion);
                    if (!(minion.IsValidTarget()))
                        return;
                    if (Config.Modes.Misc.useE)
                        E.Cast(minion);
                }
            }

            foreach (
                var monster in EntityManager.MinionsAndMonsters.GetJungleMonsters(Program._Player.Position, 1000))
            {
                if (monster.IsValidTarget())
                {
                    W.Cast(monster);
                    if (Config.Modes.Misc.useE)
                        E.Cast(monster);
                    if (!(monster.IsValidTarget()))
                        return;
                    if (Config.Modes.Misc.useQ)
                        Q.Cast(monster);
                }
            }
        }
    }
}