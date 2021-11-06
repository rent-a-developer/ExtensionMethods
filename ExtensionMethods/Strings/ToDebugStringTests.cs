using System;
using NUnit.Framework;
using FluentAssertions;
using static FluentAssertions.FluentActions;

namespace ExtensionMethods.Strings
{
    [TestFixture(Description = "Tests for the ToDebugString extension method.")]
    public class ToDebugStringTests
    {
        [Test(Description = "ToDebugString should throw an exception when the argument maximumLineLength is less than 25.")]
        public void ToDebugString_InvalidMaximumLineLength_ShouldPrintCharacters()
        {
            Invoking(() => StringExtensions.ToDebugString("A", 24)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test(Description = "ToDebugString should return 'The string is null or empty.' if the given string is null or empty.")]
        public void ToDebugString_NullOrEmptyString_ShouldPrintCharacters()
        {
            StringExtensions.ToDebugString("").Should().Be(@"The string is null or empty.");
        }

        [Test(Description = "ToDebugString should print (printable) ASCII characters as they are.")]
        public void ToDebugString_AsciiCharacters_ShouldPrintCharacters()
        {
            StringExtensions.ToDebugString("A").Should().Be(@"
Character: |A|
Index:     |0|
".TrimStart());

            StringExtensions.ToDebugString("ABC").Should().Be(@"
Character: |A|B|C|
Index:     |0|1|2|
".TrimStart());

            StringExtensions.ToDebugString("ABCDEFGHIJK").Should().Be(@"
Character: |A |B |C |D |E |F |G |H |I |J |K |
Index:     |00|01|02|03|04|05|06|07|08|09|10|
".TrimStart());
        }

        [Test(Description = "ToDebugString should print control characters as escape sequences.")]
        public void ToDebugString_ControlCharacters_ShouldPrintEscapeSequences()
        {
            StringExtensions.ToDebugString("\a\b\f\n\r\t\v").Should().Be(@"
Character: |\a|\b|\f|\n|\r|\t|\v|
Index:     |00|01|02|03|04|05|06|
".TrimStart());
        }

        [Test(Description = "ToDebugString should print unicode characters as escape sequences.")]
        public void ToDebugString_UnicodeCharacters_ShouldPrintUnicodeSequences()
        {
            StringExtensions.ToDebugString("\u9984\u9992\u9995\u9996").Should().Be(@"
Character: |\u9984|\u9992|\u9995|\u9996|
Index:     |000000|000001|000002|000003|
".TrimStart());
        }

        [Test(Description = "ToDebugString should wrap lines longer than the specified maximum line length.")]
        public void ToDebugString_MaximumLineLength_ShouldWrapLines()
        {
            StringExtensions.ToDebugString("01234567890123456789012345678901234567", 40).Should().Be(@"
Character: |0 |1 |2 |3 |4 |5 |6 |7 |8 |
Index:     |00|01|02|03|04|05|06|07|08|

Character: |9 |0 |1 |2 |3 |4 |5 |6 |7 |
Index:     |09|10|11|12|13|14|15|16|17|

Character: |8 |9 |0 |1 |2 |3 |4 |5 |6 |
Index:     |18|19|20|21|22|23|24|25|26|

Character: |7 |8 |9 |0 |1 |2 |3 |4 |5 |
Index:     |27|28|29|30|31|32|33|34|35|

Character: |6 |7 |
Index:     |36|37|
".TrimStart());
        }
    }
}
