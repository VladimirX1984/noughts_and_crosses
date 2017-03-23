using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using NoughtsAndCrosses.Game;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses {
  public partial class NoughtsAndCrossesForm : Form {
    delegate void BoolParameterDelegate(bool value);
    delegate void IntParameterDelegate(int value);
    delegate void StringParameterDelegate(string value);
    delegate void GameStateParameterDelegate(GameState gameState);

    public ArrayList buttonList = new ArrayList();

    private NoughtsAndCrossesGame _noughtsAndCrossesGame;

    public NoughtsAndCrossesForm(NoughtsAndCrossesGame noughtsAndCrossesGame) {
      InitializeComponent();
      _noughtsAndCrossesGame = noughtsAndCrossesGame;

      InitGameEventHandler();

      buttonApply.Click += (o, e) => {
        _noughtsAndCrossesGame.Apply();
      };

      checkMyFirstMove.CheckedChanged += (o, e) => {
        _noughtsAndCrossesGame.SetMyFirstMove(checkMyFirstMove.Checked);
      };

#if !FOR_JAVA
      buttonStartServer.Click += (o, e) => {
        _noughtsAndCrossesGame.StartServer();
      };
#endif

      buttonStopServer.Click += (o, e) => {
        _noughtsAndCrossesGame.StopServer();
      };

      buttonConnect.Click += (o, e) => {
        _noughtsAndCrossesGame.Apply();
        _noughtsAndCrossesGame.Connect(true, "");
      };

#if FOR_JAVA
      buttonCreateGame.Click += (o, e) => {
        _noughtsAndCrossesGame.Apply();
        _noughtsAndCrossesGame.Connect(true, "");
      };
      buttonJoinToGame.Click += (o, e) => {
        _noughtsAndCrossesGame.Connect(false, textBoxGameToken.Text);
      };
      comboBoxProtocols.SelectionChangeCommitted += (o, e) => {
        _noughtsAndCrossesGame.Protocol = (string)comboBoxProtocols.SelectedItem;
        if (_noughtsAndCrossesGame.Protocol == Connector.Protocols[0]) {
          buttonConnect.Visible = true;
          buttonCreateGame.Visible = false;
          buttonJoinToGame.Visible = false;
          textBoxGameToken.Visible = false;
        }
        else if (_noughtsAndCrossesGame.Protocol == Connector.Protocols[1]) {
          buttonConnect.Visible = false;
          buttonCreateGame.Visible = true;
          buttonJoinToGame.Visible = true;
          textBoxGameToken.Visible = true;
        }
      };
      comboBoxProtocols.SelectedItem = Connector.Protocols[1];
      buttonConnect.Visible = false;
      buttonCreateGame.Visible = true;
      buttonJoinToGame.Visible = true;
      textBoxGameToken.Visible = true;
#endif

      buttonDisConnect.Click += (o, e) => {
        _noughtsAndCrossesGame.DisConnect();
      };

      numericNumberToWin.Maximum = numericRowCellsCount.Value;
#if FOR_JAVA
      textBoxUserName.TextChanged += (o, e) => {
        _noughtsAndCrossesGame.SetUserName(textBoxUserName.Text);
      };
#endif

      TimeLimitedOperationsServiceSingleton.Create();
      TimeLimitedOperationsServiceSingleton.Start();
      timer.Tick += (o, e) => TimeLimitedOperationsServiceSingleton.Run();
      timer.Start();

      FormClosing += (o, e) => {
        _noughtsAndCrossesGame.Close();
      };

      FormClosed += (o, e) => {
        TimeLimitedOperationsServiceSingleton.Stop();
        TimeLimitedOperationsServiceSingleton.Delete();
      };
    }

    private void InitGameEventHandler() {
      _noughtsAndCrossesGame.OnGameDurationChanged += (min, sec) => {
        if (sec < 10) {
          this.Invoke(() => UpdateConnStatusLabel(String.Format("Время: {0}:0{1}", min, sec)));
        }
        else {
          this.Invoke(() => UpdateConnStatusLabel(String.Format("Время: {0}:{1}", min, sec)));
        }
      };

      _noughtsAndCrossesGame.OnGameStatusChanged += (status) => {
        this.Invoke(() => UpdateGameStatusLabel(status));
      };

      _noughtsAndCrossesGame.OnConnStatusChanged += (status) => {
        this.Invoke(() => UpdateConnStatusLabel(status));
      };

      _noughtsAndCrossesGame.OnGameStateChanged += (state) => {
        this.Invoke(() => UpdateField(state));
      };

      _noughtsAndCrossesGame.OnConnecting += () => OnConnecting();

      _noughtsAndCrossesGame.OnFirstDataReceived += (token, isObverser) => {
        this.Invoke(() => OnConnected(token, isObverser));
      };

      _noughtsAndCrossesGame.OnDisConnected += () => {
        this.Invoke(() => OnDisConnected());
      };

      _noughtsAndCrossesGame.OnServerStarted += () => {
        OnServerStarted();
      };

      _noughtsAndCrossesGame.OnServerStopped += () => {
        this.Invoke(() => OnServerStopped());
      };

      _noughtsAndCrossesGame.OnRowCellCountChanged += (rowCellCount) => {
        this.Invoke(() => UpdateRowCellCount(rowCellCount));
      };

      _noughtsAndCrossesGame.OnNumberToWinChanged += (numberToWin) => {
        this.Invoke(() => UpdateNumberToWin(numberToWin));
      };

      _noughtsAndCrossesGame.OnCellValueChanged += (number, cellValue) => {
        this.Invoke(() => SetCellValue(number, cellValue));
      };

      _noughtsAndCrossesGame.OnMyFirstMoveChanged += (bMyFirstMove) => {
        this.Invoke(() => UpdateCheckMyFirstMove(bMyFirstMove));
      };
    }

    public void SetNotifyPropertyChanged(NoughtsAndCrossesFormData noughtsAndCrossesFormData) {
      this.bindingSource.DataSource = noughtsAndCrossesFormData;
    }

    private void SetCellValue(int number, char cellValue) {
      this.Invoke(() => {
        if (number < 0 || number >= buttonList.Count) {
          return;
        }

        Button button = (Button)buttonList[number];
        button.Text = string.Format("{0}", cellValue);
      });
    }

    #region Обновление интерфейса

    private Button CreateButton(int aid, int ax, int ay, int awidth, int aheight) {
      Button button = new Button();
      button.Font = new Font("Tahoma", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                             ((byte)(204)));
      button.Location = new System.Drawing.Point(ax, ay);
      button.Name = string.Format("{0}", aid);
      button.Size = new Size(awidth, aheight);
      button.TabIndex = 0;
      button.UseCompatibleTextRendering = true;
      button.UseVisualStyleBackColor = true;
      button.Click += OnButtonClick;
      button.EnabledChanged += OnButtonEnabledChanged;
      return button;
    }

    private bool GenerateField(int acount, char[] data) {
      for (int i = 0; i < buttonList.Count; ++i) {
        Button button = (Button) buttonList[ i];
        button.Click -= OnButtonClick;
        button.EnabledChanged -= OnButtonEnabledChanged;
        this.gamePanel.Controls.Remove(button);
        button = null;
      }
      buttonList.Clear();

      int lineCount = (int) Math.Sqrt(acount);
      if (Math.Pow(lineCount, 2) != acount) {
        MessageBox.Show("Не корректное число для поля", "Ошибка", MessageBoxButtons.OK);
        return false;
      }

      int uWidth = 50;
      int uHeight = 50;

      this.gamePanel.Size = new Size(lineCount * uWidth + gamePanel.Margin.Left + gamePanel.Margin.Right,
                                     lineCount * uHeight + gamePanel.Margin.Top + gamePanel.Margin.Bottom);

      int width = gamePanel.Location.X + gamePanel.Size.Width + 20;
      int height = gamePanel.Location.Y + gamePanel.Size.Height + 40;
      if (height < 530) {
        height = 530;
      }
      Size = new Size(width, height);

      gamePanel.Location = new Point(280, 5);

      int lineNumber = 0;
      for (int i = 0; i < acount; ++i) {
        if (i > lineCount * lineNumber + lineCount - 1) {
          lineNumber++;
        }

        Button button = CreateButton(i, (i % lineCount) * uWidth, lineNumber * uHeight,
                                     uWidth, uHeight);
        if (data[i] != '?') {
          button.Text = Convert.ToString(data[i]);
        }

        buttonList.Add(button);
        gamePanel.Controls.Add(button);
      }

      labelGameStatus.Location = new Point(12, 425);
      labelConnStatus.Location = new Point(12, 470);

      return true;
    }

    private void UpdateCheckMyFirstMove(bool value) {
      checkMyFirstMove.Checked = value;
    }

    private void UpdateGameStatusLabel(string value) {
      labelGameStatus.Text = value;
    }

    private void UpdateConnStatusLabel(string value) {
      labelConnStatus.Text = value;
    }

    private void UpdateRowCellCount(int rowCellsCount) {
      numericRowCellsCount.Value = rowCellsCount;
    }

    private void UpdateNumberToWin(int aNumberToWin) {
      numericNumberToWin.Value = aNumberToWin;
    }

    private void UpdateField(GameState gameState) {
      GenerateField(gameState.Count, gameState.Data);
    }

    #endregion

    #region События - обработка их

    private void OnServerStarted() {
      buttonApply.Enabled = false;
      numericRowCellsCount.Enabled = false;
      numericNumberToWin.Enabled = false;
      checkMyFirstMove.Enabled = false;
#if !FOR_JAVA
      buttonStartServer.Visible = false;
#endif
      buttonStopServer.Visible = true;
      groupClient.Enabled = false;
    }

    private void OnServerStopped() {
      buttonApply.Enabled = true;
      numericRowCellsCount.Enabled = true;
      numericNumberToWin.Enabled = true;
      checkMyFirstMove.Enabled = true;
      buttonStopServer.Visible = false;
#if !FOR_JAVA
      buttonStartServer.Visible = true;
#endif
      groupClient.Enabled = true;
    }

    private void OnDisConnected() {
      groupIPAddress.Enabled = true;

      if (_noughtsAndCrossesGame.Protocol == Connector.Protocols[0]) {
        buttonConnect.Enabled = true;
        buttonConnect.Visible = true;
      }
#if FOR_JAVA
      else if (_noughtsAndCrossesGame.Protocol == Connector.Protocols[1]) {
        buttonCreateGame.Visible = true;
      }
      buttonConnect.Enabled = true;
      buttonCreateGame.Enabled = true;
      buttonJoinToGame.Enabled = true;
      textBoxGameToken.ReadOnly = false;
#endif
      buttonDisConnect.Visible = false;
      groupServer.Enabled = true;
    }

    private void OnConnecting() {
      textBoxGameToken.ReadOnly = true;
      groupIPAddress.Enabled = false;
      buttonConnect.Enabled = false;
#if FOR_JAVA
      buttonCreateGame.Enabled = false;
      buttonJoinToGame.Enabled = false;
#endif
      groupServer.Enabled = false;
    }

    private void OnConnected(string token, bool isObverser) {
      buttonConnect.Visible = false;
#if FOR_JAVA
      buttonCreateGame.Visible = false;
      if (isObverser) {
        buttonDisConnect.Text = "Покинуть игру";
      }
      else {
        buttonDisConnect.Text = "Сдаться";
      }
      if (!String.IsNullOrEmpty(token)) {
        textBoxGameToken.Text = token;
      }
      else {
        textBoxGameToken.ReadOnly = true;
      }
      labelConnStatus.Text = "Соединение установлено";
#else
      buttonDisConnect.Text = "Покинуть игру";
#endif
      buttonDisConnect.Visible = true;
    }

    private void OnGameEnded() {
#if !FOR_JAVA
      buttonStartServer.Enabled = true;
#endif
      groupClient.Enabled = true;
      buttonConnect.Visible = true;
      buttonDisConnect.Visible = false;
      groupServer.Enabled = true;
    }

    private void OnButtonClick(object sender, EventArgs e) {
      Button button = (Button)sender;
      _noughtsAndCrossesGame.MakeMove(button.Name);
    }

    private void OnButtonEnabledChanged(object sender, EventArgs e) {
      Button btn = (Button)sender;
      btn.BackColor = Color.White;
      btn.ForeColor = Color.Black;
    }

    private void numericRowCellsCount_ValueChanged(object sender, EventArgs e) {
      numericNumberToWin.Maximum = numericRowCellsCount.Value;
    }

    private void numericNumberToWin_ValueChanged(object sender, EventArgs e) {
      _noughtsAndCrossesGame.SetCountToWin((int)numericNumberToWin.Value);
    }

    #endregion
  }
}
