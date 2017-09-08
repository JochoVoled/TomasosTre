namespace TomasosTre.ViewModels.Home
{
    public class IndexViewModel
    {
        public CartViewModel Cart { get; set; }
        public DishCustomizationViewModel DishCustomization { get; set; }

        public IndexViewModel()
        {
            Cart = new CartViewModel();
            DishCustomization = new DishCustomizationViewModel();
        }
    }
}
