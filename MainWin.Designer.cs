using System.Drawing;
namespace WikiTexifier {
    partial class WikiTextifier {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.PageTitleLabel = new System.Windows.Forms.Label();
            this.PageTitle = new System.Windows.Forms.TextBox();
            this.FetchPageButton = new System.Windows.Forms.Button();
            this.PageText = new System.Windows.Forms.TextBox();
            this.SiteLabel = new System.Windows.Forms.Label();
            this.Site = new System.Windows.Forms.ComboBox();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.ASCIIfy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PageTitleLabel
            // 
            this.PageTitleLabel.AutoSize = true;
            this.PageTitleLabel.Location = new System.Drawing.Point(12, 9);
            this.PageTitleLabel.Name = "PageTitleLabel";
            this.PageTitleLabel.Size = new System.Drawing.Size(69, 15);
            this.PageTitleLabel.TabIndex = 0;
            this.PageTitleLabel.Text = "Article Title:";
            // 
            // PageTitle
            // 
            this.PageTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PageTitle.Location = new System.Drawing.Point(107, 6);
            this.PageTitle.Name = "PageTitle";
            this.PageTitle.Size = new System.Drawing.Size(424, 21);
            this.PageTitle.TabIndex = 1;
            // 
            // FetchPageButton
            // 
            this.FetchPageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FetchPageButton.Location = new System.Drawing.Point(537, 5);
            this.FetchPageButton.Name = "FetchPageButton";
            this.FetchPageButton.Size = new System.Drawing.Size(75, 23);
            this.FetchPageButton.TabIndex = 2;
            this.FetchPageButton.Text = "&Fetch";
            this.FetchPageButton.UseVisualStyleBackColor = true;
            this.FetchPageButton.Click += new System.EventHandler(this.Fetch);
            // 
            // PageText
            // 
            this.PageText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PageText.BackColor = System.Drawing.Color.Black;
            this.PageText.ForeColor = System.Drawing.Color.Lime;
            this.PageText.Location = new System.Drawing.Point(12, 61);
            this.PageText.Multiline = true;
            this.PageText.Name = "PageText";
            this.PageText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PageText.Size = new System.Drawing.Size(600, 339);
            this.PageText.TabIndex = 3;
            // 
            // SiteLabel
            // 
            this.SiteLabel.AutoSize = true;
            this.SiteLabel.Location = new System.Drawing.Point(12, 36);
            this.SiteLabel.Name = "SiteLabel";
            this.SiteLabel.Size = new System.Drawing.Size(33, 15);
            this.SiteLabel.TabIndex = 4;
            this.SiteLabel.Text = "Wiki:";
            // 
            // Site
            // 
            this.Site.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Site.FormattingEnabled = true;
            this.Site.Items.AddRange(new object[] {
            "http://en.wikipedia.org/w/index.php",
            "http://simple.wikipedia.org/w/index.php",
            "http://fr.wikipedia.org/w/index.php"});
            this.Site.Location = new System.Drawing.Point(107, 33);
            this.Site.Name = "Site";
            this.Site.Size = new System.Drawing.Size(424, 23);
            this.Site.TabIndex = 5;
            this.Site.Text = "http://en.wikipedia.org/w/index.php";
            // 
            // Progress
            // 
            this.Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Progress.Location = new System.Drawing.Point(12, 407);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(600, 23);
            this.Progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.Progress.TabIndex = 6;
            // 
            // ASCIIfy
            // 
            this.ASCIIfy.Location = new System.Drawing.Point(537, 32);
            this.ASCIIfy.Name = "ASCIIfy";
            this.ASCIIfy.Size = new System.Drawing.Size(75, 23);
            this.ASCIIfy.TabIndex = 7;
            this.ASCIIfy.Text = "&ASCIIfy";
            this.ASCIIfy.UseVisualStyleBackColor = true;
            this.ASCIIfy.Click += new System.EventHandler(this.ASCIIfy_Click);
            // 
            // WikiTextifier
            // 
            this.AcceptButton = this.FetchPageButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.ASCIIfy);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.Site);
            this.Controls.Add(this.SiteLabel);
            this.Controls.Add(this.PageText);
            this.Controls.Add(this.FetchPageButton);
            this.Controls.Add(this.PageTitle);
            this.Controls.Add(this.PageTitleLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name = "WikiTextifier";
            this.Text = "WikiTextifier";
            this.Load += new System.EventHandler(this.FormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PageTitleLabel;
        private System.Windows.Forms.TextBox PageTitle;
        private System.Windows.Forms.Button FetchPageButton;
        private System.Windows.Forms.TextBox PageText;
        private System.Windows.Forms.Label SiteLabel;
        private System.Windows.Forms.ComboBox Site;
        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.Button ASCIIfy;
    }
}

