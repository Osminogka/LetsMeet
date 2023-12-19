using LetsMeet.Models;
using LetsMeet.Pages.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LetsMeet.Pages
{
    public class MainTableModel : UserPageModel
    {

        //fills to manage databases

        public UserManager<IdentityUser> UserManager;
        public DataContext Context;

        public MainTableModel(UserManager<IdentityUser> usrManager, DataContext ctx)
        {
            UserManager = usrManager;
            Context = ctx;
        }

        //Information about current month

        public MonthBase Month = new MonthBase();

        //Selected day

        public int? SelectedDay { get; set; } = 999;

        //Property to create record
        [BindProperty]
        public int Hours { get; set; }
        [BindProperty]
        public int Minutes { get; set; }
        [BindProperty]
        public string Importance { get; set; }
        [BindProperty]
        public string FriendName { get; set; }
        [BindProperty]
        public string RecordName { get; set; }
        [BindProperty]
        public string RecordDis { get; set; }


        ///////////////////////////////////////////////////////////////////


        //List to fill main information

        public List<string> GroupList = new List<string>();
        public List<string> FriendListRaw = new List<string>();
        public List<string> FriendList = new List<string>();
        public List<FriendInvite> InviteListRaw = new List<FriendInvite>();
        public List<string> InviteList = new List<string>();
        public List<Record> RecordsList = new List<Record>();


        /////////////////////////////////////////////////////////////////


        //Properties for errors

        public string RecordCreateError { get; set; }

        public string InvalidInvite { get; set; }

        public string InvalidFriend { get; set; }

        public string InvalidGroup { get; set; }

        public string InvalidDay { get; set; }


        /////////////////////////////////////////////////////////////////


        /////////////////////////////GET METHODS///////////////////////////////////

        public async Task OnGetAsync()
        {
            await LoadUserData();
        }

        /////////////////////////////////////////////////////////////////



        /////////////////////////////POST METHODS///////////////////////////////////

        public async Task<IActionResult> OnPostAddFriendAsync(string friendname)
        {
            string tempUserLocal = HttpContext.User.Identity.Name;

            List<FriendInvite> checkIfAnyInvites = Context.InviteList.Where(obj => (obj.MainUserName == friendname && obj.FriendUserName == tempUserLocal) ||
                (obj.MainUserName == tempUserLocal && obj.FriendUserName == friendname)).ToList();

            if (!checkIfAnyInvites.Any())
            {
                ModelState.AddModelError("InvalidInvite", "invite doesn't exist");
                await LoadUserData();
                return Page();
            }

            foreach (FriendInvite TempInvites in checkIfAnyInvites)
                Context.InviteList.Remove(TempInvites);

            UserFriendList tempFriendRecord = new UserFriendList
            {
                MainUserName = tempUserLocal,
                FriendUserName = friendname
            };
            await Context.userFriendLists.AddAsync(tempFriendRecord);

            await Context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectFriendAsync(string friendname)
        {
            IdentityUser tempUserLocal = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var checkIfAnyInvites = Context.InviteList.Where(obj => obj.MainUserName == friendname && obj.FriendUserName == tempUserLocal.UserName);

            if (!checkIfAnyInvites.Any())
            {
                ModelState.AddModelError("InvalidInvite", "invite doesn't exist");
                await LoadUserData();
                return Page();
            }

            Context.InviteList.Remove(checkIfAnyInvites.First());

            await Context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveFriendAsync(string FriendName)
        {
            IdentityUser tempUserLocal = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var tempRecord = Context.userFriendLists.Where(obj => (obj.MainUserName == tempUserLocal.UserName && obj.FriendUserName == FriendName)
                       || (obj.MainUserName == FriendName && obj.FriendUserName == tempUserLocal.UserName)).First();

            if (tempRecord == null)
            {
                ModelState.AddModelError("InvalidFriend", "you don't have such friend");
                return Page();
            }

            Context.userFriendLists.Remove(tempRecord);
            await Context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostGetInfoAboutFriendAsync(string arg1)
        {


            return RedirectToPage();
        }

        //LeaveGroup
        public async Task<IActionResult> OnPostLeaveGroupAsync(string groupname)
        {
            IdentityUser TempUserLocal = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var check = Context.GroupRecords.SingleOrDefault(obj => obj.GroupName == groupname && obj.UserName == TempUserLocal.UserName);

            if (check == null)
            {
                ModelState.AddModelError("InvalidGroup", "you don't participate in such group");
                await LoadUserData();
                return Page();
            }

            Context.GroupRecords.Remove(check);
            await Context.SaveChangesAsync();

            return RedirectToPage();
        }

        //getdayinfo
        public async Task<IActionResult> OnPostGetDayInfoAsync(int daynumber)
        {
            if (daynumber < 0 && daynumber > 31)
            {
                ModelState.AddModelError("InvalidDay", "you entered invalid day number");
                await LoadUserData();
                return Page();
            }

            if (daynumber == 0)
            {
                HttpContext.Session.SetInt32("SelectedDay", 999);
                return RedirectToPage();
            }

            HttpContext.Session.SetInt32("SelectedDay", daynumber);

            return RedirectToPage();
        }

        //createrecord
        public async Task<IActionResult> OnPostCreateRecordAsync()
        {
            IdentityUser tempUserLocal = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (FriendName == null)
            {
                ModelState.AddModelError("RecordCreateError", "Unable to create record");
                return Page();
            }

            int tempSelectedDay = HttpContext.Session.GetInt32("SelectedDay") == null ? 99 : (int)HttpContext.Session.GetInt32("SelectedDay");
            int tempSelectedMonth = HttpContext.Session.GetInt32("SelectedMonth") == null ? 99 : (int)HttpContext.Session.GetInt32("SelectedMonth");
            if ((0 >= tempSelectedDay || tempSelectedDay > 31) || (tempSelectedMonth < 1 || tempSelectedMonth > 12))
            {
                ModelState.AddModelError("RecordCreateError", "unexisting day or month");
                return Page();
            }

            bool hasTime = false;
            TimeSpan specifiedTime = new TimeSpan();
            if (Hours >= 0 && Hours <= 23 && Minutes <= 59 && Minutes >= 0)
            {
                specifiedTime = new TimeSpan(Hours, Minutes, 0);
                hasTime = true;
            }

            Record tempRecord = new Record();

            if (FriendName.StartsWith("Group/$/"))
            {
                FriendName = FriendName.Substring("Group/$/".Length);
                var tempFriendRecord = Context.GroupRecords.FirstOrDefault(obj => obj.GroupName == FriendName && obj.UserName == tempUserLocal.UserName);
                if (tempFriendRecord == null)
                {
                    ModelState.AddModelError("RecordCreateError", "Invalid group");
                    return Page();
                }

                var checkForRecords = Context.Records.SingleOrDefault(obj => obj.CreaterUserName == tempUserLocal.UserName && obj.GroupName == FriendName && obj.RecordName == RecordName);
                if (checkForRecords != null)
                {
                    ModelState.AddModelError("RecordCreateError", "Record with such name for this group already exist");
                    return Page();
                }

                tempRecord.CreaterUserName = tempUserLocal.UserName;
                tempRecord.RelatedUserName = "none";
                tempRecord.RecordName = RecordName;
                tempRecord.RecordString = RecordDis;
                tempRecord.GroupName = FriendName;
                tempRecord.DayNumber = (int)tempSelectedDay;
                tempRecord.MonthNumber = (int)tempSelectedMonth;
                tempRecord.CreaterUserName = tempUserLocal.UserName;
                tempRecord.Importance = Importance;
                tempRecord.Time = hasTime ? specifiedTime : new TimeSpan();
            }
            else
            {
                IdentityUser tempFriend = await UserManager.FindByNameAsync(FriendName);
                var tempFriendRecord = Context.userFriendLists.FirstOrDefault(obj => (obj.MainUserName == tempUserLocal.UserName && obj.FriendUserName == FriendName)
                       || (obj.MainUserName == FriendName && obj.FriendUserName == tempUserLocal.UserName));

                if (tempFriendRecord == null)
                {
                    ModelState.AddModelError("RecordCreateError", "You don't have this user in friend list");
                    return Page();
                }

                var checkForRecords = Context.Records.SingleOrDefault(obj => obj.CreaterUserName == tempUserLocal.UserName && obj.RelatedUserName == FriendName && obj.RecordName == RecordName);
                if (checkForRecords != null)
                {
                    ModelState.AddModelError("RecordCreateError", "Record with such name and user already exist");
                    return Page();
                }

                tempRecord.CreaterUserName = tempUserLocal.UserName;
                tempRecord.RelatedUserName = FriendName;
                tempRecord.RecordName = RecordName;
                tempRecord.RecordString = RecordDis;
                tempRecord.GroupName = "None";
                tempRecord.DayNumber = (int)tempSelectedDay;
                tempRecord.MonthNumber = (int)tempSelectedMonth;
                tempRecord.CreaterUserName = tempUserLocal.UserName;
                tempRecord.Importance = Importance;
                tempRecord.Time = hasTime ? specifiedTime : new TimeSpan();
            }


            if (!tempRecord.IsValid())
            {
                ModelState.AddModelError("RecordCreateError", "Error while creating record");
                return Page();
            }

            await Context.Records.AddAsync(tempRecord);
            await Context.SaveChangesAsync();

            return RedirectToPage();
        }

        //prevmonth
        public async Task<IActionResult> OnPostPrevMonthAsync()
        {
            var tempSelectedMonth = HttpContext.Session.GetInt32("SelectedMonth");

            if (tempSelectedMonth - 1 <= 0)
                HttpContext.Session.SetInt32("SelectedMonth", 12);
            else
                HttpContext.Session.SetInt32("SelectedMonth", (int)tempSelectedMonth - 1);

            return RedirectToPage();
        }

        //nextmonth
        public async Task<IActionResult> OnPostNextMonthAsync()
        {
            var tempSelectedMonth = HttpContext.Session.GetInt32("SelectedMonth");
            if (tempSelectedMonth + 1 > 12)
                HttpContext.Session.SetInt32("SelectedMonth", 1);
            else
                HttpContext.Session.SetInt32("SelectedMonth", (int)tempSelectedMonth + 1);

            return RedirectToPage();
        }

        /////////////////////////////////////////////////////////////////



        /////////////////////////////CUSTOM METHODS///////////////////////////////////

        private async Task LoadUserData()
        {
            var tempSelectedMonth = HttpContext.Session.GetInt32("SelectedMonth");
            if (tempSelectedMonth != null)
            {
                Month = new MonthBase()
                {
                    MonthNumber = (int)tempSelectedMonth,
                    DayAmount = MonthInfo.monthDays[MonthInfo.monthDictionary[(int)tempSelectedMonth]],
                    Meetings = new List<Record>(),
                    CurrentDay = DateTime.Now.Day
                };
            }
            else
            {
                Month = new MonthBase()
                {
                    MonthNumber = DateTime.Today.Month,
                    DayAmount = MonthInfo.monthDays[DateTime.Now.ToString("MMMM")],
                    Meetings = new List<Record>(),
                    CurrentDay = DateTime.Now.Day
                };
            }

            var tempSelectedDay = HttpContext.Session.GetInt32("SelectedDay");

            if (tempSelectedDay == null)
                HttpContext.Session.SetInt32("SelectedDay", Month.CurrentDay);

            HttpContext.Session.SetInt32("SelectedMonth", Month.MonthNumber);
            HttpContext.Session.SetInt32("DayAmount", Month.DayAmount);

            IdentityUser user = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            //GroupList
            GroupList = Context.GroupRecords.Where(obj => obj.UserName == user.UserName).Select(obj => obj.GroupName).ToList();

            //FriendList
            FriendList = Context.userFriendLists.Where(obj => obj.MainUserName == user.UserName || obj.FriendUserName == user.UserName).
                Select(obj => obj.FriendUserName == user.UserName ? obj.MainUserName : obj.FriendUserName).ToList();

            //InviteList
            InviteList = Context.InviteList.Where(obj => obj.FriendUserName == user.UserName).Select(obj => obj.MainUserName).ToList();

            //Records
            RecordsList = Context.Records.Where(obj => (obj.CreaterUserName == user.UserName || obj.RelatedUserName == user.UserName) && obj.MonthNumber == Month.MonthNumber && obj.GroupName == "none").ToList();
            List<Record> tempRecords = new List<Record>();
            foreach (string tempGroup in GroupList)
                tempRecords = Context.Records.Where(obj => obj.GroupName == tempGroup && obj.MonthNumber == Month.MonthNumber).ToList();
            foreach (Record tempRec in tempRecords)
                RecordsList.Add(tempRec);
        }
    }
}
