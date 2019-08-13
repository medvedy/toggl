using Android.Support.Design.Widget;
using Android.Widget;


namespace Toggl.Droid.Activities
{
    public partial class ForgotPasswordActivity
    {
        private TextInputLayout loginEmail;
        private EditText loginEmailEditText;
        private Button resetPasswordButton;
        private ProgressBar loadingProgressBar;

        protected override void InitializeViews()
        {
            loginEmail = FindViewById<TextInputLayout>(Resource.Id.LoginEmail);
            loginEmailEditText = FindViewById<EditText>(Resource.Id.LoginEmailEditText);
            resetPasswordButton = FindViewById<Button>(Resource.Id.ResetPasswordButton);
            loadingProgressBar = FindViewById<ProgressBar>(Resource.Id.LoadingProgressBar);
        }
    }
}
