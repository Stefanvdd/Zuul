class Item
{
    public int Price {get; }
    public int Weight { get; }
    public string Description { get; }

    // Constructor
    public Item(int price, int weight, string description)
    {
        Price = price;
        Weight = weight;
        Description = description;
    }
}