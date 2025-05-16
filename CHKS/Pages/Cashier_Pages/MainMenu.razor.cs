using Microsoft.AspNetCore.Components;
using Radzen;


namespace CHKS.Pages
{
    public partial class MainMenu
    {

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager {get; set;}

    }
}

