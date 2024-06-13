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

        protected IEnumerable<CHKS.Models.mydb.Cart> Carts;
        protected IEnumerable<CHKS.Models.mydb.Connector> Connectors;
        protected IEnumerable<CHKS.Models.mydb.Inventory> Inventories;

        protected RadzenDataGrid<CHKS.Models.mydb.Connector> Grid1;
        
        protected CHKS.Models.mydb.Connector connector = new Models.mydb.Connector{GeneratedKey="", CartId=0,Product ="", Qty=0 };

        protected async Task OpenCustomerList()
        {
            if(Selection == false){

                var result = await DialogService.OpenAsync<Cars>("CustomerList",new Dictionary<string, object>{}, new DialogOptions{Width="60%"});

                if(result is CHKS.Models.mydb.Car)
                {
                    RequestingID(result);
                    

                }else if(result is null)
                {
                    Selection = false;

                }else if(result is CHKS.Models.mydb.Cart)
                {
                    Customer.CartId = result.CartId;
                    Customer.Plate = result.Plate;
                    Customer.Car = result.Car;

                    Phone = Customer.Car.Phone;
                    CarType = Customer.Car.Type;

                    SelectionState = true;
                    Selection = true;

                    Connectors = await MydbService.GetConnectors(new Query{Filter=$@"i => i.CartId == (@0)", FilterParameters = new object[] {Customer.CartId}});
                }

            }
        }

        protected async void RequestingID(Models.mydb.Car result){
            var Id = await DialogService.OpenAsync<SingleInputPopUp>("Cart ID", new Dictionary<string, object>{{"Title","Cart ID"}});
            if(Id != null && Id is int)
            {
                    Customer.CartId = Id;
                    Customer.Plate = result.Plate;
                    Phone = result.Phone;
                    CarType = result.Type;
                    SelectionState = false;
                    Selection = true;
                try{

                    await MydbService.CreateCart(Customer);
                }catch(Exception exc){
                    if(exc.Message == "Item already available"){
                        await DialogService.Alert("This cart ID already exist.","Important");
                        Customer.CartId = 0;
                        Customer.Plate = "";
                        Phone = "";
                        CarType = "";
                        SelectionState = null;
                        Selection = false;
                    }
                }finally{

                }
            }
        }


        protected void SaveCustomerCart(){

            Customer = new Models.mydb.Cart{};
            SelectionState = null;
            Selection = false;
            Phone = "";
            CarType ="";

            Connectors = Enumerable.Empty<CHKS.Models.mydb.Connector>();

        }

        protected async Task AddItemtoCart()
        {
            var Product = await DialogService.OpenAsync<Inventories>("Select Product", new Dictionary<string, object>{{"IsDialog","true"}});
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
            if((args.Qty-OldOne.Qty)<=args.Inventory.Stock)
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
            }else{
                await DialogService.Alert("Not enough in Stock! Only have " + args.Inventory.Stock + " Left.");
                await Grid1.Reload();
            }
            
        }

        protected async Task GridRowCreate(CHKS.Models.mydb.Connector args)
        {
            await MydbService.CreateConnector(args);
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