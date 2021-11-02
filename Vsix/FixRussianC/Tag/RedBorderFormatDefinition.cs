using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VsixNamespace.FixRussianC.Tag
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/RedBorderFormatDefinition")]
    [UserVisible(true)]
    internal sealed class RedBorderFormatDefinition : MarkerFormatDefinition
    {

        public RedBorderFormatDefinition()
        {
            Setup();
        }

        private void Setup()
        {
            this.Fill = Brushes.Transparent;
            //this.Fill.Opacity = 0.5;

            this.Border = new Pen(Brushes.Red, 1.0)
            {
                DashStyle = new DashStyle(new[] { 2.0, 4.0 }, 1),
                Thickness = 1,
            };

            this.DisplayName = "Found Russian C";
            this.ZOrder = 4;
        }
    }
}
