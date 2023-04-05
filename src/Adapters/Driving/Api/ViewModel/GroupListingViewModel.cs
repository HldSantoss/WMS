using System.ComponentModel.DataAnnotations;

namespace Api.ViewModel
{
    public class CreateGroupListingViewModel
    {
        public CreateGroupListingViewModel(string code, string name)
        {
            Code = code;
            Name = name;
        }

        [Required(ErrorMessage = "{0} é obrigatório")]
        public string Code { get; init; }
        
        [Required(ErrorMessage = "{0} é obrigatório")]
        public string Name { get; init; }
    }

    public class UserViewModel
    {
        public UserViewModel(int lineId, string userId)
        {
            LineId = lineId;
            UserId = userId;
        }

        public int LineId { get; set; }
        public string UserId { get; set; }
    }

    public class GroupListingViewModel
    {
        public GroupListingViewModel(string code,
                                     string name,
                                     long docEntry,
                                     DateTime createDate,
                                     TimeSpan createTime,
                                     DateTime updateDate,
                                     TimeSpan updateTime,
                                     List<UserViewModel> users)
        {
            Code = code;
            Name = name;
            DocEntry = docEntry;
            CreateDate = createDate;
            CreateTime = createTime;
            UpdateDate = updateDate;
            UpdateTime = updateTime;
            Users = users;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public long DocEntry { get; set; }
        public DateTime CreateDate { get; set; }
        public TimeSpan CreateTime { get; set; }
        public DateTime UpdateDate { get; set; }
        public TimeSpan UpdateTime { get; set; }
        public List<UserViewModel> Users { get; set; }
    }
}