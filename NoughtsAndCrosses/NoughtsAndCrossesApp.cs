using System;
using System.Windows.Forms;

namespace NoughtsAndCrosses {
  static class NoughtsAndCrossesApp {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      NoughtsAndCrossesGame noughtsAndCrossesGame = new NoughtsAndCrossesGame();
      NoughtsAndCrossesForm noughtsAndCrossesForm = new NoughtsAndCrossesForm(noughtsAndCrossesGame);
      NoughtsAndCrossesFormData noughtsAndCrossesFormData = new NoughtsAndCrossesFormData();
      noughtsAndCrossesForm.SetNotifyPropertyChanged(noughtsAndCrossesFormData);
      noughtsAndCrossesGame.SetFormData(noughtsAndCrossesFormData);

      Application.Run(noughtsAndCrossesForm);
    }
  }
}
