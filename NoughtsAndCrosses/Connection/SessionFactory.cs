using System;

namespace NoughtsAndCrosses.Connection {
  /// <summary>
  /// ��������� �������� � �������� ������
  /// </summary>
  public interface ISessionFactory {
    /// <summary>
    /// ������� ������ ��� ������� ����������
    /// </summary>
    /// <param name="aServer">������</param>
    /// <param name="aConnection">����������</param>
    /// <returns>���������� ������</returns>
    Session CreateSession(ConnectManager server, IConnectionInfo connection);

    /// <summary>
    /// ���������� ������
    /// </summary>
    /// <param name="aSession">������</param>
    void DestroySession(Session aSession);
  }
}
