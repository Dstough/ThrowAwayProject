using System.Collections.Generic;

namespace ThrowAwayProjects.Models
{
    public class UserListViewModel : BaseViewModel
    {
        public List<UserViewModel> list { get; set; }
        public int page { get; set; }

        public UserListViewModel()
        {
            page = 0;
            list = new List<UserViewModel>();
        }
    }
}