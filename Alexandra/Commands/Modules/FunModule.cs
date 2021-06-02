using System;
using System.IO;
using System.Threading.Tasks;
using Alexandra.Commands.Bases;
using Alexandra.Common.Extensions;
using Alexandra.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Name("Fun")]
    [Description("A few miscellaneous and fun commands")]
    public class FunModule : LexGuildModuleBase
    {
        private readonly ColorService _colorService;
        private readonly Random _random;

        public FunModule(ColorService colorService, Random random)
        {
            _colorService = colorService;
            _random = random;
        }

        [Command("avatar", "av")]
        [Description("Receive a portrait of yourself or another person")]
        public DiscordCommandResult AvatarAsync([Description("The member for whom you wish to receive a portrait")] IMember member = null)
        {
            member ??= Context.Author;

            var eb = new LocalEmbed()
                .WithTitle($"A portrait of {member.Name}#{member.Discriminator}")
                .WithLexColor()
                .WithImageUrl(member.GetAvatarUrl());

            return Response(eb);
        }

        [Command("userinfo", "whois")]
        public DiscordCommandResult UserInfo(IMember member = null)
        {
            member ??= Context.Author;
            
            var eb = new LocalEmbed()
                .WithTitle(member.Tag)
                .WithThumbnailUrl(member.GetAvatarUrl())
                .WithLexColor()
                .AddField("Id", member.Id, true)
                .AddField("Nickname", member.Nick ?? "No nickname in this server", true)
                .AddField("Joined At", member.JoinedAt.Value.ToString("f"), true)
                .AddField("Is Bot", member.IsBot ? "Yes" : "No")
                .AddField("Created At", member.CreatedAt());

            return Response(eb);
        }

        [Command("color"), RunMode(RunMode.Parallel)]
        [Description("Brews random colors")]
        public async Task RandomColorAsync()
        {
            var color = Color.Random;
            var colorImagePath = _colorService.GetColorImage(color.ToString());
            using (var colorImage = LocalAttachment.File(colorImagePath, "colorImage.png"))
            {
                var eb = new LocalEmbed()
                    .WithColor(color)
                    .WithDescription($"Hex: {color.ToString()}\nRGB: {color.R} {color.G} {color.B}")
                    .WithImageUrl("attachment://colorImage.png");

                var mb = new LocalMessage()
                    .WithAttachments(colorImage)
                    .WithEmbed(eb);

                await Response(mb);
            }
            
            File.Delete(colorImagePath);
        }
        
        [Command("color"), RunMode(RunMode.Parallel)]
        [Description("Brews random colors")]
        public async Task RandomColorAsync(Color color)
        {
            var colorImagePath = _colorService.GetColorImage(color.ToString());
            using (var colorImage = LocalAttachment.File(colorImagePath, "colorImage.png"))
            {
                var eb = new LocalEmbed()
                    .WithColor(color)
                    .WithDescription($"Hex: {color.ToString()}\nRGB: {color.R} {color.G} {color.B}")
                    .WithImageUrl("attachment://colorImage.png");

                var mb = new LocalMessage()
                    .WithAttachments(colorImage)
                    .WithEmbed(eb);

                await Response(mb);
            }
            
            File.Delete(colorImagePath);
        }

        [Command("choose", "choice")]
        [Description("Receive a choice")]
        public DiscordCommandResult ChoiceAsync([Description("The string of choices"), Remainder] string choice)
        {
            var choices = choice.Split("|");

            if (choices.Length < 2)
                return Response("I require more options.");

            return Response(choices.Random(_random));
        }
    }
}