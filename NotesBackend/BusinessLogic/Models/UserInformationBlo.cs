using System;
using System.Collections.Generic;
using System.Text;

namespace PS_223020_Server.BusinessLogic.Core.Models
{
    public class UserInformationBlo
    {
        public int Id { get; set; }
        public bool IsBoy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTimeOffset Birthday { get; set; }
        public string AvatarUrl { get; set; }
    }
}