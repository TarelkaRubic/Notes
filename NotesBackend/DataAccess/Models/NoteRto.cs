using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccess.Models
{
    [Table("Note")]
    public class NoteRto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreateTimestamp { get; set; }

        #region Relationship
        public int UserId { get; set; }
        public UserRto User { get; set; }
        #endregion
    }
}
