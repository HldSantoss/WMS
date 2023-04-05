using System;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class ActivityLog
    {
        public ActivityLog()
        {
        }

        public ActivityLog(string userName, string userId, string actionUser, string docReference)
        {
            UserName = userName;
            UserId = userId;
            ActionUser = actionUser;
            DocReference = docReference;
        }

        [JsonPropertyName("U_UserName")]
        public string UserName { get; set; }

        [JsonPropertyName("U_UserId")]
        public string UserId { get; set; }

        [JsonPropertyName("U_UserAction")]
        public string ActionUser { get; set; }

        [JsonPropertyName("U_DocReference")]
        public string DocReference { get; set; }
    }
}


