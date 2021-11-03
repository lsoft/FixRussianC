using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace FixRussianC2019.FixRussianC.Tag
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(RussianCTag))]
    internal sealed class RussianCTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return buffer.Properties.GetOrCreateSingletonProperty(() => new RussianCTagger(buffer)) as ITagger<T>;
        }
    }
}
