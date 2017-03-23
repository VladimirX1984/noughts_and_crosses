namespace NoughtsAndCrosses {
  partial class NoughtsAndCrossesForm {
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
      this.components = new System.ComponentModel.Container();
      this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.checkMyFirstMove = new System.Windows.Forms.CheckBox();
      this.gamePanel = new System.Windows.Forms.Panel();
#if !FOR_JAVA
      this.buttonStartServer = new System.Windows.Forms.Button();
#else
      this.labelUserName = new System.Windows.Forms.Label();
      this.textBoxUserName = new System.Windows.Forms.TextBox();
#endif
      this.buttonConnect = new System.Windows.Forms.Button();
#if FOR_JAVA
      this.buttonCreateGame = new System.Windows.Forms.Button();
      this.buttonJoinToGame = new System.Windows.Forms.Button();
      this.textBoxGameToken = new System.Windows.Forms.TextBox();
#endif
      this.buttonDisConnect = new System.Windows.Forms.Button();
      this.groupServer = new System.Windows.Forms.GroupBox();
      this.buttonStopServer = new System.Windows.Forms.Button();
      this.labelIPAddress = new System.Windows.Forms.Label();
      this.numericNumberToWin = new System.Windows.Forms.NumericUpDown();
      this.numericRowCellsCount = new System.Windows.Forms.NumericUpDown();
      this.buttonApply = new System.Windows.Forms.Button();
      this.labelNumberToWin = new System.Windows.Forms.Label();
      this.labelRowCellsCount = new System.Windows.Forms.Label();
      this.groupClient = new System.Windows.Forms.GroupBox();
      this.groupIPAddress = new System.Windows.Forms.GroupBox();
      this.labelPoint1 = new System.Windows.Forms.Label();
      this.labelPoint2 = new System.Windows.Forms.Label();
      this.labelPoint3 = new System.Windows.Forms.Label();
      this.textIPAddr1 = new System.Windows.Forms.TextBox();
      this.textIPAddr2 = new System.Windows.Forms.TextBox();
      this.textIPAddr3 = new System.Windows.Forms.TextBox();
      this.textIPAddr4 = new System.Windows.Forms.TextBox();
#if FOR_JAVA
      this.labelPort = new System.Windows.Forms.Label();
      this.textPort = new System.Windows.Forms.TextBox();
      this.comboBoxProtocols = new System.Windows.Forms.ComboBox();
      this.labelConnStatus = new System.Windows.Forms.Label();
#endif
      this.labelGameStatus = new System.Windows.Forms.Label();
      this.timer = new System.Windows.Forms.Timer();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
      this.gamePanel.SuspendLayout();
      this.groupServer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericNumberToWin)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericRowCellsCount)).BeginInit();
      this.groupClient.SuspendLayout();
      this.groupIPAddress.SuspendLayout();
      this.SuspendLayout();
      //
      // checkMyFirstMove
      //
      this.checkMyFirstMove.AutoSize = true;
      this.checkMyFirstMove.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingSource,
                                             "bCheckMyFirstMove", true));
      this.checkMyFirstMove.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                           System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.checkMyFirstMove.Location = new System.Drawing.Point(6, 19);
      this.checkMyFirstMove.Name = "checkMyFirstMove";
      this.checkMyFirstMove.Size = new System.Drawing.Size(119, 18);
      this.checkMyFirstMove.TabIndex = 1;
      this.checkMyFirstMove.Text = "Мой первый ход";
      this.checkMyFirstMove.UseVisualStyleBackColor = true;
      //
      // gamePanel
      //
      this.gamePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.gamePanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.gamePanel.Location = new System.Drawing.Point(280, 11);
      this.gamePanel.Name = "gamePanel";
      this.gamePanel.Size = new System.Drawing.Size(150, 150);
      this.gamePanel.TabIndex = 3;
#if !FOR_JAVA
      //
      // buttonStartServer
      //
      this.buttonStartServer.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                            System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.buttonStartServer.Location = new System.Drawing.Point(6, 177);
      this.buttonStartServer.Name = "buttonStartServer";
      this.buttonStartServer.Size = new System.Drawing.Size(150, 25);
      this.buttonStartServer.TabIndex = 16;
      this.buttonStartServer.Text = "Запустить сервер";
      this.buttonStartServer.UseVisualStyleBackColor = true;
#else
      //
      // labelUserName
      //
      this.labelUserName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                        System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelUserName.Location = new System.Drawing.Point(6, 180);
      this.labelUserName.Name = "labelUserName";
      this.labelUserName.Size = new System.Drawing.Size(50, 25);
      this.labelUserName.Text = "Имя:";
      //
      // textBoxUserName
      //
      this.textBoxUserName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                          System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.textBoxUserName.Location = new System.Drawing.Point(56, 177);
      this.textBoxUserName.MaxLength = 12;
      this.textBoxUserName.Name = "textBoxUserName";
      this.textBoxUserName.Size = new System.Drawing.Size(120, 25);
      this.textBoxUserName.Text = "";
      //
      // comboBoxProtocols
      //
      this.comboBoxProtocols.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                            System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.comboBoxProtocols.Location = new System.Drawing.Point(196, 177);
      this.comboBoxProtocols.MaxLength = 12;
      this.comboBoxProtocols.Name = "comboBoxProtocols";
      this.comboBoxProtocols.Size = new System.Drawing.Size(60, 25);
      this.comboBoxProtocols.Items.Add("TCP");
      this.comboBoxProtocols.Items.Add("HTTP");
      this.comboBoxProtocols.SelectedIndex = 0;
#endif
      //
      // groupServer
      //
      this.groupServer.Controls.Add(this.buttonStopServer);
      this.groupServer.Controls.Add(this.labelIPAddress);
      this.groupServer.Controls.Add(this.numericNumberToWin);
      this.groupServer.Controls.Add(this.numericRowCellsCount);
      this.groupServer.Controls.Add(this.buttonApply);
      this.groupServer.Controls.Add(this.labelNumberToWin);
      this.groupServer.Controls.Add(this.labelRowCellsCount);
      this.groupServer.Controls.Add(this.checkMyFirstMove);
#if !FOR_JAVA
      this.groupServer.Controls.Add(this.buttonStartServer);
#else
      this.groupServer.Controls.Add(this.labelUserName);
      this.groupServer.Controls.Add(this.textBoxUserName);
      this.groupServer.Controls.Add(this.comboBoxProtocols);
#endif
      this.groupServer.Location = new System.Drawing.Point(12, 2);
      this.groupServer.Name = "groupServer";
      this.groupServer.Size = new System.Drawing.Size(261, 208);
      this.groupServer.TabIndex = 7;
      this.groupServer.TabStop = false;
#if FOR_JAVA
      this.groupServer.Text = "Настройки";
#else
      this.groupServer.Text = "Серверная часть";
#endif
      //
      // buttonStopServer
      //
      this.buttonStopServer.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                           System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.buttonStopServer.Location = new System.Drawing.Point(6, 177);
      this.buttonStopServer.Name = "buttonStopServer";
      this.buttonStopServer.Size = new System.Drawing.Size(150, 25);
      this.buttonStopServer.TabIndex = 17;
      this.buttonStopServer.Text = "Остановить сервер";
      this.buttonStopServer.UseVisualStyleBackColor = true;
      this.buttonStopServer.Visible = false;
      //
      // labelIPAddress
      //
      this.labelIPAddress.AutoSize = true;
      this.labelIPAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "textIPAddress",
                                           true));
      this.labelIPAddress.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                         System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelIPAddress.Location = new System.Drawing.Point(6, 122);
      this.labelIPAddress.Name = "labelIPAddress";
      this.labelIPAddress.Size = new System.Drawing.Size(88, 14);
      this.labelIPAddress.TabIndex = 13;
      this.labelIPAddress.Text = "Ваш IP-адрес: ";
      //
      // numericNumberToWin
      //
      this.numericNumberToWin.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                             System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.numericNumberToWin.Location = new System.Drawing.Point(210, 83);
      this.numericNumberToWin.Maximum = new decimal(new int[] { 9, 0, 0, 0 });
      this.numericNumberToWin.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
      this.numericNumberToWin.Name = "numericNumberToWin";
      this.numericNumberToWin.Size = new System.Drawing.Size(45, 22);
      this.numericNumberToWin.TabIndex = 12;
      this.numericNumberToWin.Value = new decimal(new int[] { 3, 0, 0, 0 });
      this.numericNumberToWin.ValueChanged += new System.EventHandler(this.numericNumberToWin_ValueChanged);
      //
      // numericRowCellsCount
      //
      this.numericRowCellsCount.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource,
                                                 "rowCellsCount", true));
      this.numericRowCellsCount.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                               System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.numericRowCellsCount.Location = new System.Drawing.Point(210, 40);
      this.numericRowCellsCount.Maximum = new decimal(new int[] { 9, 0, 0, 0 });
      this.numericRowCellsCount.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
      this.numericRowCellsCount.Name = "numericRowCellsCount";
      this.numericRowCellsCount.Size = new System.Drawing.Size(45, 22);
      this.numericRowCellsCount.TabIndex = 11;
      this.numericRowCellsCount.Value = new decimal(new int[] { 3, 0, 0, 0 });
      this.numericRowCellsCount.ValueChanged += new System.EventHandler(this.numericRowCellsCount_ValueChanged);
      //
      // buttonApply
      //
      this.buttonApply.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                      System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.buttonApply.Location = new System.Drawing.Point(6, 148);
      this.buttonApply.Name = "buttonApply";
      this.buttonApply.Size = new System.Drawing.Size(150, 23);
      this.buttonApply.TabIndex = 10;
      this.buttonApply.Text = "Применить";
      this.buttonApply.UseVisualStyleBackColor = true;
      //
      // labelNumberToWin
      //
      this.labelNumberToWin.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                           System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelNumberToWin.Location = new System.Drawing.Point(6, 70);
      this.labelNumberToWin.Name = "labelNumberToWin";
      this.labelNumberToWin.Size = new System.Drawing.Size(177, 45);
      this.labelNumberToWin.TabIndex = 8;
      this.labelNumberToWin.Text =
        "Число отмеченных клеток в ряду или диагонале для достижения победы:";
      //
      // labelRowCellsCount
      //
      this.labelRowCellsCount.AutoSize = true;
      this.labelRowCellsCount.Font = new System.Drawing.Font("Tahoma", 9F);
      this.labelRowCellsCount.Location = new System.Drawing.Point(6, 43);
      this.labelRowCellsCount.Name = "labelRowCellsCount";
      this.labelRowCellsCount.Size = new System.Drawing.Size(166, 14);
      this.labelRowCellsCount.TabIndex = 6;
      this.labelRowCellsCount.Text = "Количество клеток в ряду:";
      //
      // buttonConnect
      //
      this.buttonConnect.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                        System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.buttonConnect.Location = new System.Drawing.Point(6, 69);
      this.buttonConnect.Name = "buttonConnect";
      this.buttonConnect.Size = new System.Drawing.Size(188, 25);
      this.buttonConnect.TabIndex = 14;
      this.buttonConnect.Text = "Соединиться со сервером";
      this.buttonConnect.UseVisualStyleBackColor = true;
#if FOR_JAVA
      //
      // buttonCreateGame
      //
      this.buttonCreateGame.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                           System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.buttonCreateGame.Location = new System.Drawing.Point(6, 69);
      this.buttonCreateGame.Name = "buttonCreateGame";
      this.buttonCreateGame.Size = new System.Drawing.Size(188, 25);
      this.buttonCreateGame.TabIndex = 14;
      this.buttonCreateGame.Text = "Создать игру";
      this.buttonCreateGame.UseVisualStyleBackColor = true;
      this.buttonCreateGame.Visible = false;
      //
      // buttonJoinToGame
      //
      this.buttonJoinToGame.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                           System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.buttonJoinToGame.Location = new System.Drawing.Point(6, 109);
      this.buttonJoinToGame.Name = "buttonJoinToGame";
      this.buttonJoinToGame.Size = new System.Drawing.Size(168, 25);
      this.buttonJoinToGame.TabIndex = 14;
      this.buttonJoinToGame.Text = "Присоединиться к игре";
      this.buttonJoinToGame.UseVisualStyleBackColor = true;
      this.buttonJoinToGame.Visible = false;
      //
      // textBoxGameToken
      //
      this.textBoxGameToken.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                           System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.textBoxGameToken.Location = new System.Drawing.Point(180, 109);
      this.textBoxGameToken.Name = "textBoxGameToken";
      this.textBoxGameToken.Size = new System.Drawing.Size(75, 25);
      this.textBoxGameToken.TabIndex = 14;
      this.textBoxGameToken.Text = "";
      this.textBoxGameToken.Visible = false;
#endif
      //
      // buttonDisConnect
      //
      this.buttonDisConnect.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                           System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.buttonDisConnect.Location = new System.Drawing.Point(6, 69);
      this.buttonDisConnect.Name = "buttonDisConnect";
      this.buttonDisConnect.Size = new System.Drawing.Size(128, 25);
      this.buttonDisConnect.TabIndex = 15;
      this.buttonDisConnect.Text = "Сдаться";
      this.buttonDisConnect.UseVisualStyleBackColor = true;
      this.buttonDisConnect.Visible = false;
      //
      // groupClient
      //
      this.groupClient.Controls.Add(this.groupIPAddress);
      this.groupClient.Controls.Add(this.buttonDisConnect);
      this.groupClient.Controls.Add(this.buttonConnect);
#if FOR_JAVA
      this.groupClient.Controls.Add(this.buttonCreateGame);
      this.groupClient.Controls.Add(this.buttonJoinToGame);
      this.groupClient.Controls.Add(this.textBoxGameToken);
#endif
      this.groupClient.Location = new System.Drawing.Point(12, 220);
      this.groupClient.Name = "groupClient";
      this.groupClient.Size = new System.Drawing.Size(261, 200);
      this.groupClient.TabIndex = 8;
      this.groupClient.TabStop = false;
#if FOR_JAVA
      this.groupClient.Text = "Создание и присоединение к игре";
#else
      this.groupClient.Text = "Клиентская часть";
#endif

      //
      // groupIPAddress
      //
      this.groupIPAddress.Controls.Add(this.labelPoint1);
      this.groupIPAddress.Controls.Add(this.labelPoint2);
      this.groupIPAddress.Controls.Add(this.labelPoint3);
      this.groupIPAddress.Controls.Add(this.textIPAddr1);
      this.groupIPAddress.Controls.Add(this.textIPAddr2);
      this.groupIPAddress.Controls.Add(this.textIPAddr3);
      this.groupIPAddress.Controls.Add(this.textIPAddr4);
#if FOR_JAVA
      this.groupIPAddress.Controls.Add(this.labelPort);
      this.groupIPAddress.Controls.Add(this.textPort);
#endif
      this.groupIPAddress.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                         System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.groupIPAddress.Location = new System.Drawing.Point(6, 15);
      this.groupIPAddress.Name = "groupIPAddress";
      this.groupIPAddress.Size = new System.Drawing.Size(250, 48);
      this.groupIPAddress.TabIndex = 16;
      this.groupIPAddress.TabStop = false;
      this.groupIPAddress.Text = "IP-адрес сервера";
      //
      // labelPoint1
      //
      this.labelPoint1.AutoSize = true;
      this.labelPoint1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold,
                                                      System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelPoint1.Location = new System.Drawing.Point(34, 25);
      this.labelPoint1.Name = "labelPoint1";
      this.labelPoint1.Size = new System.Drawing.Size(11, 14);
      this.labelPoint1.TabIndex = 10;
      this.labelPoint1.Text = ".";
      //
      // labelPoint2
      //
      this.labelPoint2.AutoSize = true;
      this.labelPoint2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold,
                                                      System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelPoint2.Location = new System.Drawing.Point(79, 25);
      this.labelPoint2.Name = "labelPoint2";
      this.labelPoint2.Size = new System.Drawing.Size(11, 14);
      this.labelPoint2.TabIndex = 11;
      this.labelPoint2.Text = ".";
      //
      // labelPoint3
      //
      this.labelPoint3.AutoSize = true;
      this.labelPoint3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold,
                                                      System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelPoint3.Location = new System.Drawing.Point(124, 25);
      this.labelPoint3.Name = "labelPoint3";
      this.labelPoint3.Size = new System.Drawing.Size(11, 14);
      this.labelPoint3.TabIndex = 12;
      this.labelPoint3.Text = ".";
      //
      // textIPAddr1
      //
      this.textIPAddr1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "textIPAddress1", true));
      this.textIPAddr1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                      System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.textIPAddr1.Location = new System.Drawing.Point(2, 20);
      this.textIPAddr1.MaxLength = 3;
      this.textIPAddr1.Name = "textIPAddr1";
      this.textIPAddr1.Size = new System.Drawing.Size(30, 22);
      this.textIPAddr1.TabIndex = 6;
      this.textIPAddr1.Text = "127";
      //
      // textIPAddr2
      //
      this.textIPAddr2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "textIPAddress2", true));
      this.textIPAddr2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                      System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.textIPAddr2.Location = new System.Drawing.Point(47, 20);
      this.textIPAddr2.MaxLength = 3;
      this.textIPAddr2.Name = "textIPAddr2";
      this.textIPAddr2.Size = new System.Drawing.Size(30, 22);
      this.textIPAddr2.TabIndex = 7;
      this.textIPAddr2.Text = "0";
      //
      // textIPAddr3
      //
      this.textIPAddr3.AcceptsTab = true;
      this.textIPAddr3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "textIPAddress3", true));
      this.textIPAddr3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                      System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.textIPAddr3.Location = new System.Drawing.Point(92, 20);
      this.textIPAddr3.MaxLength = 3;
      this.textIPAddr3.Name = "textIPAddr3";
      this.textIPAddr3.Size = new System.Drawing.Size(30, 22);
      this.textIPAddr3.TabIndex = 8;
      this.textIPAddr3.Text = "0";
      //
      // textIPAddr4
      //
      this.textIPAddr4.AcceptsTab = true;
      this.textIPAddr4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "textIPAddress4", true));
      this.textIPAddr4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                      System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.textIPAddr4.Location = new System.Drawing.Point(137, 20);
      this.textIPAddr4.MaxLength = 3;
      this.textIPAddr4.Name = "textIPAddr4";
      this.textIPAddr4.Size = new System.Drawing.Size(30, 22);
      this.textIPAddr4.TabIndex = 9;
      this.textIPAddr4.Text = "1";
#if FOR_JAVA
      //
      // labelPort
      //
      this.labelPort.AutoSize = true;
      this.labelPort.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold,
                                                    System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelPort.Location = new System.Drawing.Point(170, 23);
      this.labelPort.Name = "labelPort";
      this.labelPort.Size = new System.Drawing.Size(25, 14);
      this.labelPort.TabIndex = 12;
      this.labelPort.Text = "порт:";
      //
      // textPort
      //
      this.textPort.AcceptsTab = true;
      this.textPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "textClientPort", true));
      this.textPort.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                   System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.textPort.Location = new System.Drawing.Point(210, 20);
      this.textPort.MaxLength = 4;
      this.textPort.Name = "textPort";
      this.textPort.Size = new System.Drawing.Size(35, 22);
      this.textPort.TabIndex = 9;
      this.textPort.Text = "8080";
#endif
      //
      // labelGameState
      //
      this.labelGameStatus.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular,
                                                          System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelGameStatus.Location = new System.Drawing.Point(12, 430);
      this.labelGameStatus.Name = "labelGameState";
      this.labelGameStatus.Size = new System.Drawing.Size(264, 45);
      this.labelGameStatus.TabIndex = 9;
      this.labelGameStatus.Text = "Текстовое описание";
      //
      // labelConnStatus
      //
      this.labelConnStatus.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular,
                                                          System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelConnStatus.Location = new System.Drawing.Point(12, 450);
      this.labelConnStatus.Name = "labelConnStatus";
      this.labelConnStatus.Size = new System.Drawing.Size(264, 45);
      this.labelConnStatus.TabIndex = 10;
      this.labelConnStatus.Text = "Связи нет";
      //
      // timer
      //
      this.timer.Interval = 2000;
      this.timer.Enabled = true;
      //
      // NoughtsAndCrossesForm
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(502, 488);
      this.Controls.Add(this.labelConnStatus);
      this.Controls.Add(this.labelGameStatus);
      this.Controls.Add(this.groupClient);
      this.Controls.Add(this.groupServer);
      this.Controls.Add(this.gamePanel);
      this.Name = "NoughtsAndCrossesForm";
      this.ShowIcon = false;
      this.Text = "Крестики-нолики";
      ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
      this.gamePanel.ResumeLayout(false);
      this.groupServer.ResumeLayout(false);
      this.groupServer.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericNumberToWin)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericRowCellsCount)).EndInit();
      this.groupClient.ResumeLayout(false);
      this.groupIPAddress.ResumeLayout(false);
      this.groupIPAddress.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.BindingSource bindingSource;
    private System.Windows.Forms.CheckBox checkMyFirstMove;
    private System.Windows.Forms.Panel gamePanel;

#if !FOR_JAVA
    private System.Windows.Forms.Button buttonStartServer;
#else
    private System.Windows.Forms.Label labelUserName;
    private System.Windows.Forms.TextBox textBoxUserName;
#endif
    private System.Windows.Forms.Button buttonConnect;
#if FOR_JAVA
    private System.Windows.Forms.Button buttonCreateGame;
    private System.Windows.Forms.Button buttonJoinToGame;
    private System.Windows.Forms.TextBox textBoxGameToken;
#endif
    private System.Windows.Forms.Button buttonDisConnect;
    private System.Windows.Forms.GroupBox groupServer;
    private System.Windows.Forms.Label labelRowCellsCount;
    private System.Windows.Forms.Label labelNumberToWin;
    private System.Windows.Forms.Button buttonApply;
    private System.Windows.Forms.NumericUpDown numericRowCellsCount;
    private System.Windows.Forms.NumericUpDown numericNumberToWin;
    private System.Windows.Forms.GroupBox groupClient;
    private System.Windows.Forms.Label labelGameStatus;
    private System.Windows.Forms.Label labelIPAddress;
    private System.Windows.Forms.Label labelPoint1;
    private System.Windows.Forms.Label labelPoint2;
    private System.Windows.Forms.Label labelPoint3;
    private System.Windows.Forms.TextBox textIPAddr1;
    private System.Windows.Forms.TextBox textIPAddr2;
    private System.Windows.Forms.TextBox textIPAddr3;
    private System.Windows.Forms.TextBox textIPAddr4;
#if FOR_JAVA
    private System.Windows.Forms.Label labelPort;
    private System.Windows.Forms.TextBox textPort;
    private System.Windows.Forms.ComboBox comboBoxProtocols;
    private System.Windows.Forms.Label labelConnStatus;
#endif

    private System.Windows.Forms.Button buttonStopServer;
    private System.Windows.Forms.GroupBox groupIPAddress;
    private System.Windows.Forms.Timer timer;

  }
}

