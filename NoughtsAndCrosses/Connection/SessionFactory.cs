using System;

namespace NoughtsAndCrosses.Connection {
  /// <summary>
  /// Интерфейс создания и удаления сессий
  /// </summary>
  public interface ISessionFactory {
    /// <summary>
    /// Создает сессию для данного соединения
    /// </summary>
    /// <param name="aServer">Сервер</param>
    /// <param name="aConnection">Соединение</param>
    /// <returns>Возвращает сессию</returns>
    Session CreateSession(ConnectManager server, IConnectionInfo connection);

    /// <summary>
    /// Уничтожает сессию
    /// </summary>
    /// <param name="aSession">Сессия</param>
    void DestroySession(Session aSession);
  }
}
