using System;
using System.Windows.Forms;

namespace NoughtsAndCrosses.Utils {
  /// <summary>
  /// Класс-расширение для добавление дополнительных методов классу Control
  /// </summary>
  public static class ControlExtensions {
    /// <summary>
    /// Выполнить указанное действие в родном для control'а потоке
    /// </summary>
    /// <param name="control">Control с которым нужно совершить определенное действие.</param>
    /// <param name="action">Действие которое нужно совершить.</param>
    public static void Invoke(this Control control, Action action) {
      if (control.InvokeRequired) {
        control.Invoke(new MethodInvoker(action), null);
      }
      else {
        action.Invoke();
      }
    }
  }
}
