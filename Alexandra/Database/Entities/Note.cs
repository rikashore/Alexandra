using System;
using System.ComponentModel.DataAnnotations;
using Alexandra.Common.Extensions;
using Disqord;

namespace Alexandra.Database.Entities
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        
        public Snowflake OwnerId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        
        public override string ToString()
        {
            var article = CreatedOn.Date == DateTime.Today ? "at" : "on";
            return $"{Content} (Created {article} {CreatedOn.Humanize()})";
        }
    }
}