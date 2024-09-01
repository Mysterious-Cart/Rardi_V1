using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

using Blazor.Diagrams;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Anchors;
using GeoMPoint = Blazor.Diagrams.Core.Geometry.Point;
using CHKS.Models.Node;

using Product = CHKS.Models.mydb.Inventory;


namespace CHKS.Pages
{
    public partial class Inventories
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
        protected mydbService mydbService { get; set; }

        [Inject]
        protected PublicCommand PublicCommand {get; set;}

        [Inject]
        protected SecurityService Security { get; set; }
        
        

        protected override void OnInitialized()
        {
            GenerateFlowDiagram();
            CreateNote();
            
        }

        private async Task RetrieveInformation(){

        }

        protected BlazorDiagram Diagram { get; set; } = null!;

        private void GenerateFlowDiagram(){
            var options = new BlazorDiagramOptions
            {
                AllowMultiSelection = true,
                AllowPanning = true,
                GridSnapToCenter = true,
                GridSize = 10,
                Zoom =
                {
                    Enabled = false,
                },
                Links =
                {
                    DefaultRouter = new NormalRouter(),
                    DefaultPathGenerator = new SmoothPathGenerator()
                },
                
            };

            Diagram = new BlazorDiagram(options);
            
        }
        

        private void CreateNote(){
            
            Diagram.RegisterComponent<ProductWidget.ProductNode, ProductWidget>();
            var firstNode = Diagram.Nodes.Add(new NodeModel(position: new GeoMPoint(50, 50))
            {
                Title = "Node 1"
            });
            var secondNode = Diagram.Nodes.Add(new NodeModel(position: new GeoMPoint(200, 100))
            {
                Title = "Node 2"
            });
            var thirdNode = Diagram.Nodes.Add(new ProductWidget.ProductNode(position: new GeoMPoint(200, 100))
            {
                Title = "Node 3",
            });

            var leftPort = secondNode.AddPort(PortAlignment.Left);
            var rightPort = secondNode.AddPort(PortAlignment.Right);
            thirdNode.AddPort(PortAlignment.Left);

            // The connection point will be the intersection of
            // a line going from the target to the center of the source
            var sourceAnchor = new ShapeIntersectionAnchor(firstNode);
            // The connection point will be the port's position
            var targetAnchor = new SinglePortAnchor(leftPort);
            var link = Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));
            Diagram.Links.Added
        }
        
    }
}