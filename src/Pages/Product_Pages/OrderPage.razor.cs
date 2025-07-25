using System.Linq.Dynamic.Core;
using CHKS.Entity;
using CHKS.Models;
using CHKS.Pages.Component.Popup;
using CHKS.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CHKS.Pages
{
    public partial class OrderPage
    {
        [Inject] protected InventoryControlService InventoryService { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }

        private readonly List<Order> Orderlist = [];

        private OrdersInfo ordersinfo = new();

        private string search = "";

        protected async override Task OnInitializedAsync()
        {
            await InventoryService.GetOngoingOrders();
        }

        private void GetOrderDetail()
        {
            ordersinfo.TotalOrder = Orderlist.Count;
            ordersinfo.TotalProductOrdered = Orderlist.Sum(i => i.Amount);
            ordersinfo.TotalNetWorth = 0;
        }

        private async Task OnOrderButtonClick(bool isConfirm, Order order)
        {
            var isSure = await DialogService.ShowMessageBox(
                "Proceed?",
                message: "Are you sure?",
                "Yes",
                "Cancel",
                null,
                new()
                {
                    FullWidth = true,
                }
            ) ?? false;

            if (isSure == false) return;

            try
            {
                if (isConfirm)
                {
                    await InventoryService.ConfirmOrder(order.Id);
                }
                else
                {
                    await InventoryService.CancelOrder(order.Id);
                }


            }
            catch (Exception exc)
            {
                await DialogService.ShowMessageBox("Failed to process order", $"ERROR: {exc.Message}", "Ok");
            }

            await InventoryService.GetOngoingOrders();

        }
        private async Task GetOngoingOrders(string searchText) => await InventoryService.GetOngoingOrders(searchText);
        private async Task NewOrder()
        {
            var order = await DialogService.ShowAsync<NewOrder>("New Order");
            var result = await order.Result;
            if (!result.Canceled)
            {
                await InventoryService.GetOngoingOrders();
            }
        }
    }
    public class OrdersInfo
    {
        public int TotalOrder { get; set; }
        public int SuggestProductForOrder { get; set; }
        public int TotalProductOrdered { get; set; }
        public decimal TotalNetWorth { get; set; } 
    }
}

