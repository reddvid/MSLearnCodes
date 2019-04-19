using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace DataBindingSample
{
    public class ColorListLogic : ObservableObject
    {
        public List<ColorDescriptor> LotsOfColors { get; private set; }

        public ColorListLogic()
        {
            LotsOfColors = new List<ColorDescriptor>
            {
                new ColorDescriptor(Colors.Red, "red"),
                new ColorDescriptor(Colors.Yellow, "yellow"),
                new ColorDescriptor(Colors.Blue, "blue"),
                new ColorDescriptor(Colors.Green, "green"),
                new ColorDescriptor(Colors.White, "white"),
                new ColorDescriptor(Colors.Aquamarine, "aquamarine"),
                new ColorDescriptor(Colors.Black, "black"),
            };

            SelectedColor = LotsOfColors[0];
        }

        private ColorDescriptor _selectedColor;

        public ColorDescriptor SelectedColor
        {
            get => _selectedColor;
            set => Set(ref _selectedColor, value);
        }

        public ObservableCollection<ColorDescriptor> FavoriteColors { get; } = new ObservableCollection<ColorDescriptor>();

        public void AddSelectedColorToFavorites()
        {
            FavoriteColors.Add(SelectedColor);
        }

        private ColorDescriptor _selectedFavoriteColor;

        public ColorDescriptor SelectedFavoriteColor
        {
            get => _selectedFavoriteColor;
            set
            {
                Set(ref _selectedFavoriteColor, value);
                RaisePropertyChanged(nameof(IsRemoveFavoriteColorButtonVisible));
            }
        }

        public bool IsRemoveFavoriteColorButtonVisible => SelectedFavoriteColor != null;

        public void RemoveFavoriteColor()
        {
            FavoriteColors.Remove(SelectedFavoriteColor);
        }
    }
}
