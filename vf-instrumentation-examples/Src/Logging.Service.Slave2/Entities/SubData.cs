namespace Slave2.Entities
{
    public class SubData
    {
        public int Id { get; set; }
        public int DataId { get; set; }
        public string Value { get; set; }
        public Data Data { get; set; }
    }
}