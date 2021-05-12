using System;
using System.Threading.Tasks;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Checks
{
    public class RequireBusinessHours : DiscordCheckAttribute
    {
        private static int StartHour { get; set; }
        private static int EndHour { get; set; }

        public RequireBusinessHours()
        {
            StartHour = 9;
            EndHour = 20;
        }

        public RequireBusinessHours(int startHour, int endHour)
        {
            StartHour = startHour;
            EndHour = endHour;
        }
        
        private static readonly DateTime Now = DateTime.Now;
        private readonly DateTime _startingDateTime = new DateTime(Now.Year, Now.Month, Now.Day, StartHour, 0, 0);
        private readonly DateTime _endingDateTime = new DateTime(Now.Year, Now.Month, Now.Day, EndHour, 0, 0);

        public override ValueTask<CheckResult> CheckAsync(DiscordCommandContext context)
        {
            if (Now > _startingDateTime && Now < _endingDateTime)
                return Success();

            return Failure($"This command is only usable between {StartHour} Hours and {EndHour} Hours");
        }
    }
}