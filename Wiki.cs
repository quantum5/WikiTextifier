using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Text;
using System.Diagnostics;

namespace WikiTexifier {
    class WikiPage {
        HtmlDocument doc;
        List<string> sections;
        string cleaned;
        
        static Regex newLine = new Regex(@"[\r\n]+");
        static Regex newLineList = new Regex(@"[\r\n]+(\ +-)");
        static Regex newLineTooMuch = new Regex(@"\r\n\r\n[\r\n]+");
        static Regex bulletOrNumber = new Regex(@"^\s*(?:-|\d+\.)");
        /**
         * I know this is a monster regex, it means:
         *  1. capture a line of bullet or number
         *  2. negatively look ahead for a bullet or number
         * This regex will match the last element of the list
         * Note: `$[\r\n]+` is required to consume the newline
         */
        static Regex endOfList = new Regex(
            @"(^\s*(?:-|\d+\.).*?$[\r\n]+)(?!\s*(?:-|\d+\.))",
            RegexOptions.Multiline);
        static Regex tableRow = new Regex(@"(?<=\|\r\n)\r\n(?=\+)|(?<=\+\r\n)\r\n(?=\|)");
        static HashSet<string> blackSections = new HashSet<string> {
                "REFERENCES", "EXTERNAL LINKS", "FURTHER READING",
                "SEE ALSO", "POPULAR CULTURE", "IN CULTURE",
            };
        static int blackSize = 0;

        static WikiPage() {
            foreach (var phrase in blackSections)
                if (phrase.Length > blackSize)
                    blackSize = phrase.Length;
        }

        public WikiPage(string page) {
            doc = new HtmlDocument();
            load(page);
        }

        public void load(string page) {
            doc.LoadHtml(page);
        }

        protected IEnumerable<HtmlNode> SelectNodes(string xpath) {
            return doc.DocumentNode.SafeSelectNodes(xpath);
        }

        protected void remove(string xpath) {
            foreach (var node in SelectNodes(xpath))
                node.Remove();
        }

        protected void update(string xpath, Func<string, string> worker) {
            foreach (var node in SelectNodes(xpath))
                node.InnerHtml = worker(node.InnerText);
        }

        private void removeRedirectNotice() {
            var redirect = doc.DocumentNode.SelectSingleNode("//dl/dd/i");
            if (redirect != null &&
                    (redirect.InnerText.Contains("redirect") ||
                     redirect.InnerText.Contains("disambiguation")))
                redirect.Remove();
        }

        private void handleInfoBoxes() {
            foreach (var node in SelectNodes("//table[contains(@class, 'infobox')]")) {
                /*foreach (var image in node.SafeSelectNodes("//tr/td/img"))
                    // That box with images
                    // <img>   <td>   <tr>
                    image.ParentNode.ParentNode.Remove();*/
                
                string infobox = "";
                var caption = node.SelectSingleNode("//caption");
                if (caption != null)
                    infobox += caption.InnerText.Trim() + ":\r\n";

                foreach (var th in node.SafeSelectNodes("//tr/th")) {
                    var text = new StringBuilder();
                    var tr = th.ParentNode;

                    if (tr == null) continue;
                    text.Append("  - ").Append(th.InnerText);
                    th.Remove();
                    var data = tr.InnerText;
                    if (!string.IsNullOrWhiteSpace(data))
                        text.Append(": ")
                            .Append(string.Join(", ",
                                data.Trim().Split(
                                    new char[]{'\r', '\n'},
                                    StringSplitOptions.RemoveEmptyEntries)));
                    tr.InnerHtml = text.ToString();
                    tr.Name = "p";

                    tr.Attributes.RemoveAll();
                }

                foreach (var tr in node.SafeSelectNodes("//tr"))
                    tr.Remove();

                node.AppendChild(HtmlNode.CreateNode("<p></p>"));

                node.Name = "p";
                node.Attributes.RemoveAll();
            }
        }

        private void handleWikiTable() {
            foreach (var table in SelectNodes("//table[@class='wikitable']")) {
                StringBuilder TextTable = new StringBuilder();
                //Debug.WriteLine(table.InnerText);
                
                var caption = table.SelectSingleNode("caption");
                if (caption != null) {
                    TextTable.AppendLine(string.Format("------{0}------", caption.InnerText));
                    Debug.WriteLine(caption.InnerText);
                }

                List<List<string>> data = new List<List<string>>();
                List<int> columnLen = new List<int>();
                foreach (var tr in table.SafeSelectNodes("tr")) {
                    int cid = 0;
                    List<string> row = new List<string>();
                    //Debug.WriteLine("row");
                    foreach (var column in tr.SafeSelectNodes("td | th")) {
                        string text = column.InnerText;
                        if (column.Name == "th")
                            text = string.Format("*{0}*", text);

                        row.Add(text);
                        //Debug.WriteLine(text);

                        if (cid >= columnLen.Count)
                            columnLen.Add(text.Length);
                        else if (text.Length > columnLen[cid])
                            columnLen[cid] = text.Length;
                        ++cid;
                    }
                    data.Add(row);
                }
                Debug.WriteLine(string.Format("Rows: {0}", data.Count));
                Debug.WriteLine(string.Format("Columns: {0}", columnLen.Count));

                WriteRowSeparator(TextTable, columnLen);
                foreach (var row in data) {
                    TextTable.Append("|");
                    int cid = 0;
                    foreach (var column in row) {
                        TextTable.Append("\xa0");
                        TextTable.Append(column);
                        TextTable.Append(new string('\xa0', columnLen[cid] - column.Length));
                        TextTable.Append("\xa0|");
                        ++cid;
                    }
                    TextTable.Append("\n");
                    WriteRowSeparator(TextTable, columnLen);
                }

                table.Name = "p";
                table.Attributes.RemoveAll();
                table.RemoveAllChildren();
                table.InnerHtml = TextTable.ToString();//.Replace("\n", "<br/>");
            }
        }

        private void WriteRowSeparator(StringBuilder text, List<int> columnLen) {
            text.Append("+");
            foreach (var column in columnLen) {
                text.Append(new string('-', column + 2));
                text.Append("+");
            }
            text.Append("\n");
        }

        public void clean() {
            removeRedirectNotice();

            remove("//sup[@class='reference']");
            remove("//sup[@class='Template-Fact']");

            update("//sup", (str) => str.SuperscriptInt());
            update("//sub", (str) => str.SubscriptInt());

            handleWikiTable();
            handleInfoBoxes();

            remove("//div");
            //remove("//table");
            remove("//span[@class='editsection']");
            remove("//span[@class='mw-editsection']");

            // Formfeed to split sections
            update("//span[@class='mw-headline']", (str) => '\f' + str + '\a');

            HtmlNode parent = null;
            int index = 0;
            foreach (var node in SelectNodes("//li")) {
                int indent = node.Ancestors().Where((n) => n.Name == "li").Count();
                string prepend = string.Empty;
                if (parent != node.ParentNode) {
                    parent = node.ParentNode;
                    index = 0;
                } else ++index;
                if (node.ParentNode.Name == "ul") {
                    prepend = new string(' ', indent * 4) + "  - ";
                } else if (node.ParentNode.Name == "ol") {
                    int length = indent * 4 + 2;
                    prepend = index.ToString().PadLeft(length, ' ') + ". ";
                }
                node.InnerHtml = prepend + node.InnerHtml;
            }

            foreach (var node in SelectNodes("//blockquote")) {
                node.InnerHtml = "    " + node.InnerText.Replace("\n", "\n    ");
            }
        }

        protected bool isParagraph(string line) {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line))
                return false;
            if (bulletOrNumber.IsMatch(line))
                return false;
            return true;
        }

        public void split() {
            cleaned = doc.DocumentNode.InnerText.Replace("\n", "\r\n");
            cleaned = System.Net.WebUtility.HtmlDecode(cleaned);
            var sections = cleaned.Split(new char[] {'\f'},
                StringSplitOptions.RemoveEmptyEntries);
            this.sections = sections.Select((section) => {
                if (section.Length == 0)
                    return string.Empty;
                section = newLine.Replace(section, "\r\n").Trim();
                section = String.Join("\r\n",
                    section.Split(new string[] {"\r\n"},
                                  StringSplitOptions.RemoveEmptyEntries)
                           .Select((line) => {
                                line = line.TrimEnd();
                                // All lines that are not in a list will
                                // will have two new lines as they are
                                // assumed to be paragraphs
                                if (isParagraph(line)) {
                                    line.Trim();
                                    line += "\r\n";
                                }
                                return line;
                           })).TrimEnd();
                section = newLineList.Replace(section, "\r\n$1");
                section = newLineTooMuch.Replace(section, "\r\n\r\n");
                section = endOfList.Replace(section, "$1\r\n");
                if (!section.Contains('\a'))
                    return section;
                else
                    //section = newLine.Replace(section, ":\r\n", 1);
                    section = section.Replace('\a', ':');
                section = tableRow.Replace(section, string.Empty);

                if (section.Substring(0, Math.Min(blackSize, section.Length))
                        .ToUpper().StartsWith(blackSections))
                    return string.Empty;
                if (!section.Contains('\n'))
                    return string.Empty;

                return section;
            }).Where((section) => section.Length > 0).ToList();
        }

        public IEnumerable<string> Sections() {
            return sections;
        }

        public override string ToString() {
            clean();
            split();
            return string.Join("\r\n---\r\n", sections);
        }
    }
}
