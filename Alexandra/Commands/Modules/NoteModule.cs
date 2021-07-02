using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexandra.Commands.Bases.ModuleBases;
using Alexandra.Common.Extensions;
using Alexandra.Common.Utilities;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("note", "notes")]
    [Name("Note")]
    [Description("Take a few notes")]
    public class NoteModule : LexNoteModuleBase
    {
        [Command("create", "make")]
        [Description("Creates a note for you, you can retrieve it later")]
        public async Task<DiscordCommandResult> MakeNoteAsync([Description("The content of your new note"), Remainder] string content)
        {
            await NoteService.CreateNoteAsync(content, Context.Author.Id, DateTime.Now);
            return Response("Note Created!");
        }

        [Command("list")]
        [Description("Retrieve all your notes")]
        public async Task<DiscordCommandResult> ListNotesAsync()
        {
            var notes = await NoteService.RetrieveNotesAsync(Context.Author.Id);

            switch (notes.Count)
            {
                case 0:
                    return Response("It seems you do not have any notes");
                case <= 5:
                {
                    var eb = new LexEmbed();

                    foreach (var note in notes)
                        eb.AddField($"Note {note.Id}", note.ToString());

                    var content = $"I have retrieved {notes.Count} {(notes.Count == 1 ? "note" : "notes")}";
                    return Response(content, eb);
                }
                default:
                {
                    var fieldBuilders = new List<LocalEmbedField>(notes.Count);

                    foreach (var note in notes)
                        fieldBuilders.Add(new LocalEmbedField().WithName($"Note {note.Id}").WithValue(note.ToString()));
                    
                    var config = FieldBasedPageProviderConfiguration.Default.WithContent($"You have {notes.Count} notes");
                    return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                }
            }
        }

        [Command("get", "retrieve")]
        [Description("retrieve a particular note")]
        public async Task<DiscordCommandResult> GetNoteAsync([Description("The Id of the note")] int id)
        {
            var note = await NoteService.RetrieveNoteAsync(id);

            if (note is null)
                return NoteNotFoundResponse();

            var owner = await Context.Bot.FetchUserAsync(note.OwnerId);

            var eb = new LexEmbed()
                .WithTitle($"Note {note.Id}")
                .WithDescription(note.ToString())
                .WithAuthor(owner);

            return Response(eb);
        }

        [Command("delete", "del", "remove")]
        [Description("Delete a particular note")]
        public async Task<DiscordCommandResult> DeleteNoteAsync([Description("The Id of the note")] int id)
        {
            var note = await NoteService.RetrieveNoteAsync(id);

            if (note is null)
                return NoteNotFoundResponse();
            if (note.OwnerId != Context.Author.Id)
                return InvalidAccessResponse();

            await NoteService.RemoveNoteAsync(note);
            return Response("I have removed that note");
        }

        [Command("edit", "change", "modify")]
        [Description("change the contents of your note")]
        public async Task<DiscordCommandResult> EditNote([Description("The Id of the note")] int id,
            [Description("The new content of your note"), Remainder] string content)
        {
            var note = await NoteService.RetrieveNoteAsync(id);

            if (note is null)
                return NoteNotFoundResponse();
            if (note.OwnerId != Context.Author.Id)
                return InvalidAccessResponse();

            await NoteService.EditNoteAsync(note, content);
            return Response("The contents of that note have changed");
        }
    }
}