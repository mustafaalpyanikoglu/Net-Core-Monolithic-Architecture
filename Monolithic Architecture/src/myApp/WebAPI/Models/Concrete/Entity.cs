using ServiceStack.DataAnnotations;

namespace WebAPI.Models.Concrete
{
    public class Entity
    {
        public int Id { get; set; }

        public Entity()
        {
        }

        public Entity(int id) : this()
        {
            Id = id;
        }
    }
}
