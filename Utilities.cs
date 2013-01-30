using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace WikiTexifier {
    public enum ProgressBarState : int {
        Normal = 1,
        Error = 2,
        Paused = 3,
    }

    public static class Utilities {
        const int WM_USER = 0x400;
        const int PBM_SETSTATE = WM_USER + 16;
        const int PBM_GETSTATE = WM_USER + 17;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const string superscript_int = "⁰¹²³⁴⁵⁶⁷⁸⁹";
        const string normal_int = "0123456789";
        const string subscript_int = "₀₁₂₃₄₅₆₇₈₉";

        public static ProgressBarState GetState(this ProgressBar bar) {
            return (ProgressBarState)SendMessage(bar.Handle, PBM_GETSTATE, IntPtr.Zero, IntPtr.Zero);
        }

        public static void SetState(this ProgressBar bar, ProgressBarState state) {
            SendMessage(bar.Handle, PBM_SETSTATE, (IntPtr)state, IntPtr.Zero);
        }

        public static bool StartsWith(this string str, IEnumerable<string> choices) {
            foreach (var choice in choices) {
                if (str.StartsWith(choice))
                    return true;
            }
            return false;
        }

        public static string SuperscriptInt(this string str) {
            return new string(str.Select((c) => c >= '0' && c <= '9' ?
                superscript_int[c - '0'] : c).ToArray());
        }

        public static string SubscriptInt(this string str) {
            return new string(str.Select((c) => c >= '0' && c <= '9' ?
                subscript_int[c - '0'] : c).ToArray());
        }

        public static IEnumerable<HtmlNode> SafeSelectNodes(this HtmlNode node, string xpath) {
            return (IEnumerable<HtmlNode>)node.SelectNodes(xpath) ??
                    new List<HtmlNode>();
        }

        static Dictionary<char, char> unicode = new Dictionary<char, char> {
            {'\u00a0', ' '}, // Non-breaking space
            {'\u0126', 'H'}, // Ħ
            {'\u0127', 'h'}, // ħ
            {'\u0131', 'i'}, // ı
            {'\u0138', 'k'}, // ĸ
            {'\u013f', 'L'}, // Ŀ
            {'\u0141', 'L'}, // Ł
            {'\u0140', 'l'}, // ŀ
            {'\u0142', 'l'}, // ł
            {'\u014a', 'N'}, // Ŋ
            {'\u0149', 'n'}, // ŉ
            {'\u014b', 'n'}, // ŋ
            {'\u00d8', 'O'}, // Ø
            {'\u00f8', 'o'}, // ø
            {'\u017f', 's'}, // ſ
            {'\u0166', 'T'}, // Ŧ
            {'\u0167', 't'}, // ŧ
            {'\u00ab', '"'}, // « left-pointing double angle quotation mark
            {'\u00ad', '-'}, // ­ soft hyphen
            {'\u00b4', '\''},// ´ acute accent
            {'\u00bb', '"'}, // » right-pointing double angle quotation mark
            {'\u00f7', '/'}, // ÷ division sign
            {'\u01c0', '|'}, // ǀ latin letter dental click
            {'\u01c3', '!'}, // ǃ latin letter retroflex click
            {'\u02b9', '\''},// ʹ modifier letter prime
            {'\u02ba', '"'}, // ʺ modifier letter double prime
            {'\u02bc', '\''},// ʼ modifier letter apostrophe
            {'\u02c4', '^'}, // ˄ modifier letter up arrowhead
            {'\u02c6', '^'}, // ˆ modifier letter circumflex accent
            {'\u02c8', '\''},// ˈ modifier letter vertical line
            {'\u02cb', '`'}, // ˋ modifier letter grave accent
            {'\u02cd', '_'}, // ˍ modifier letter low macron
            {'\u02dc', '~'}, // ˜ small tilde
            {'\u0300', '`'}, // ̀ combining grave accent
            {'\u0301', '\''},// ́ combining acute accent
            {'\u0302', '^'}, // ̂ combining circumflex accent
            {'\u0303', '~'}, // ̃ combining tilde
            {'\u030b', '"'}, // ̋ combining double acute accent
            {'\u030e', '"'}, // ̎ combining double vertical line above
            {'\u0331', '_'}, // ̱ combining macron below
            {'\u0332', '_'}, // ̲ combining low line
            {'\u0338', '/'}, // ̸ combining long solidus overlay
            {'\u0589', ':'}, // ։ armenian full stop
            {'\u05c0', '|'}, // ׀ hebrew punctuation paseq
            {'\u05c3', ':'}, // ׃ hebrew punctuation sof pasuq
            {'\u066a', '%'}, // ٪ arabic percent sign
            {'\u066d', '*'}, // ٭ arabic five pointed star
            {'\u2010', '-'}, // ‐ hyphen
            {'\u2011', '-'}, // ‑ non-breaking hyphen
            {'\u2012', '-'}, // ‒ figure dash
            {'\u2013', '-'}, // – en dash
            {'\u2014', '-'}, // — em dash
            {'\u2017', '_'}, // ‗ double low line
            {'\u2018', '\''},// ‘ left single quotation mark
            {'\u2019', '\''},// ’ right single quotation mark
            {'\u201a', ','}, // ‚ single low-9 quotation mark
            {'\u201b', '\''},// ‛ single high-reversed-9 quotation mark
            {'\u201c', '"'}, // “ left double quotation mark
            {'\u201d', '"'}, // ” right double quotation mark
            {'\u201e', '"'}, // „ double low-9 quotation mark
            {'\u201f', '"'}, // ‟ double high-reversed-9 quotation mark
            {'\u2032', '\''},// ′ prime
            {'\u2033', '"'}, // ″ double prime
            {'\u2035', '`'}, // ‵ reversed prime
            {'\u2036', '"'}, // ‶ reversed double prime
            {'\u2038', '^'}, // ‸ caret
            {'\u2039', '<'}, // ‹ single left-pointing angle quotation mark
            {'\u203a', '>'}, // › single right-pointing angle quotation mark
            {'\u203d', '?'}, // ‽ interrobang
            {'\u2044', '/'}, // ⁄ fraction slash
            {'\u204e', '*'}, // ⁎ low asterisk
            {'\u2052', '%'}, // ⁒ commercial minus sign
            {'\u2053', '~'}, // ⁓ swung dash
            {'\u20e5', '\\'},// ⃥ combining reverse solidus overlay
            {'\u2212', '-'}, // − minus sign
            {'\u2215', '/'}, // ∕ division slash
            {'\u2216', '\\'},// ∖ set minus
            {'\u2217', '*'}, // ∗ asterisk operator
            {'\u2223', '|'}, // ∣ divides
            {'\u2236', ':'}, // ∶ ratio
            {'\u223c', '~'}, // ∼ tilde operator
            {'\u2303', '^'}, // ⌃ up arrowhead
            {'\u2329', '<'}, // 〈 left-pointing angle bracket
            {'\u232a', '>'}, // 〉 right-pointing angle bracket
            {'\u266f', '#'}, // ♯ music sharp sign
            {'\u2731', '*'}, // ✱ heavy asterisk
            {'\u2758', '|'}, // ❘ light vertical bar
            {'\u2762', '!'}, // ❢ heavy exclamation mark ornament
            {'\u27e6', '['}, // ⟦ mathematical left white square bracket
            {'\u27e8', '<'}, // ⟨ mathematical left angle bracket
            {'\u27e9', '>'}, // ⟩ mathematical right angle bracket
            {'\u2983', '{'}, // ⦃ left white curly bracket
            {'\u2984', '}'}, // ⦄ right white curly bracket
            {'\u3003', '"'}, // 〃 ditto mark
            {'\u3008', '<'}, // 〈 left angle bracket
            {'\u3009', '>'}, // 〉 right angle bracket
            {'\u301b', ']'}, // 〛 right white square bracket
            {'\u301c', '~'}, // 〜 wave dash
            {'\u301d', '"'}, // 〝 reversed double prime quotation mark
            {'\u301e', '"'}, // 〞 double prime quotation mark 
        };

        public static string Asciify(this string str) {
            str = str.Normalize(NormalizationForm.FormKD);
            str = str.Replace("a\u0308", "ae") // ä
                     .Replace("n\u0303", "ny") // ñ
                     .Replace("o\u0308", "oe") // ö
                     .Replace("u\u0308", "ue") // ü
                     .Replace("y\u0308", "yu") // ÿ
                     .Replace("\u00df", "ss")  // ß
                     .Replace("\u00c6", "AE")  // Æ
                     .Replace("\u00e6", "ae")  // æ
                     .Replace("\u0132", "IJ")  // Ĳ
                     .Replace("\u0133", "ij")  // ĳ
                     .Replace("\u0152", "OE")  // Œ
                     .Replace("\u0153", "oe")  // œ
                     .Replace("\u00d0", "TH")  // Ð
                     .Replace("\u00f0", "th")  // ð
                     .Replace("\u0110", "DJ")  // Đ
                     .Replace("\u0111", "dj")  // đ
                     .Replace("\u00de", "TH")  // Þ
                     .Replace("\u00fe", "th"); // þ
            var build = new StringBuilder(str.Length);
            foreach (char c in str)
                build.Append(unicode.ContainsKey(c) ? unicode[c] : c);
            str = build.ToString();
            for (int i = 0; i < 10; ++i)
                str = str.Replace(superscript_int[i], normal_int[i])
                         .Replace(subscript_int[i], normal_int[i]);
            return Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(str)
                .Where((data) => data < 0x80).ToArray()); ;
        }
    }
}
