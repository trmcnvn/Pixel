using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Pixel.Models {
  public class UploaderManager {
    [ImportMany]
    public IEnumerable<IUploader> Uploaders { get; private set; }

    public IUploader ActiveUploader { get; private set; }

    internal void LoadUploaders() {
      var assCat = new AssemblyCatalog(Assembly.GetExecutingAssembly());
      Uploaders = new CompositionContainer(new AggregateCatalog(assCat)).GetExportedValues<IUploader>();
    }

    public void Initialize(string uploaderName) {
      ActiveUploader = Uploaders.FirstOrDefault(u => u.Name == uploaderName);
      if (ActiveUploader == null) {
        ActiveUploader = Uploaders.First(u => u.Name == "Imgur");
        return;
      }
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