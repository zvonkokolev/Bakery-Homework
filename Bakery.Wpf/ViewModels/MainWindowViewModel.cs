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
        private double _selectedPriceFilterFrom;
        private double _selectedPriceFilterTo;
        private double _avg;

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
        public double SelectedPriceFilterFrom 
        {
            get => _selectedPriceFilterFrom;
            set
            {
                _selectedPriceFilterFrom = value;
                OnPropertyChanged(nameof(SelectedPriceFilterFrom));
            }
        }
        public double SelectedPriceFilterTo
        {
            get => _selectedPriceFilterTo;
            set
            {
                _selectedPriceFilterTo = value;
                OnPropertyChanged(nameof(SelectedPriceFilterTo));
            }
        }
        public double Avg 
        {
            get => _avg = Average();
            set
            {
                _avg = value;
                OnPropertyChanged(nameof(Avg));
            }
        }
        public double Average()
        {
            double result = 0;
            foreach (var item in Products)
            {
                result += item.Price;
            }
            return result / Products.Count;
        }
        public RelayCommand CmdPriceFilter { get; private set; }
        public RelayCommand CmdNewProduct { get; set; }
        public RelayCommand CmdEditProduct { get; set; }


        public MainWindowViewModel(IWindowController controller) : base(controller)
        {

            LoadCommandsAsync();
        }

        private void LoadCommandsAsync()
        {
            CmdPriceFilter = new RelayCommand(
                execute: async _ =>
                {
                    await using IUnitOfWork unitOfWork = new UnitOfWork();
                    var a = await unitOfWork.Products
                    .GetFilteredProduct(SelectedPriceFilterFrom, SelectedPriceFilterTo);
                    Products = new ObservableCollection<ProductDto>(a);
                    SelectedProduct = Products.FirstOrDefault();
                    Avg = Average();
                }
                ,
                canExecute: _ => SelectedPriceFilterFrom > 0 
                    || SelectedPriceFilterTo > 0
                )
                ;
            CmdNewProduct = new RelayCommand(
                execute: _ =>
                {

                }
                ,
                canExecute: _ => SelectedProduct != null
                )
                ;
            CmdEditProduct = new RelayCommand(
                execute: _ =>
                {

                }
                ,
                canExecute: _ => SelectedProduct != null
                )
                ;
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
