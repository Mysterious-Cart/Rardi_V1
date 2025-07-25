using Microsoft.AspNetCore.Components;
using CHKS.Services;
using MudBlazor;
using Microsoft.AspNetCore.WebUtilities;

namespace CHKS.Pages
{
    public partial class Login
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }
        [Inject] 
        private HttpClient Http { get; set; }
        
        protected string redirectUrl;
        protected string error;
        protected string info;
        protected bool errorVisible = false;
        protected bool infoVisible = false;
        
        private class LoginFormModel
        {
            [Label("Username")]
            public string Username { get; set; }
            [Label("Password")]
            public string Password { get; set; }
        }
        
        private LoginFormModel LoginModel = new LoginFormModel();

        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            error = queryParams.TryGetValue("error", out var errorValue) ? errorValue.ToString() : null;
            info = queryParams.TryGetValue("info", out var infoValue) ? infoValue.ToString() : null;
            redirectUrl = queryParams.TryGetValue("redirectUrl", out var redirectValue) ? redirectValue.ToString() : null;

            errorVisible = !string.IsNullOrEmpty(error);
            infoVisible = !string.IsNullOrEmpty(info);

            await base.OnInitializedAsync();
        }
    }
}