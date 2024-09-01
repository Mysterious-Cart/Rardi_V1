using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using CHKS.Models;
using System.Globalization;

namespace CHKS
{
    public partial class PublicCommand
    {

        private readonly mydbService mydbService;
        private readonly NavigationManager navigationManager;
        private readonly SecurityService security;

        public PublicCommand(mydbService MydbService, NavigationManager navigationManager, SecurityService security)
        {
            this.mydbService = MydbService;
            this.navigationManager = navigationManager;
            this.security = security;
        }

        static readonly char[] GENERATORKEY = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
        public static string GenerateRandomText(int Length = 10) => string.Join("",GENERATORKEY.OrderBy(x => Guid.NewGuid()).Take(Length));
        public static string GenerateRandomText(string Extenstion,int Length = 10) => string.Concat(string.Join("",GENERATORKEY.OrderBy(x => Guid.NewGuid()).Take(Length)),":", Extenstion);

        static readonly int[] Key = {1,2,3,4,5,6,7,8,9};
        public static int GenerateRandomNumber(int Length) => int.Parse(string.Join("",Key.OrderBy(x => Guid.NewGuid()).Take(Length)));
        public static int GenerateRandomNumber(int Extenstion, int Length) => Extenstion + int.Parse(string.Join("",Key.OrderBy(x => Guid.NewGuid()).Take(Length)));
        
        //From Here are database Retrieval API, Change here whenever database format Changes
        public async Task<IEnumerable<Models.mydb.History>> RetreiveCustomerHistory(bool GetDeleted = false){
            try{
                if(GetDeleted == false){
                    return await mydbService.GetHistories(new Query{Filter=$@"i => i.IsDeleted == 0"});
                }else{
                    return await mydbService.GetHistories(new Query{Filter=$@"i => i.IsDeleted == 1"});

                }
            }catch(Exception exc){
                throw new Exception(exc.Message);
            }
        }


        public async Task<IEnumerable<Models.mydb.Car>> RetreiveCustomer(bool GetDeleted = false){
            try{
                if(GetDeleted == false){
                    return await mydbService.GetCars(new Query{Filter=$@"i => i.IsDeleted == 0"});
                }else{
                    return await mydbService.GetCars(new Query{Filter=$@"i => i.IsDeleted == 1"});

                }
            }catch(Exception exc){
                throw new Exception(exc.Message);
            }
        }


        public async Task<IEnumerable<Models.mydb.Cart>> RetreiveCart(){
            try{
                return await mydbService.GetCarts();
            }catch(Exception exc){
                throw new Exception(exc.Message);
            }
        }


        public async Task<IEnumerable<Models.mydb.Inventory>> RetreiveProduct(bool GetDeleted = false){
            try{
                if(GetDeleted == false){
                    return await mydbService.GetInventories(new Query{Filter=$@"i => i.IsDeleted == 0"});
                }else{
                    return await mydbService.GetInventories(new Query{Filter=$@"i => i.IsDeleted == 1"});
                }
                
            }catch(Exception exc){
                throw new Exception(exc.Message);
            }
        }

        //End Database Retrieval API

        static readonly Dictionary<int, string> ModeCoordinator = new Dictionary<int, string>(){{0,"ជួសជុល"},{1,"រងចាំប្រាក់"},{2, "ជំពាក់"}};
        public static string GetCustomerStateFromKey(int Key) => ModeCoordinator[Key];
        
        static string InfoReportString(string User)=> "Deleted By:" + User + "("+ DateTime.Now.ToString() +")";
        
        /// <summary>
        ///     Write a record to Record Table.
        /// </summary>
        /// <param name="Note"> Note to write off </param>
        /// <param name="user"> User to write off as </param>
        /// <returns> Return true if succeed; otherwise false </returns>
        public async Task<bool> RecordWriteOff(string Note, string User){
            
            Models.mydb.Changesrecord Record = new(){
                DateOfChange = DateTime.Now.ToString(),
                Info = Note,
                User = User,
            };
            
            var result = await mydbService.CreateChangesrecord(Record);
            bool WriteOffResult = false;
            if(result != null){
                WriteOffResult = true;
            }
            return WriteOffResult;
        }
        


        /// <summary>
        ///     Write a product as delete
        /// </summary>
        /// <param name="Product"> The Product to simulate delete </param>
        /// <returns> Return true if succeed; otherwise false </returns>
        public async Task<bool> PseudoDeleteInventory(Models.mydb.Inventory Product){
            Product.IsDeleted = 1;
            Product.Info = InfoReportString(security.User?.Name);
            var result = await mydbService.UpdateInventory(Product.Code, Product);
            return result!=null?true:false;
        }

        /// <summary>
        ///     Write a Customer as delete
        /// </summary>
        /// <param name="Customer"> The Customer to simulate delete </param>
        /// <returns> Return true if succeed; otherwise false </returns>
        public async Task PseudoDeleteCustomer(Models.mydb.Car Customer){
            Customer.IsDeleted = 1;
            Customer.Info = InfoReportString(security.User?.Name);
            await mydbService.UpdateCar(Customer.Key, Customer);
        }

        /// <summary>
        ///     Sum all payment type paid by customer
        /// </summary>
        /// <param name="Customer"> The Customer to simulate delete </param>
        /// <returns> The result payment summed up </returns>
        public async Task<(string, string, string, string)> GetAllPaymentFormFromHistory(){

            string Bank;
            string Dollar;
            string Baht;
            string Riel;

            IEnumerable<Models.mydb.History> CustomerHistory = await mydbService.GetHistories();

            Bank = Math.Round(CustomerHistory.Sum(i => i.Bank.GetValueOrDefault()),2).ToString();
            Dollar = Math.Round(CustomerHistory.Sum(i => i.Dollar.GetValueOrDefault()),2).ToString();
            Baht = Math.Round(CustomerHistory.Sum(i => i.Baht.GetValueOrDefault()),2).ToString();
            Riel = Math.Round(CustomerHistory.Sum(i => i.Riel.GetValueOrDefault()),2).ToString();

            return (Bank, Dollar, Baht, Riel);
        }
        
    }
}