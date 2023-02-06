using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Interfaces;

/// <summary>
/// Интерфейс, хранящий файлы изображений. Ключами являются имена файлов (не пути). 
/// Get-методы возвращают <see cref="FileResult"/>
/// </summary>
public interface IFileStorage
{
    public FileResult StoreCopy(string copyFrom, bool deleteOriginal = false);

    public FileResult Replace(string copyFrom, string name, bool deleteOriginal = false);

    public void Delete(string name);

    public string GetPath(string name);
}
