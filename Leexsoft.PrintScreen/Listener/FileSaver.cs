using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Leexsoft.PrintScreen.Listener
{
    public class FileSaver
    {
        private int CurrentNo { get; set; }
        private string PicExtension { get; set; }

        public FileSaver()
        {
            this.CurrentNo = 0;
            this.PicExtension = ".jpg";
        }

        public void SaveImg(Bitmap bmp, string savePath)
        {
            if (bmp != null)
            {
                bmp.Save(this.GetFileName(savePath), ImageFormat.Jpeg);
            }
        }

        private string GetFileName(string savePath)
        {
            if (this.CurrentNo <= 0)
            {
                var directoryInfo = new DirectoryInfo(savePath);
                var files = directoryInfo.GetFiles(string.Format("*{0}", this.PicExtension));
                if (files.Count() > 0)
                {
                    this.CurrentNo = files.Select(f =>
                    {
                        int name = 0;
                        if (int.TryParse(Path.GetFileNameWithoutExtension(f.FullName), out name))
                        {
                            return name;
                        }
                        else
                        {
                            return 0;
                        }
                    }).Max();
                }
            }
            var filename = string.Format("{0:d5}{1}", ++this.CurrentNo, this.PicExtension);
            return Path.Combine(savePath, filename);
        }
    }
}
