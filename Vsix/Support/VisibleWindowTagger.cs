using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace VsixNamespace.Support
{
    /// <summary>
    /// Helper base class for writing simple taggers.
    /// </summary>
    /// <typeparam name="T">The type of tags that will be produced by this tagger.</typeparam>
    internal abstract class VisibleWindowTagger<T> : ITagger<T> where T : ITag
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;


        public VisibleWindowTagger(ITextBuffer buffer)
        {
            buffer.Changed += (sender, args) => HandleBufferChanged(args);
        }

        public IEnumerable<ITagSpan<T>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // Here we grab whole lines so that matches that only partially fall inside the spans argument are detected.
            // Note that the spans argument can contain spans that are sub-spans of lines or intersect multiple lines.
            foreach (var line in GetIntersectingLines(spans))
            {
                if (this.CheckAndCreateTagSpan(line, out var tagSpans))
                {
                    foreach(var tagSpan in tagSpans)
                        yield return tagSpan;
                }
            }
        }

        protected abstract bool CheckAndCreateTagSpan(
            ITextSnapshotLine line,
            out List<ITagSpan<T>> tagSpans
            );


        IEnumerable<ITextSnapshotLine> GetIntersectingLines(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;
            int lastVisitedLineNumber = -1;
            ITextSnapshot snapshot = spans[0].Snapshot;
            foreach (var span in spans)
            {
                int firstLine = snapshot.GetLineNumberFromPosition(span.Start);
                int lastLine = snapshot.GetLineNumberFromPosition(span.End);

                for (int i = Math.Max(lastVisitedLineNumber, firstLine); i <= lastLine; i++)
                {
                    yield return snapshot.GetLineFromLineNumber(i);
                }

                lastVisitedLineNumber = lastLine;
            }
        }
        
        /// <summary>
        /// Handle buffer changes. The default implementation expands changes to full lines and sends out
        /// a <see cref="TagsChanged"/> event for these lines.
        /// </summary>
        /// <param name="args">The buffer change arguments.</param>
        protected virtual void HandleBufferChanged(TextContentChangedEventArgs args)
        {
            if (args.Changes.Count == 0)
                return;

            var temp = TagsChanged;
            if (temp == null)
                return;

            // Combine all changes into a single span so that
            // the ITagger<>.TagsChanged event can be raised just once for a compound edit
            // with many parts.

            ITextSnapshot snapshot = args.After;

            int start = args.Changes[0].NewPosition;
            int end = args.Changes[args.Changes.Count - 1].NewEnd;

            SnapshotSpan totalAffectedSpan = new SnapshotSpan(
                snapshot.GetLineFromPosition(start).Start,
                snapshot.GetLineFromPosition(end).End);

            temp(this, new SnapshotSpanEventArgs(totalAffectedSpan));
        }
    }
}
