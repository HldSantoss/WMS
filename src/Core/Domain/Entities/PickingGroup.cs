using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public record PickingGroupUsers
    {
        public PickingGroupUsers(string u_UserId)
        {
            U_UserId = u_UserId;
        }

        public string U_UserId { get; init; }
    }

    public record PickingGroup
    {
        public PickingGroup(List<PickingGroupValue> pickingGroupItems)
        {
            PickingGroupItems = pickingGroupItems;
        }

        [JsonPropertyName("value")]
        public List<PickingGroupValue> PickingGroupItems { get; init; }
    }

    public record PickingGroupValue
    {
        public PickingGroupValue(string code, List<PickingGroupUsers> users)
        {
            Code = code;
            Users = users;
        }

        public string Code { get; init; }
        
        [JsonPropertyName("PICKINGGROUPUSERSCollection")]
        public List<PickingGroupUsers> Users { get; init; }
    }
}