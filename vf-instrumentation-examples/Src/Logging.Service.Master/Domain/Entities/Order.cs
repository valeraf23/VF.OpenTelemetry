using Domain.Common;

namespace Domain.Entities
{
    public class Order : AuditableEntity
    {
        public DeliveryStatus DeliveryStatus { get; set; }
        public string Title { get; set; }
        public string BuyerName { get; set; }
        public bool IsBundle { get; set; }
    }
}