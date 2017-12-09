namespace ReactiveSearch.Models
{
    internal class Station
    {
        public Station(string id, string name)
        {
            Id = id ?? string.Empty;
            Name = name ?? string.Empty;
        }

        public string Id { get; }

        public string Name { get; }

        public override string ToString() => Name;
    }
}
