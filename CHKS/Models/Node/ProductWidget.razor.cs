using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using DPoint = Blazor.Diagrams.Core.Geometry.Point;

using Product = CHKS.Models.mydb.Inventory;

namespace CHKS.Models.Node
{
    public partial class ProductWidget
    {
        [Inject]
        public PublicCommand PublicCommand {get; set;}

        public class ProductNode : NodeModel{
            public ProductNode(DPoint? position = null) : base(position) { }

            public Product Product {get; set;}
        }
        
    }
}