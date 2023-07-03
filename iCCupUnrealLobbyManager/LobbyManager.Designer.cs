namespace UnrealLobbyManager
{
    partial class LobbyManager
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ActivateOtherInfo = new System.Windows.Forms.CheckBox();
            this.CacheCleaner = new System.Windows.Forms.Timer(this.components);
            this.FireMode = new System.Windows.Forms.CheckBox();
            this.CommandLineText = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.CheckForKick = new System.Windows.Forms.Timer(this.components);
            this.name_lab1 = new System.Windows.Forms.Label();
            this.mainInf_lab1 = new System.Windows.Forms.Label();
            this.other_lab1 = new System.Windows.Forms.Label();
            this.ban_btn1 = new System.Windows.Forms.Button();
            this.name_lab2 = new System.Windows.Forms.Label();
            this.mainInf_lab2 = new System.Windows.Forms.Label();
            this.other_lab2 = new System.Windows.Forms.Label();
            this.ban_btn2 = new System.Windows.Forms.Button();
            this.name_lab3 = new System.Windows.Forms.Label();
            this.mainInf_lab3 = new System.Windows.Forms.Label();
            this.other_lab3 = new System.Windows.Forms.Label();
            this.ban_btn3 = new System.Windows.Forms.Button();
            this.name_lab4 = new System.Windows.Forms.Label();
            this.mainInf_lab4 = new System.Windows.Forms.Label();
            this.other_lab4 = new System.Windows.Forms.Label();
            this.ban_btn4 = new System.Windows.Forms.Button();
            this.name_lab5 = new System.Windows.Forms.Label();
            this.mainInf_lab5 = new System.Windows.Forms.Label();
            this.other_lab5 = new System.Windows.Forms.Label();
            this.ban_btn5 = new System.Windows.Forms.Button();
            this.name_lab6 = new System.Windows.Forms.Label();
            this.mainInf_lab6 = new System.Windows.Forms.Label();
            this.other_lab6 = new System.Windows.Forms.Label();
            this.ban_btn6 = new System.Windows.Forms.Button();
            this.name_lab7 = new System.Windows.Forms.Label();
            this.mainInf_lab7 = new System.Windows.Forms.Label();
            this.other_lab7 = new System.Windows.Forms.Label();
            this.ban_btn7 = new System.Windows.Forms.Button();
            this.name_lab8 = new System.Windows.Forms.Label();
            this.mainInf_lab8 = new System.Windows.Forms.Label();
            this.other_lab8 = new System.Windows.Forms.Label();
            this.ban_btn8 = new System.Windows.Forms.Button();
            this.name_lab9 = new System.Windows.Forms.Label();
            this.mainInf_lab9 = new System.Windows.Forms.Label();
            this.other_lab9 = new System.Windows.Forms.Label();
            this.ban_btn9 = new System.Windows.Forms.Button();
            this.name_lab10 = new System.Windows.Forms.Label();
            this.mainInf_lab10 = new System.Windows.Forms.Label();
            this.other_lab10 = new System.Windows.Forms.Label();
            this.ban_btn10 = new System.Windows.Forms.Button();
            this.name_lab11 = new System.Windows.Forms.Label();
            this.mainInf_lab11 = new System.Windows.Forms.Label();
            this.other_lab11 = new System.Windows.Forms.Label();
            this.ban_btn11 = new System.Windows.Forms.Button();
            this.name_lab12 = new System.Windows.Forms.Label();
            this.mainInf_lab12 = new System.Windows.Forms.Label();
            this.other_lab12 = new System.Windows.Forms.Label();
            this.ban_btn12 = new System.Windows.Forms.Button();
            this.stopUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ActivateOtherInfo
            // 
            this.ActivateOtherInfo.AutoSize = true;
            this.ActivateOtherInfo.Checked = true;
            this.ActivateOtherInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ActivateOtherInfo.Location = new System.Drawing.Point(417, 350);
            this.ActivateOtherInfo.Name = "ActivateOtherInfo";
            this.ActivateOtherInfo.Size = new System.Drawing.Size(302, 17);
            this.ActivateOtherInfo.TabIndex = 14;
            this.ActivateOtherInfo.Text = "Получать дополнительную инфу (флаг, сезоны, скайп)";
            this.ActivateOtherInfo.UseVisualStyleBackColor = true;
            this.ActivateOtherInfo.Visible = false;
            // 
            // CacheCleaner
            // 
            this.CacheCleaner.Enabled = true;
            this.CacheCleaner.Interval = 3000000;
            this.CacheCleaner.Tick += new System.EventHandler(this.CacheCleaner_Tick);
            // 
            // FireMode
            // 
            this.FireMode.AutoSize = true;
            this.FireMode.Checked = true;
            this.FireMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FireMode.Location = new System.Drawing.Point(310, 350);
            this.FireMode.Name = "FireMode";
            this.FireMode.Size = new System.Drawing.Size(66, 17);
            this.FireMode.TabIndex = 37;
            this.FireMode.Text = "FireStyle";
            this.FireMode.UseVisualStyleBackColor = true;
            // 
            // CommandLineText
            // 
            this.CommandLineText.Location = new System.Drawing.Point(170, 378);
            this.CommandLineText.Name = "CommandLineText";
            this.CommandLineText.Size = new System.Drawing.Size(322, 20);
            this.CommandLineText.TabIndex = 36;
            this.CommandLineText.TextChanged += new System.EventHandler(this.CommandLineText_TextChanged);
            this.CommandLineText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CommandLineText_KeyUp);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(69, 381);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(84, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "Ввод команды:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "/ban username reason",
            "/unban username",
            "/printstats",
            "/exit"});
            this.comboBox1.Location = new System.Drawing.Point(530, 377);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(189, 21);
            this.comboBox1.TabIndex = 38;
            this.comboBox1.Text = "Список команд:";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // CheckForKick
            // 
            this.CheckForKick.Enabled = true;
            this.CheckForKick.Interval = 500;
            this.CheckForKick.Tick += new System.EventHandler(this.CheckForKick_Tick);
            // 
            // name_lab1
            // 
            this.name_lab1.AutoSize = true;
            this.name_lab1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab1.Location = new System.Drawing.Point(22, 23);
            this.name_lab1.Name = "name_lab1";
            this.name_lab1.Size = new System.Drawing.Size(207, 15);
            this.name_lab1.TabIndex = 0;
            this.name_lab1.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab1
            // 
            this.mainInf_lab1.AutoSize = true;
            this.mainInf_lab1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab1.Location = new System.Drawing.Point(325, 23);
            this.mainInf_lab1.Name = "mainInf_lab1";
            this.mainInf_lab1.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab1.TabIndex = 0;
            this.mainInf_lab1.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab1
            // 
            this.other_lab1.AutoSize = true;
            this.other_lab1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab1.Location = new System.Drawing.Point(609, 23);
            this.other_lab1.Name = "other_lab1";
            this.other_lab1.Size = new System.Drawing.Size(123, 15);
            this.other_lab1.TabIndex = 0;
            this.other_lab1.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn1
            // 
            this.ban_btn1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn1.Location = new System.Drawing.Point(237, 18);
            this.ban_btn1.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn1.Name = "ban_btn1";
            this.ban_btn1.Size = new System.Drawing.Size(75, 21);
            this.ban_btn1.TabIndex = 39;
            this.ban_btn1.Text = "BAN";
            this.ban_btn1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn1.UseVisualStyleBackColor = true;
            this.ban_btn1.Click += new System.EventHandler(this.ban_btn1_Click);
            // 
            // name_lab2
            // 
            this.name_lab2.AutoSize = true;
            this.name_lab2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab2.Location = new System.Drawing.Point(22, 50);
            this.name_lab2.Name = "name_lab2";
            this.name_lab2.Size = new System.Drawing.Size(207, 15);
            this.name_lab2.TabIndex = 0;
            this.name_lab2.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab2
            // 
            this.mainInf_lab2.AutoSize = true;
            this.mainInf_lab2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab2.Location = new System.Drawing.Point(325, 50);
            this.mainInf_lab2.Name = "mainInf_lab2";
            this.mainInf_lab2.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab2.TabIndex = 0;
            this.mainInf_lab2.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab2
            // 
            this.other_lab2.AutoSize = true;
            this.other_lab2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab2.Location = new System.Drawing.Point(609, 50);
            this.other_lab2.Name = "other_lab2";
            this.other_lab2.Size = new System.Drawing.Size(123, 15);
            this.other_lab2.TabIndex = 0;
            this.other_lab2.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn2
            // 
            this.ban_btn2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn2.Location = new System.Drawing.Point(237, 45);
            this.ban_btn2.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn2.Name = "ban_btn2";
            this.ban_btn2.Size = new System.Drawing.Size(75, 21);
            this.ban_btn2.TabIndex = 39;
            this.ban_btn2.Text = "BAN";
            this.ban_btn2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn2.UseVisualStyleBackColor = true;
            this.ban_btn2.Click += new System.EventHandler(this.ban_btn2_Click);
            // 
            // name_lab3
            // 
            this.name_lab3.AutoSize = true;
            this.name_lab3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab3.Location = new System.Drawing.Point(22, 75);
            this.name_lab3.Name = "name_lab3";
            this.name_lab3.Size = new System.Drawing.Size(207, 15);
            this.name_lab3.TabIndex = 0;
            this.name_lab3.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab3
            // 
            this.mainInf_lab3.AutoSize = true;
            this.mainInf_lab3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab3.Location = new System.Drawing.Point(325, 75);
            this.mainInf_lab3.Name = "mainInf_lab3";
            this.mainInf_lab3.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab3.TabIndex = 0;
            this.mainInf_lab3.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab3
            // 
            this.other_lab3.AutoSize = true;
            this.other_lab3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab3.Location = new System.Drawing.Point(609, 75);
            this.other_lab3.Name = "other_lab3";
            this.other_lab3.Size = new System.Drawing.Size(123, 15);
            this.other_lab3.TabIndex = 0;
            this.other_lab3.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn3
            // 
            this.ban_btn3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn3.Location = new System.Drawing.Point(237, 70);
            this.ban_btn3.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn3.Name = "ban_btn3";
            this.ban_btn3.Size = new System.Drawing.Size(75, 21);
            this.ban_btn3.TabIndex = 39;
            this.ban_btn3.Text = "BAN";
            this.ban_btn3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn3.UseVisualStyleBackColor = true;
            this.ban_btn3.Click += new System.EventHandler(this.ban_btn3_Click);
            // 
            // name_lab4
            // 
            this.name_lab4.AutoSize = true;
            this.name_lab4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab4.Location = new System.Drawing.Point(22, 101);
            this.name_lab4.Name = "name_lab4";
            this.name_lab4.Size = new System.Drawing.Size(207, 15);
            this.name_lab4.TabIndex = 0;
            this.name_lab4.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab4
            // 
            this.mainInf_lab4.AutoSize = true;
            this.mainInf_lab4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab4.Location = new System.Drawing.Point(325, 101);
            this.mainInf_lab4.Name = "mainInf_lab4";
            this.mainInf_lab4.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab4.TabIndex = 0;
            this.mainInf_lab4.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab4
            // 
            this.other_lab4.AutoSize = true;
            this.other_lab4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab4.Location = new System.Drawing.Point(609, 101);
            this.other_lab4.Name = "other_lab4";
            this.other_lab4.Size = new System.Drawing.Size(123, 15);
            this.other_lab4.TabIndex = 0;
            this.other_lab4.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn4
            // 
            this.ban_btn4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn4.Location = new System.Drawing.Point(237, 96);
            this.ban_btn4.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn4.Name = "ban_btn4";
            this.ban_btn4.Size = new System.Drawing.Size(75, 21);
            this.ban_btn4.TabIndex = 39;
            this.ban_btn4.Text = "BAN";
            this.ban_btn4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn4.UseVisualStyleBackColor = true;
            this.ban_btn4.Click += new System.EventHandler(this.ban_btn4_Click);
            // 
            // name_lab5
            // 
            this.name_lab5.AutoSize = true;
            this.name_lab5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab5.Location = new System.Drawing.Point(22, 127);
            this.name_lab5.Name = "name_lab5";
            this.name_lab5.Size = new System.Drawing.Size(207, 15);
            this.name_lab5.TabIndex = 0;
            this.name_lab5.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab5
            // 
            this.mainInf_lab5.AutoSize = true;
            this.mainInf_lab5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab5.Location = new System.Drawing.Point(325, 127);
            this.mainInf_lab5.Name = "mainInf_lab5";
            this.mainInf_lab5.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab5.TabIndex = 0;
            this.mainInf_lab5.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab5
            // 
            this.other_lab5.AutoSize = true;
            this.other_lab5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab5.Location = new System.Drawing.Point(609, 127);
            this.other_lab5.Name = "other_lab5";
            this.other_lab5.Size = new System.Drawing.Size(123, 15);
            this.other_lab5.TabIndex = 0;
            this.other_lab5.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn5
            // 
            this.ban_btn5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn5.Location = new System.Drawing.Point(237, 122);
            this.ban_btn5.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn5.Name = "ban_btn5";
            this.ban_btn5.Size = new System.Drawing.Size(75, 21);
            this.ban_btn5.TabIndex = 39;
            this.ban_btn5.Text = "BAN";
            this.ban_btn5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn5.UseVisualStyleBackColor = true;
            this.ban_btn5.Click += new System.EventHandler(this.ban_btn5_Click);
            // 
            // name_lab6
            // 
            this.name_lab6.AutoSize = true;
            this.name_lab6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab6.Location = new System.Drawing.Point(22, 153);
            this.name_lab6.Name = "name_lab6";
            this.name_lab6.Size = new System.Drawing.Size(207, 15);
            this.name_lab6.TabIndex = 0;
            this.name_lab6.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab6
            // 
            this.mainInf_lab6.AutoSize = true;
            this.mainInf_lab6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab6.Location = new System.Drawing.Point(325, 153);
            this.mainInf_lab6.Name = "mainInf_lab6";
            this.mainInf_lab6.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab6.TabIndex = 0;
            this.mainInf_lab6.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab6
            // 
            this.other_lab6.AutoSize = true;
            this.other_lab6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab6.Location = new System.Drawing.Point(609, 153);
            this.other_lab6.Name = "other_lab6";
            this.other_lab6.Size = new System.Drawing.Size(123, 15);
            this.other_lab6.TabIndex = 0;
            this.other_lab6.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn6
            // 
            this.ban_btn6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn6.Location = new System.Drawing.Point(237, 148);
            this.ban_btn6.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn6.Name = "ban_btn6";
            this.ban_btn6.Size = new System.Drawing.Size(75, 21);
            this.ban_btn6.TabIndex = 39;
            this.ban_btn6.Text = "BAN";
            this.ban_btn6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn6.UseVisualStyleBackColor = true;
            this.ban_btn6.Click += new System.EventHandler(this.ban_btn6_Click);
            // 
            // name_lab7
            // 
            this.name_lab7.AutoSize = true;
            this.name_lab7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab7.Location = new System.Drawing.Point(22, 180);
            this.name_lab7.Name = "name_lab7";
            this.name_lab7.Size = new System.Drawing.Size(207, 15);
            this.name_lab7.TabIndex = 0;
            this.name_lab7.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab7
            // 
            this.mainInf_lab7.AutoSize = true;
            this.mainInf_lab7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab7.Location = new System.Drawing.Point(325, 180);
            this.mainInf_lab7.Name = "mainInf_lab7";
            this.mainInf_lab7.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab7.TabIndex = 0;
            this.mainInf_lab7.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab7
            // 
            this.other_lab7.AutoSize = true;
            this.other_lab7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab7.Location = new System.Drawing.Point(609, 180);
            this.other_lab7.Name = "other_lab7";
            this.other_lab7.Size = new System.Drawing.Size(123, 15);
            this.other_lab7.TabIndex = 0;
            this.other_lab7.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn7
            // 
            this.ban_btn7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn7.Location = new System.Drawing.Point(237, 175);
            this.ban_btn7.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn7.Name = "ban_btn7";
            this.ban_btn7.Size = new System.Drawing.Size(75, 21);
            this.ban_btn7.TabIndex = 39;
            this.ban_btn7.Text = "BAN";
            this.ban_btn7.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn7.UseVisualStyleBackColor = true;
            this.ban_btn7.Click += new System.EventHandler(this.ban_btn7_Click);
            // 
            // name_lab8
            // 
            this.name_lab8.AutoSize = true;
            this.name_lab8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab8.Location = new System.Drawing.Point(22, 208);
            this.name_lab8.Name = "name_lab8";
            this.name_lab8.Size = new System.Drawing.Size(207, 15);
            this.name_lab8.TabIndex = 0;
            this.name_lab8.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab8
            // 
            this.mainInf_lab8.AutoSize = true;
            this.mainInf_lab8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab8.Location = new System.Drawing.Point(325, 208);
            this.mainInf_lab8.Name = "mainInf_lab8";
            this.mainInf_lab8.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab8.TabIndex = 0;
            this.mainInf_lab8.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab8
            // 
            this.other_lab8.AutoSize = true;
            this.other_lab8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab8.Location = new System.Drawing.Point(609, 208);
            this.other_lab8.Name = "other_lab8";
            this.other_lab8.Size = new System.Drawing.Size(123, 15);
            this.other_lab8.TabIndex = 0;
            this.other_lab8.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn8
            // 
            this.ban_btn8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn8.Location = new System.Drawing.Point(237, 203);
            this.ban_btn8.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn8.Name = "ban_btn8";
            this.ban_btn8.Size = new System.Drawing.Size(75, 21);
            this.ban_btn8.TabIndex = 39;
            this.ban_btn8.Text = "BAN";
            this.ban_btn8.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn8.UseVisualStyleBackColor = true;
            this.ban_btn8.Click += new System.EventHandler(this.ban_btn8_Click);
            // 
            // name_lab9
            // 
            this.name_lab9.AutoSize = true;
            this.name_lab9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab9.Location = new System.Drawing.Point(22, 235);
            this.name_lab9.Name = "name_lab9";
            this.name_lab9.Size = new System.Drawing.Size(207, 15);
            this.name_lab9.TabIndex = 0;
            this.name_lab9.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab9
            // 
            this.mainInf_lab9.AutoSize = true;
            this.mainInf_lab9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab9.Location = new System.Drawing.Point(325, 235);
            this.mainInf_lab9.Name = "mainInf_lab9";
            this.mainInf_lab9.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab9.TabIndex = 0;
            this.mainInf_lab9.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab9
            // 
            this.other_lab9.AutoSize = true;
            this.other_lab9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab9.Location = new System.Drawing.Point(609, 235);
            this.other_lab9.Name = "other_lab9";
            this.other_lab9.Size = new System.Drawing.Size(123, 15);
            this.other_lab9.TabIndex = 0;
            this.other_lab9.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn9
            // 
            this.ban_btn9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn9.Location = new System.Drawing.Point(237, 230);
            this.ban_btn9.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn9.Name = "ban_btn9";
            this.ban_btn9.Size = new System.Drawing.Size(75, 21);
            this.ban_btn9.TabIndex = 39;
            this.ban_btn9.Text = "BAN";
            this.ban_btn9.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn9.UseVisualStyleBackColor = true;
            this.ban_btn9.Click += new System.EventHandler(this.ban_btn9_Click);
            // 
            // name_lab10
            // 
            this.name_lab10.AutoSize = true;
            this.name_lab10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab10.Location = new System.Drawing.Point(22, 263);
            this.name_lab10.Name = "name_lab10";
            this.name_lab10.Size = new System.Drawing.Size(207, 15);
            this.name_lab10.TabIndex = 0;
            this.name_lab10.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab10
            // 
            this.mainInf_lab10.AutoSize = true;
            this.mainInf_lab10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab10.Location = new System.Drawing.Point(325, 263);
            this.mainInf_lab10.Name = "mainInf_lab10";
            this.mainInf_lab10.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab10.TabIndex = 0;
            this.mainInf_lab10.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab10
            // 
            this.other_lab10.AutoSize = true;
            this.other_lab10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab10.Location = new System.Drawing.Point(609, 263);
            this.other_lab10.Name = "other_lab10";
            this.other_lab10.Size = new System.Drawing.Size(123, 15);
            this.other_lab10.TabIndex = 0;
            this.other_lab10.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn10
            // 
            this.ban_btn10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn10.Location = new System.Drawing.Point(237, 258);
            this.ban_btn10.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn10.Name = "ban_btn10";
            this.ban_btn10.Size = new System.Drawing.Size(75, 21);
            this.ban_btn10.TabIndex = 39;
            this.ban_btn10.Text = "BAN";
            this.ban_btn10.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn10.UseVisualStyleBackColor = true;
            this.ban_btn10.Click += new System.EventHandler(this.ban_btn10_Click);
            // 
            // name_lab11
            // 
            this.name_lab11.AutoSize = true;
            this.name_lab11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab11.Location = new System.Drawing.Point(22, 291);
            this.name_lab11.Name = "name_lab11";
            this.name_lab11.Size = new System.Drawing.Size(207, 15);
            this.name_lab11.TabIndex = 0;
            this.name_lab11.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab11
            // 
            this.mainInf_lab11.AutoSize = true;
            this.mainInf_lab11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab11.Location = new System.Drawing.Point(325, 291);
            this.mainInf_lab11.Name = "mainInf_lab11";
            this.mainInf_lab11.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab11.TabIndex = 0;
            this.mainInf_lab11.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab11
            // 
            this.other_lab11.AutoSize = true;
            this.other_lab11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab11.Location = new System.Drawing.Point(609, 291);
            this.other_lab11.Name = "other_lab11";
            this.other_lab11.Size = new System.Drawing.Size(123, 15);
            this.other_lab11.TabIndex = 0;
            this.other_lab11.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn11
            // 
            this.ban_btn11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn11.Location = new System.Drawing.Point(237, 286);
            this.ban_btn11.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn11.Name = "ban_btn11";
            this.ban_btn11.Size = new System.Drawing.Size(75, 21);
            this.ban_btn11.TabIndex = 39;
            this.ban_btn11.Text = "BAN";
            this.ban_btn11.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn11.UseVisualStyleBackColor = true;
            this.ban_btn11.Click += new System.EventHandler(this.ban_btn11_Click);
            // 
            // name_lab12
            // 
            this.name_lab12.AutoSize = true;
            this.name_lab12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.name_lab12.Location = new System.Drawing.Point(22, 319);
            this.name_lab12.Name = "name_lab12";
            this.name_lab12.Size = new System.Drawing.Size(207, 15);
            this.name_lab12.TabIndex = 0;
            this.name_lab12.Text = "WWWWWWWWWWWWWWWWWW";
            // 
            // mainInf_lab12
            // 
            this.mainInf_lab12.AutoSize = true;
            this.mainInf_lab12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainInf_lab12.Location = new System.Drawing.Point(325, 319);
            this.mainInf_lab12.Name = "mainInf_lab12";
            this.mainInf_lab12.Size = new System.Drawing.Size(269, 15);
            this.mainInf_lab12.TabIndex = 0;
            this.mainInf_lab12.Text = "MAININFOMAININFOMAININFOMAININFOMAININFO";
            // 
            // other_lab12
            // 
            this.other_lab12.AutoSize = true;
            this.other_lab12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.other_lab12.Location = new System.Drawing.Point(609, 319);
            this.other_lab12.Name = "other_lab12";
            this.other_lab12.Size = new System.Drawing.Size(123, 15);
            this.other_lab12.TabIndex = 0;
            this.other_lab12.Text = "OTHEROTHEROTHER";
            // 
            // ban_btn12
            // 
            this.ban_btn12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ban_btn12.Location = new System.Drawing.Point(237, 314);
            this.ban_btn12.Margin = new System.Windows.Forms.Padding(1);
            this.ban_btn12.Name = "ban_btn12";
            this.ban_btn12.Size = new System.Drawing.Size(75, 21);
            this.ban_btn12.TabIndex = 39;
            this.ban_btn12.Text = "BAN";
            this.ban_btn12.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ban_btn12.UseVisualStyleBackColor = true;
            this.ban_btn12.Click += new System.EventHandler(this.ban_btn12_Click);
            // 
            // stopUpdateCheckBox
            // 
            this.stopUpdateCheckBox.AutoSize = true;
            this.stopUpdateCheckBox.Location = new System.Drawing.Point(170, 350);
            this.stopUpdateCheckBox.Name = "stopUpdateCheckBox";
            this.stopUpdateCheckBox.Size = new System.Drawing.Size(107, 17);
            this.stopUpdateCheckBox.TabIndex = 37;
            this.stopUpdateCheckBox.Text = "Stop form update";
            this.stopUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // LobbyManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 412);
            this.Controls.Add(this.ban_btn12);
            this.Controls.Add(this.ban_btn11);
            this.Controls.Add(this.ban_btn10);
            this.Controls.Add(this.ban_btn9);
            this.Controls.Add(this.ban_btn8);
            this.Controls.Add(this.ban_btn7);
            this.Controls.Add(this.ban_btn6);
            this.Controls.Add(this.ban_btn5);
            this.Controls.Add(this.ban_btn4);
            this.Controls.Add(this.ban_btn3);
            this.Controls.Add(this.ban_btn2);
            this.Controls.Add(this.ban_btn1);
            this.Controls.Add(this.other_lab12);
            this.Controls.Add(this.other_lab11);
            this.Controls.Add(this.other_lab10);
            this.Controls.Add(this.other_lab9);
            this.Controls.Add(this.other_lab8);
            this.Controls.Add(this.other_lab7);
            this.Controls.Add(this.other_lab6);
            this.Controls.Add(this.other_lab5);
            this.Controls.Add(this.other_lab4);
            this.Controls.Add(this.other_lab3);
            this.Controls.Add(this.other_lab2);
            this.Controls.Add(this.other_lab1);
            this.Controls.Add(this.mainInf_lab12);
            this.Controls.Add(this.mainInf_lab11);
            this.Controls.Add(this.mainInf_lab10);
            this.Controls.Add(this.mainInf_lab9);
            this.Controls.Add(this.mainInf_lab8);
            this.Controls.Add(this.mainInf_lab7);
            this.Controls.Add(this.mainInf_lab6);
            this.Controls.Add(this.mainInf_lab5);
            this.Controls.Add(this.mainInf_lab4);
            this.Controls.Add(this.mainInf_lab3);
            this.Controls.Add(this.mainInf_lab2);
            this.Controls.Add(this.mainInf_lab1);
            this.Controls.Add(this.name_lab12);
            this.Controls.Add(this.name_lab11);
            this.Controls.Add(this.name_lab10);
            this.Controls.Add(this.name_lab9);
            this.Controls.Add(this.name_lab8);
            this.Controls.Add(this.name_lab7);
            this.Controls.Add(this.name_lab6);
            this.Controls.Add(this.name_lab5);
            this.Controls.Add(this.name_lab4);
            this.Controls.Add(this.name_lab3);
            this.Controls.Add(this.name_lab2);
            this.Controls.Add(this.name_lab1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.stopUpdateCheckBox);
            this.Controls.Add(this.FireMode);
            this.Controls.Add(this.CommandLineText);
            this.Controls.Add(this.ActivateOtherInfo);
            this.Controls.Add(this.label16);
            this.Name = "LobbyManager";
            this.Text = "iCCup Unreal Lobby Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LobbyManager_FormClosing);
            this.Load += new System.EventHandler(this.LobbyManager_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox ActivateOtherInfo;
        private System.Windows.Forms.Timer CacheCleaner;
        private System.Windows.Forms.CheckBox FireMode;
        private System.Windows.Forms.TextBox CommandLineText;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Timer CheckForKick;
        private System.Windows.Forms.Label name_lab1;
        private System.Windows.Forms.Label mainInf_lab1;
        private System.Windows.Forms.Label other_lab1;
        private System.Windows.Forms.Button ban_btn1;
        private System.Windows.Forms.Label name_lab2;
        private System.Windows.Forms.Label mainInf_lab2;
        private System.Windows.Forms.Label other_lab2;
        private System.Windows.Forms.Button ban_btn2;
        private System.Windows.Forms.Label name_lab3;
        private System.Windows.Forms.Label mainInf_lab3;
        private System.Windows.Forms.Label other_lab3;
        private System.Windows.Forms.Button ban_btn3;
        private System.Windows.Forms.Label name_lab4;
        private System.Windows.Forms.Label mainInf_lab4;
        private System.Windows.Forms.Label other_lab4;
        private System.Windows.Forms.Button ban_btn4;
        private System.Windows.Forms.Label name_lab5;
        private System.Windows.Forms.Label mainInf_lab5;
        private System.Windows.Forms.Label other_lab5;
        private System.Windows.Forms.Button ban_btn5;
        private System.Windows.Forms.Label name_lab6;
        private System.Windows.Forms.Label mainInf_lab6;
        private System.Windows.Forms.Label other_lab6;
        private System.Windows.Forms.Button ban_btn6;
        private System.Windows.Forms.Label name_lab7;
        private System.Windows.Forms.Label mainInf_lab7;
        private System.Windows.Forms.Label other_lab7;
        private System.Windows.Forms.Button ban_btn7;
        private System.Windows.Forms.Label name_lab8;
        private System.Windows.Forms.Label mainInf_lab8;
        private System.Windows.Forms.Label other_lab8;
        private System.Windows.Forms.Button ban_btn8;
        private System.Windows.Forms.Label name_lab9;
        private System.Windows.Forms.Label mainInf_lab9;
        private System.Windows.Forms.Label other_lab9;
        private System.Windows.Forms.Button ban_btn9;
        private System.Windows.Forms.Label name_lab10;
        private System.Windows.Forms.Label mainInf_lab10;
        private System.Windows.Forms.Label other_lab10;
        private System.Windows.Forms.Button ban_btn10;
        private System.Windows.Forms.Label name_lab11;
        private System.Windows.Forms.Label mainInf_lab11;
        private System.Windows.Forms.Label other_lab11;
        private System.Windows.Forms.Button ban_btn11;
        private System.Windows.Forms.Label name_lab12;
        private System.Windows.Forms.Label mainInf_lab12;
        private System.Windows.Forms.Label other_lab12;
        private System.Windows.Forms.Button ban_btn12;
        private System.Windows.Forms.CheckBox stopUpdateCheckBox;
    }
}

