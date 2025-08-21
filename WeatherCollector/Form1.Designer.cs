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
            downloadWeatherButton = new Button();
            progressBar = new ProgressBar();
            createDocButton = new Button();
            button3 = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            numberForecastDaysComboBox = new ComboBox();
            label5 = new Label();
            dataSourceGroupBox = new GroupBox();
            yrCheckBox = new CheckBox();
            ventuskyCheckBox = new CheckBox();
            gismeteoCheckBox = new CheckBox();
            gidroMCCheckBox = new CheckBox();
            dataSourceGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // downloadWeatherButton
            // 
            downloadWeatherButton.Location = new Point(98, 182);
            downloadWeatherButton.Name = "downloadWeatherButton";
            downloadWeatherButton.Size = new Size(256, 88);
            downloadWeatherButton.TabIndex = 0;
            downloadWeatherButton.Text = "Скачать";
            downloadWeatherButton.UseVisualStyleBackColor = true;
            downloadWeatherButton.Click += downloadWeatherButton_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(16, 303);
            progressBar.Maximum = 140;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(870, 29);
            progressBar.TabIndex = 1;
            // 
            // createDocButton
            // 
            createDocButton.Enabled = false;
            createDocButton.Location = new Point(384, 182);
            createDocButton.Name = "createDocButton";
            createDocButton.Size = new Size(256, 88);
            createDocButton.TabIndex = 2;
            createDocButton.Text = "Сформировать таблицу";
            createDocButton.UseVisualStyleBackColor = true;
            createDocButton.Click += createDocButton_Click;
            // 
            // button3
            // 
            button3.Location = new Point(767, 12);
            button3.Name = "button3";
            button3.Size = new Size(119, 29);
            button3.TabIndex = 3;
            button3.Text = "Информация";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(94, 20);
            label1.TabIndex = 4;
            label1.Text = "Инструкция:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 32);
            label2.Name = "label2";
            label2.Size = new Size(553, 20);
            label2.TabIndex = 5;
            label2.Text = "1. Нажмите кнопку \"Скачать\" для скачивания данных о погоде из источников.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(16, 52);
            label3.Name = "label3";
            label3.Size = new Size(754, 20);
            label3.TabIndex = 6;
            label3.Text = "2. Дождитесь скачивания. Как только данные скачаются, станет доступна кнопка \"Сформировать таблицу\".";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 72);
            label4.Name = "label4";
            label4.Size = new Size(712, 20);
            label4.TabIndex = 7;
            label4.Text = "3. Нажмите кнопку \"Сформировать таблицу\" для формирования Excel таблицы с данными о погоде.";
            // 
            // numberForecastDaysComboBox
            // 
            numberForecastDaysComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            numberForecastDaysComboBox.Location = new Point(416, 109);
            numberForecastDaysComboBox.Name = "numberForecastDaysComboBox";
            numberForecastDaysComboBox.Size = new Size(40, 28);
            numberForecastDaysComboBox.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(16, 112);
            label5.Name = "label5";
            label5.Size = new Size(394, 20);
            label5.TabIndex = 9;
            label5.Text = "Количество дней, на которое будет составлен прогноз.";
            label5.UseWaitCursor = true;
            // 
            // dataSourceGroupBox
            // 
            dataSourceGroupBox.Controls.Add(yrCheckBox);
            dataSourceGroupBox.Controls.Add(ventuskyCheckBox);
            dataSourceGroupBox.Controls.Add(gismeteoCheckBox);
            dataSourceGroupBox.Controls.Add(gidroMCCheckBox);
            dataSourceGroupBox.Location = new Point(716, 109);
            dataSourceGroupBox.Name = "dataSourceGroupBox";
            dataSourceGroupBox.Size = new Size(170, 161);
            dataSourceGroupBox.TabIndex = 10;
            dataSourceGroupBox.TabStop = false;
            dataSourceGroupBox.Text = "Источники данных";
            // 
            // yrCheckBox
            // 
            yrCheckBox.AutoSize = true;
            yrCheckBox.Checked = true;
            yrCheckBox.CheckState = CheckState.Checked;
            yrCheckBox.Location = new Point(13, 116);
            yrCheckBox.Name = "yrCheckBox";
            yrCheckBox.Size = new Size(43, 24);
            yrCheckBox.TabIndex = 3;
            yrCheckBox.Text = "Yr";
            yrCheckBox.UseVisualStyleBackColor = true;
            // 
            // ventuskyCheckBox
            // 
            ventuskyCheckBox.AutoSize = true;
            ventuskyCheckBox.Location = new Point(13, 86);
            ventuskyCheckBox.Name = "ventuskyCheckBox";
            ventuskyCheckBox.Size = new Size(88, 24);
            ventuskyCheckBox.TabIndex = 2;
            ventuskyCheckBox.Text = "Ventusky";
            ventuskyCheckBox.UseVisualStyleBackColor = true;
            // 
            // gismeteoCheckBox
            // 
            gismeteoCheckBox.AutoSize = true;
            gismeteoCheckBox.Checked = true;
            gismeteoCheckBox.CheckState = CheckState.Checked;
            gismeteoCheckBox.Location = new Point(13, 56);
            gismeteoCheckBox.Name = "gismeteoCheckBox";
            gismeteoCheckBox.Size = new Size(94, 24);
            gismeteoCheckBox.TabIndex = 1;
            gismeteoCheckBox.Text = "Gismeteo";
            gismeteoCheckBox.UseVisualStyleBackColor = true;
            // 
            // gidroMCCheckBox
            // 
            gidroMCCheckBox.AutoSize = true;
            gidroMCCheckBox.Checked = true;
            gidroMCCheckBox.CheckState = CheckState.Checked;
            gidroMCCheckBox.Location = new Point(13, 26);
            gidroMCCheckBox.Name = "gidroMCCheckBox";
            gidroMCCheckBox.Size = new Size(139, 24);
            gidroMCCheckBox.TabIndex = 0;
            gidroMCCheckBox.Text = "Гидрометцентр";
            gidroMCCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(898, 344);
            Controls.Add(dataSourceGroupBox);
            Controls.Add(label5);
            Controls.Add(numberForecastDaysComboBox);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button3);
            Controls.Add(createDocButton);
            Controls.Add(progressBar);
            Controls.Add(downloadWeatherButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Собиратель Погоды 2.3.0";
            dataSourceGroupBox.ResumeLayout(false);
            dataSourceGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Button downloadWeatherButton;
        private ProgressBar progressBar;
        private Button createDocButton;
        private Button button3;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ComboBox numberForecastDaysComboBox;
        private Label label5;
        private GroupBox dataSourceGroupBox;
        private CheckBox yrCheckBox;
        private CheckBox ventuskyCheckBox;
        private CheckBox gismeteoCheckBox;
        private CheckBox gidroMCCheckBox;
    }
}