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
        protected Models.mydb.Cart Customer = new Models.mydb.Cart{Plate="", CartId=0, };

        protected bool? SelectionState = null;//False for Customer Mode, True for Cart Modes
        protected bool Selection = false;//False for selecting, True for already selected

        protected string Phone;
        protected string CarType;       
        protected string RowTotal = "0";

        protected string TodayTotal = "$0";

        protected bool VisibilityOfServiceCharge = false;
        protected string ServiceType;
        protected decimal ServiceFees=0;

        protected static string today = DateTime.Now.ToString("dd/MM/yy");

        protected IEnumerable<CHKS.Models.mydb.Cart> Carts;
        protected IEnumerable<CHKS.Models.mydb.Connector> Connectors;
        protected IEnumerable<CHKS.Models.mydb.Inventory> Inventories;

        protected RadzenDataGrid<CHKS.Models.mydb.Connector> Grid1;
        
        protected CHKS.Models.mydb.Connector connector = new Models.mydb.Connector{GeneratedKey="", CartId=0,Product ="", Qty=0 };

        protected async Task OpenExpenseMenu()
        {
            await DialogService.OpenAsync<ExpenseMenu>("Expense Menu");
        }
        protected async override Task OnInitializedAsync()
        {
            Retotal();
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
                Total = ServiceFees,
            };
            await MydbService.CreateConnector(Service);
            ServiceType = "";
            ServiceFees = 0;
            Connectors = await MydbService.GetConnectors();
            Connectors = Connectors.Where(i => i.CartId == Customer.CartId);
            await Grid1.Reload();
        }

        
        protected async Task OpenCustomerList()
        {
            if(Selection == false){

                var result = await DialogService.OpenAsync<Cars>("CustomerList",new Dictionary<string, object>{}, new DialogOptions{Width="60%", Height="80%"});

                if(result is CHKS.Models.mydb.Car)
                {
                    var Id = await DialogService.OpenAsync<SingleInputPopUp>("Cart ID", new Dictionary<string, object>{{"Title","Cart ID"}});
                    if(Id != null && Id is int)
                    {
                            Customer.CartId = Id;
                            Customer.Plate = result.Plate;
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
                                ResetToDefault();
                                VisibilityOfServiceCharge = false;
                            }
                        }
                    }

                }else if(result is null)
                {
                    Selection = false;

                }else if(result is CHKS.Models.mydb.Cart)
                {
                    Customer.CartId = result.CartId;
                    Customer.Plate = result.Plate;
                    Customer.Car = result.Car;
                    Customer.Total = result.Total;

                    Phone = Customer.Car.Phone;
                    CarType = Customer.Car.Type;

                    SelectionState = true;
                    Selection = true;

                    Connectors = await MydbService.GetConnectors(new Query{Filter=$@"i => i.CartId == (@0)", FilterParameters = new object[] {Customer.CartId}});
                    VisibilityOfServiceCharge = true;
                }

            }
        }

        protected async Task ResetTotal(){
            if(Connectors != Enumerable.Empty<Models.mydb.Connector>())
            {
                Customer.Total = Connectors.Sum(i => i.Total);
                await MydbService.UpdateCart(Customer.CartId, Customer);
            }

        }

        protected async Task DeleteCustomer(){
            if(await DialogService.Confirm("Are you sure?","Important!") == true)
            {
                if(Connectors == Enumerable.Empty<Models.mydb.Connector>() && Connectors != null){
                    await MydbService.DeleteCart(Customer.CartId);
                    ResetToDefault();
                }else if(Connectors != null){
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
                    await MydbService.DeleteCart(Customer.CartId);
                    ResetToDefault();
                }
            }
        }


        protected async Task CashOut(){
            if(Connectors != Enumerable.Empty<Models.mydb.Connector>()){
                if(await DialogService.Confirm("Are you sure?","Important!") == true){
                    List<decimal?> payment = await DialogService.OpenAsync<PaymentForm>("Payment",new Dictionary<string, object>{{"Total",Customer.Total}}, new DialogOptions{Width="35%"});

                    if(payment != null){
                        string time = "";
                            if(await DialogService.Confirm("Do you wish to change Cashout Date?","Note" ) == false){
                                time = DateTime.Now.ToString();
                            }else{
                                
                                time = await DialogService.OpenAsync<SingleInputPopUp>("Choosing Date", new Dictionary<string, object>{{"Title","Choosing Date"}});
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
                                        Export = i.Inventory.Export,
                                        Qty = i.Qty,
                                        CartId = time,
                                        Note = i.Note
                                    };

                                    await MydbService.CreateHistoryconnector(historyconnector);
                                }else{
                                    Models.mydb.Historyconnector historyconnector = new Models.mydb.Historyconnector{
                                        Id = i.GeneratedKey + time,
                                        Product = i.Product,
                                        Export = i.Total,
                                        Qty = i.Qty,
                                        CartId = time,
                                        Note = i.Note
                                    };

                                    await MydbService.CreateHistoryconnector(historyconnector);
                                }

                            };

                            await MydbService.DeleteCart(Customer.CartId);
                            ResetToDefault();
                            VisibilityOfServiceCharge = false;
                    }
                }
            }else{await DialogService.Alert("You have no items in cart.","Note!");}
        }

        


        protected async void ResetToDefault(){

            Customer = new Models.mydb.Cart{};
            SelectionState = null;
            Selection = false;
            Phone = "";
            CarType ="";

            Connectors = Enumerable.Empty<CHKS.Models.mydb.Connector>();
            VisibilityOfServiceCharge = false;

        }

        protected async Task AddItemtoCart()
        {
            var Product = await DialogService.OpenAsync<Inventories>("Select Product", new Dictionary<string, object>{{"IsDialog","true"}}, new DialogOptions{Width="80%", Height="80%"});
            if(Product != null){
                connector.CartId = Customer.CartId;
                connector.GeneratedKey = String.Concat(Customer.CartId, Product.Name);
                connector.Product = Product.Name;
                connector.Note = "";
                connector.Qty = Product.Stock;// Stock here are not in stock, it the chosen value pass from the user input;

                try {
                    connector.Total = Product.Stock * Product.Export;
                    await MydbService.CreateConnector(connector);
                    await Grid1.Reload();
                }catch(Exception exc){
                    if(exc.Message == "Item already available"){
                        var GetQty = await MydbService.GetConnectorByGeneratedKey(connector.GeneratedKey);
                        connector.Qty = GetQty.Qty + connector.Qty;
                        connector.Total = connector.Qty * Product.Export;
                        await MydbService.UpdateConnector(connector.GeneratedKey, connector);
                        await Grid1.Reload();
                    }
                }finally{
                    Connectors = await MydbService.GetConnectors(new Query{Filter=$@"i => i.CartId == (@0)", FilterParameters = new object[] {Customer.CartId}});
                    Customer.Total = Connectors.Sum(i => i.Total);
                    await MydbService.UpdateCart(Customer.CartId, Customer);
                }
            }


        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await Grid1.InsertRow(new CHKS.Models.mydb.Connector());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, CHKS.Models.mydb.Connector Product)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await MydbService.DeleteConnector(Product.GeneratedKey);

                    if (deleteResult != null)
                    {
                        await Grid1.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Inventory"
                    
                });
            }
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Connector args)
        {
            Models.mydb.Connector OldOne = await MydbService.GetConnectorByGeneratedKey(args.GeneratedKey);
            if((args.Qty-OldOne.Qty)<=args.Inventory.Stock && args.Product != "Service Charge")
                {
                args.Total = args.Qty * args.Inventory.Export.GetValueOrDefault(1);
                Models.mydb.Inventory Temp = new Models.mydb.Inventory{
                    Name = args.Product,
                    Import = args.Inventory.Import,
                    Export = args.Inventory.Export,
                    Stock = args.Inventory.Stock - (args.Qty - OldOne.Qty),
                };
                await MydbService.UpdateInventory(args.Product, Temp);
                await MydbService.UpdateConnector(args.GeneratedKey, args);
                await ResetTotal();
            }else if(args.Product =="Service Charge"){
                args.Qty = 1;
                await MydbService.UpdateConnector(args.GeneratedKey, args);
            }else{
                await DialogService.Alert("Not enough in Stock! Only have " + args.Inventory.Stock + " Left.");
                await Grid1.Reload();
            }
            
        }
        

        protected async Task GridRowCreate(CHKS.Models.mydb.Connector args)
        {
            await MydbService.CreateConnector(args);
            await ResetTotal();
            await Grid1.Reload();
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Connector data)
        {
            await Grid1.EditRow(data);
        }

        protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Connector data)
        {
            await Grid1.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Connector data)
        {
            Grid1.CancelEditRow(data);
            await MydbService.CancelConnectorChanges(data);
        }
        
    }

    
}