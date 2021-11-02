using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using VsixNamespace.Support;

namespace VsixNamespace.FixRussianC.Tag
{
    /// <summary>
    /// Provides color swatch adornments in place of color constants.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a sample usage of the <see cref="IntraTextAdornmentTagTransformer"/> utility class.
    /// </para>
    /// </remarks>
    internal sealed class RussianCAdornmentTagger
        : IntraTextAdornmentTagger<RussianCTag, Button>

    {
        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<RussianCTag>> colorTagger)
        {
            return view.Properties.GetOrCreateSingletonProperty<RussianCAdornmentTagger>(
                () => new RussianCAdornmentTagger(view, colorTagger.Value));
        }

        private readonly ITagAggregator<RussianCTag> _colorTagger;

        private RussianCAdornmentTagger(IWpfTextView view, ITagAggregator<RussianCTag> colorTagger)
            : base(view)
        {
            this._colorTagger = colorTagger;
        }

        public void Dispose()
        {
            _colorTagger.Dispose();

            view.Properties.RemoveProperty(typeof(RussianCAdornmentTagger));
        }

        // To produce adornments that don't obscure the text, the adornment tags
        // should have zero length spans. Overriding this method allows control
        // over the tag spans.
        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, RussianCTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            ITextSnapshot snapshot = spans[0].Snapshot;

            var colorTags = _colorTagger.GetTags(spans);

            foreach (IMappingTagSpan<RussianCTag> dataTagSpan in colorTags)
            {
                NormalizedSnapshotSpanCollection colorTagSpans = dataTagSpan.Span.GetSpans(snapshot);

                // Ignore data tags that are split by projection.
                // This is theoretically possible but unlikely in current scenarios.
                if (colorTagSpans.Count != 1)
                    continue;

                SnapshotSpan adornmentSpan = new SnapshotSpan(colorTagSpans[0].End, 0);

                yield return Tuple.Create(adornmentSpan, (PositionAffinity?)PositionAffinity.Successor, dataTagSpan.Tag);
            }
        }

        protected override Button CreateAdornment(RussianCTag dataTag, SnapshotSpan span)
        {
            var view = new Button();
            view.Content = "Found russian C";
            view.Foreground = Brushes.Red;
            view.Visibility = Visibility.Collapsed;

            return view;
        }

        protected override bool UpdateAdornment(Button adornment, RussianCTag dataTag)
        {
            return true;
        }
    }
}
