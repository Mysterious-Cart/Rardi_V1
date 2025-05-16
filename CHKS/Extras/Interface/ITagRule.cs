namespace Rardi_V1.CHKS.Extras.Interface
{
    public interface ITagRule
    {
        
        void ApplyRule();
        bool ValidateTag(string tag);
    }
}