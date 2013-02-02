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

        public static string Asciify(this string str) {
            str = str.Normalize(NormalizationForm.FormKD);
            str = str.Replace("a\u0308", "ae") // ä
                     .Replace("n\u0303", "ny") // ñ
                     .Replace("o\u0308", "oe") // ö
                     .Replace("u\u0308", "ue") // ü
                     .Replace("y\u0308", "yu") // ÿ
                     .Replace("A\u0308", "AE") // Ä
                     .Replace("N\u0303", "NY") // Ñ
                     .Replace("O\u0308", "OE") // Ö
                     .Replace("U\u0308", "UE") // Ü
                     .Replace("Y\u0308", "YU") // Ÿ
                     .Replace("\u0418\u0306", "j") // Й
                     .Replace("\u0438\u0306", "j") // й
                ;
            var build = new StringBuilder(str.Length);
            foreach (char c in str)
                if (Data.Unicode.map.ContainsKey(c))
                    build.Append(Data.Unicode.map[c]);
                else
                    build.Append(c);
            str = build.ToString();
            return Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(str)
                .Where((d) => d < 0x80).ToArray());
        }
    }
}
