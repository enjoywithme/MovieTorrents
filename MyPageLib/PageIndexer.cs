using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MyPageLib.PoCo;

namespace MyPageLib
{
    public class PageIndexer
    {
        public static PageIndexer Instance { get; } =new();

        public event EventHandler IndexStopped;
        public event EventHandler<string> IndexFileChanged; 

        public bool IsRunning { get; private set; }
        private bool _stopPending;

        public void Start()
        {
            if(IsRunning) return;
            _stopPending=false;
            Task.Run(Processing);

        }

        public void Stop()
        {
            _stopPending=true;

        }


        private void Processing()
        {
            IsRunning = true;
            foreach (var folder in MyPageSettings.Instance.ScanFolders)
            {
                foreach (var file in Directory.EnumerateFiles(folder, "*.piz", SearchOption.AllDirectories))
                {
                    Debug.WriteLine(file);
                    IndexFileChanged?.Invoke(this,file);


                    if(_stopPending) break;

                    var poCo = new PageDocumentPoCo() { FilePath = file, 
                        Name = Path.GetFileNameWithoutExtension(file),
                        Indexed = 0
                    };
                    poCo.CheckInfo();
                    MyPageDb.Instance.InsertUpdateDocument(poCo);

                }
            }

            IsRunning = false;
            IndexStopped?.Invoke(this,EventArgs.Empty);
        }

        

    }
}
