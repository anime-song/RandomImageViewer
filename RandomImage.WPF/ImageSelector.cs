using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomImageViewer.WPF
{
    public sealed class ImageSelector
    {
        private readonly IEnumerable<FileInfo> imagesPath;

        public ImageSelector(DirectoryInfo imageDirectory)
        {
            imagesPath = imageDirectory.EnumerateFiles();

            imagesPath = imagesPath.Where(file =>
            {
                if (file.Extension == ".jpg" ||
                    file.Extension == ".jpeg" ||
                    file.Extension == ".png" ||
                    file.Extension == ".jfif")
                {
                    return true;
                }
                return false;
            });
        }

        public int ImageFilesCount => imagesPath.Count();

        public string Next()
        {
            int index = new Random().Next(ImageFilesCount);
            FileInfo fileInfo = imagesPath
                .Skip(index)
                .Take(1)
                .First();

            return fileInfo.FullName;
        }

        public string Previous()
        {
            int index = new Random().Next(ImageFilesCount);
            FileInfo fileInfo = imagesPath
                .Skip(index)
                .Take(1)
                .First();

            return fileInfo.FullName;
        }
    }
}