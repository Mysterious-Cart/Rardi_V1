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

namespace CHKS
{
    public partial class PublicCommand
    {

        private readonly mydbService mydbService;
        private readonly NavigationManager navigationManager;

        public PublicCommand(mydbService MydbService, NavigationManager navigationManager)
        {
            this.mydbService = MydbService;
            this.navigationManager = navigationManager;
        }

        static readonly char[] GENERATORKEY = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
        public static string GenerateRandomKey() => string.Join("",GENERATORKEY.OrderBy(x => Guid.NewGuid()).Take(10));

        static readonly int[] Key = {1,2,3,4,5,6,7,8,9};
        public static int GenerateRandomKey(int Length) => int.Parse(string.Join("",Key.OrderBy(x => Guid.NewGuid()).Take(Length)));
        
    }
}