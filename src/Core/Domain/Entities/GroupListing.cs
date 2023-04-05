using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class GroupListingList
    {
        public GroupListingList(IEnumerable<GroupListing> groupListing)
        {
            GroupListing = groupListing;
        }

        [JsonPropertyName("value")]
        public IEnumerable<GroupListing> GroupListing { get; set; }
    }

    public class GroupListing
    {
        public GroupListing(string code, string name, long docEntry, DateTime createDate, TimeSpan createTime, DateTime updateDate, TimeSpan updateTime, long? bpl, List<User> users)
        {
            Code = code;
            Name = name;
            DocEntry = docEntry;
            CreateDate = createDate;
            CreateTime = createTime;
            UpdateDate = updateDate;
            UpdateTime = updateTime;
            Bpl = bpl;
            Users = users;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public long DocEntry { get; set; }
        public DateTime CreateDate { get; set; }
        public TimeSpan CreateTime { get; set; }
        public DateTime UpdateDate { get; set; }
        public TimeSpan UpdateTime { get; set; }

        [JsonPropertyName("U_CT_Branch")]
        public long? Bpl { get; set; }

        [JsonPropertyName("PICKINGGROUPUSERSCollection")]
        public List<User> Users { get; set; }
    }

    public class User
    {
        public User(int lineId, string userId)
        {
            LineId = lineId;
            UserId = userId;
        }

        public int LineId { get; set; }
        
        [JsonPropertyName("U_UserId")]
        public string UserId { get; set; }
    }
}