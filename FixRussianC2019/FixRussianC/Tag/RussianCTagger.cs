using System.Text.RegularExpressions;
using FixRussianC2019.Support;
using Microsoft.VisualStudio.Text;

namespace FixRussianC2019.FixRussianC.Tag
{
    internal sealed class RussianCTagger : RegexTagger<RussianCTag>
    {
        public RussianCTagger(ITextBuffer buffer)
            : base(buffer, new[]
            {
                new Regex(@"[A-z][Сс]|[Сс][A-z]", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase),
            })
        {
        }


        protected override RussianCTag TryCreateTagForMatch(Match match)
        {
            return new RussianCTag();
        }
    }
}
