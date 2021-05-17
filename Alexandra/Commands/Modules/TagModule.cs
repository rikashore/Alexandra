﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexandra.Common.Extensions;
using Alexandra.Database.Entities;
using Alexandra.Disqord;
using Alexandra.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;

// Most of the newer commands are from 230Daniel
// https://github.com/230Daniel
namespace Alexandra.Commands.Modules
{
    [Group("tag", "tags", "t")]
    [Name("Tag")]
    [Description("Commands relating to tags")]
    public class TagModule : LexGuildModuleBase
    {
        private readonly TagService _tagService;
        private readonly CommandService _commandService;
        
        public TagModule(TagService tagService, CommandService commandService)
        {
            _tagService = tagService;
            _commandService = commandService;
        }
        
        [Command("")]
        public async Task<DiscordCommandResult> GetTagAsync(string tagName)
        {
            var tag = await _tagService.RetrieveTagAsync(tagName, Context.GuildId);

            if (tag is null)
                return await TagNotFoundResponse(tagName);

            return Response(tag.Content);
        }

        [Command("add", "create")]
        public async Task<DiscordCommandResult> AddTagAsync(string tagName, [Remainder] string content)
        {
            if (await _tagService.RetrieveTagAsync(tagName, Context.GuildId) is not null)
                return Response("It seems a tag with this name already exists.");
            if (!IsTagNameValid(tagName))
                return Response($"The tag name \"{tagName}\" is forbidden, please choose another name.");
            if (content.Length > 2000)
                return Response("A tag's content can't be longer than 200 characters long.");

            var tag = new Tag
            {
                GuildId = Context.GuildId,
                OwnerId = Context.Message.Author.Id,
                Name = tagName,
                Content = content,
                CreatedAt = DateTimeOffset.UtcNow
            };
            await _tagService.AddTagAsync(tag);
            
            return Response($"I have successfully added {tagName}.");
        }

        [Command("delete", "del", "remove")]
        public async Task<DiscordCommandResult> DeleteTagAsync(string tagName)
        {
            var tag = await _tagService.RetrieveTagAsync(tagName, Context.GuildId);

            if (tag is null)
                return Response("It seems no tag with that name could be found.");
            if (tag.OwnerId != Context.Author.Id)
                return Response("I cannot let you delete other's tags.");

            await _tagService.DeleteTagAsync(tag);
            return Response("I have successfully deleted that tag.");
        }

        [Command("edit")]
        public async Task<DiscordCommandResult> EditTagAsync(string tagName, [Remainder] string newContent)
        {
            var tag = await _tagService.RetrieveTagAsync(tagName, Context.GuildId);

            if (tag is null)
                return Response("It seems no tag with that name could be found.");
            if (tag.OwnerId != Context.Author.Id)
                return Response("I cannot let you edit other's tags.");

            await _tagService.EditTagAsync(tag, newContent);
            return Response("I have edited that tag successfully.");
        }

        [Command("list")]
        public async Task<DiscordCommandResult> ListTagsAsync()
        {
            var tags = await _tagService.RetrieveTagsAsync(Context.GuildId);
            tags = tags.OrderByDescending(x => x.Uses).ToList();

            var i = 0;
            var tagStrings = tags.Select(x => $"`#{++i}` {x.Name} ({Mention.User(x.OwnerId)})\n");
            var stringPages = new List<string>();
            
            var current = "";
            foreach (var tagString in tagStrings)
            {
                if((current + tagString).Length <= 2048)
                    current += tagString;
                else
                {
                    stringPages.Add(current);
                    current = tagString;
                }
            }
            if (!string.IsNullOrWhiteSpace(current))
                stringPages.Add(current);

            var pages = stringPages.Select(x => new Page(
                    new LocalEmbedBuilder()
                        .WithLexColor()
                        .WithTitle("Tags")
                        .WithDescription(x)
                        .WithFooter($"Page {stringPages.IndexOf(x) + 1} of {stringPages.Count}")))
                .ToList();

            return pages.Count switch
            {
                0 => Response("There are no tags for this server."),
                1 => Response(pages[0].Embed),
                _ => Pages(pages)
            };
        }

        [Command("info")]
        public async Task<DiscordCommandResult> TagInfoAsync([Remainder] string tagName)
        {
            var tag = await _tagService.RetrieveTagAsync(tagName, Context.GuildId);
            if (tag is null)
                return await TagNotFoundResponse(tagName);

            var member = Context.Guild.GetMember(tag.OwnerId) ??
                         await Context.Guild.FetchMemberAsync(tag.OwnerId);

            var eb = new LocalEmbedBuilder()
                .WithTitle($"Tag: {tag.Name}")
                .WithLexColor()
                .AddField("Owner", member is null ? $"{tag.OwnerId} (not in server)" : member.Mention)
                .AddField("Uses", tag.Uses, true)
                .AddField("Rank", $"#{await _tagService.GetTagRankAsync(tag)}", true)
                .AddField("Created At", $"{tag.CreatedAt:yyyy-MM-dd}", true);

            return Response(eb);
        }
        
        [Command("claim")]
        public async Task<DiscordCommandResult> Claim([Remainder] string name)
        {
            var tag = await _tagService.RetrieveTagAsync(name ,Context.GuildId);
            if (tag is null) 
                return await TagNotFoundResponse(name);
            
            var member = Context.Guild.GetMember(tag.OwnerId) ?? 
                         await Context.Guild.FetchMemberAsync(tag.OwnerId);
                
            if (member is not null)
                return Response($"The owner of the tag \"{name}\" is still in the server.");

            tag.OwnerId = Context.Message.Author.Id;
            await _tagService.UpdateTagAsync(tag);
            
            return Response($"Ownership of the tag \"{name}\" was successfully transferred to you.");
        }
        
        [Command("transfer")]
        public async Task<DiscordCommandResult> Transfer(string name, [Remainder] [RequireNotBot] IMember member)
        {
            var tag = await _tagService.RetrieveTagAsync(name,Context.GuildId);
            if (tag is null) 
                return await TagNotFoundResponse(name);
            
            if (tag.OwnerId != Context.Author.Id)
                return Response($"The tag \"{name}\" does not belong to you.");

            tag.OwnerId = member.Id;
            await _tagService.UpdateTagAsync(tag);
            
            return Response($"Ownership of the tag \"{name}\" was successfully transferred to {member.Mention}.");
        }
        
        private bool IsTagNameValid(string name)
            => _commandService
                .GetAllModules()
                .First(x => x.Type == typeof(TagModule)).Commands
                .All(x => x.Aliases
                    .All(y => !string.Equals(y, name, StringComparison.CurrentCultureIgnoreCase)));
    }
}