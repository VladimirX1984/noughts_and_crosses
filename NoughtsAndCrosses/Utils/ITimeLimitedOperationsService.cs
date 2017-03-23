using System;

namespace NoughtsAndCrosses.Utils {
  public delegate void OperationDoHandler();

  public interface ITimeLimitedOperationsService : IService {
    /// <summary>
    /// Добавить обработчик
    /// </summary>
    /// <param name="operation">операция - функция делегат</param>
    void AddOperation(OperationDoHandler operation);

    /// <summary>
    /// Проверяет, обработчик уже есть в списке, и возвращает true, если есть
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    bool ContainsOperation(OperationDoHandler operation);

    /// <summary>
    /// Удалить обработчик
    /// </summary>
    /// <param name="operation">операция - функция делегат</param>
    void RemoveOperation(OperationDoHandler operation);

    /// <summary>
    /// Обход списка операций и их выполнение
    /// </summary>
    void Run();
  }
}
