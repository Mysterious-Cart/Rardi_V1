public class ProductSetting(bool isTracking, bool isWarning)
{

    public bool AllowTracking { get; set; } = isTracking;
    public bool AllowWarning { get; set; } = isWarning;
}