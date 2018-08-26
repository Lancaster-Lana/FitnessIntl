
namespace RevolutionarySHOP.ViewModels
{
    public class AccountSettingsViewModel //: : INotifyPropertyChanged
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
    }
}