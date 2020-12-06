using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Bakery.Persistence;
using Bakery.Wpf.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Bakery.Wpf.ViewModels
{
    public class ProductWindowViewModel : BaseViewModel
    {
        private Product _selectedProduct;
        private Product _temporaryProduct;
        private string _productNr;
        private string _name;
        private double _price;

        public string Title { get; set; }
        public RelayCommand CmdSaveProduct { get; set; }
        public RelayCommand CmdUnload { get; set; }
        public string ProductNr 
        {
            get => _productNr;
            set
            {
                _productNr = value;
                OnPropertyChanged(nameof(ProductNr));
            } 
        }
        public string Name 
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public double Price 
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        public ProductWindowViewModel(IWindowController controller) : base(controller)
        {
            LoadCommandsAsync();
        }
        public ProductWindowViewModel(IWindowController controller, ProductDto product) : base(controller)
        {
            LoadCommandsAsync();
        }
        private void LoadCommandsAsync()
        {
            CmdSaveProduct = new RelayCommand(
                execute: async _ =>
                {
                    await using IUnitOfWork unitOfWork = new UnitOfWork();
                    Product product = new Product
                    {
                        ProductNr = ProductNr,
                        Name = Name,
                        Price = Price
                    };
                    SelectedProduct = product;
                    
                    await unitOfWork.Products
                        .AddProductAsync(SelectedProduct);
                    try
                    {
                        await unitOfWork.SaveChangesAsync();
                    }
                    catch (ValidationException e)
                    {
                        throw new ValidationException(e.Message);
                    }

                }
                ,
                canExecute: _ => ProductNr != null 
                )
                ;
            CmdUnload = new RelayCommand(
                execute: _ =>
                {
                    SelectedProduct = _temporaryProduct;
                }
                ,
                canExecute: _ => ProductNr != null
                )
                ;
        }
        public static async Task<ProductWindowViewModel> Create(IWindowController controller, Product product)
        {
            var model = new ProductWindowViewModel(controller);
            await model.LoadProduct(product);
            return model;
        }
        private async Task LoadProduct(Product product)
        {
            await using IUnitOfWork unitOfWork = new UnitOfWork();
            if(product != null)
            {
                SelectedProduct = await unitOfWork.Products.GetProductByIdAsync(product.Id);
                _temporaryProduct = SelectedProduct;
                ProductNr = SelectedProduct.ProductNr;
                Name = SelectedProduct.Name;
                Price = SelectedProduct.Price;
                Title = "Produkt bearbeiten";
            }
            else
            {
                Title = "Produkt anlegen";
            }
        }
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
