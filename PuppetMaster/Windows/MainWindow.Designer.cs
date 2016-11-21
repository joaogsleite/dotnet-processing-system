namespace DADStorm {
    partial class MainWindow {
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
            this.exit = new System.Windows.Forms.Button();
            this.logs = new System.Windows.Forms.TextBox();
            this.start_op = new System.Windows.Forms.Button();
            this.interval = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Button();
            this.crash = new System.Windows.Forms.Button();
            this.freeze = new System.Windows.Forms.Button();
            this.unfreeze = new System.Windows.Forms.Button();
            this.load_config = new System.Windows.Forms.Button();
            this.load_script = new System.Windows.Forms.Button();
            this.Intro = new System.Windows.Forms.Label();
            this.open_file = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // exit
            // 
            this.exit.Location = new System.Drawing.Point(215, 475);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(75, 23);
            this.exit.TabIndex = 0;
            this.exit.Text = "Exit";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // logs
            // 
            this.logs.CausesValidation = false;
            this.logs.Location = new System.Drawing.Point(12, 127);
            this.logs.MaxLength = 0;
            this.logs.Multiline = true;
            this.logs.Name = "logs";
            this.logs.ReadOnly = true;
            this.logs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logs.Size = new System.Drawing.Size(480, 285);
            this.logs.TabIndex = 1;
            // 
            // start_op
            // 
            this.start_op.Location = new System.Drawing.Point(12, 418);
            this.start_op.Name = "start_op";
            this.start_op.Size = new System.Drawing.Size(75, 23);
            this.start_op.TabIndex = 2;
            this.start_op.Text = "Start OP";
            this.start_op.UseVisualStyleBackColor = true;
            this.start_op.Click += new System.EventHandler(this.start_op_Click);
            // 
            // interval
            // 
            this.interval.Location = new System.Drawing.Point(93, 418);
            this.interval.Name = "interval";
            this.interval.Size = new System.Drawing.Size(75, 23);
            this.interval.TabIndex = 3;
            this.interval.Text = "Interval";
            this.interval.UseVisualStyleBackColor = true;
            this.interval.Click += new System.EventHandler(this.interval_Click);
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(174, 418);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(75, 23);
            this.status.TabIndex = 4;
            this.status.Text = "Status";
            this.status.UseVisualStyleBackColor = true;
            this.status.Click += new System.EventHandler(this.status_Click);
            // 
            // crash
            // 
            this.crash.Location = new System.Drawing.Point(255, 418);
            this.crash.Name = "crash";
            this.crash.Size = new System.Drawing.Size(75, 23);
            this.crash.TabIndex = 5;
            this.crash.Text = "Crash";
            this.crash.UseVisualStyleBackColor = true;
            this.crash.Click += new System.EventHandler(this.crash_Click);
            // 
            // freeze
            // 
            this.freeze.Location = new System.Drawing.Point(336, 418);
            this.freeze.Name = "freeze";
            this.freeze.Size = new System.Drawing.Size(75, 23);
            this.freeze.TabIndex = 6;
            this.freeze.Text = "Freeze";
            this.freeze.UseVisualStyleBackColor = true;
            this.freeze.Click += new System.EventHandler(this.freeze_Click);
            // 
            // unfreeze
            // 
            this.unfreeze.Location = new System.Drawing.Point(417, 418);
            this.unfreeze.Name = "unfreeze";
            this.unfreeze.Size = new System.Drawing.Size(75, 23);
            this.unfreeze.TabIndex = 7;
            this.unfreeze.Text = "Unfreeze";
            this.unfreeze.UseVisualStyleBackColor = true;
            this.unfreeze.Click += new System.EventHandler(this.unfreeze_Click);
            // 
            // load_config
            // 
            this.load_config.Location = new System.Drawing.Point(93, 84);
            this.load_config.Name = "load_config";
            this.load_config.Size = new System.Drawing.Size(120, 30);
            this.load_config.TabIndex = 8;
            this.load_config.Text = "Load config file";
            this.load_config.UseVisualStyleBackColor = true;
            this.load_config.Click += new System.EventHandler(this.load_config_Click);
            // 
            // load_script
            // 
            this.load_script.Location = new System.Drawing.Point(298, 84);
            this.load_script.Name = "load_script";
            this.load_script.Size = new System.Drawing.Size(120, 30);
            this.load_script.TabIndex = 9;
            this.load_script.Text = "Load script file";
            this.load_script.UseVisualStyleBackColor = true;
            this.load_script.Click += new System.EventHandler(this.load_script_Click);
            // 
            // Intro
            // 
            this.Intro.AutoSize = true;
            this.Intro.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Intro.Location = new System.Drawing.Point(12, 21);
            this.Intro.Name = "Intro";
            this.Intro.Size = new System.Drawing.Size(440, 34);
            this.Intro.TabIndex = 10;
            this.Intro.Text = "Load a configuration file to start replicas. \r\nThen, you can load script files to" +
                " give commands to created replicas.";
            // 
            // open_file
            // 
            this.open_file.FileName = "dadstorm.config";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 510);
            this.Controls.Add(this.Intro);
            this.Controls.Add(this.load_script);
            this.Controls.Add(this.load_config);
            this.Controls.Add(this.unfreeze);
            this.Controls.Add(this.freeze);
            this.Controls.Add(this.crash);
            this.Controls.Add(this.status);
            this.Controls.Add(this.interval);
            this.Controls.Add(this.start_op);
            this.Controls.Add(this.logs);
            this.Controls.Add(this.exit);
            this.Name = "MainWindow";
            this.Text = "PuppetMaster";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button exit;
        private System.Windows.Forms.TextBox logs;
        private System.Windows.Forms.Button start_op;
        private System.Windows.Forms.Button interval;
        private System.Windows.Forms.Button status;
        private System.Windows.Forms.Button crash;
        private System.Windows.Forms.Button freeze;
        private System.Windows.Forms.Button unfreeze;
        private System.Windows.Forms.Button load_config;
        private System.Windows.Forms.Button load_script;
        private System.Windows.Forms.Label Intro;
        private System.Windows.Forms.OpenFileDialog open_file;
    }
}

