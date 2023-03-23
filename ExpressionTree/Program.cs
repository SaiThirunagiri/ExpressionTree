using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

class Program
{
    static void Main(string[] args)
    {
        // Sample data
        var products = new List<Product>
        {
            new Product { Name = "iPhone 13", Price = 999.99M, Category = "Phones" },
            new Product { Name = "Samsung Galaxy S21", Price = 799.99M, Category = "Phones" },
            new Product { Name = "iPad Pro", Price = 1099.99M, Category = "Tablets" },
            new Product { Name = "Samsung Galaxy Tab S7", Price = 599.99M, Category = "Tablets" },
            new Product { Name = "Sony WH-1000XM4", Price = 349.99M, Category = "Headphones" },
            new Product { Name = "Bose QuietComfort 35 II", Price = 299.99M, Category = "Headphones" },
            new Product { Name = "Dell XPS 13", Price = 1299.99M, Category = "Laptops" },
            new Product { Name = "MacBook Pro", Price = 1799.99M, Category = "Laptops" }
        };

        // Define search criteria
        var criteria = new ProductSearchCriteria
        {
            Category = "Phones",
            PriceMin = 500.00M,
            PriceMax = 1000.00M
        };

        // Build expression tree for search criteria
        var predicate = ExpressionTreeHelper.BuildSearchCriteriaExpression<Product>(criteria);

        // Filter products using expression tree
        var filteredProducts = products.Where(predicate.Compile());

        // Output filtered products
        Console.WriteLine("Filtered products:");
        foreach (var product in filteredProducts)
        {
            Console.WriteLine($"{product.Name} - ${product.Price} ({product.Category})");
        }
    }
}

class Product
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
}

class ProductSearchCriteria
{
    public string? Name { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string? Category { get; set; }
}

class ExpressionTreeHelper
{
    public static Expression<Func<T, bool>>? BuildSearchCriteriaExpression<T>(ProductSearchCriteria criteria)
    {
        // Create parameter expression for the object being searched
        var parameter = Expression.Parameter(typeof(T), "p");

        // Build up expression tree based on search criteria
        Expression expression = null;
        if (!string.IsNullOrEmpty(criteria.Name))
        {
            var nameEquals = Expression.Equal(Expression.Property(parameter, "Name"), Expression.Constant(criteria.Name));
            expression = expression == null ? nameEquals : Expression.AndAlso(expression, nameEquals);
        }
        if (criteria.PriceMin.HasValue)
        {
            var priceGreaterThanOrEqual = Expression.GreaterThanOrEqual(Expression.Property(parameter, "Price"), Expression.Constant(criteria.PriceMin.Value));
            expression = expression == null ? priceGreaterThanOrEqual : Expression.AndAlso(expression, priceGreaterThanOrEqual);
        }
        if (criteria.PriceMax.HasValue)
        {
            var priceLessThanOrEqual = Expression.LessThanOrEqual(Expression.Property(parameter, "Price"), Expression.Constant(criteria.PriceMax.Value));
            expression = expression == null ? priceLessThanOrEqual : Expression.AndAlso(expression, priceLessThanOrEqual);
        }
        if (!string.IsNullOrEmpty(criteria.Category))
        {
            var categoryEquals = Expression.Equal(Expression.Property(parameter, "Category"), Expression.Constant(criteria.Category));
            expression = expression == null ? categoryEquals : Expression.AndAlso(expression, categoryEquals);
        }

        // Build lambda expression from expression tree
        if (expression == null)
        {
            return null;
        }
        else
        {
            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            return lambda;
        }
    }
}