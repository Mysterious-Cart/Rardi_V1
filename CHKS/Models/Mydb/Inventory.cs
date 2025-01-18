using System;
using System.Collections.Generic;
using CHKS.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("inventory")]
    public partial class Inventory : ITableFormat<Inventory>
    {
        
        [Required]
        public decimal Stock { get; set; } = 0;

        [Required]
        public decimal Import { get; set; } = 0;

        [Required]
        public decimal Export { get; set; } = 0;

        public string Barcode { get; set; } = "";

        [Column("Normalize_Name")]
        public string NormalizeName { get; set; } = "";

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Info { get; set; } = "";

        public short? IsDeleted { get; set; } = 0;

        public int Sold_Total {get; set;} = 0;

        public int Optimal_Stock {get; set;} = 0;

        [Key]
        [Required]
        public Guid Id { get;} = Guid.NewGuid();

        public ICollection<Connector> Connectors {get; set;}

        public ICollection<Historyconnector> HistoryConnectors {get; set;}

        public ICollection<Tags> Tags{get; set;}

        public static async Task<Inventory> Create(mydbService service, Inventory inventory){
            return await service.CreateInventory(inventory);
        }

        public static async Task<bool> Remove(mydbService service, Guid Id){
            try{
                var result = await service.DeleteInventory(Id);
                return result is not null? true: false;
            }catch(Exception Exc){
                return false;
            }
        }

        public async Task AddTag( mydbService service,Tags tag){
            await service.InventoryAddTag(Id, tag);
        }

        public static async Task<Inventory> Update(mydbService service, Inventory Item){
            return await service.UpdateInventory(Item);
        }

        public int GetSoldTotal(){
            return Sold_Total;
        }

    }
}