using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Shield.App.Controls;
using Shield.App.Helpers;
using Shield.DataAccess.DTOs;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Shield.App.Dialogs;
public sealed partial class CreateContractDialog : UserControl, INotifyPropertyChanged
{
    private string _planPath = string.Empty;
    public string PlanPath
    {
        get => _planPath;
        set
        {
            if (value != _planPath)
            {
                _planPath = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _photoPath = string.Empty;
    public string PhotoPath
    {
        get => _photoPath;
        set
        {
            if (value != _photoPath)
            {
                _photoPath = value;
                NotifyPropertyChanged();
            }
        }
    }

    public string Bailee => BaileeTB.Text;
    public string Address => AddressTB.Text;
    public string Comment => CommentTB.Text;
    public List<string> Owners => OwnersControls.Select(x => x.Value).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
    public StorageFile Plan;
    public StorageFile Picture;

    public bool IsEdited = false;

    private ObservableCollection<RemovableTextBox> OwnersControls { get; set; } = new();

    public delegate void EditedHandler(object sender);

    public event EditedHandler Edited;
    public event PropertyChangedEventHandler PropertyChanged;

    public CreateContractDialog()
    {
        Edited += (s) => IsEdited = true;

        this.InitializeComponent();
    }

    public CreateContractDialog(ContractDto contract)
    {
        Edited += (s) => IsEdited = true;

        this.InitializeComponent();

        BaileeTB.Text = contract.Bailee;
        AddressTB.Text = contract.Address;
        CommentTB.Text = contract.Comment;

        if (contract.Owners != null)
        {
            foreach (var ownerName in contract.Owners.Split(';'))
            {
                AddOwner(ownerName);
            }
        }
    }

    private void AddOwner(string? name = null)
    {
        var rtb = new RemovableTextBox();
        rtb.TextChanged += (s, tbs, e) => Edited?.Invoke(s);
        if (name != null) rtb.Value = name;
        rtb.RemoveRequested += (sender) =>
        {
            OwnersControls.Remove(rtb);
        };
        OwnersControls.Add(rtb);
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void AddOwnerButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        AddOwner();
        Edited?.Invoke(sender);
    }

    private async void LoadPhotoButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {        
        var file = await FilePickerHelper.PickSingleFile();

        if (file != null)
        {
            PhotoPath = file.Name;
            PhotoPathTB.Foreground = new SolidColorBrush(Colors.White);
            Picture = file;
            Edited?.Invoke(sender);
        }
        else
        {
            PhotoPath = "������";
            PhotoPathTB.Foreground = new SolidColorBrush(Colors.IndianRed);
        }
    }

    private async void LoadPlanButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var file = await FilePickerHelper.PickSingleFile("rvt");

        if (file != null)
        {
            PlanPath = file.Name;
            PlanPathTB.Foreground = new SolidColorBrush(Colors.White);
            Plan = file;
            Edited?.Invoke(sender);
        }
        else
        {
            PlanPath = "������";
            PlanPathTB.Foreground = new SolidColorBrush(Colors.IndianRed);
        }
    }

    private void BaileeTB_TextChanged(object sender, TextChangedEventArgs e)
    {
        Edited?.Invoke(sender);
    }

    private void AddressTB_TextChanged(object sender, TextChangedEventArgs e)
    {
        Edited?.Invoke(sender);
    }

    private void CommentTB_TextChanged(object sender, TextChangedEventArgs e)
    {
        Edited?.Invoke(sender);
    }
}
