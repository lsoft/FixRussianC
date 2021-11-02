using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using VsixNamespace.Support;

namespace VsixNamespace.FixRussianC.Tag
{
    /// <summary>
    /// Determines which spans of text likely refer to color values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a data-only component. The tagging system is a good fit for presenting data-about-text.
    /// The <see cref="RussianCAdornmentTagger"/> takes color tags produced by this tagger and creates corresponding UI for this data.
    /// </para>
    /// </remarks>
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
