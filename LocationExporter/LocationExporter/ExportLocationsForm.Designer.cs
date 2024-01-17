namespace LocationExporter
{
    partial class LocationExportDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Export_Button = new System.Windows.Forms.Button();
            this.Quit_button = new System.Windows.Forms.Button();
            this.FileChooser_Button = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_Regions = new System.Windows.Forms.ComboBox();
            this.label_OutputDirectory = new System.Windows.Forms.Label();
            this.Setup_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Export_Button
            // 
            this.Export_Button.Location = new System.Drawing.Point(131, 327);
            this.Export_Button.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Export_Button.Name = "Export_Button";
            this.Export_Button.Size = new System.Drawing.Size(363, 83);
            this.Export_Button.TabIndex = 0;
            this.Export_Button.Text = "Export";
            this.Export_Button.UseVisualStyleBackColor = true;
            this.Export_Button.Click += new System.EventHandler(this.Export_Button_Click);
            // 
            // Quit_button
            // 
            this.Quit_button.Location = new System.Drawing.Point(877, 327);
            this.Quit_button.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Quit_button.Name = "Quit_button";
            this.Quit_button.Size = new System.Drawing.Size(341, 83);
            this.Quit_button.TabIndex = 1;
            this.Quit_button.Text = "Quit";
            this.Quit_button.UseVisualStyleBackColor = true;
            this.Quit_button.Click += new System.EventHandler(this.Quit_button_Click);
            // 
            // FileChooser_Button
            // 
            this.FileChooser_Button.Location = new System.Drawing.Point(75, 29);
            this.FileChooser_Button.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.FileChooser_Button.Name = "FileChooser_Button";
            this.FileChooser_Button.Size = new System.Drawing.Size(611, 67);
            this.FileChooser_Button.TabIndex = 2;
            this.FileChooser_Button.Text = "Chose Output Directory";
            this.FileChooser_Button.UseVisualStyleBackColor = true;
            this.FileChooser_Button.Click += new System.EventHandler(this.FileChooser_Button_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(101, 188);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(127, 32);
            this.progressLabel.TabIndex = 4;
            this.progressLabel.Text = "Progress";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(104, 253);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1056, 41);
            this.progressBar1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(752, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 32);
            this.label1.TabIndex = 6;
            this.label1.Text = "Region";
            // 
            // comboBox_Regions
            // 
            this.comboBox_Regions.FormattingEnabled = true;
            this.comboBox_Regions.Location = new System.Drawing.Point(877, 36);
            this.comboBox_Regions.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.comboBox_Regions.Name = "comboBox_Regions";
            this.comboBox_Regions.Size = new System.Drawing.Size(316, 39);
            this.comboBox_Regions.TabIndex = 7;
            this.comboBox_Regions.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label_OutputDirectory
            // 
            this.label_OutputDirectory.AutoSize = true;
            this.label_OutputDirectory.Location = new System.Drawing.Point(96, 117);
            this.label_OutputDirectory.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label_OutputDirectory.Name = "label_OutputDirectory";
            this.label_OutputDirectory.Size = new System.Drawing.Size(228, 32);
            this.label_OutputDirectory.TabIndex = 8;
            this.label_OutputDirectory.Text = "Output Directory:";
            // 
            // Setup_button
            // 
            this.Setup_button.Location = new System.Drawing.Point(549, 327);
            this.Setup_button.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Setup_button.Name = "Setup_button";
            this.Setup_button.Size = new System.Drawing.Size(275, 83);
            this.Setup_button.TabIndex = 9;
            this.Setup_button.Text = "Setup";
            this.Setup_button.UseVisualStyleBackColor = true;
            this.Setup_button.Click += new System.EventHandler(this.Setup_button_Click);
            // 
            // LocationExportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1293, 441);
            this.Controls.Add(this.Setup_button);
            this.Controls.Add(this.label_OutputDirectory);
            this.Controls.Add(this.comboBox_Regions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.FileChooser_Button);
            this.Controls.Add(this.Quit_button);
            this.Controls.Add(this.Export_Button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "LocationExportDialog";
            this.Text = "Breakthru Beverage Location Export Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Export_Button;
        private System.Windows.Forms.Button Quit_button;
        private System.Windows.Forms.Button FileChooser_Button;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_Regions;
        private System.Windows.Forms.Label label_OutputDirectory;
        private System.Windows.Forms.Button Setup_button;
    }
}

