using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoughtsAndCrosses.Connection.HTTP {
  public class HttpConnectionInfo : IConnectionInfo {

    #region Реализация интерфейса IConnectionInfo

    public Session session { get; set; }

    #endregion

    public IHttpConnection httpConnection;
  }
}
