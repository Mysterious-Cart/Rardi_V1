using CHKS.Models.Interface;
using CHKS.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("cart")]
    public class Cart : IContainer<Connector>, IModelClass
    {
        [Column("CarID")]
        [Required]
        public string Car_Id { get; set; }

        public Car Car { get; set; }

        [Key]
        [Column("CartID")]
        [Required]
        public int CartId { get; set; } = new Random().Next(0, 1000);

        [Required]
        public decimal Total { get; set; } = 0;

        [Required]
        public short Status {get; set;} = 0;

        public ICollection<Connector> Connectors { get; set; }

        public async Task<Connector> Add(mydbService service, Connector Item){
            Item.CartId = this.CartId;
            await service.CreateConnector(Item);
            return Item;
        }

        public async Task<Connector> Remove(mydbService service, Guid ItemId){
            var Item = await service.GetConnectorById(ItemId);
            await service.DeleteConnector(ItemId);
            return Item;
        }
    }
}