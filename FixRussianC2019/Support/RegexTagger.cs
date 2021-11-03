//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace FixRussianC2019.Support
{
    /// <summary>
    /// Helper base class for writing simple taggers based on regular expressions.
    /// </summary>
    /// <remarks>
    /// Regular expressions are expected to be single-line.
    /// </remarks>
    /// <typeparam name="T">The type of tags that will be produced by this tagger.</typeparam>
    internal abstract class RegexTagger<T> : VisibleWindowTagger<T> where T : ITag
    {
        private readonly IEnumerable<Regex> _matchExpressions;

        public RegexTagger(ITextBuffer buffer, IEnumerable<Regex> matchExpressions)
            : base(buffer)
        {
            if (matchExpressions.Any(re => (re.Options & RegexOptions.Multiline) == RegexOptions.Multiline))
            {
                throw new ArgumentException("Multiline regular expressions are not supported.");
            }

            _matchExpressions = matchExpressions;
        }

        /// <inheritdoc />
        protected override bool CheckAndCreateTagSpan(
            ITextSnapshotLine line,
            out List<ITagSpan<T>> tagSpans
            )
        {
            tagSpans = new List<ITagSpan<T>>();

            string text = line.GetText();

            foreach (var regex in _matchExpressions)
            {
                foreach (var match in regex.Matches(text).Cast<Match>())
                {
                    T tag = TryCreateTagForMatch(match);
                    if (tag != null)
                    {
                        SnapshotSpan span = new SnapshotSpan(line.Start + match.Index, match.Length);
                        tagSpans.Add(new TagSpan<T>(span, tag));
                    }
                }
            }

            return tagSpans.Count > 0;
        }


        /// <summary>
        /// Overridden in the derived implementation to provide a tag for each regular expression match.
        /// If the return value is <c>null</c>, this match will be skipped.
        /// </summary>
        /// <param name="match">The match to create a tag for.</param>
        /// <returns>The tag to return from <see cref="GetTags"/>, if non-<c>null</c>.</returns>
        protected abstract T TryCreateTagForMatch(Match match);
    }
}
