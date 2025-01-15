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


namespace CHKS.Pages
{
    public partial class MainMenu
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager {get; set;}

        [Inject]
        protected mydbService MydbService {get; set;}

        [Inject]
        protected StockControlService StockControl {get; set;}

        [Inject]
        protected CartControlService CartControl {get; set;}

        Cart Current_Cart = null;
        protected Models.mydb.Car CustomerLists;

        protected IEnumerable<Cart> Carts = [];

        protected Models.mydb.History HistoryCustomer = new();
        protected IEnumerable<Historyconnector> Historyconnectors = [];

        protected string today = DateTime.Now.ToString("dd/MM/yy");

        protected IEnumerable<Connector> CartItem = [];
        protected List<Models.mydb.Inventory> _Inventories = [];
        protected List<Inventory> Inventories = [];
        protected IEnumerable<Models.mydb.History> RecentHistory = [];

        protected RadzenDataGrid<Inventory> Grid1;
        protected RadzenDataGrid<Models.mydb.Dailyexpense> grid2;
        
        protected Models.mydb.Connector connector = new(){};

        [Inject]
        protected SecurityService Security { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            
        }

        private async void ToggleCartMode(){
            if(Current_Cart.Status == 3){
                Current_Cart.Status = 0;
            }else{
                Current_Cart.Status += 1;
            }

            await UpdateCart(Current_Cart);
        }

        protected async Task UpdateCart(Cart Customer){
            await MydbService.UpdateCart(Customer.CartId, Customer);
        }

        protected async Task OpenExpenseMenu()
        {
            await DialogService.OpenAsync<ExpenseMenu>("Expense Menu");
        }
        protected async override Task OnInitializedAsync()
        {
            Console.WriteLine("StateChanged");
            await Task.Run(async () => {
                _Inventories = (await MydbService.GetInventories()).ToList();
                Inventories = _Inventories.ToList();
                
            }).ContinueWith(async (i) => {
                Carts = await MydbService.GetCarts();

            });
        }
        
        private async Task Search(ChangeEventArgs args)
        {
            var result = _Inventories.Where(i => i.Name.Contains(args.Value.ToString())).ToList();
            Inventories = result;
            await Grid1.Reload();
        }

        partial void OnCartOpen();
        partial void OnCartClose();

        protected async Task CreateCart(){
            Car car = await DialogService.OpenAsync<NewCart>("New Cart", new Dictionary<string, object>{}, new DialogOptions{Height="650px"});
            if(car is not null){
                Cart NewCart = new(){
                    Car_Id = car.Plate
                };
                await MydbService.CreateCart(NewCart);
                await OpenCart(NewCart.CartId);
            }
        }

        protected void CloseCart(){
            if(Current_Cart is not null){
                Current_Cart = null;
                OnCartClose();
                StateHasChanged();
            }
        }

        protected async Task OpenCart(int CartId){
            Current_Cart = await MydbService.GetCartByCartId(CartId);
            await LoadCartItem(CartId);
            OnCartOpen();
            StateHasChanged();
            
        }

        protected async Task LoadCartItem(int CartId){
            CartItem = await CartControl.GetCartItemFromCart(CartId);
        }

        protected async Task LoadRecentCashout(){
            RecentHistory = await MydbService.GetHistories();
            List<Models.mydb.History> tempHis = new();
            foreach(var i in RecentHistory.ToList()){
                char[] seperator = {':','('};
                string[] Temp = i.CashoutDate.Split(seperator,2);
                if( DateTime.ParseExact(Temp[0],"dd/MM/yyyy",null) == DateTime.Today){
                    tempHis.Add(i);
                }
            }
            RecentHistory = [];
            RecentHistory = tempHis;

        }

        protected async Task EditProduct(Models.mydb.Historyconnector Product){
            var UpdatedItems = await DialogService.OpenAsync<SingleInputPopUp>(Product.ProductId.ToString(), new Dictionary<string, object>{{"Info",new string[]{"EditItem", Product.Export.ToString(), Product.Qty.ToString(), Product.Note}}}, new DialogOptions{Width="250px"});
            if(UpdatedItems != null){
                Product.Qty = decimal.Parse(UpdatedItems[0]);
                Product.Export = decimal.Parse(UpdatedItems[1]);
                Product.Note = UpdatedItems[2];
                try{
                    await MydbService.UpdateHistoryconnector(Product.Id, Product);
                }catch(Exception exc){

                }
            }
        }

        protected async void ResetTotal(){
            Current_Cart.Total = CartItem.Sum(i => i.PriceOverwrite * i.Qty ).GetValueOrDefault();
            await MydbService.UpdateCart(Current_Cart.CartId, Current_Cart);

        }

        private bool showRecentCashout = false;
        protected async Task ToggleRecentCashout(){
            showRecentCashout = !showRecentCashout;

        }

        protected async Task DeleteCustomer(){
            if(await DialogService.Confirm("តើអ្នកច្បាស់ដែរឫទេ?","សំខាន់!", new ConfirmOptions{OkButtonText="Yes", CancelButtonText="No"}) == true)
            {
                if(CartItem != null){
                    await ClearAllProduct();
                    await MydbService.DeleteCart(Current_Cart.CartId);
                    await ResetToDefault();
                    await Toasting("អតិថជនត្រូវបានលុប");
                }else if(connector == null || connector == Enumerable.Empty<Models.mydb.Connector>()){
                    await MydbService.DeleteCart(Current_Cart.CartId);
                    await ResetToDefault();
                    await Toasting("អតិថជនត្រូវបានលុប");
                }
                
            }
            
        }

        protected string ToastState = "";
        
        protected async Task ChangeQty(Models.mydb.Connector Product, int Changes){
            
            
        }

        protected string ToastString = "";
        protected async Task Toasting(string ToastText){
            ToastState = "show";
            ToastString = ToastText;
            await Task.Delay(700);
            ToastState = "";
        }

        protected async Task CreateNewProduct(){
            await DialogService.OpenAsync<NewProduct>("Create Product", new Dictionary<string, object>{}, new DialogOptions{Width="fit-content", Height="fit-content"});
        }


        protected async Task CashOut(){
            if(Current_Cart is not null ){
                if(CartItem.Any()){
                    if(await DialogService.Confirm("តើអ្នកច្បាស់ដែរឫទេ?","សំខាន់!") == true){
                        Current_Cart.Total = CartItem.Sum(i => i.Qty * i.PriceOverwrite??i.Inventory.Export);
                        List<decimal?> payment = await DialogService.OpenAsync<PaymentForm>("គិតលុយ",
                        new Dictionary<string, object>{{"Total", Current_Cart.Total}}, 
                        new DialogOptions{Width="35%"});

                        if(payment != null){    
                            
                            History History = new(){
                                Plate = Current_Cart.Car_Id,
                                Total = Current_Cart.Total,
                                Bank = payment[0],
                                Dollar = payment[1],
                                Baht = payment[2],
                                Riel = payment[3],
                                User = Security.User?.Name,
                                
                            };

                            await MydbService.CreateHistory(History);

                            foreach(var i in CartItem.ToList())
                            {   
                                Historyconnector historyconnector = new(){
                                    ProductId = i.ProductId,
                                    Export = i.PriceOverwrite??i.Inventory.Export,
                                    Qty = i.Qty,
                                    CartId = History.Id,
                                    Note = i.Note
                                };

                                await MydbService.CreateHistoryconnector(historyconnector);
                                await MydbService.DeleteConnector(i.Id);

                            };

                            await MydbService.DeleteCart(Current_Cart.CartId);
                            await ResetToDefault();
                        }                            
                    }
                }else{await DialogService.Alert("បញ្ចាក់អ្នកគ្មានទំនេញនៅក្នុងអត្ថជននេះទេ.","បញ្ចាក់!");}
            }
        }

        protected async Task PrintReceipt(){
            
        }

        protected async Task ClearAllProduct(){

            if(CartItem != null){
                foreach(var i in CartItem.ToList())
                {   
                    if(i.Inventory.Name != "Service Charge")
                    {
                        IEnumerable<Inventory> product = await MydbService.GetInventories();
                        var items =  product.Where(v => v.Id == i.Inventory.Id);
                        product.FirstOrDefault().Stock += i.Qty;
                        await MydbService.UpdateInventory( product.FirstOrDefault());
                        await MydbService.DeleteConnector(i.Id);
                    }else{
                        await MydbService.DeleteConnector(i.Id);
                    }

                };                
            }
        }
        


        protected async Task ResetToDefault(){
            if(Current_Cart is not null){
                Current_Cart = null;
                Historyconnectors = [];
                HistoryCustomer = new();
                CartItem = [];                          
            }
        }
       
        readonly Dictionary<short, string> CartModeTranslate =  new(){
            {0,"កំពុងធ្វើ"},
            {1,"មិនទានគិតលុយ"},
            {2,"គិតលុយរួច"},
            {3,"ជំពាក់"}
       };
       
    }
}

