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
    string choice;
    do
    {
        Console.WriteLine("1) Display Categories");
        Console.WriteLine("2) Add Category");
        Console.WriteLine("3) Display Category and related products");
        Console.WriteLine("4) Display all Categories and their related products");
        Console.WriteLine("5) Add Product");
        Console.WriteLine("6) Edit Product");
        Console.WriteLine("7) Display Products");
        Console.WriteLine("8) Display a specific Product");
        Console.WriteLine("\"q\" to quit");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");
        if (choice == "1")
        {
            var query = db.Categories.OrderBy(p => p.CategoryName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName} - {item.Description}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (choice == "2")
        {
            Category category = new Category();
            Console.WriteLine("Enter Category Name:");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter the Category Description:");
            category.Description = Console.ReadLine();
            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    // TODO: save category to db
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }
        else if (choice == "3")
        {
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
                Console.WriteLine($"\t{p.ProductName}");
            }
        }
        else if (choice == "4")
        {
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName}");
                foreach (Product p in item.Products)
                {
                    Console.WriteLine($"\t{p.ProductName}");
                }
            }
        }
        else if (choice == "5")
        {
            Console.WriteLine("Enter Product Name:");
            string productName = Console.ReadLine();
            Console.WriteLine("Enter Discontinued (true or false):");
            bool isDiscontinued = bool.Parse(Console.ReadLine());
            productService.AddProduct(productName, isDiscontinued);
        }
        else if (choice == "6")
        {
            Console.WriteLine("Enter Product Id:");
            int productId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter Product Name:");
            string productName = Console.ReadLine();
            Console.WriteLine("Enter Discontinued (true or false):");
            bool isDiscontinued = bool.Parse(Console.ReadLine());
            productService.EditProduct(productId, productName, isDiscontinued);
        }
        else if (choice == "7")
        {
            Console.WriteLine("Enter filter (all, discontinued, active):");
            string filter = Console.ReadLine();
            productService.DisplayProducts(filter);
        }
        else if (choice == "8")
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
        Console.WriteLine();
    } while (choice.ToLower() != "q");

}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");