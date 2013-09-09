using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading;
using Pixel.Properties;

namespace Pixel.Models {
  public class UploaderManager {
    [ImportMany]
    public IEnumerable<IUploader> Uploaders { get; private set; }

    public IUploader ActiveUploader { get; private set; }

    internal void LoadUploaders() {
      var assCat = new AssemblyCatalog(Assembly.GetExecutingAssembly());
      var plugins = new DirectoryCatalog(App.PluginDirectory);
      var aggCat = new AggregateCatalog(assCat, plugins);
      Uploaders = new CompositionContainer(aggCat).GetExportedValues<IUploader>();
    }

    public void Initialize() {
      ActiveUploader = Uploaders.FirstOrDefault(u => u.Name == Settings.Default.ImageUploader);
      if (ActiveUploader == null) return;
      ActiveUploader.ImageUploaded += (s, e) => OnImageUploaded(e);
      ActiveUploader.Initialize();
    }

    public void Upload(string filePath) {
      if (ActiveUploader == null) return;
      ActiveUploader.Upload(filePath);
    }

    public event EventHandler<UploaderEventArgs> ImageUploaded;

    protected virtual void OnImageUploaded(UploaderEventArgs e) {
      var threadSafeHandler = Interlocked.CompareExchange(ref ImageUploaded, null, null);
      if (threadSafeHandler == null) return;
      threadSafeHandler(this, e);
    }
  }
}