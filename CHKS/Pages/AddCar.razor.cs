using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace CHKS.Pages
{
    public partial class AddCar
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public mydbService mydbService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            car = new CHKS.Models.mydb.Car();

            cartsForPlate = await mydbService.GetCarts();
        }
        protected bool errorVisible;
        protected CHKS.Models.mydb.Car car;

        protected IEnumerable<CHKS.Models.mydb.Cart> cartsForPlate;

        protected async Task FormSubmit()
        {
            try
            {
                await mydbService.CreateCar(car);
                DialogService.Close(car);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}