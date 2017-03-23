namespace NoughtsAndCrosses.Utils {
  /// <summary>
  /// Интерфейс от которого наследуются все сервисы
  /// </summary>
  public interface IService {

    string Name { get; set; }

    /// <summary>
    /// Запустить сервис.
    /// </summary>
    void Start();

    /// <summary>
    /// Определяем запущен ли сервис на данный момент.
    /// </summary>
    /// <returns>
    ///   <c>true</c> если сервис запущен; иначе, <c>false</c>.
    /// </returns>
    bool IsStarted();

    /// <summary>
    /// Остановить сервис.
    /// </summary>
    void Stop();

    /// <summary>
    /// Провоцировать новую рабочую итерацию
    /// </summary>
    void Reset();
  }
}
