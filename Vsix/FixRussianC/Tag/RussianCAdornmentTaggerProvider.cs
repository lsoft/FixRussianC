using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace VsixNamespace.FixRussianC.Tag
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [ContentType("projection")]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class RussianCAdornmentTaggerProvider : IViewTaggerProvider
    {
        #pragma warning disable 649 // "field never assigned to" -- field is set by MEF.
        [Import]
        internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService;
        #pragma warning restore 649

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer != textView.TextBuffer)
                return null;

            return RussianCAdornmentTagger.GetTagger(
                (IWpfTextView)textView,
                new Lazy<ITagAggregator<RussianCTag>>(
                    () => BufferTagAggregatorFactoryService.CreateTagAggregator<RussianCTag>(textView.TextBuffer)))
                as ITagger<T>;
        }
    }
}
