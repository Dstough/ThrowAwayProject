namespace ThrowAwayProjects.Models
{
    public class BaseViewModel
    {
        public string ErrorMessage { get; set; }

        public BaseViewModel()
        {
            ErrorMessage = null;
        }

        public BaseViewModel(BaseViewModel viewModel)
        {
            ErrorMessage = viewModel.ErrorMessage;
        }
    }
}