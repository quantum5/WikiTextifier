using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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

        private void FormLoad(object sender, EventArgs e) {
            Font monospace;
            try {
                monospace = new Font(new FontFamily("Consolas"), 11);
            } catch (ArgumentException) {
                monospace = new Font(FontFamily.GenericMonospace, 11);
            }
            PageText.Font = monospace;

            this.Font = new Font(FontFamily.GenericSansSerif, 9);

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
    }
}
