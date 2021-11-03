using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FixRussianC2019.Support;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace FixRussianC2019.FixRussianC.Tag
{
    internal sealed class RussianCAdornmentTagger
        : IntraTextAdornmentTagger<RussianCTag, Button>

    {
        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<RussianCTag>> tagAggregator)
        {
            return view.Properties.GetOrCreateSingletonProperty<RussianCAdornmentTagger>(
                () => new RussianCAdornmentTagger(view, tagAggregator.Value));
        }

        private readonly ITagAggregator<RussianCTag> _tagAggregator;

        private RussianCAdornmentTagger(IWpfTextView view, ITagAggregator<RussianCTag> tagAggregator)
            : base(view)
        {
            _tagAggregator = tagAggregator;
        }

        protected override void DoDispose()
        {
            _tagAggregator.Dispose();

            _view.Properties.RemoveProperty(typeof(RussianCAdornmentTagger));
        }

        // To produce adornments that don't obscure the text, the adornment tags
        // should have zero length spans. Overriding this method allows control
        // over the tag spans.
        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, RussianCTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            ITextSnapshot snapshot = spans[0].Snapshot;

            var tags = _tagAggregator.GetTags(spans);

            foreach (IMappingTagSpan<RussianCTag> dataTagSpan in tags)
            {
                NormalizedSnapshotSpanCollection tagSpans = dataTagSpan.Span.GetSpans(snapshot);

                // Ignore data tags that are split by projection.
                // This is theoretically possible but unlikely in current scenarios.
                if (tagSpans.Count != 1)
                    continue;

                SnapshotSpan adornmentSpan = new SnapshotSpan(tagSpans[0].End, 0);

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
