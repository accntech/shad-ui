using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadUI.Demo.ViewModels
{
    [Page("poupmask")]
    public sealed partial class PoupMaskViewModel : ViewModelBase, INavigable
    {
        private readonly PageManager _pageManager;

        private readonly ToastManager _toastManager;
        private readonly LoginViewModel _loginViewModel;

        public PoupMaskViewModel(
            PageManager pageManager,
            ToastManager toastManager,
            LoginViewModel loginViewModel)
        {
            _pageManager = pageManager;
            _userControlManager = new DialogManager();
            _toastManager = toastManager;
            _loginViewModel = loginViewModel;

            var path = Path.Combine(AppContext.BaseDirectory, "viewModels", "DialogViewModel.cs");
            AlertDialogCode = WrapCode(path.ExtractByLineRange(62, 78).CleanIndentation());
            DestructiveAlertDialogCode = WrapCode(path.ExtractByLineRange(83, 100).CleanIndentation());
            CustomDialogCode = WrapCode(path.ExtractByLineRange(105, 122).CleanIndentation());
        }

        [RelayCommand]
        private void BackPage()
        {
            _pageManager.Navigate<DateViewModel>();
        }

        [RelayCommand]
        private void NextPage()
        {
            _pageManager.Navigate<InputViewModel>();
        }

        private string WrapCode(string code)
        {
            return $"""
                using CommunityToolkit.Mvvm.Input;

                //..other code

                {code}

                //..rest of the code

                """;
        }

        [ObservableProperty]
        private string _alertDialogCode = string.Empty;
        [ObservableProperty]
        private DialogManager _userControlManager;

        [RelayCommand]
        private void ShowDialog()
        {
            _userControlManager
                .CreateDialog(
                    "Are you absolutely sure?",
                    "This action cannot be undone. This will permanently delete your account and remove your data from our servers.")
                .WithPrimaryButton("Continue",
                    () => _toastManager.CreateToast("Delete account")
                        .WithContent("Account deleted successfully!")
                        .DismissOnClick()
                        .ShowSuccess())
                .WithCancelButton("Cancel")
                .WithMaxWidth(512)
                .Dismissible().Show();
        }

        [ObservableProperty]
        private string _destructiveAlertDialogCode = string.Empty;

        [RelayCommand]
        private void ShowDestructiveStyleDialog()
        {
            _userControlManager
                .CreateDialog(
                    "Are you absolutely sure?",
                    "This action cannot be undone. This will permanently delete your account and remove your data from our servers.")
                .WithPrimaryButton("Continue",
                    () => _toastManager.CreateToast("Delete account")
                        .WithContent("Account deleted successfully!")
                        .DismissOnClick()
                        .ShowSuccess()
                    , DialogButtonStyle.Destructive)
                .WithCancelButton("Cancel")
                .WithMaxWidth(512)
                .Dismissible()
                .Show();
        }

        [ObservableProperty]
        private string _customDialogCode = string.Empty;

        [RelayCommand]
        private void ShowCustomDialog()
        {
            _loginViewModel.Initialize();
            _userControlManager.CreateDialog(_loginViewModel)
                .Dismissible()
                .WithSuccessCallback(vm =>
                    _toastManager.CreateToast("Sign in successful")
                        .WithContent($"Hi {vm.Email}, welcome back!")
                        .DismissOnClick()
                        .ShowSuccess())
                .WithCancelCallback(() =>
                    _toastManager.CreateToast("Sign in cancelled")
                        .WithContent("Please sign in to continue.")
                        .DismissOnClick()
                        .ShowWarning())
                .Show();
        }
    }
}
