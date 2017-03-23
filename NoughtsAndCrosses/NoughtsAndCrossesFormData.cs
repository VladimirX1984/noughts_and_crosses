using System;
using System.ComponentModel;

namespace NoughtsAndCrosses {
  public class NoughtsAndCrossesFormData : INotifyPropertyChanged {
    private bool checkMyFirstMove;
    public bool bCheckMyFirstMove {
      get {
        return checkMyFirstMove;
      }
      set {
        checkMyFirstMove = value;
        NotifyPropertyChanged("bCheckMyFirstMove");
      }
    }

    private int numericRowCellsCount;
    public int rowCellsCount {
      get {
        return numericRowCellsCount;
      }
      set {
        numericRowCellsCount = value;
        NotifyPropertyChanged("rowCellsCount");
      }
    }

    private string labelIPAddress;
    public string textIPAddress {
      get {
        return labelIPAddress;
      }
      set {
        labelIPAddress = value;
        NotifyPropertyChanged("textIPAddress");
      }
    }

    private string textIPAddr1 = "127";
    public string textIPAddress1 {
      get {
        return textIPAddr1;
      }
      set {
        textIPAddr1 = value;
        NotifyPropertyChanged("textIPAddress1");
      }
    }

    private string textIPAddr2 = "0";
    public string textIPAddress2 {
      get {
        return textIPAddr2;
      }
      set {
        textIPAddr2 = value;
        NotifyPropertyChanged("textIPAddress2");
      }
    }

    private string textIPAddr3 = "0";
    public string textIPAddress3 {
      get {
        return textIPAddr3;
      }
      set {
        textIPAddr3 = value;
        NotifyPropertyChanged("textIPAddress3");
      }
    }

    private string textIPAddr4 = "1";
    public string textIPAddress4 {
      get {
        return textIPAddr4;
      }
      set {
        textIPAddr4 = value;
        NotifyPropertyChanged("textIPAddress4");
      }
    }
#if FOR_JAVA
    private string textPort = "8080";
    public string textClientPort {
      get {
        return textPort;
      }
      set {
        textPort = value;
        NotifyPropertyChanged("textClientPort");
      }
    }
#endif
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(String info) {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(info));
      }
    }
  }
}
