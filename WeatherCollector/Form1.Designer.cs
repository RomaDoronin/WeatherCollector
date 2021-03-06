namespace WeatherCollector
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numberForecastDaysComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(199, 182);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(256, 88);
            this.button1.TabIndex = 0;
            this.button1.Text = "Скачать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 303);
            this.progressBar.Maximum = 140;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(870, 29);
            this.progressBar.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(461, 182);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(256, 88);
            this.button2.TabIndex = 2;
            this.button2.Text = "Сформировать таблицу";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(767, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(119, 29);
            this.button3.TabIndex = 3;
            this.button3.Text = "Информация";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Инструкция:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(520, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "1. Нажмите кнопку \"Скачать\" для скачивания данных о погоде с сервера.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(754, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "2. Дождитесь скачивания. Как только данные скачаются, станет доступна кнопка \"Сфо" +
    "рмировать таблицу\".";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(712, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "3. Нажмите кнопку \"Сформировать таблицу\" для формирования Excel таблицы с данными" +
    " о погоде.";
            // 
            // numberForecastDaysComboBox
            // 
            this.numberForecastDaysComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.numberForecastDaysComboBox.Location = new System.Drawing.Point(416, 109);
            this.numberForecastDaysComboBox.Name = "numberForecastDaysComboBox";
            this.numberForecastDaysComboBox.Size = new System.Drawing.Size(40, 28);
            this.numberForecastDaysComboBox.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(394, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Количество дней, на которое будет составлен прогноз.";
            this.label5.UseWaitCursor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 344);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numberForecastDaysComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Собиратель Погоды 1.1.1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button1;
        private ProgressBar progressBar;
        private Button button2;
        private Button button3;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ComboBox numberForecastDaysComboBox;
        private Label label5;
    }
}