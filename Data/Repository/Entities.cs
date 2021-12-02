using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Doctors
{
    public class BaseEntity
    {
        [Key]
        public string Id { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdateDate { get; set; }
    }

    public class ChatUser : BaseEntity
    {
        public string Email { get; set; }

        public List<ChatUser> Accepted = new List<ChatUser>();
        public ChatUser(string Email)
        {

        }
    }
}
