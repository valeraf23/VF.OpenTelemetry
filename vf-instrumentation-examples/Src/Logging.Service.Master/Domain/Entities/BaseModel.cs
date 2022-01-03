namespace Domain.Entities
{
    public abstract class BaseModel<TKey>
    {
        public TKey Id { get; set; }
    }
}