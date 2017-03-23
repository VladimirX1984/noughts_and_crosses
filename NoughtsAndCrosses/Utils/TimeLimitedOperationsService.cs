using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NoughtsAndCrosses.Utils {
  /// <summary>
  /// Сервис для выполнения заказанных операции через промежутки времени, определяемые внешным таймером
  /// </summary>
  public class TimeLimitedOperationsService : ITimeLimitedOperationsService {

    #region Реализация интерфейса IService

    public string Name { get; set; }

    private bool bStarted = false;

    public void Start() {
      bStarted = true;
    }

    public bool IsStarted() {
      return bStarted;
    }

    public void Stop() {
      bStarted = false;
    }

    public void Reset() {

    }

    #endregion

    #region Реализация интерфейса ITimeLimitedOperationsService

    /// <summary>
    /// Добавить обработчик
    /// </summary>
    /// <param name="operation">операция - функция делегат</param>
    public void AddOperation(OperationDoHandler operation) {
      if (false == _mutex.WaitOne(100)) {
        return;
      }

      try {
        if (false == _operations.Contains(operation)) {
          _operations.Add(operation);
        }
      }
      finally {
        _mutex.ReleaseMutex();
      }
    }

    /// <summary>
    /// Проверяет, обработчик уже есть в списке, и возвращает true, если есть
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    public bool ContainsOperation(OperationDoHandler operation) {
      return _operations.Exists(it => it == operation);
    }

    /// <summary>
    /// Удалить обработчик
    /// </summary>
    /// <param name="operation">операция - функция делегат</param>
    public void RemoveOperation(OperationDoHandler operation) {
      if (false == _mutex.WaitOne(100)) {
        return;
      }

      try {
        if (_operations.Contains(operation)) {
          _operations.Remove(operation);
        }
      }
      finally {
        _mutex.ReleaseMutex();
      }
    }

    /// <summary>
    /// Обход списка операций и их выполнение
    /// </summary>
    public void Run() {
      if (!IsStarted()) {
        return;
      }

      if (_mutex.WaitOne(100)) {
        try {
          int count = _operations.Count;
          for (int i = 0; i < _operations.Count; ++i) {
            var operation = _operations[i];
            operation();
            if (count > _operations.Count) {
              --i;
            }
          }
        }
        finally {
          _mutex.ReleaseMutex();
        }
      }
    }

    #endregion

    private Mutex _mutex = new Mutex();

    private readonly List<OperationDoHandler> _operations = new List<OperationDoHandler>();

    public TimeLimitedOperationsService() {
      Name = "TimeLimitedOperationsService";
    }

    public TimeLimitedOperationsService(string name) {
      Name = name;
    }
  }

  public sealed class TimeLimitedOperationsServiceSingleton : Singleton<TimeLimitedOperationsService> {
    private TimeLimitedOperationsServiceSingleton() {
    }

    public static void Start() {
      GetInstance().Start();
    }

    public static void Stop() {
      GetInstance().Stop();
    }

    public static bool IsStarted() {
      return GetInstance().IsStarted();
    }

    public static void AddOperation(OperationDoHandler operation) {
      GetInstance().AddOperation(operation);
    }

    public static void RemoveOperation(OperationDoHandler operation) {
      GetInstance().RemoveOperation(operation);
    }

    public static void Run() {
      GetInstance().Run();
    }
  }

  public class BaseTimeLimitedOperationsService<TService, T> : TimeLimitedOperationsService
    where TService : TimeLimitedOperationsService, new ()
    where T : new () {
    /// <summary>
    /// Ссылка на экземпляр класса
    /// </summary>
    private static TService _instance;

    /// <summary>
    /// Создать экземпляр класса.
    /// </summary>
    public static void Create() {
      _instance = new TService();
    }

    /// <summary>
    /// Получить ссылку на экземпляр класса.
    /// </summary>
    /// <returns>Ссылка на экземпляр класса</returns>
    public static TService GetInstance() {
      if (_instance == null) {
        Create();
      }
      return _instance;
    }

    /// <summary>
    /// Удалить экземпляр класса.
    /// </summary>
    public static void Delete() {
      _instance = default(TService);
    }

    public static new bool IsStarted() {
      return GetInstance().IsStarted();
    }

    public static new void AddOperation(OperationDoHandler operation) {
      GetInstance().AddOperation(operation);
    }

    public static new void RemoveOperation(OperationDoHandler operation) {
      GetInstance().RemoveOperation(operation);
    }

    public static new void Run() {
      GetInstance().Run();
    }

    protected BaseTimeLimitedOperationsService()
    : base() {
      Name = "BaseTimeLimitedOperationsService<T>";
    }
  }
}
