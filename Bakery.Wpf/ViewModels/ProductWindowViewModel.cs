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

        public string Title { get; set; }
        public RelayCommand CmdSaveProduct { get; set; }
        public RelayCommand CmdUnload { get; set; }

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
                    await unitOfWork.Products
                        .AddProductAsync(SelectedProduct);
                }
                ,
                canExecute: _ => SelectedProduct != null
                )
                ;
            CmdUnload = new RelayCommand(
                execute: _ =>
                {
                    SelectedProduct = _temporaryProduct;
                }
                ,
                canExecute: _ => SelectedProduct != null
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
