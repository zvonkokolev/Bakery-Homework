using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Bakery.Persistence;
using Bakery.Wpf.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bakery.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private ProductDto _selectedProduct;
        private ObservableCollection<ProductDto> _products;

        public ObservableCollection<ProductDto> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }
        public ProductDto SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
                // Validate();
            } 
        }
        public MainWindowViewModel(IWindowController controller) : base(controller)
        {

            LoadCommands();
        }

        private void LoadCommands()
        {

        }

        public static async Task<MainWindowViewModel> Create(IWindowController controller)
        {
            var model = new MainWindowViewModel(controller);
            await model.LoadProducts();
            return model;
        }

        /// <summary>
        /// Produkte laden. Produkte können nach Preis gefiltert werden. 
        /// Preis liegt zwischen from und to
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private async Task LoadProducts()
        {
            await using IUnitOfWork unitOfWork = new UnitOfWork();

            ProductDto[] products = await unitOfWork.Products.GetAllProductDtosAsync();
            Products = new ObservableCollection<ProductDto>(products);

            SelectedProduct = Products.FirstOrDefault();
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
