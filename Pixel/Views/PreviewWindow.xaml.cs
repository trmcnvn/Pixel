using System;
using Microsoft.Win32;
using Pixel.ViewModels;
using Pixel.Views.Converters;
using ReactiveUI;

namespace Pixel.Views {
  /// <summary>
  ///   Interaction logic for PreviewWindow.xaml
  /// </summary>
  public partial class PreviewWindow : IViewFor<PreviewWindowViewModel> {
    public PreviewWindow(string file) {
      InitializeComponent();
      ViewModel = new PreviewWindowViewModel(file);

      this.OneWayBind(ViewModel, x => x.ImageSource, x => x.Image.Source, null, null, new StringToImageSourceConverter());
      this.BindCommand(ViewModel, x => x.SaveDlgCommand, x => x.MenuSave);
      this.BindCommand(ViewModel, x => x.CloseCommand, x => x.MenuClose);
      this.BindCommand(ViewModel, x => x.UploadCommand, x => x.ButtonUpload);
      this.BindCommand(ViewModel, x => x.CloseCommand, x => x.ButtonCancel);
      this.BindCommand(ViewModel, x => x.SaveDlgCommand, x => x.SaveKey);
      this.BindCommand(ViewModel, x => x.CloseCommand, x => x.EscapeKey);

      this.WhenAnyObservable(x => x.ViewModel.CloseCommand).Subscribe(_ => Close());
      this.WhenAnyObservable(x => x.ViewModel.SaveDlgCommand).Subscribe(_ => {
        var dialog = new SaveFileDialog {
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
          Filter = "All Files (*.*)|*.*",
          Title = "Save Image",
          FileName =
            string.Format("{0}.{1}", DateTime.Now.ToString("yy-MM-dd-hh-mm-ss"),
              App.Settings.ImageFormat.ToString().ToLower()),
          OverwritePrompt = true
        };
        dialog.ShowDialog();
        ViewModel.SaveFileCommand.Execute(dialog.FileName);
      });
    }

    #region IViewFor<PreviewWindowViewModel> Members

    object IViewFor.ViewModel {
      get { return ViewModel; }
      set { ViewModel = value as PreviewWindowViewModel; }
    }

    public PreviewWindowViewModel ViewModel { get; set; }

    #endregion
  }
}