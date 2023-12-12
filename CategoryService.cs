using Microsoft.Extensions.Logging;
using NWConsole.Model;
using NLog;
using Microsoft.EntityFrameworkCore;


public class CategoryService
{
    private readonly NWContext _db;
    private readonly NLog.ILogger _logger;

    public CategoryService(NWContext db, NLog.ILogger logger)
    {
        _db = db;
        _logger = logger;
        
    }

    // Add a new category
    public void AddCategory(string categoryName, string description)
    {
        var category = new Category { CategoryName = categoryName, Description = description };
        _db.Categories.Add(category);
        _db.SaveChanges();
    }

    // Edit a specified category
    public void EditCategory(int categoryId, string newCategoryName, string newDescription)
    {
        var category = _db.Categories.Find(categoryId);
        if (category != null)
        {
            category.CategoryName = newCategoryName;
            category.Description = newDescription;
            _db.SaveChanges();
        }
    }

    // Display all categories
    public void DisplayAllCategories()
    {
        var categories = _db.Categories.ToList();
        foreach (var category in categories)
        {
            Console.WriteLine($"Category Name: {category.CategoryName}, Description: {category.Description}");
        }
    }

    // Display all categories and their related active product data
    public void DisplayAllCategoriesWithActiveProducts()
    {
        var categories = _db.Categories.Include(c => c.Products).ToList();
        foreach (var category in categories)
        {
            Console.WriteLine($"Category Name: {category.CategoryName}");
            var activeProducts = category.Products.Where(p => !p.Discontinued).ToList();
            foreach (var product in activeProducts)
            {
                Console.WriteLine($"Product Name: {product.ProductName}");
            }
        }
    }

    // Display a specific category and its related active product data
    public void DisplayCategoryWithActiveProductsById(int categoryId)
    {
        var category = _db.Categories.Include(c => c.Products)
                                    .SingleOrDefault(c => c.CategoryId == categoryId && c.Products.Any(p => !p.Discontinued));

        if (category != null)
        {
            Console.WriteLine($"Category: {category.CategoryName}");
            foreach (var product in category.Products.Where(p => !p.Discontinued))
            {
                Console.WriteLine($"Product: {product.ProductName}");
            }
        }
        else
        {
            Console.WriteLine($"No category found with ID: {categoryId}");
        }
    }
    public void DisplayCategoryWithActiveProductsByName(string categoryName)
    {
        var category = _db.Categories.Include(c => c.Products)
                                    .SingleOrDefault(c => c.CategoryName == categoryName && c.Products.Any(p => !p.Discontinued));

        if (category != null)
        {
            Console.WriteLine($"Category: {category.CategoryName}");
            foreach (var product in category.Products.Where(p => !p.Discontinued))
            {
                Console.WriteLine($"Product: {product.ProductName}");
            }
        }
        else
        {
            Console.WriteLine($"No category found with name: {categoryName}");
        }
    }
    public void DeleteCategory(int categoryId)
    {
        var category = _db.Categories.Include(c => c.Products).SingleOrDefault(c => c.CategoryId == categoryId);
        if (category != null)
        {
            foreach (var product in category.Products)
            {
                product.CategoryId = null; // Set the CategoryId of the related products to null to prevent orphaned records
            }
            _db.Categories.Remove(category);
            _db.SaveChanges();
            _logger.Info($"Category with ID {categoryId} deleted");
        }
        else
        {
            _logger.Error($"No category found with ID: {categoryId}");
        }
    }
}