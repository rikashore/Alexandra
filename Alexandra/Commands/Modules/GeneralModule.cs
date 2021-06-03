using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alexandra.Commands.Bases;
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Http;
using Disqord.Rest;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Name("General")]
    public class GeneralModule : LexGuildModuleBase
    {
        [Command("ping")]
        [Qmmands.Description("A game of ping-pong shall occur")]
        public async Task PingAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var msg = await Response("Pong: *loading* response time");
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = $"Pong: {stopwatch.ElapsedMilliseconds}ms response time");
        }

        [Command("info")]
        [Qmmands.Description("Receive information about Alexandra")]
        public DiscordCommandResult InfoAsync()
        {
            var authorString = Context.Bot.GetUser(LexGlobals.AuthorId).ToString();

            var embedBuilder = new LocalEmbed()
                .WithLexColor()
                .WithTitle(Context.Bot.CurrentUser.Name)
                .AddField("Author", authorString)
                .AddField("Source Code", Markdown.Link("GitHub", LexGlobals.LexRepo), true)
                .AddField("Octokit.net", Markdown.Link("GitHub", LexGlobals.OctokitRepo), true)
                .AddField("Library", Markdown.Link("Disqord " + Library.Version, Library.RepositoryUrl), true);

            return Response(embedBuilder);
        }

        [Command("quote")]
        [Description("Quote the words of another")]
        public async Task<DiscordCommandResult> QuoteMessageAsync(string quoteUrl)
        {
            var regex = Discord.MessageJumpLinkRegex;

            if (!regex.IsMatch(quoteUrl))
                return Response("It seems you have not given me a valid jump URL");

            var res = regex.Match(quoteUrl);
            var channelId = Convert.ToUInt64(res.Groups["channel_id"].Value);
            var messageId = Convert.ToUInt64(res.Groups["message_id"].Value);

            IChannel channel;
            try
            {
                channel = await Context.Bot.FetchChannelAsync(channelId);
            }
            catch (RestApiException e) when (e.StatusCode == HttpResponseStatusCode.Forbidden)
            {
                return Response("It seems I am unable to access that channel");
            }

            if (channel is not IGuildChannel guildChannel)
            {
                return Response("I cannot read messages from a DM");
            }
            
            if (!Context.CurrentMember.GetChannelPermissions(guildChannel).ReadMessageHistory)
                return Response("I don't have the necessary permissions to view this channel");

            if (!Context.Author.GetChannelPermissions(guildChannel).ReadMessageHistory)
                return Response("You don't have the necessary permissions to view this channel");

            var message = await Context.Bot.FetchMessageAsync(channelId, messageId);
            
            var eb = new LocalEmbed()
                .WithAuthor(message.Author.ToString(), message.Author.GetAvatarUrl())
                .WithDescription(message.Content)
                .WithLexColor()
                .WithFooter($"Id: {messageId}")
                .WithTimestamp(message.CreatedAt());

            return Response(eb);
        }

        [Command("quote")]
        [Description("Quote the words of another")]
        public DiscordCommandResult QuoteMessageAsync()
        {
            var messageRef = Context.Message.ReferencedMessage.GetValueOrDefault();
            if (messageRef is null)
                return Response("I require a Jump URL or a reference to a message to quote");
            
            var eb = new LocalEmbed()
                .WithAuthor(messageRef.Author.ToString(), messageRef.Author.GetAvatarUrl())
                .WithDescription(messageRef.Content)
                .WithLexColor()
                .WithFooter($"Id: {messageRef.Id}")
                .WithTimestamp(messageRef.CreatedAt());

            return Response(eb);
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
                var builder = new LocalEmbed()
                    .WithLexColor();
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

                return builder.Fields.Count == 0 ? Reply("Nothing to display.") : Reply(builder);
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
                    var eb = new LocalEmbed()
                        .WithTitle(foundCommand.Name)
                        .WithDescription(foundCommand.Description ?? "No Description")
                        .WithLexColor()
                        .AddField("Module", foundCommand.Module is null ? "Top level command" : foundCommand.Module.Name)
                        .AddField("Aliases", foundCommand.Aliases is null
                            ? "No aliases"
                            : string.Join(", ", foundCommand.Aliases.Select(Markdown.Code)))
                        .AddField("Parameters", foundCommand.Parameters.Count == 0
                                ? "No parameters"
                                : string.Join(' ', foundCommand.Parameters.Select(FormatParameter)));

                    if (foundCommand.Parameters.Count != 0)
                        eb.AddField("Parameter Descriptions", string.Join('\n', 
                            foundCommand.Parameters.Select(x => $"{x.Name}: {x.Description ?? "No description"}")));

                    return Reply(eb);
                }
                else
                {
                    var eb = new LocalEmbed()
                        .WithTitle(foundModule.Name)
                        .WithDescription(foundModule.Description ?? "No Description")
                        .WithLexColor()
                        .AddField("Submodules",
                            foundModule.Submodules.Count == 0
                                ? "No submodules"
                                : string.Join('\n', foundModule.Submodules.Select(x => Markdown.Code(x.Name))))
                        .AddField("Commands", string.Join('\n', foundModule.Commands.Where(x => !string.IsNullOrEmpty(x.Name))
                            .Select(x => Markdown.Code(x.Name))));

                    return Reply(eb);
                }
            }
        }

        private static string FormatParameter(Parameter parameter)
        {
            string format;
            if (parameter.IsMultiple)
            {
                format = "{0}[]";
            }
            else
            {
                var str = parameter.IsRemainder ? "{0}…" : "{0}";
                format = parameter.IsOptional ? "[" + str + "]" : "<" + str + ">";
            }
            return string.Format(format, parameter.Name);
        }
    }
}