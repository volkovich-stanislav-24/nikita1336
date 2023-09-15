namespace Application1.Models
{
    internal class Switch : Device
    {
        public Switch(string name) : base(name)
        {
            MaxConnections = 4;
        }
    }
}
