using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexandra.Common.Extensions;
using Alexandra.Common.Utilities;
using Alexandra.Database.Helpers;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("note", "notes")]
    public class NoteModule : DiscordModuleBase
    {
        private readonly NoteHelper _noteHelper;

        public NoteModule(NoteHelper noteHelper)
        {
            _noteHelper = noteHelper;
        }

        [Command("create", "make")]
        [Qmmands.Description("Creates a note for you, you can retrieve it later")]
        public async Task<DiscordCommandResult> MakeNote([Remainder] string content)
        {
            await _noteHelper.CreateNoteAsync(content, Context.Author.Id, DateTime.Now);
            return Response("Note Created!");
        }

        [Command("list")]
        [Description("Retrieve all your notes")]
        public async Task<DiscordCommandResult> ListNotes()
        {
            var notes = await _noteHelper.RetrieveNotesAsync(Context.Author.Id);

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
        public async Task<DiscordCommandResult> GetNote(int id)
        {
            var note = await _noteHelper.RetrieveNoteAsync(id);

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
        public async Task<DiscordCommandResult> DeleteNote(int id)
        {
            var note = await _noteHelper.RetrieveNoteAsync(id);

            if (note is null)
                return Response("It seems no note with that ID was found");
            else if (note.OwnerId != Context.Author.Id)
                return Response("I cannot allow you to delete another user's note");

            await _noteHelper.DeleteNoteAsync(note);
            return Response("I have removed that note");
        }

        [Command("edit", "change", "modify")]
        [Description("change the contents of your note")]
        public async Task<DiscordCommandResult> EditNote(int id, [Remainder] string content)
        {
            var note = await _noteHelper.RetrieveNoteAsync(id);

            if (note is null)
                return Response("It seems no note with that ID was found");
            if (note.OwnerId != Context.Author.Id)
                return Response("I cannot allow you to delete another user's note");

            await _noteHelper.EditNoteAsync(note, content);
            return Response("The contents of that note have changed");
        }
        
    }
}