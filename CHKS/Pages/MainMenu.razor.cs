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

        [Inject]
        protected PublicCommand PublicCommand {get; set;}


        protected Models.mydb.Car CustomerLists;

        protected Models.mydb.Cart Customer = new(){Plate=null};    
        protected IEnumerable<CHKS.Models.mydb.Cart> Carts;

        protected Models.mydb.History HistoryCustomer = new(){};
        protected IEnumerable<Models.mydb.Historyconnector> Historyconnectors;

        protected string today = DateTime.Now.ToString("dd/MM/yy");

        protected IEnumerable<Models.mydb.Connector> Connectors;
        protected IEnumerable<Models.mydb.Inventory> Inventories;
        protected IEnumerable<Models.mydb.History> RecentHistory;
        protected IEnumerable<Models.mydb.InventoryProductgroup> Productgroups;
        protected IEnumerable<Models.mydb.InventoryOption> ProductOptions;
        protected IEnumerable<Models.mydb.InventoryCaroption> CarOptions;   

        protected RadzenDataGrid<CHKS.Models.mydb.Connector> Grid1;
        protected RadzenDataGrid<Models.mydb.Dailyexpense> grid2;
        
        protected Models.mydb.Connector connector = new(){};

        protected List<string> SortingType = new(){
            "All",
            "Product",
            "Product Brand",
            "Car"
        };

        [Inject]
        protected SecurityService Security { get; set; }


        private string ToggleIcon = "";
        private string ToggleButtonClass = "";
        protected async void Togglebutton(){
            if(Customer.Plate != null && Customer.CartId != -1){
                if(ToggleButtonClass == ""  ){
                    ToggleButtonClass = "Show";
                    ToggleIcon ="check";
                    Customer.Company = 1;

                    await UpdateCart(Customer);
                }else{
                    ToggleButtonClass =  "";
                    Customer.Company = 0;
                    ToggleIcon ="";
                    await UpdateCart(Customer);
                }
            }else if(Customer.CartId == -1){
                if(ToggleButtonClass == ""  ){
                    ToggleButtonClass = "Show";
                    ToggleIcon ="check";
                    HistoryCustomer.Company = 1;
                    await MydbService.UpdateHistory(HistoryCustomer.CashoutDate, HistoryCustomer);
                }else{
                    ToggleButtonClass =  "";
                    HistoryCustomer.Company = 0;
                    ToggleIcon ="";
                    await MydbService.UpdateHistory(HistoryCustomer.CashoutDate, HistoryCustomer);
                }
            }
        }

        protected async Task UpdateCart(Models.mydb.Cart Customer){
            await MydbService.UpdateCart(Customer.CartId, Customer);
        }

        protected async Task OpenExpenseMenu()
        {
            await DialogService.OpenAsync<ExpenseMenu>("Expense Menu");
        }
        protected async override Task OnInitializedAsync()
        {
            CarOptions = await MydbService.GetInventoryCaroptions();
            Productgroups = await MydbService.GetInventoryProductgroups();
            ProductOptions = await MydbService.GetInventoryOptions();
            Inventories = await MydbService.GetInventories();
            Carts = await MydbService.GetCarts();
            await LoadRecentCashout();
        }

        

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            
            search = $"{args.Value}";

            Inventories = await MydbService.GetInventories(new Query { Filter = $@"i => i.Name.Contains(@0) || i.Barcode.Contains(@0) ", FilterParameters = new object[] { search , "Service Charge"} });
        }


        protected async Task CreateCustomer(){
            if(Customer.Plate == null){
                var result = await DialogService.OpenAsync<Cars>("CustomerList",new Dictionary<string, object>{}, new DialogOptions{Width="60%", Height="80%"});
                if(result != null){
                    
                    async Task<int> GetKey(){
                        int GenKey = PublicCommand.GenerateRandomKey(2);
                        Models.mydb.Cart C = await MydbService.GetCartByCartId(GenKey);
                        return C==null?GenKey:await GetKey();
                    }

                    Customer.CartId = await GetKey();
                    Customer.Plate = result.Plate;
                    Customer.Creator = Security.User?.NormalizedUserName;       
                    Customer.Company = 0;

                    await MydbService.CreateCart(Customer);
                    
                    Models.mydb.Cart C = await MydbService.GetCartByCartId(Customer.CartId);
                    Customer = new();
                    await OpenCart(C);
                }
            }
            
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
            RecentHistory = Enumerable.Empty<Models.mydb.History>();
            RecentHistory = tempHis;

        }

        protected async Task EditProduct(Models.mydb.Connector Product){
            var UpdatedItems = await DialogService.OpenAsync<SingleInputPopUp>(Product.Product, new Dictionary<string, object>{{"Info",new string[]{"EditItem", Product.PriceOverwrite.ToString(), Product.Qty.ToString(), Product.Note}}}, new DialogOptions{Width="250px"});
            if(UpdatedItems != null){
                Product.Qty = decimal.Parse(UpdatedItems[0]);
                Product.PriceOverwrite = decimal.Parse(UpdatedItems[1]);
                Product.Note = UpdatedItems[2];
                try{
                    await MydbService.UpdateConnector(Product.GeneratedKey, Product);
                }catch(Exception exc){

                }
            }
        }

        protected async Task EditProduct(Models.mydb.Historyconnector Product){
            var UpdatedItems = await DialogService.OpenAsync<SingleInputPopUp>(Product.Product, new Dictionary<string, object>{{"Info",new string[]{"EditItem", Product.Export.ToString(), Product.Qty.ToString(), Product.Note}}}, new DialogOptions{Width="250px"});
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

        protected async Task OpenCart(Models.mydb.Cart Cart){
            if(Customer.Plate == null){
                if(Cart.Company == 1){
                    ToggleButtonClass = "Show";
                    ToggleIcon = "check";
                }
                Customer.CartId = Cart.CartId;
                Customer.Plate = Cart.Plate;
                Customer.Car = Cart.Car;
                Customer.Creator = Cart.Creator;
                Customer.Company = Cart.Company;

                Connectors = await MydbService.GetConnectors(new Query{Filter=$@"i => i.CartId == (@0)", FilterParameters = new object[] {Customer.CartId}});
                Customer.Total = Connectors.Sum(i => i.PriceOverwrite * i.Qty );       
                await Toasting("បើកអតិថជន");
            } else{
                await ResetToDefault();
                await OpenCart(Cart);
            } 
        }

        protected async Task OpenCart(Models.mydb.History history){
            if(Customer.Plate == null && await DialogService.Confirm("តើអ្នកទេ?","សំខាន់", new ConfirmOptions{OkButtonText="ច្បាស់", CancelButtonText="ទៅវិញ"})==true){
                
                if(history.Company == 1){
                    ToggleButtonClass = "Show";
                    ToggleIcon = "check";
                }
                Customer.CartId = -1;
                Customer.Plate = history.Plate;
                Customer.Car = history.Car;
                Customer.Creator = history.User;
                Customer.Company = history.Company;
                HistoryCustomer = history;
                Historyconnectors = await MydbService.GetHistoryconnectors();
                Historyconnectors = Historyconnectors.Where(i => i.CartId== history.CashoutDate);
                Customer.Total = Historyconnectors.Sum(i => i.Export * i.Qty );       
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
            
            if(Product.Inventory.Stock > 0 && Product.Qty >= 1 && Changes == 1 || Changes == -1){
                
                Product.Inventory.Stock -= Changes;
                await MydbService.UpdateInventory(Product.Inventory.Name, Product.Inventory);
                Product.Qty += Changes;
                await MydbService.UpdateConnector(Product.GeneratedKey, Product);
                if(Product.Qty == 0){
                    Product.Inventory.Stock += Product.Qty;
                    await MydbService.UpdateInventory(Product.Inventory.Name, Product.Inventory);
                    await MydbService.DeleteConnector(Product.GeneratedKey);

                }
                await Toasting(Changes > 0?"ដកទំនេញចេញពីស្តុក":"ថែមទំនេញចូលស្តុក");
            }else if(Product.Inventory.Stock >= 0 && Changes > 1 ){
                Product.Inventory.Stock += Product.Qty;
                await MydbService.UpdateInventory(Product.Inventory.Name, Product.Inventory);
                await MydbService.DeleteConnector(Product.GeneratedKey);
                await Toasting("ថែមទំនេញចូលស្តុក");
            }else{
                await Toasting("អស់ស្តុក");
            }
        }

        protected async Task ChangeQty(Models.mydb.Historyconnector Product, int Changes){
            
            if(Product.Inventory.Stock > 0 && Product.Qty >= 1 && Changes == 1 || Changes == -1){
                
                Product.Inventory.Stock -= Changes;
                await MydbService.UpdateInventory(Product.Inventory.Name, Product.Inventory);
                Product.Qty += Changes;
                await MydbService.UpdateHistoryconnector(Product.Id, Product);
                if(Product.Qty == 0){
                    Product.Inventory.Stock += Product.Qty.GetValueOrDefault();
                    await MydbService.UpdateInventory(Product.Inventory.Name, Product.Inventory);
                    await MydbService.DeleteHistoryconnector(Product.Id);
                }
                await Toasting(Changes > 0?"ដកទំនេញចេញពីស្តុក":"ថែមទំនេញចូលស្តុក");
            }else if(Product.Inventory.Stock >= 0 && Changes > 1 ){
                Product.Inventory.Stock += Product.Qty.GetValueOrDefault();
                await MydbService.UpdateInventory(Product.Inventory.Name, Product.Inventory);
                await MydbService.DeleteHistoryconnector(Product.Id);
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

        protected async Task CreateNewProduct(){
            await DialogService.OpenAsync<NewProduct>("Create Product", new Dictionary<string, object>{}, new DialogOptions{Width="fit-content", Height="fit-content"});
        }


        protected async Task CashOut(){
            if(Customer.Plate !=  null ){
                if(Connectors.Any() == true){
                    if(await DialogService.Confirm("តើអ្នកច្បាស់ដែរឫទេ?","សំខាន់!") == true){
                        List<decimal?> payment = await DialogService.OpenAsync<PaymentForm>("គិតលុយ",new Dictionary<string, object>{{"Total",Customer.Total}}, new DialogOptions{Width="35%"});

                        if(payment != null){

                            string time = "";

                            if(await DialogService.Confirm("តើអ្នកចង់ប្តូរថ្ងៃទីគិតលុយដែរឫទេ?","Note", new ConfirmOptions{OkButtonText = "Yes", CancelButtonText="No",CloseDialogOnEsc=false, ShowClose=false}) == false){
                                time = DateTime.Now.ToString("dd/MM/yyyy") + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                            }else{
                                
                                time = await DialogService.OpenAsync<SingleInputPopUp>("រើសថ្ងៃទី", new Dictionary<string, object>{{"Info", new string[]{"Choosing Date"}}}, new DialogOptions{Width="20%"});
                                time = time!=null? time + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")": DateTime.Now.ToString("dd/MM/yyyy") + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
                            }
                            Models.mydb.History History = new Models.mydb.History{
                                CashoutDate = time,
                                Plate = Customer.Plate,
                                Total = Customer.Total,
                                Bank = payment[0],
                                Dollar = payment[1],
                                Baht = payment[2],
                                Riel = payment[3],
                                Company = (sbyte?)Customer.Company,
                                User = Security.User?.Name,
                                
                            };
                            await MydbService.CreateHistory(History);

                            foreach(var i in Connectors.ToList())
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
                                await MydbService.DeleteConnector(i.GeneratedKey);

                            };

                            await MydbService.DeleteCart(Customer.CartId);
                            await ResetToDefault();
                            await LoadRecentCashout();
                            await Toasting("គិតលុយបានសម្រេច់");
                        }                            
                    }
                }else{await DialogService.Alert("បញ្ចាក់អ្នកគ្មានទំនេញនៅក្នុងអត្ថជននេះទេ.","បញ្ចាក់!");}
            }
        }

        protected async Task PrintReceipt(){
            if(Customer.Plate != null){
                
                if(Customer.CartId == -1){
                    await MydbService.CreateCart(Customer);
                    if(Historyconnectors.Any() != false){
                        foreach(var product in Historyconnectors.ToList()){
                            connector = new(){
                                CartId = Customer.CartId,
                                Product = product.Inventory.Name,
                                Qty = product.Qty.GetValueOrDefault(1),
                                GeneratedKey = string.Concat(Customer.CartId,product.Product),
                                PriceOverwrite = product.Export,
                                Note = product.Note
                            };
                            await MydbService.CreateConnector(connector);
                            connector = new();
                        }
                    }
                    
                    await DialogService.OpenAsync<PrintPage>("",new Dictionary<string, object>{{"Id",Customer.CartId}},new DialogOptions{ShowTitle=false, Height = "fit-content"});
                    
                    await ClearAllProduct();
                    await MydbService.DeleteCart(Customer.CartId);
                    await ResetToDefault();
                    

                }else{
                    await DialogService.OpenAsync<PrintPage>("",new Dictionary<string, object>{{"Id",Customer.CartId}},new DialogOptions{ShowTitle=false, Height = "fit-content"});
                }
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
                    }else{
                        await MydbService.DeleteConnector(i.GeneratedKey);
                    }

                };                
            }
        }
        


        protected async Task ResetToDefault(){
            if(Customer.Plate != null){
                
                Customer = new Models.mydb.Cart();
                ToggleButtonClass = "";
                ToggleIcon = "";
                Historyconnectors = Enumerable.Empty<Models.mydb.Historyconnector>();
                HistoryCustomer = new();
                Connectors = Enumerable.Empty<CHKS.Models.mydb.Connector>();
                          
            }
        }

        protected async Task AddItemtoCart(Models.mydb.Inventory Product)
        {
            if(Customer.Plate != null){
                if(Customer.CartId == -1){
                    IEnumerable<Models.mydb.Historyconnector> TempConnector = Historyconnectors;
                    TempConnector = TempConnector.Where(i => i.Product == Product.Name);
                    if(Product.Stock!=0 && TempConnector.Any() == false){
                        var Qty = await DialogService.OpenAsync<SingleInputPopUp>(Product.Name, new Dictionary<string, object>{{"Info",new string[]{"Qty", Product.Export.ToString(), Product.Stock.ToString()}}}, new DialogOptions{Width="250px"});

                        if(Qty != null && decimal.Parse(Qty[0]) <= Product.Stock){
                            Product.Stock -=  decimal.Parse(Qty[0]);
                            await MydbService.UpdateInventory(Product.Name, Product);

                            Models.mydb.Historyconnector TempCon = new(){
                                CartId = HistoryCustomer.CashoutDate,
                                Id = String.Concat(Customer.CartId, Product.Name, DateTime.Now.ToString()),
                                Product = Product.Name,
                                Note = Qty[2],
                                Qty = decimal.Parse(Qty[0]),// Stock here are not in stock, it the chosen value pass from the user input;
                                Export = decimal.Parse(Qty[1])

                            };
                            try {
                                await MydbService.CreateHistoryconnector(TempCon);
                                List<Models.mydb.Historyconnector> TempList = Historyconnectors.ToList();
                                TempList.Add(TempCon);
                                Historyconnectors = TempList;
                            }catch(Exception exc){
                                if(exc.Message == "Item already available"){
                                    var GetQty = await MydbService.GetHistoryconnectorById(TempCon.Id);
                                    TempCon.Qty = GetQty.Qty + TempCon.Qty;
                                    await MydbService.UpdateHistoryconnector(TempCon.Id, TempCon);
                                }
                            }finally{
                                Customer.Total = Historyconnectors.Sum(i => i.Export * i.Qty );
                                HistoryCustomer.Total = Customer.Total;
                                await MydbService.UpdateHistory(HistoryCustomer.CashoutDate, HistoryCustomer );
                            }  
                        }else if(Qty != null && decimal.Parse(Qty[0]) > Product.Stock){
                            await DialogService.Alert("Too much! Not available In stock. Have Left: " + Product.Stock + ".","Warning");
                        }

                    }else if(Product.Stock !=0){
                        Models.mydb.Historyconnector Item = TempConnector.ToList().FirstOrDefault();
                        Item.Qty += 1;
                        await MydbService.UpdateHistoryconnector(Item.Id, Item);
                        Product.Stock -= 1;
                        await MydbService.UpdateInventory(Product.Name, Product);
                    }else if(Product.Stock == 0){
                        if(await DialogService.Confirm("គ្នានទំនេញក្នុងស្តុក។ តើអ្នកចង់ថែមមួយទំនេញក្នុងស្តុកមួយឫ?","សំខាន់!", new ConfirmOptions{OkButtonText ="+1", CancelButtonText="មិនយក"})==true){
                            Product.Stock += 1;
                            await MydbService.UpdateInventory(Product.Name, Product);
                            await AddItemtoCart(Product);
                        }
                    } 
                }else{
                    IEnumerable<Models.mydb.Connector> TempConnector = Connectors.Where(i => i.Inventory.Name == Product.Name && i.CartId == Customer.CartId);
                    if(Product.Stock!=0 && TempConnector.ToList().Any() == false){
                        var Qty = await DialogService.OpenAsync<SingleInputPopUp>(Product.Name, new Dictionary<string, object>{{"Info",new string[]{"Qty", Product.Export.ToString(), Product.Stock.ToString()}}}, new DialogOptions{Width="250px"});

                        if(Qty != null && decimal.Parse(Qty[0]) <= Product.Stock){
                            Product.Stock -=  decimal.Parse(Qty[0]);
                            await MydbService.UpdateInventory(Product.Name, Product);

                            connector = new(){
                                CartId = Customer.CartId,
                                GeneratedKey = String.Concat(Customer.CartId, Product.Name),
                                Product = Product.Name,
                                Note = Qty[2],
                                Qty = decimal.Parse(Qty[0]),// Stock here are not in stock, it the chosen value pass from the user input;
                                PriceOverwrite = decimal.Parse(Qty[1])
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
                        }else if(Qty != null && decimal.Parse(Qty[0]) > Product.Stock){
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
}

