using IntegradorApi.Data.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;

namespace IntegradorApi.ViewModels;

public class ConnectionStatusViewModel : INotifyPropertyChanged {
  public Connection Connection { get; }

  private SolidColorBrush _statusBrush = new(Colors.Gray);
  public SolidColorBrush StatusBrush {
    get => _statusBrush;
    set {
      _statusBrush = value;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusBrush)));
    }
  }

  public ConnectionStatusViewModel(Connection connection) {
    Connection = connection;
  }

  public event PropertyChangedEventHandler? PropertyChanged;
}
