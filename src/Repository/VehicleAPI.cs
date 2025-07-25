using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;

public class VehicleAPI(HttpClient client)
{
    private readonly HttpClient httpClient = client;
    public record Vehicle_Response(int Total, IEnumerable<Vehicle> vehicles);
    public async Task<Vehicle_Response> GetVehicleDetails([Optional] int? Page, [Optional] int? PageSize, [Optional] string Search_Text)
    {
        string header = "https://data.opendatasoft.com/api/explore/v2.1/catalog/datasets/all-vehicles-model@public/records?";

        string query = "select=id%2C%20model%2C%20make%2C%20year&order_by=year%20DESC";

        string page_filter = Page is not null && PageSize is not null ?
            $"&limit={PageSize}&offset={Page * PageSize}" : "&limit=20&offset=0";
        bool is_Exact = false;

        if (Search_Text.Contains('=')) { is_Exact = true; Search_Text = Search_Text.Replace("=", ""); }

        string search_filter = Search_Text is not null && Search_Text != "" ?
            is_Exact ?
            $"&where=model%20%3D%20%22{Search_Text}%22%20or%20make%20%3D%20%22{Search_Text}%22" :
            $"&where=search(model%2C%22{Search_Text}%22)%20or%20search(make%2C%22{Search_Text}%22)" :
            "";
        string year_filter =
            Search_Text is not null && DateOnly.TryParse(Search_Text + "-1-1", out DateOnly result)
            ? $"%20or%20year%20%3D%20date'{Search_Text}'" : "";
        var response = await httpClient.GetAsync(
            header + query + page_filter + search_filter + year_filter
        );

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            List<Vehicle> vehicles = [];
            var json_object = JsonArray.Create(JsonDocument.Parse(content).RootElement.GetProperty("results"));
            foreach (var item in json_object)
            {
                vehicles.Add(JsonConvert.DeserializeObject<Vehicle>(item.ToString()));
            }
            int total_count = JsonDocument.Parse(content).RootElement.GetProperty("total_count").GetInt32();
            return new Vehicle_Response(total_count, vehicles);
        }

        return null;
    }
    public async Task<Vehicle_Response> GetVehicleDetails(string Make, string Model, string Year)
    {
        string header = "https://data.opendatasoft.com/api/explore/v2.1/catalog/datasets/all-vehicles-model@public/records?";

        string query = "select=id%2C%20model%2C%20make%2C%20year&order_by=year%20DESC";

        string page_filter = "&limit=20&offset=0";

        string search_filter =
            $"&where=search(model%2C%22{Model}%22)%20or%20search(make%2C%22{Make}%22)";
        string year_filter = string.IsNullOrEmpty(Year)?"":$"%20or%20year%20%3D%20date'{Year}'";
        var response = await httpClient.GetAsync(
            header + query + page_filter + search_filter + year_filter
        );

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            List<Vehicle> vehicles = [];
            var json_object = JsonArray.Create(JsonDocument.Parse(content).RootElement.GetProperty("results"));
            foreach (var item in json_object)
            {
                vehicles.Add(JsonConvert.DeserializeObject<Vehicle>(item.ToString()));
            }
            int total_count = JsonDocument.Parse(content).RootElement.GetProperty("total_count").GetInt32();
            return new Vehicle_Response(total_count, vehicles);
        }

        return null;
    }
}