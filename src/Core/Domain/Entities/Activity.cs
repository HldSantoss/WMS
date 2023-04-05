using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Activity
    {
        public Activity(string date_activity, string username, string action_activity, string reference, string branch)
        {
            Date_activity = date_activity;
            Username = username;
            Action_activity = action_activity;
            Reference = reference;
            Branch = branch;
        }

        public int Id { get; set; }
        public string Date_activity { get; set; }
        public string Username { get; set; }
        public string Action_activity { get; set; }
        public string Reference { get; set; }
        public string Branch { get; set; }
    }
}
