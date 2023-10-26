namespace WebAPI.Persistence.Repositories;

//this class defines models that perform database operations
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
