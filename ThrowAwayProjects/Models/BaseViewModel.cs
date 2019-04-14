using System;
using Microsoft.AspNetCore.Http;
using ThrowAwayData;
using Newtonsoft.Json;

namespace ThrowAwayProjects.Models
{
    public class BaseViewModel
    {
        public string Status { get; set; }
        public string Privilege { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateSessionStarted { get; set; }
        public UserIdentity User { get; set; }

        public BaseViewModel()
        {
            Status = "Unknown";
            Privilege = "Unknown";
            ErrorMessage = null;
            DateSessionStarted = DateTime.Now;
            User = null;
        }

        public BaseViewModel(BaseViewModel viewModel)
        {
            Status = viewModel.Status;
            Privilege = viewModel.Privilege;
            ErrorMessage = viewModel.ErrorMessage;
            DateSessionStarted = viewModel.DateSessionStarted;
            User = viewModel.User;
        }
    }
}