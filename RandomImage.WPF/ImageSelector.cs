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
        private int currentIndex = 0;

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

            Random random = new Random();
            imagesPath = imagesPath
                .Select(x => new { Number = random.Next(), Item = x })
                .OrderBy(x => x.Number)
                .Select(x => x.Item)
                .ToList();
        }

        public int ImageFilesCount => imagesPath.Count();

        public string Next()
        {
            currentIndex += 1;

            if (currentIndex >= ImageFilesCount)
            {
                currentIndex = 0;
            }

            FileInfo fileInfo = imagesPath
                .Skip(currentIndex)
                .Take(1)
                .First();

            return fileInfo.FullName;
        }

        public string Previous()
        {
            currentIndex -= 1;

            if (currentIndex < 0)
            {
                currentIndex = ImageFilesCount - 1;
            }

            FileInfo fileInfo = imagesPath
                .Skip(currentIndex)
                .Take(1)
                .First();

            return fileInfo.FullName;
        }
    }
}