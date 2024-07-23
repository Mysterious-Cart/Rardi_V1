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
    public partial class MainMenu
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
        protected mydbService MydbService {get; set;}


        protected CHKS.Models.mydb.Car CustomerLists;
        protected Models.mydb.Cart Customer = new(){Plate=null};

        protected bool? SelectionState = null;//False for Customer Mode, True for Cart Modes
        protected bool Selection = false;//False for selecting, True for already selected

        protected string Phone;
        protected string CarType;       
        protected string RowTotal = "0";

        protected string CustomerDataExpand = "customerDatalist-expanded-false";
        protected string icon = "start";

        protected string TodayTotal = "$0";

        protected bool VisibilityOfServiceCharge = false;
        protected string ServiceType;
        protected decimal ServiceFees=0;

        protected static string today = DateTime.Now.ToString("dd/MM/yy");

        protected IEnumerable<Models.mydb.Dailyexpense> Dailyexpenses;
        protected IEnumerable<CHKS.Models.mydb.Cart> Carts;
        protected IEnumerable<CHKS.Models.mydb.Connector> Connectors;
        protected IEnumerable<CHKS.Models.mydb.Inventory> Inventories;

        protected RadzenDataGrid<CHKS.Models.mydb.Connector> Grid1;
        protected RadzenDataGrid<Models.mydb.Dailyexpense> grid2;
        
        protected CHKS.Models.mydb.Connector connector = new(){GeneratedKey="", CartId=0,Product ="", Qty=0 };

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task OpenExpenseMenu()
        {
            await DialogService.OpenAsync<ExpenseMenu>("Expense Menu");
        }
        protected async override Task OnInitializedAsync()
        {
            Inventories = await MydbService.GetInventories();
            Carts = await MydbService.GetCarts();
            Dailyexpenses = await MydbService.GetDailyexpenses();
            Dailyexpenses = Dailyexpenses.Where(i => i.Key.Contains(DateTime.Now.ToString("dd/MM/yyyy")));
            Retotal();
        }

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            
            search = $"{args.Value}";

            Inventories = await MydbService.GetInventories(new Query { Filter = $@"i => i.Name.Contains(@0) || i.Barcode.Contains(@0) ", FilterParameters = new object[] { search , "Service Charge"} });
        }

        protected async void Retotal(){
            IEnumerable<Models.mydb.History> history = await MydbService.GetHistories();
            history = history.Where(i => i.CashoutDate.Contains(today));
            TodayTotal = "$"+ history.Sum(i => i.Total).ToString();
        }       

        protected async Task AddServiceCharge(){

            Models.mydb.Connector Service = new Models.mydb.Connector{
                CartId = Customer.CartId,
                Product = "Service Charge",
                Qty = 1,
                Note = ServiceType,
                GeneratedKey = Customer.CartId + ServiceType,
            };

            await MydbService.CreateConnector(Service);
            ServiceType = "";
            ServiceFees = 0;
            Connectors = await MydbService.GetConnectors();
            Connectors = Connectors.Where(i => i.CartId == Customer.CartId);
            Customer.Total = Connectors.Sum(i => i.PriceOverwrite * i.Qty );
            await Grid1.Reload();
        }

        protected async Task CreateCustomer(){
            if(Customer.Plate == null){
                var result = await DialogService.OpenAsync<Cars>("CustomerList",new Dictionary<string, object>{}, new DialogOptions{Width="60%", Height="80%"});
                if(result != null){
                    var Id = await DialogService.OpenAsync<SingleInputPopUp>("បង្កើតលេខតំណាង", new Dictionary<string, object>{{"Info",new string[]{"Cart ID"}}}, new DialogOptions{Width="450px"});
                    if(Id != null && Id is int)
                    {
                        Customer.CartId = Id;
                        Customer.Plate = result.Plate;
                        Customer.Creator = Security.User?.NormalizedUserName;
                        Phone = result.Phone;
                        CarType = result.Type;
                        Customer.Total = 0;
                        SelectionState = false;
                        Selection = true;
                        VisibilityOfServiceCharge = true;
                        
                        try{
                            await MydbService.CreateCart(Customer);
                        }catch(Exception exc){
                            if(exc.Message == "Item already available"){
                                await DialogService.Alert("This cart ID already exist.","Important");
                                await ResetToDefault();
                                VisibilityOfServiceCharge = false;
                            }
                        }finally{
                            Customer = new();
                        }
                    }
                }
            }
            
        }

        protected async Task OpenCart(Models.mydb.Cart Cart){
            if(Customer.Plate == null){
                Customer.CartId = Cart.CartId;
                Customer.Plate = Cart.Plate;
                Customer.Car = Cart.Car;
                Customer.Creator = Cart.Creator;

                Phone = Customer.Car.Phone;
                CarType = Customer.Car.Type;

                SelectionState = true;
                Selection = true;

                Connectors = await MydbService.GetConnectors(new Query{Filter=$@"i => i.CartId == (@0)", FilterParameters = new object[] {Customer.CartId}});
                VisibilityOfServiceCharge = true;
                Customer.Total = Connectors.Sum(i => i.PriceOverwrite * i.Qty );       
                await Toasting("បើកអតិថជន");
            }   
        }

        protected async Task ResetTotal(){
            if(Connectors != Enumerable.Empty<Models.mydb.Connector>())
            {
                Customer.Total = Connectors.Sum(i => i.PriceOverwrite * i.Qty );
                await MydbService.UpdateCart(Customer.CartId, Customer);
            }

        }

        protected async Task DeleteCustomer(){
            if(await DialogService.Confirm("តើអ្នកច្បាស់ដែរឫទេ?","សំខាន់!", new ConfirmOptions{OkButtonText="Yes", CancelButtonText="No"}) == true)
            {
                if(Connectors != null){
                    await ClearAllProduct();
                    await MydbService.DeleteCart(Customer.CartId);
                    await ResetToDefault();
                    await Toasting("អតិថជនត្រូវបានលុប");
                }else if(connector == null || connector == Enumerable.Empty<Models.mydb.Connector>()){
                    await MydbService.DeleteCart(Customer.CartId);
                    await ResetToDefault();
                    await Toasting("អតិថជនត្រូវបានលុប");
                }
                
            }
            
        }

        protected string ToastState = "";
        
        protected async Task ChangeQty(Models.mydb.Connector Product, int Changes){
            
            if(Product.Inventory.Stock > 0 && Product.Qty != 0 && Changes == 1 || Changes == -1){
                Product.Inventory.Stock -= Changes;
                await MydbService.UpdateInventory(Product.Inventory.Name, Product.Inventory);
                Product.Qty += Changes;
                await MydbService.UpdateConnector(Product.GeneratedKey, Product);
                await Toasting(Changes > 0?"ដកទំនេញចេញពីស្តុក":"ថែមទំនេញចូលស្តុក");
            }else if(Product.Inventory.Stock >= 0 && Changes > 1){
                Product.Inventory.Stock += Product.Qty;
                await MydbService.UpdateInventory(Product.Inventory.Name, Product.Inventory);
                await MydbService.DeleteConnector(Product.GeneratedKey);
                await Toasting("ថែមទំនេញចូលស្តុក");
            }else{
                await Toasting("អស់ស្តុក");
            }
        }

        protected string ToastString = "";
        protected async Task Toasting(string ToastText){
            ToastState = "show";
            ToastString = ToastText;
            await Task.Delay(700);
            ToastState = "";
        }


        protected async Task CashOut(){
            if(Customer.Plate  !=  null){
                if(Connectors.ToList().Any() == true){
                    if(await DialogService.Confirm("តើអ្នកច្បាស់ដែរឫទេ?","សំខាន់!") == true){
                        List<decimal?> payment = await DialogService.OpenAsync<PaymentForm>("គិតលុយ",new Dictionary<string, object>{{"Total",Customer.Total}}, new DialogOptions{Width="35%"});

                        if(payment != null){

                            string time = "";

                            if(await DialogService.Confirm("តើអ្នកចង់ប្តូរថ្ងៃទីគិតលុយដែរឫទេ?","Note", new ConfirmOptions{OkButtonText = "Yes", CancelButtonText="No",CloseDialogOnEsc=false, ShowClose=false}) == false){
                                time = DateTime.Now.ToString("dd/MM/yyyy") + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                            }else{
                                
                                time = await DialogService.OpenAsync<SingleInputPopUp>("រើសថ្ងៃទី", new Dictionary<string, object>{{"Info", new string[]{"Choosing Date"}}}, new DialogOptions{Width="20%"});
                                time = time!=null? time + ":" + Customer.CartId + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")": DateTime.Now.ToString("dd/MM/yyyy") + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                            }
                            
                            Models.mydb.History History = new Models.mydb.History{
                                CashoutDate = time,
                                Plate = Customer.Plate,
                                Total = Customer.Total,
                                Bank = payment[0],
                                Dollar = payment[1],
                                Baht = payment[2],
                                Riel = payment[3],
                            };
                            await MydbService.CreateHistory(History);


                            foreach(var i in Connectors.ToList())
                            {   
                                if(i.Product != "Service Charge")
                                {
                                    Models.mydb.Historyconnector historyconnector = new Models.mydb.Historyconnector{
                                        Id = i.GeneratedKey + time,
                                        Product = i.Product,
                                        Export = i.PriceOverwrite,
                                        Qty = i.Qty,
                                        CartId = time,
                                        Note = i.Note
                                    };

                                    await MydbService.CreateHistoryconnector(historyconnector);
                                }

                            };

                            await MydbService.DeleteCart(Customer.CartId);
                            await ResetToDefault();
                            VisibilityOfServiceCharge = false;
                            await Toasting("គិតលុយបានសម្រេច់");
                        }
                    }
                }else{await DialogService.Alert("បញ្ចាក់អ្នកគ្មានទំនេញនៅក្នុងអត្ថជននេះទេ.","បញ្ចាក់!");}
            }
        }

        protected async Task PrintReceipt(){
            if(Customer.Plate != null){
                await DialogService.OpenAsync<PrintPage>("",new Dictionary<string, object>{{"Id",Customer.CartId}},new DialogOptions{ShowTitle=false, Height = "fit-content"});
            }
        }

        protected async Task ClearAllProduct(){

            if(Connectors != null){
                foreach(var i in Connectors.ToList())
                {   
                    if(i.Product != "Service Charge")
                    {
                        Models.mydb.Inventory product = await MydbService.GetInventoryByName(i.Product);
                        product.Stock += i.Qty;
                        await MydbService.UpdateInventory(i.Product, product);
                        await MydbService.DeleteConnector(i.GeneratedKey);
                    }else
                    {
                        await MydbService.DeleteConnector(i.GeneratedKey);
                    }

                };                
            }
        }
        


        protected async Task ResetToDefault(){
            if(Customer.Plate != null){
                
                Customer = new Models.mydb.Cart();
                SelectionState = null;
                Selection = false;
                Phone = "N/A";
                CarType ="N/A";
                RowTotal = "0 $";

                Connectors = Enumerable.Empty<CHKS.Models.mydb.Connector>();                
            }
        }

        protected async Task AddItemtoCart(Models.mydb.Inventory Product)
        {
            if(Customer.Plate != null){
                IEnumerable<Models.mydb.Connector> TempConnector = Connectors.Where(i => i.Inventory.Name == Product.Name && i.CartId == Customer.CartId);
                if(Product.Stock!=0 && TempConnector.ToList().Any() == false){
                    var Qty = await DialogService.OpenAsync<SingleInputPopUp>(Product.Name, new Dictionary<string, object>{{"Info",new string[]{"Qty", Product.Export.ToString(), Product.Stock.ToString()}}}, new DialogOptions{Width="250px"});

                    if(Qty is Array && Qty[0] != null && Qty[0] is decimal && Qty[0] <= Product.Stock){
                        Models.mydb.Inventory Temp = new(){};
                        Product.Stock -=  Qty[0];
                        await MydbService.UpdateInventory(Product.Name, Product);
                        Temp.Export = Qty[1];
                        Temp.Import = Qty[2];
                        Temp.Name = Product.Name;
                        Temp.Stock = Qty[0];

                        connector = new(){
                            CartId = Customer.CartId,
                            GeneratedKey = String.Concat(Customer.CartId, Product.Name),
                            Product = Temp.Name,
                            Note = "",
                            Qty = Temp.Stock,// Stock here are not in stock, it the chosen value pass from the user input;
                            PriceOverwrite = Temp.Export,
                            Discount = Temp.Import
                        };
                        try {
                            await MydbService.CreateConnector(connector);
                            await Grid1.Reload();
                        }catch(Exception exc){
                            if(exc.Message == "Item already available"){
                                var GetQty = await MydbService.GetConnectorByGeneratedKey(connector.GeneratedKey);
                                connector.Qty = GetQty.Qty + connector.Qty;
                                await MydbService.UpdateConnector(connector.GeneratedKey, connector);
                                await Grid1.Reload();
                            }
                        }finally{
                            Connectors = await MydbService.GetConnectors(new Query{Filter=$@"i => i.CartId == (@0)", FilterParameters = new object[] {Customer.CartId}});
                            Customer.Total = Connectors.Sum(i => i.PriceOverwrite * i.Qty );
                            await MydbService.UpdateCart(Customer.CartId, Customer);
                        }  
                    }else if(Qty is Array && Qty[0] != null && Qty[0] > Product.Stock){
                        await DialogService.Alert("Too much! Not available In stock. Have Left: " + Product.Stock + ".","Warning");
                    }

                }else if(Product.Stock !=0){
                    Models.mydb.Connector Item = TempConnector.ToList().FirstOrDefault();
                    Item.Qty += 1;
                    await MydbService.UpdateConnector(Item.GeneratedKey, Item);
                    Product.Stock -= 1;
                    await MydbService.UpdateInventory(Product.Name, Product);
                }else if(Product.Stock == 0){
                    if(await DialogService.Confirm("គ្នានទំនេញក្នុងស្តុក។ តើអ្នកចង់ថែមមួយទំនេញក្នុងស្តុកមួយឫ?","សំខាន់!", new ConfirmOptions{OkButtonText ="+1", CancelButtonText="មិនយក"})==true){
                        Product.Stock += 1;
                        await MydbService.UpdateInventory(Product.Name, Product);
                        await AddItemtoCart(Product);
                    }
                }                
                
            }

        }


    }
}