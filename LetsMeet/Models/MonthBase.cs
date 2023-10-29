using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace LetsMeet.Models
{
    public class MonthBase
    {
        public int MonthNumber { get; set; }
        public int DayAmount { get; set; }
        public List<Record>? Meetings { get; set; }
        public int CurrentDay { get; set; }
    }

    public class MonthInfo
    {
        public static Dictionary<string, int> monthDays = new Dictionary<string, int>()
        {
            { "January", 31 },
            { "February", 28 }, // Assuming a non-leap year
            { "March", 31 },
            { "April", 30 },
            { "May", 31 },
            { "June", 30 },
            { "July", 31 },
            { "August", 31 },
            { "September", 30 },
            { "October", 31 },
            { "November", 30 },
            { "December", 31 }
        };

        public static Dictionary<int, string> monthDictionary = new Dictionary<int, string>
        {
            { 1, "January" },
            { 2, "February" },
            { 3, "March" },
            { 4, "April" },
            { 5, "May" },
            { 6, "June" },
            { 7, "July" },
            { 8, "August" },
            { 9, "September" },
            { 10, "October" },
            { 11, "November" },
            { 12, "December" }
        };
    }

    public class Record
    {
        public long RecordId { get; set; }

        [NotMapped]
        public List<string> UserList { get; set; } = new List<string>();

        [Required]
        public string CreaterUserName { get; set; } = String.Empty;

        [Required(ErrorMessage = "Creator unknown")]
        public string RecordCreatorId { get; set; } = String.Empty;

        public string GroupName {get;set;}

        public string RelatedUserId { get; set; } = String.Empty;

        [Required(ErrorMessage = "Unknown day")]
        public int DayNumber { get; set; }

        [Required(ErrorMessage = "Unkonow month")]
        public int MonthNumber { get; set; }

        [MinLength(1, ErrorMessage = "To short")]
        [MaxLength(256, ErrorMessage = "Message is too long")]
        [Required(ErrorMessage = "Provide name to record")]
        public string RecordName { get; set; } = String.Empty;

        [MinLength(1, ErrorMessage = "To short")]
        [MaxLength(256, ErrorMessage = "Message is too long")]
        public string RecordString { get; set; } = String.Empty;

        public bool IsValid()
        {
            return
                this.RecordCreatorId != null &&
                this.GroupName != null &&
                this.RelatedUserId != null &&
                this.RecordName != null &&
                this.RecordString != null &&
                this.DayNumber != 0 &&
                this.MonthNumber != 0;
        }
    }

    public class Group
    {
        public long GroupId { get; set; }

        [Required(ErrorMessage = "Group must have name")]  
        public string GroupName { get; set; } = String.Empty;

        [NotMapped]
        public List<string>? Members { get; set; } = new List<string>();

        [Required(ErrorMessage = "Must provide a creator")]
        public string CreatorId { get; set; } = String.Empty;
    }

    public class UserInfo
    {
        [Key]
        public long RecordId { get; set; }

        [Required(ErrorMessage = "User is missing")]
        public string UserName { get; set; } = String.Empty;

        [NotMapped]
        public IEnumerable<string>? FriendsList { get; set; }

        [NotMapped]
        public IEnumerable<Group>? GroupList { get; set; }
    }

    public class GroupRecords
    {
        [Key]
        public long RecordId { get; set; }

        [Required(ErrorMessage = "User must be provided")]
        public string GroupNameThatRecordBelong { get; set; }

        [Required(ErrorMessage = "User must be provided")]
        public string UserIdThatBelongsToGroup { get; set; }
    }

    public class UserFriendList
    {
        [Key]
        public long RecordId { get; set; }

        [Required(ErrorMessage = "Main userId must be provided")]
        public string MainUserId { get; set; }

        [Required(ErrorMessage = "FriendId must be provided")]
        public string FriendUserId { get; set; }
    }

    public class FriendInvite
    {
        [Key]
        public long RecordId { get; set; }

        [Required(ErrorMessage = "UserId is missing")]
        public string MainUserId { get; set; }

        [Required(ErrorMessage = "UserId is missing")]
        public string FriendUserName { get; set; }
    }

    public class User
    {
        [Key]
        public long RecordId { get; set; }

        [Required(ErrorMessage = "Username is missing")]
        public string UserName { get; set; }
    }
}
