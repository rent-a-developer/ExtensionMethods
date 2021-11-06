using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionMethods.Strings
{
    /// <summary>
    /// Provides extension methods for the <see cref="String"/> type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a debug string for the content of <paramref name="text"/> suitable for analyzing the string.
        /// </summary>
        /// <param name="text">The string to get a debug string for.</param>
        /// <param name="maximumLineLength">The maximum number of characters each line in the returned string may have. Lines longer than that are wrapped.</param>
        /// <returns>A debug string for the content of <paramref name="text"/> suitable for analyzing the string.</returns>
        /// <example>
        /// <code>
        /// "ABC".ToDebugString();
        /// </code>
        /// Returns:
        /// Character: |A|B|C|
        /// Index:     |0|1|2|
        /// </example>
        /// <example>
        /// <code>
        /// "ABC\r\nEF".ToDebugString();
        /// </code>
        /// Returns:
        /// Character: |A |B |C |\r|\n|E |F |
        /// Index:     |00|01|02|03|04|05|06|
        /// </example>
        public static String ToDebugString(this String text, Int32 maximumLineLength = 80)
        {
            if (maximumLineLength < 25)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumLineLength), $"The argument {nameof(maximumLineLength)} must be equal to or greater than 25.");
            }

            if (String.IsNullOrEmpty(text))
            {
                return "The string is null or empty.";
            }

            var result = new StringBuilder();

            var characters = text.ToCharArray();
            var numberOfCharacters = characters.Length;
            var numberOfDigitsInNumberOfCharacters = numberOfCharacters == 1 ? 1 : (Int32)Math.Floor(Math.Log10(numberOfCharacters) + 1);

            var escapeSequences = characters.Select(a => GetEscapeSequence(a)).ToList();
            var maximumEscapeSequenceLength = escapeSequences.Max(a => a.Length);
            
            var columnContentWidth = Math.Max(numberOfDigitsInNumberOfCharacters, maximumEscapeSequenceLength);
            var columnWidth = columnContentWidth + 1 /* 1 for the pipe symbol in each column */;
            var maximumNumberOfColumnsPerLine = (Int32) Math.Max(1, Math.Floor((maximumLineLength - 11 /* 11 for the prefix "Character: " */) / (Double) columnWidth));

            var escapeSequencesChunks = Chunk(escapeSequences, maximumNumberOfColumnsPerLine);

            var characterIndex = 0;
            foreach (var escapeSequencesChunk in escapeSequencesChunks)
            {
                if (result.Length > 0)
                {
                    result.AppendLine();
                }

                result.Append("Character: |");
                foreach (var escapeSequence in escapeSequencesChunk)
                {
                    result.Append(escapeSequence.PadRight(columnContentWidth));
                    result.Append("|");
                }
                result.AppendLine();

                result.Append("Index:     |");
                for (var i = characterIndex; i < characterIndex + escapeSequencesChunk.Count; i++)
                {
                    result.Append(i.ToString(new String('0', columnContentWidth)));
                    result.Append("|");
                }
                result.AppendLine();

                characterIndex += escapeSequencesChunk.Count;
            }

            return result.ToString();
        }

        private static IEnumerable<List<T>> Chunk<T>(List<T> source, Int32 chunkSize)
        {
            for (int i = 0; i < source.Count; i += chunkSize)
            {
                yield return source.GetRange(i, Math.Min(chunkSize, source.Count - i));
            }
        }

        private static String GetEscapeSequence(Char character)
        {
            return character switch
            {
                '\a' => "\\a",
                '\b' => "\\b",
                '\f' => "\\f",
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                '\v' => "\\v",
                <= (Char)254 => character.ToString(),
                _ => "\\u" + ((Int32)character).ToString("X4")
            };
        }
    }
}
