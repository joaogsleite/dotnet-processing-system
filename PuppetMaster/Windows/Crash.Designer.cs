﻿namespace DADStorm {
    partial class Crash {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.op = new System.Windows.Forms.ComboBox();
            this.cancel = new System.Windows.Forms.Button();
            this.start = new System.Windows.Forms.Button();
            this.repl = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Select replica";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Select operator";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // op
            // 
            this.op.FormattingEnabled = true;
            this.op.Location = new System.Drawing.Point(11, 36);
            this.op.Name = "op";
            this.op.Size = new System.Drawing.Size(167, 21);
            this.op.TabIndex = 8;
            this.op.SelectedIndexChanged += new System.EventHandler(this.op_SelectedIndexChanged);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(103, 135);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(11, 135);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(75, 23);
            this.start.TabIndex = 6;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // repl
            // 
            this.repl.FormattingEnabled = true;
            this.repl.Location = new System.Drawing.Point(11, 92);
            this.repl.Name = "repl";
            this.repl.Size = new System.Drawing.Size(167, 21);
            this.repl.TabIndex = 11;
            this.repl.SelectedIndexChanged += new System.EventHandler(this.repl_SelectedIndexChanged);
            // 
            // Crash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 170);
            this.Controls.Add(this.repl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.op);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.start);
            this.Name = "Crash";
            this.Text = "Crash";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox op;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.ComboBox repl;
    }
}