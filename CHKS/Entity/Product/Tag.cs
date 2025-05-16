using CHKS.Models;
using CHKS.Models.mydb;

namespace CHKS.Entity
{
    public class Tag(Guid Id, string Name, string Description) : IEquatable<Tag>
    {
        public Guid Id { get; set; } = Id;
        public string Name { get; set; } = Name;
        public string Description { get; set; } = Description;
        
        public static Tag FromTagModel(Tags tags)
        {
            return new Tag(tags.Id, tags.Tag, tags.Description);
        }

        public static Models.mydb.Tags ToTag(Tag tags)
        {
            return new Tags { Id = tags.Id, Tag = tags.Name, Description = tags.Description };
        }

        public bool Equals(Tag other)
        {
            if(other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
        public override bool Equals(object obj) => obj is Tag tag && Equals(tag);
        public override int GetHashCode() => Id.GetHashCode();


    }
}