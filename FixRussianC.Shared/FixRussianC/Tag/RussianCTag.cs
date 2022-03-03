using Microsoft.VisualStudio.Text.Tagging;

namespace FixRussianC2019.FixRussianC.Tag
{
    public class RussianCTag : TextMarkerTag
    {
        public RussianCTag(
            )
            : base("MarkerFormatDefinition/RedBorderFormatDefinition")
        {
        }
    }
}
