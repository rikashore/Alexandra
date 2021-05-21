using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    public class GeneralModule : DiscordModuleBase
    {
        [Command("ping")]
        [Description("A game of ping-pong shall occur")]
        public async Task PingAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var msg = await Response("Pong: *loading* response time");
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = $"Pong: {stopwatch.ElapsedMilliseconds}ms response time");
        }

        [Command("info")]
        [Description("Receive information about Alexandra")]
        public DiscordCommandResult InfoAsync()
        {
            var authorString = Context.Bot.GetUser(LexGlobals.AuthorId).ToString();

            var embedBuilder = new LocalEmbedBuilder()
                .WithLexColor()
                .WithTitle(Context.Bot.CurrentUser.Name)
                .AddField("Author", authorString)
                .AddField("Source Code", Markdown.Link("GitHub", LexGlobals.LexRepo), true)
                .AddField("Octokit.net", Markdown.Link("GitHub", LexGlobals.OctokitRepo), true)
                .AddField("Library", Markdown.Link("Disqord " + Library.Version, Library.RepositoryUrl), true);

            return Response(embedBuilder);
        }

        [Command("help")]
        public DiscordCommandResult Help(params string[] path)
        {
            var service = Context.Bot.Commands;
            var topLevelModules = service.TopLevelModules.ToArray();
            IReadOnlyList<Module> modules = topLevelModules.Where(x => x.Aliases.Count != 0).ToArray();
            IReadOnlyList<Command> commands = topLevelModules.Except(modules).SelectMany(x => x.Commands).ToArray();
            if (path.Length == 0)
            {
                var builder = new LocalEmbedBuilder()
                    .WithColor(0x2F3136);
                if (modules.Count != 0)
                {
                    var aliases = modules.Select(x => x.Aliases[0])
                        .OrderBy(x => x)
                        .Select(x => Markdown.Code(x));
                    builder.AddField("Modules", string.Join(", ", aliases));
                }

                if (commands.Count != 0)
                {
                    var aliases = commands.Select(x => x.Aliases[0])
                        .OrderBy(x => x)
                        .Select(x => Markdown.Code(x));
                    builder.AddField("Commands", string.Join(", ", aliases));
                }

                if (builder.Fields.Count == 0)
                    return Reply("Nothing to display.");
                return Reply(builder);
            }
            else
            {
                var comparison = service.StringComparison;
                Module foundModule = null;
                Command foundCommand = null;
                for (var i = 0; i < path.Length; i++)
                {
                    if (foundModule != null)
                    {
                        modules = foundModule.Submodules;
                        commands = foundModule.Commands;
                        foundModule = null;
                    }

                    var currentAlias = path[i].Trim();
                    foreach (var module in modules)
                    {
                        foreach (var alias in module.Aliases)
                        {
                            if (currentAlias.Equals(alias, comparison))
                            {
                                foundModule = module;
                                break;
                            }
                        }
                    }

                    if (foundModule != null)
                        continue;
                    foreach (var command in commands)
                    {
                        foreach (var alias in command.Aliases)
                        {
                            if (currentAlias.Equals(alias, comparison))
                            {
                                foundCommand = command;
                                break;
                            }
                        }
                    }

                    if (foundCommand != null)
                        break;
                }

                if (foundModule == null && foundCommand == null)
                    return Reply("No module or command found matching the input.");
                if (foundCommand != null)
                {
                    var eb = new LocalEmbedBuilder()
                        .WithTitle(foundCommand.Name)
                        .WithDescription(foundCommand.Description ?? "No Description")
                        .WithLexColor()
                        .AddField("Module", foundCommand.Module is null ? "Top level command" : foundCommand.Module.Name)
                        .AddField("Aliases", foundCommand.Aliases is null
                            ? "No aliases"
                            : string.Join(", ", foundCommand.Aliases.Select(Markdown.Code)))
                        .AddField("Parameters", foundCommand.Parameters is null
                                ? "No parameters"
                                : string.Join(' ', foundCommand.Parameters.Select(FormatParameter)));

                    return Reply(eb);
                }
                else
                {
                    var eb = new LocalEmbedBuilder()
                        .WithTitle(foundModule.Name)
                        .WithDescription(foundModule.Description ?? "No Description")
                        .WithLexColor()
                        .AddField("Submodules",
                            foundModule.Submodules is null
                                ? "No submodules"
                                : string.Join(' ', foundModule.Submodules.Select(x => Markdown.Code(x.Name))))
                        .AddField("Commands", string.Join(' ', foundModule.Commands.Select(x => Markdown.Code(x.Name))));

                    return Reply(eb);
                }
            }
        }

        private string FormatParameter(Parameter parameter)
        {
            string format;
            if (parameter.IsMultiple)
            {
                format = "{0}[]";
            }
            else
            {
                string str = parameter.IsRemainder ? "{0}…" : "{0}";
                format = parameter.IsOptional ? "[" + str + "]" : "<" + str + ">";
            }
            return string.Format(format, parameter.Name);
        }
    }
}