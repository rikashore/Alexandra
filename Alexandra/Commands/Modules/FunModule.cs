using System;
using System.Threading.Tasks;
using Alexandra.Commands.Bases;
using Alexandra.Common.Extensions;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Name("Fun")]
    [Description("A few miscellaneous and fun commands")]
    public class FunModule : LexGuildModuleBase
    {

        [Command("avatar", "av")]
        [Description("Receive a portrait of yourself or another person")]
        public DiscordCommandResult AvatarAsync([Description("The member for whom you wish to receive a portrait")] IMember member = null)
        {
            member ??= Context.Author;

            var eb = new LocalEmbedBuilder()
                .WithTitle($"A portrait of {member.Name}#{member.Discriminator}")
                .WithLexColor()
                .WithImageUrl(member.GetAvatarUrl());

            return Response(eb);
        }

        [Group("color")]
        [Name("Color")]
        [Description("Brew some random colors")]
        public class ColorModule : DiscordModuleBase
        {
            private readonly Random _random;

            public ColorModule(Random random)
            {
                _random = random;
            }

            private int RandomHexColor()
                => _random.Next(0x1000000);

            private byte[] RandomRgbColor()
            {
                var color = new byte[3];
                _random.NextBytes(color);
                return color;
            }

            [Command("hex")]
            [Description("Brew random shades through hex")]
            public async Task HexColorsAsync([Description("The amount of shades to brew")] int amount = 1)
            {
                if (amount <= 5)
                {
                    for (int i = 1; i <= amount; i++)
                    {
                        var color = RandomHexColor();
                        var colorEmbed = new LocalEmbedBuilder()
                            .WithDescription($"#{color:X6}")
                            .WithColor(new Color(color));

                        await Response(colorEmbed);
                    }

                    return;
                }

                var colorPages = new Page[amount];

                for (int i = 0; i < amount; i++)
                {
                    var color = RandomHexColor();

                    var colorEmbed = new LocalEmbedBuilder()
                        .WithDescription($"#{color:X6}")
                        .WithColor(new Color(color));

                    colorPages[i] = colorEmbed;
                }

                await Pages(colorPages);
            }

            [Command("rgb")]
            [Description("Brew random shades through rgb")]
            public async Task RgbColorsAsync([Description("The amount of shades to brew")] int amount = 1)
            {
                if (amount <= 5)
                {
                    for (int i = 1; i <= amount; i++)
                    {
                        var color = RandomRgbColor();
                        var colorEmbed = new LocalEmbedBuilder()
                            .WithDescription(string.Join(", ", color))
                            .WithColor(new Color(color[0], color[1], color[2]));

                        await Response(colorEmbed);
                    }

                    return;
                }

                var colorPages = new Page[amount];

                for (int i = 0; i < amount; i++)
                {
                    var color = RandomRgbColor();
                    var colorEmbed = new LocalEmbedBuilder()
                        .WithDescription(string.Join(", ", color))
                        .WithColor(new Color(color[0], color[1], color[2]));

                    colorPages[i] = colorEmbed;
                }

                await Pages(colorPages);
            }

            [Command("hsv")]
            [Description("Brew random shades through hsv")]
            public async Task HsvColorsAsync([Description("The amount of shades to brew")] int amount = 1)
            {
                if (amount <= 5)
                {
                    for (int i = 1; i <= amount; i++)
                    {
                        var color = Color.Random;

                        var colorEmbed = new LocalEmbedBuilder()
                            .WithDescription(color.ToString())
                            .WithColor(color);

                        await Response(colorEmbed);
                    }

                    return;
                }

                var colorPages = new Page[amount];

                for (int i = 0; i < amount; i++)
                {
                    var color = Color.Random;

                    var colorEmbed = new LocalEmbedBuilder()
                        .WithDescription(color.ToString())
                        .WithColor(color);

                    colorPages[i] = colorEmbed;
                }

                await Pages(colorPages);
            }
        }

        [Command("choose", "choice")]
        [Description("Receive a choice")]
        public DiscordCommandResult ChoiceAsync(
            [Description("The string of choices"), Remainder] string choice)
        {
            var choices = choice.Split("|");

            if (choices.Length < 2)
                return Response("I require more options.");

            return Response(choices.Random());
        }
    }
}