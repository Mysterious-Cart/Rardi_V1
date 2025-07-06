using System.Linq.Dynamic.Core;
using CHKS.Models.mydb;
using CHKS.Pages.Component.Popup;
using CHKS.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Radzen;

namespace CHKS.Pages
{
    public partial class OrderPage
    {
        [Inject] protected InventoryControlService InventoryService { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }

        private IEnumerable<Order_Model> Orderlist = [];


        private OrdersInfo ordersinfo = new();

        private string search = "";

        protected async override Task OnInitializedAsync()
        {
            await GetOngoingOrders();
        }

        private async Task GetOngoingOrders(string query = "")
        {
            var order_list_query = await InventoryService.GetOrders();
            Orderlist = order_list_query.Include(i => i.Product).Where(i => i.Product.Name.Contains(query))
                .OrderBy(i => i.IsCancelled)
                .ThenBy(i => i.IsOrderReceived)
                .ThenBy(i => i.OrderReceivedDate).ToList();

            GetOrderDetail();
        }

        private void GetOrderDetail()
        {
            ordersinfo.TotalOrder = Orderlist.Count();
            ordersinfo.TotalProductOrdered = Orderlist.Sum(i => i.Amount);
            ordersinfo.TotalNetWorth = Orderlist.Sum(i => i.Product.Import * i.Amount);
        }

        private async Task OnOrderButtonClick(bool isConfirm, Order_Model order)
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

            await GetOngoingOrders();

        }

        private async Task NewOrder()
        {
            var order = await DialogService.ShowAsync<NewOrder>("New Order");
            var result = await order.Result;
            if (!result.Canceled)
            {
                await GetOngoingOrders();
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

