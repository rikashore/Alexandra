using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexandra.Common.Extensions;
using Alexandra.Common.Utilities;
using Alexandra.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("note", "notes")]
    public class NoteModule : DiscordModuleBase
    {
        private readonly NoteService _noteService;

        public NoteModule(NoteService noteService)
        {
            _noteService = noteService;
        }

        [Command("create", "make")]
        [Description("Creates a note for you, you can retrieve it later")]
        public async Task<DiscordCommandResult> MakeNoteAsync([Remainder] string content)
        {
            await _noteService.CreateNoteAsync(content, Context.Author.Id, DateTime.Now);
            return Response("Note Created!");
        }

        [Command("list")]
        [Description("Retrieve all your notes")]
        public async Task<DiscordCommandResult> ListNotesAsync()
        {
            var notes = await _noteService.RetrieveNotesAsync(Context.Author.Id);

            switch (notes.Count)
            {
                case 0:
                    return Response("It seems you do not have any notes");
                case <= 5:
                {
                    var eb = new LocalEmbedBuilder().WithLexColor();

                    foreach (var note in notes)
                        eb.AddField($"Note {note.Id}", note.ToString());

                    var content = $"I have retrieved {notes.Count} {(notes.Count == 1 ? "note" : "notes")}";
                    return Response(content, eb);
                }
                default:
                {
                    var fieldBuilders = new List<LocalEmbedFieldBuilder>(notes.Count);

                    foreach (var note in notes)
                        fieldBuilders.Add(new LocalEmbedFieldBuilder().WithName($"Note {note.Id}").WithValue(note.ToString()));
                    
                    var config = FieldBasedPageProviderConfiguration.Default.WithContent($"You have {notes.Count} notes");
                    return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                }
            }
        }

        [Command("get", "retrieve")]
        [Description("retrieve a particular note")]
        public async Task<DiscordCommandResult> GetNoteAsync(int id)
        {
            var note = await _noteService.RetrieveNoteAsync(id);

            if (note is null)
                return Response("A note with that Id does not exist");

            var owner = await Context.Bot.FetchUserAsync(note.OwnerId);
            
            var eb = new LocalEmbedBuilder()
                .WithTitle($"Note {note.Id}")
                .WithDescription(note.ToString())
                .WithAuthor(owner)
                .WithLexColor();

            return Response(eb);
        }

        [Command("delete", "del", "remove")]
        [Description("Delete a particular note")]
        public async Task<DiscordCommandResult> DeleteNoteAsync(int id)
        {
            var note = await _noteService.RetrieveNoteAsync(id);

            if (note is null)
                return Response("It seems no note with that ID was found");
            if (note.OwnerId != Context.Author.Id)
                return Response("I cannot allow you to delete another user's note");

            await _noteService.DeleteNoteAsync(note);
            return Response("I have removed that note");
        }

        [Command("edit", "change", "modify")]
        [Description("change the contents of your note")]
        public async Task<DiscordCommandResult> EditNote(int id, [Remainder] string content)
        {
            var note = await _noteService.RetrieveNoteAsync(id);

            if (note is null)
                return Response("It seems no note with that ID was found");
            if (note.OwnerId != Context.Author.Id)
                return Response("I cannot allow you to delete another user's note");

            await _noteService.EditNoteAsync(note, content);
            return Response("The contents of that note have changed");
        }
        
    }
}