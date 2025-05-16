using Newtonsoft.Json.Converters;

namespace CHKS.Models.Class
{
    public class DateSerializer : IsoDateTimeConverter
    {
        public DateSerializer()
        {
            base.DateTimeFormat = "yyyy'-'MM'-'dd";
        }
    }
}
