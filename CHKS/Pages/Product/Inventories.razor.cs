using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using CHKS.Models.mydb;
using CHKS.Pages.Component.Popup;
using Microsoft.EntityFrameworkCore;
using CHKS.Models.Interface;
using CHKS.Services;

namespace CHKS.Pages
{
    public partial class Inventories
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected MudBlazor.IDialogService MudDialogService { get; set; }

        [Inject]
        protected IDbProvider _dbProvider {get; set;}

        [Parameter]
        public string ProductIdParam {get; set;} = "";

        private List<Inventory> _inventories = [];
        private IEnumerable<Inventory> inventories = [];
        private Inventory Product;
        private int TotalSale = 0;
        private decimal Revenue = 0;
        private decimal Acquisition = 0;


        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {   
            await GetProductFromInventory();
            GetUriQuery();
            if(!string.IsNullOrEmpty(ProductIdParam)){
                Select_Product(inventories.Where(i => i.Id == Guid.Parse(ProductIdParam)).First());
            }
        }

        private async Task GetProductFromInventory(){
            var query_product = await _dbProvider.GetData<Inventory>([nameof(Inventory.HistoryConnectors), nameof(Inventory.Tags)]);
            _inventories = query_product.OrderByDescending(i => i.Sold_Total).AsEnumerable().ToList();
            inventories = _inventories;
            
        }

        private async Task Search(ChangeEventArgs args){
            inventories = _inventories.AsParallel().Where(i => i.Name.Contains(args.Value.ToString()));
            
        }

        private void GetUriQuery(){
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParam = System.Web.HttpUtility.ParseQueryString(uri.Query);
            ProductIdParam = queryParam["ProductIdParam"];
        }
        
        private void Select_Product(Inventory ChoosenProduct){
            Product = ChoosenProduct;
            TotalSale = ChoosenProduct.GetSoldTotal();
            Revenue = ChoosenProduct.HistoryConnectors?.Sum(i => i.Export * i.Qty)??0;
            Acquisition = ChoosenProduct.Stock * ChoosenProduct.Import;
        }

    }
}