using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Interfaces;

public interface IFileRepository
{
    public FileResult StoreCopy(string copyFrom, bool deleteOriginal = false);

    public FileResult Replace(string copyFrom, string name, bool deleteOriginal = false);

    public void Delete(string name);

    public string GetPath(string name);
}
