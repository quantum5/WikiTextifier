using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WikiTexifier {
    public partial class WikiTextifier : Form {
        public WikiTextifier() {
            InitializeComponent();
        }

        private void Fetch(object sender, EventArgs e) {
            PageText.Enabled = false;

            var web = new WebClient();
            var uri = new Uri(String.Format("{0}?title={1}&action=render",
                Site.Text, Uri.EscapeDataString(PageTitle.Text)));
            web.Encoding = Encoding.UTF8;
            web.Headers["User-Agent"] = "Mozilla/5.0";
            Progress.Style = ProgressBarStyle.Continuous;
            Progress.SetState(ProgressBarState.Normal);
            Progress.Value = 0;
            web.DownloadProgressChanged += (obj, args) => {
                Progress.Maximum = (int) args.TotalBytesToReceive;
                Progress.Value = (int) args.BytesReceived;
            };
            web.DownloadStringCompleted += Process;
            web.DownloadStringAsync(uri);
        }

        void FormatText(object work) {
            string text = new WikiPage((string)work).ToString();

            this.Invoke(new Action(delegate {
                PageText.Text = text;
                Progress.Style = ProgressBarStyle.Continuous;
                PageText.Enabled = true;
            }));
        }

        void Process(object sender, DownloadStringCompletedEventArgs e) {
            if (e.Error != null) {
                Progress.Value = Progress.Maximum;
                Progress.SetState(ProgressBarState.Error);
                System.Diagnostics.Debug.WriteLine(e.Error);
                PageText.Enabled = true;
                MessageBox.Show(e.Error.Message, "Fatal Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Progress.Value = Progress.Maximum;
            Progress.Style = ProgressBarStyle.Marquee;

            new Thread(FormatText).Start(e.Result);
        }

        RegistryKey config = Registry.CurrentUser.CreateSubKey("Software\\WikiTextifier");

        private RegistryKey GetFontKey() {
            RegistryKey font = config.CreateSubKey("Font");
            if (font.GetValue("name") as string == null)
                font.SetValue("name", "Consolas", RegistryValueKind.String);
            if (font.GetValue("size") as int? == null)
                font.SetValue("size", 11, RegistryValueKind.DWord);
            return font;
        }

        private Font GetFont() {
            Font monospace;
            RegistryKey font = GetFontKey();
            try {
                monospace = new Font(new FontFamily((string) font.GetValue("name")),
                            (int) font.GetValue("size"));
            } catch (ArgumentException) {
                monospace = new Font(FontFamily.GenericMonospace, (int) font.GetValue("size"));
            }
            return monospace;
        }

        private void FormLoad(object sender, EventArgs e) {
            Font monospace = GetFont();
            PageText.Font = monospace;
            FontSelect.Font = monospace;
            UpdatePreview(monospace);

            this.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        private void ASCIIfy_Click(object sender, EventArgs e) {
            new Thread(() => {
                PageText.Invoke(new Action(delegate {
                    Progress.Style = ProgressBarStyle.Marquee;
                    PageText.Enabled = false;
                }));
                
                string text = PageText.Text.Asciify();

                PageText.Invoke(new Action(delegate {
                    PageText.Text = text;
                    PageText.Enabled = true;
                    Progress.Style = ProgressBarStyle.Continuous;
                }));
            }).Start();
        }

        private void SelectFont_Click(object sender, EventArgs e) {
            FontSelect.ShowDialog();
            RegistryKey font = GetFontKey();
            font.SetValue("name", FontSelect.Font.FontFamily.Name, RegistryValueKind.String);
            font.SetValue("size", FontSelect.Font.Size, RegistryValueKind.DWord);
            PageText.Font = GetFont();
            UpdatePreview(PageText.Font);
        }

        private void UpdatePreview(Font font) {
            FontView.Font = font;
            FontView.Text = string.Format("Font: {0}, Size: {1} pt", font.Name, font.SizeInPoints);
            FontView.Size = PageTitle.Size;
        }
    }
}
