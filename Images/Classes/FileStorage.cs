using DataAccessLibrary.Classes.Options;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLibrary.Interfaces.IFileStorage;

namespace DataAccessLibrary.Classes
{
    /// <summary>
    /// Имплементация <see cref="IFileStorage"/>, хранящая изображения в файловой системе в директории, указанной в
    /// <see cref="FileStorageOptions"/>. Названия файлов генерируются с помощью Guid.
    /// </summary>
    public class FileStorage : IFileStorage
    {
        private readonly IOptions<FileStorageOptions> _options;

        public FileStorage(IOptions<FileStorageOptions> options)
        {
            _options = options;
        }

        private string NameToPath(string name) => Path.Combine(_options.Value.fileStorageAbsolutePath, name);

        public FileResult StoreCopy(string copyFrom, bool deleteOriginal = false)
        {
            string newName = Guid.NewGuid().ToString() + Path.GetExtension(copyFrom);

            string storePath = NameToPath(newName);

            File.Copy(copyFrom, storePath);

            if (deleteOriginal)
                File.Delete(copyFrom);

            return new FileResult(newName, storePath);
        }

        public void Delete(string name)
        {
            string path = NameToPath(name);
            File.Delete(path);
        }

        public FileResult Replace(string copyFrom, string name, bool deleteOriginal = false)
        {

            Delete(name);
            return StoreCopy(copyFrom, deleteOriginal);
           
        }

        public string GetPath(string name)
        {
            string path = NameToPath(name);

            if (!File.Exists(path))
                throw new FileNotFoundException();

            return path;
        }
    }
}
