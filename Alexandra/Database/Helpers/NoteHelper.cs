using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexandra.Database.Entities;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Alexandra.Database.Helpers
{
    public class NoteHelper
    {
        private readonly LexDbContext _lexDbContext;

        public NoteHelper(LexDbContext lexDbContext)
        {
            _lexDbContext = lexDbContext;
        }

        public async Task<Note> RetrieveNoteAsync(int id)
        {
            var note = await _lexDbContext.Notes
                .FindAsync(id);

            return note;
        }

        public async Task<List<Note>> RetrieveNotesAsync(Snowflake ownerId)
        {
            var notes = await _lexDbContext.Notes.AsNoTracking()
                .Where(x => x.OwnerId == ownerId)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

            return notes;
        }

        public async Task CreateNoteAsync(string content, Snowflake ownerId, DateTime createdOn)
        {
            _lexDbContext.Add(new Note {Content = content, OwnerId = ownerId, CreatedOn = createdOn});
            await _lexDbContext.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(Note note)
        {
            _lexDbContext.Notes.Remove(note);
            await _lexDbContext.SaveChangesAsync();
        }

        public async Task EditNoteAsync(Note note, string content)
        {
            note.Content = content;
            await _lexDbContext.SaveChangesAsync();
        }
    }
}