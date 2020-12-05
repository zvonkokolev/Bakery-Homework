using Bakery.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Bakery.ImportConsole
{
    public class ImportController
    {
        public static IEnumerable<Product> ReadFromCsv()
        {
            string pathProducts = "Products.csv";
            string pathOrderItems = "OrderItems.csv";

            string[][] orderItemsMatrix = MyFile.ReadStringMatrixFromCsv(pathOrderItems, true);
            string[][] productsMatrix = MyFile.ReadStringMatrixFromCsv(pathProducts, true);

            List<Customer> customers = new List<Customer>();
            List<Product> products = new List<Product>();
            List<Order> orders = new List<Order>();

            List<string> customersNr = orderItemsMatrix.Select(c => c[2]).Distinct().ToList();
            List<string> productsNr = productsMatrix.Select(p => p[0]).Distinct().ToList();
            List<string> ordersNr = orderItemsMatrix.Select(p => p[0]).Distinct().ToList();

            List<Product> productsAll = productsMatrix
                .Select(p => new Product
                {
                    ProductNr = p[0],
                    Name = p[1],
                    Price = Double.Parse(p[2])
                })
                .ToList()
                ;
            List<Customer> customersAll = orderItemsMatrix
                .Select(c => new Customer
                {
                    CustomerNr  = c[2],
                    LastName    = c[3],
                    FirstName   = c[4]
                })
                .ToList()
                ;
            List<Order> ordersAll = orderItemsMatrix
                .Select(o => new Order
                {
                    OrderNr = o[0],
                    Date = Convert.ToDateTime(o[1]),
                    Customer = customersAll.Where(c => c.CustomerNr.Equals(o[2])).FirstOrDefault()
                })
                .ToList()
                ;
            foreach (string customerNr in customersNr)
            {
                customers.Add(customersAll.Where(c => c.CustomerNr.Equals(customerNr)).FirstOrDefault());
            }
            foreach (string productNr in productsNr)
            {
                Product product = productsAll.Where(p => p.ProductNr.Equals(productNr)).FirstOrDefault();
                List<OrderItem> ordItm = new List<OrderItem>();
                foreach (var item in orderItemsMatrix)
                {
                    Order order = ordersAll.Where(o => o.OrderNr.Equals(item[0]) && product.ProductNr.Equals(item[5])).FirstOrDefault();
                    if(order != null)
                    {
                        OrderItem oi = new OrderItem
                        {
                            Order = order,
                            Product = product,
                            Amount = int.Parse(item[6])
                        };
                        ordItm.Add(oi);
                    }
                }
                product.OrderItems = ordItm;
                products.Add(product);
            }      
            return products;
        }
    }
}
