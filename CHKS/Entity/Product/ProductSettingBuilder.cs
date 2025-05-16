using CHKS.Models.mydb;
using CHKS.Extras.Class.DTOs;

namespace CHKS.Models.Builder;

public class ProductSettingBuilder
{
    private bool _allowTracking;
    private bool _allowWarning;
    private int _optimalStock;

    public static ProductSettingBuilder Default()
    {
        return new();
    }

    public ProductSettingBuilder SetAllowTracking(bool allowTracking)
    {
        _allowTracking = allowTracking;
        return this;
    }
    public ProductSettingBuilder SetAllowWarning(bool allowWarning)
    {
        _allowWarning = allowWarning;
        return this;
    }

    public ProductSettingBuilder SetOptimalStock(int optimalStock)
    {
        _optimalStock = optimalStock;
        return this;
    }
    
    public ProductSetting Build()
    {
        return new ProductSetting(_allowTracking, _allowWarning);
    }

}
