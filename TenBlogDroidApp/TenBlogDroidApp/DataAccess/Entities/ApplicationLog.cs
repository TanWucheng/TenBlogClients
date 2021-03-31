using System;
using SQLite;

namespace TenBlogDroidApp.DataAccess.Entities
{
    [Table("ApplicationLog")]
    public class ApplicationLog
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [MaxLength(128)]
        public string NameSpace { get; set; }

        [MaxLength(128)]
        public string ClassName { get; set; }

        [MaxLength(128)]
        public string FuncName { get; set; }

        public MessageType MessageType { get; set; }

        public string Message { get; set; }

        public DateTime LogTime { get; set; }
    }
}