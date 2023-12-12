﻿using NLog;
using System.Linq;
using NWConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWContext();
    var productService = new ProductService(db, logger);
    var categoryService = new CategoryService(db, logger);
    string choice;
    do
    {
        Console.WriteLine("1) Display Categories");
        Console.WriteLine("2) Add Category");
        Console.WriteLine("3) Edit a specific category");
        Console.WriteLine("4) Display all Categories with active products");
        Console.WriteLine("5) Display specific Category with active products");
        Console.WriteLine("6) Delete Category");
        Console.WriteLine("7) Add Product");
        Console.WriteLine("8) Edit Product");
        Console.WriteLine("9) Display Products");
        Console.WriteLine("10) Display a specific Product");
        Console.WriteLine("11) Delete Product");
        Console.WriteLine("\"q\" to quit");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");
        if (choice == "1")
        {
            categoryService.DisplayAllCategories();
        }
        else if (choice == "2")
        {
            Console.WriteLine("Enter Category Name:");
            string categoryName = Console.ReadLine();
            Console.WriteLine("Enter the Category Description:");
            string description = Console.ReadLine();
            categoryService.AddCategory(categoryName, description);
        }
        else if (choice == "3")
        {
            Console.WriteLine("Enter Category ID:");
            int categoryId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter new Category Name:");
            string newCategoryName = Console.ReadLine();
            Console.WriteLine("Enter new Description:");
            string newDescription = Console.ReadLine();
            categoryService.EditCategory(categoryId, newCategoryName, newDescription);
        }
        else if (choice == "4")
        {
            categoryService.DisplayAllCategoriesWithActiveProducts();
        }
        else if (choice == "5")
        {
            Console.WriteLine("Enter Category ID or Name:");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int categoryId))
            {
                categoryService.DisplayCategoryWithActiveProductsById(categoryId);
            }
            else
            {
                string categoryName = input;
                categoryService.DisplayCategoryWithActiveProductsByName(categoryName);
            }
        }
        else if (choice == "6")
        {
            Console.WriteLine("Enter Category ID:");
            int categoryId = int.Parse(Console.ReadLine());
            categoryService.DeleteCategory(categoryId);
        }
        else if (choice == "7")
        {
            Console.WriteLine("Enter Product Name:");
            string productName = Console.ReadLine();
            Console.WriteLine("Enter Discontinued (true or false):");
            bool isDiscontinued = bool.Parse(Console.ReadLine());
            productService.AddProduct(productName, isDiscontinued);
        }
        else if (choice == "8")
        {
            Console.WriteLine("Enter Product Id:");
            int productId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter Product Name:");
            string productName = Console.ReadLine();
            Console.WriteLine("Enter Discontinued (true or false):");
            bool isDiscontinued = bool.Parse(Console.ReadLine());
            productService.EditProduct(productId, productName, isDiscontinued);
        }
        else if (choice == "9")
        {
            Console.WriteLine("Enter filter (all, discontinued, active):");
            string filter = Console.ReadLine();
            productService.DisplayProducts(filter);
        }
        else if (choice == "10")
        {
            Console.WriteLine("Enter 1 to search by name, 2 to search by ID:");
            string searchChoice = Console.ReadLine();
            if (searchChoice == "1")
            {
                Console.WriteLine("Enter Product Name:");
                string productName = Console.ReadLine();
                productService.DisplayProduct(productName);
            }
            else if (searchChoice == "2")
            {
                Console.WriteLine("Enter Product ID:");
                int productId = int.Parse(Console.ReadLine());
                productService.DisplayProduct(productId);
            }
        }
        else if (choice == "11")
        {
                Console.WriteLine("Enter Product ID:");
                int productId = int.Parse(Console.ReadLine());
                productService.DeleteProduct(productId);
         }
        Console.WriteLine();
    } while (choice.ToLower() != "q");

}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");