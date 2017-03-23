using System;
using System.Data;
using System.Diagnostics;

namespace NoughtsAndCrosses.Utils {
  /// <summary>
  /// Шаблон для "превращения" класса в одиночку
  /// </summary>
  /// <typeparam name="T">Тип класса одиночки</typeparam>
  public class Singleton<T> where T : new () {
    /// <summary>
    /// Создать единственный экземпляр класса.
    /// </summary>
    public static void Create() {
      Debug.Assert(_instance == null, "_instance == null",
                   "Ожидается, что _instance будет null");

      if (_instance != null) {
        throw new DuplicateNameException(String.Format("Ожидается, что _instance типа {0} будет null",
                                                       typeof(T)));
      }

      InternalCreate();
    }

    /// <summary>
    /// Внутренний метод для создания экземпляра класса.
    /// </summary>
    private static void InternalCreate() {
      _instance = new T();
    }

    /// <summary>
    /// Удалить единственный экземпляр класса.
    /// </summary>
    public static void Delete() {
      Debug.Assert(_instance != null, "_instance != null",
                   "Ожидается, что _instance будет не null");

      if (_instance == null) {
        throw new DuplicateNameException(String.Format("Ожидается, что _instance типа {0} не будет null",
                                                       typeof(T)));
      }

      InternalDelete();
    }

    /// <summary>
    /// Внутренний метод для удаления экземпляра класса.
    /// </summary>
    private static void InternalDelete() {
      _instance = default(T);
    }

    /// <summary>
    /// Получить ссылку на единственный экземпляр класса.
    /// </summary>
    /// <returns>Ссылка на экземпляр класса</returns>
    public static T GetInstance() {
      if (_instance == null) {
        InternalCreate();
      }

      return _instance;
    }

    /// <summary>
    /// Ссылка на единственный экземпляр класса // really ? wtf !!
    /// </summary>
    private static T _instance;
  }
}
