using System;

namespace Slave2.Entities
{
    public class Order : AuditableEntity
    {
        public Guid OrderId { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string Title { get; set; }
        public string BuyerName { get; set; }
        public bool IsBundle { get; set; }
    }
}