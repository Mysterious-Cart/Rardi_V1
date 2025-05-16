using CHKS.Models;
using CHKS.Models.mydb;

namespace CHKS.Extras.Class.DTOs
{
    public class TagsDTO(Guid Id, string Name, string Description) : IEquatable<TagsDTO>
    {
        public Guid Id { get; set; } = Id;
        public string Name { get; set; } = Name;
        public string Description { get; set; } = Description;
        
        public static TagsDTO FromTags(Tags tags)
        {
            return new TagsDTO(tags.Id, tags.Tag, tags.Description);
        }

        public static Tags ToTags(TagsDTO tags)
        {
            return new Tags { Id = tags.Id, Tag = tags.Name, Description = tags.Description };
        }

        public bool Equals(TagsDTO other)
        {
            if(other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
        public override bool Equals(object obj) => obj is TagsDTO tag && Equals(tag);
        public override int GetHashCode() => Id.GetHashCode();


    }
}