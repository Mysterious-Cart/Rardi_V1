using Microsoft.AspNetCore.Components;
using CHKS.Services;

namespace CHKS.Pages
{
    public partial class Login
    {

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected string redirectUrl;
        protected string error;
        protected string info;
        protected bool errorVisible = false;
        protected bool infoVisible = false;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var query = System.Web.HttpUtility.ParseQueryString(new Uri(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).ToString()).Query);

            error = query.Get("error");

            info = query.Get("info");

            redirectUrl = query.Get("redirectUrl");

            errorVisible = !string.IsNullOrEmpty(error);

            infoVisible = !string.IsNullOrEmpty(info);

        }
    }
}