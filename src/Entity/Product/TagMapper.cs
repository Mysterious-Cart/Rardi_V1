namespace CHKS.Mappers;
using System.Linq.Expressions;
using Models;
using Entity;

public static class TagMapper
{

    /// <summary>
    /// Converts a Tag entity to a Tags model.
    /// </summary>
    /// <param name="tags">The Tag entity to convert.</param>
    /// <returns>A Tags model.</returns>
    public static Tag ToTag(TagsModel tags) =>
        new Tag(tags.Id, tags.Tag, tags.Description);
}

public class TagExpressionMapper
{
    /// <summary>
    /// Converts a Tags model to a Tag entity.
    /// This expression can be used in LINQ queries to project Tags to Tag.
    /// </summary>
    public static Expression<Func<TagsModel, Tag>> ToTag =>
        tags => new Tag(tags.Id, tags.Tag, tags.Description);

    
}