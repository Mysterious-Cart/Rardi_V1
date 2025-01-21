using System;
using System.Collections.Generic;
using CHKS.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("inventory")]
    public class Inventory : IModelClass
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

        [Required]
        public short Status {get; set;} = 0;

        [Key]
        [Required]
        public Guid Id { get;} = Guid.NewGuid();

        public ICollection<Connector> Connectors {get; set;}

        public ICollection<Historyconnector> HistoryConnectors {get; set;}

        public ICollection<Tags> Tags{get; set;}
        public int GetSoldTotal(){
            return Sold_Total;
        }

    }
}