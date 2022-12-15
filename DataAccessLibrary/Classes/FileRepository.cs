using DataAccessLibrary.Classes.Options;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLibrary.Interfaces.IFileRepository;

namespace DataAccessLibrary.Classes
{
    public class FileRepository : IFileRepository
    {
        private readonly IOptions<FileRepositoryOptions> _options;

        public FileRepository(IOptions<FileRepositoryOptions> options)
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
