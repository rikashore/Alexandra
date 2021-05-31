using Disqord.Bot;

namespace Alexandra.Common.Globals
{
    public class EvalGuildGlobals : EvalGlobals
    {
        public override DiscordGuildCommandContext Context { get; }

        public EvalGuildGlobals(DiscordGuildCommandContext context) : base(context)
        {
            Context = context;
        }
    }
}