using CHKS.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace CHKS.Shared
{
    public partial class MainLayout
    {


        [Inject]
        protected SecurityService Security { get; set; }

        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }
    }
}
