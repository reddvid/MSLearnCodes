using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace DataBindingSample
{
    public class ColorDescriptor : ObservableObject
    {
        public Color Color { get; set; }
        public string Name { get; set; }
        public ColorDescriptor(Color color, string name)
        {
            Color = color;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name} (#{Color.R:X2}{Color.G:X2}{Color.B:X2})";
        }
    }
}
