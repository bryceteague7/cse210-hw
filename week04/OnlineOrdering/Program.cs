using System;

class Program
{
    static void Main()
    {
        // ORDER 1
        Address a1 = new Address("123 Apple St", "Chicago", "IL", "USA");
        Customer c1 = new Customer("John Smith", a1);
        Order o1 = new Order(c1);

        o1.AddProduct(new Product("Keyboard", "K100", 25.99, 2));
        o1.AddProduct(new Product("Mouse", "M200", 15.49, 1));

        Console.WriteLine("===== ORDER 1 =====");
        Console.WriteLine(o1.GetPackingLabel());
        Console.WriteLine(o1.GetShippingLabel());
        Console.WriteLine($"Total Price: ${o1.GetTotalPrice():0.00}");
        Console.WriteLine();

        // ORDER 2
        Address a2 = new Address("89 Sunset Blvd", "Toronto", "ON", "Canada");
        Customer c2 = new Customer("Emily Johnson", a2);
        Order o2 = new Order(c2);

        o2.AddProduct(new Product("Laptop", "L500", 799.99, 1));
        o2.AddProduct(new Product("Headphones", "H700", 49.95, 2));

        Console.WriteLine("===== ORDER 2 =====");
        Console.WriteLine(o2.GetPackingLabel());
        Console.WriteLine(o2.GetShippingLabel());
        Console.WriteLine($"Total Price: ${o2.GetTotalPrice():0.00}");
    }
}
