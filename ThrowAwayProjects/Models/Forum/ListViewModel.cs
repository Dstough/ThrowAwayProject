using System.Collections.Generic;

namespace ThrowAwayProjects.Models
{
    public class ListViewModel
    {
        public int page { get; set; }
        public int maxRecordCount { get; set; }
        public List<ThreadViewModel> threads { get; set; }

        public ListViewModel(int _page, int _maxRecordCount)
        {
            page = _page;
            maxRecordCount = _maxRecordCount;
            threads = new List<ThreadViewModel>();
        }
        public ListViewModel() : this(0, 20)
        {
        }
    }
}
