namespace Data_Recorder
{
    partial class Form1
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
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.comPortTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connectionString = new System.Windows.Forms.Label();
            this.startRecordingDataButton = new System.Windows.Forms.Button();
            this.stopRecordingDataButton = new System.Windows.Forms.Button();
            this.phaseNameText = new System.Windows.Forms.Label();
            this.counterText = new System.Windows.Forms.Label();
            this.Data = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(298, 10);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(298, 39);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(75, 23);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // comPortTextBox
            // 
            this.comPortTextBox.Location = new System.Drawing.Point(192, 12);
            this.comPortTextBox.Name = "comPortTextBox";
            this.comPortTextBox.Size = new System.Drawing.Size(100, 20);
            this.comPortTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Please enter COM port (e.g. 1):";
            // 
            // connectionString
            // 
            this.connectionString.AutoSize = true;
            this.connectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectionString.Location = new System.Drawing.Point(438, 25);
            this.connectionString.Name = "connectionString";
            this.connectionString.Size = new System.Drawing.Size(179, 20);
            this.connectionString.TabIndex = 4;
            this.connectionString.Text = "Waiting for conenction...";
            // 
            // startRecordingDataButton
            // 
            this.startRecordingDataButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startRecordingDataButton.Location = new System.Drawing.Point(49, 123);
            this.startRecordingDataButton.Name = "startRecordingDataButton";
            this.startRecordingDataButton.Size = new System.Drawing.Size(155, 47);
            this.startRecordingDataButton.TabIndex = 7;
            this.startRecordingDataButton.Text = "Start Recording Data";
            this.startRecordingDataButton.UseVisualStyleBackColor = true;
            this.startRecordingDataButton.Click += new System.EventHandler(this.startRecordingDataButton_Click);
            // 
            // stopRecordingDataButton
            // 
            this.stopRecordingDataButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopRecordingDataButton.Location = new System.Drawing.Point(442, 123);
            this.stopRecordingDataButton.Name = "stopRecordingDataButton";
            this.stopRecordingDataButton.Size = new System.Drawing.Size(155, 47);
            this.stopRecordingDataButton.TabIndex = 8;
            this.stopRecordingDataButton.Text = "Stop Recording Data";
            this.stopRecordingDataButton.UseVisualStyleBackColor = true;
            this.stopRecordingDataButton.Click += new System.EventHandler(this.stopRecordingDataButton_Click);
            // 
            // phaseNameText
            // 
            this.phaseNameText.AutoSize = true;
            this.phaseNameText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phaseNameText.ForeColor = System.Drawing.Color.Maroon;
            this.phaseNameText.Location = new System.Drawing.Point(204, 269);
            this.phaseNameText.Name = "phaseNameText";
            this.phaseNameText.Size = new System.Drawing.Size(0, 24);
            this.phaseNameText.TabIndex = 9;
            this.phaseNameText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // counterText
            // 
            this.counterText.AutoSize = true;
            this.counterText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.counterText.ForeColor = System.Drawing.Color.Maroon;
            this.counterText.Location = new System.Drawing.Point(204, 307);
            this.counterText.Name = "counterText";
            this.counterText.Size = new System.Drawing.Size(0, 24);
            this.counterText.TabIndex = 10;
            this.counterText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Data
            // 
            this.Data.AutoSize = true;
            this.Data.Location = new System.Drawing.Point(290, 183);
            this.Data.Name = "Data";
            this.Data.Size = new System.Drawing.Size(0, 13);
            this.Data.TabIndex = 13;
            this.Data.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 432);
            this.Controls.Add(this.Data);
            this.Controls.Add(this.counterText);
            this.Controls.Add(this.phaseNameText);
            this.Controls.Add(this.stopRecordingDataButton);
            this.Controls.Add(this.startRecordingDataButton);
            this.Controls.Add(this.connectionString);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comPortTextBox);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.connectButton);
            this.Name = "Form1";
            this.Text = "Data Recorder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.TextBox comPortTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label connectionString;
        private System.Windows.Forms.Button startRecordingDataButton;
        private System.Windows.Forms.Button stopRecordingDataButton;
        private System.Windows.Forms.Label phaseNameText;
        private System.Windows.Forms.Label counterText;
        private System.Windows.Forms.Label Data;
    }
}

