using ThrowAwayData;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ThrowAwayProjects.Models
{
    public class UserViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Passphrase { get; set; }
        public bool Authenticated { get; set; }
        public bool Banned { get; set; }
        public bool Dead { get; set; }
        public List<SelectListItem> GroupOptions { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public UserViewModel() : this(new UserIdentity())
        {
        }
        public UserViewModel(UserIdentity user)
        {
            Id = user.Id ?? 0;
            GroupId = user.GroupId;
            Email = user.Email;
            UserName = user.UserName;
            Authenticated = user.Authenticated;
            Banned = user.Banned;
            Dead = user.Dead;
            GroupOptions = new List<SelectListItem>();
            GroupName = "";
        }
    }
}