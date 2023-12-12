using Microsoft.Extensions.Logging;
using NWConsole.Model;
using NLog;

public class ProductService
{
    private readonly NWContext _db;
    private readonly NLog.ILogger _logger;

    public ProductService(NWContext db, NLog.ILogger logger)
    {
        _db = db;
        _logger = logger;
    }

    // Add new record to the Products table
    public void AddProduct(string productName, bool isDiscontinued)
{
    var product = new Product { ProductName = productName, Discontinued = isDiscontinued };
    _db.Products.Add(product);
    _db.SaveChanges();
    _logger.Info($"Product {productName} added");
}

    // Edit a specified record from the Products table
    public void EditProduct(int productId, string newProductName, bool newIsDiscontinued)
    {
            var product = _db.Products.Find(productId);
    if (product != null)
    {
        product.ProductName = newProductName;
        product.Discontinued = newIsDiscontinued;
        _db.SaveChanges();
        _logger.Info($"Product {productId} edited");
    }
    }

    // Display all records in the Products table
    public void DisplayProducts(string filter)
    {
           IEnumerable<Product> products;
    switch (filter)
    {
        case "all":
            products = _db.Products;
            break;
        case "discontinued":
            products = _db.Products.Where(p => p.Discontinued);
            break;
        default:
            products = _db.Products.Where(p => !p.Discontinued);
            break;
    }

    foreach (var product in products)
    {
        Console.WriteLine($"{product.ProductName} {(product.Discontinued ? "(Discontinued)" : "")}");
    }
    }

// Display a specific Product by name
    public void DisplayProduct(string productName)
    {
        var product = _db.Products.FirstOrDefault(p => p.ProductName == productName);
        if (product != null)
        {
            Console.WriteLine($"Product ID: {product.ProductId}");
            Console.WriteLine($"Product Name: {product.ProductName}");
            Console.WriteLine($"Discontinued: {product.Discontinued}");
        }
        else
        {
            Console.WriteLine($"No product found with name: {productName}");
        }
    }

// Display a specific Product by ID
    public void DisplayProduct(int productId)
    {
        var product = _db.Products.Find(productId);
        if (product != null)
        {
            Console.WriteLine($"Product ID: {product.ProductId}");
            Console.WriteLine($"Product Name: {product.ProductName}");
            Console.WriteLine($"Discontinued: {product.Discontinued}");
        }
        else
        {
            Console.WriteLine($"No product found with ID: {productId}");
        }
    }
    public void DeleteProduct(int productId)
    {
        var product = _db.Products.Find(productId);
        if (product != null)
        {
            _db.Products.Remove(product);
            _db.SaveChanges();
            _logger.Info($"Product with ID {productId} deleted");
        }
        else
        {
            _logger.Error($"No product found with ID: {productId}");
        }
    }
}