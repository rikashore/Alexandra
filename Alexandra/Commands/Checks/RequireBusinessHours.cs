using System;
using System.Threading.Tasks;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Checks
{
    public class RequireBusinessHours : DiscordCheckAttribute
    {
        private int StartHour { get; } = 9;
        private int EndHour { get; } = 20;

        public RequireBusinessHours()
        { }

        public RequireBusinessHours(int startHour, int endHour)
        {
            StartHour = startHour;
            EndHour = endHour;
        }
        
        // TODO: Figure out how to do night shifts
        public override ValueTask<CheckResult> CheckAsync(DiscordCommandContext context)
        {
            if (DateTime.UtcNow.Hour > StartHour && DateTime.UtcNow.Hour < EndHour)
                return Success();

            return Failure($"This command is only usable between {StartHour} Hours and {EndHour} Hours");
        }
    }
}