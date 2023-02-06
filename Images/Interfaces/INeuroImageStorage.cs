
using DapperImageStore.Models;
using DataAccessLibrary.Models;
using RazorPages.Identity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLibrary.Interfaces;


/// <summary>
/// Интерфейс, к которому обращаются другие проекты для получения и сохранения изображений. Ключами используются числовые id.
/// Все get-методы возвращают <see cref="NeuroImageResult"/>.
/// </summary>
public interface INeuroImageStorage
{

    /// <summary>
    /// Сохраняет изображение в базе данных. Файл копируется из copyImageFrom, 
    /// при необходимости оригинальный файл по этому адресу можно удалить.
    /// </summary>
    Task<NeuroImageResult> StoreCopy(string copyImageFrom, NeuroImageInfo info, bool deleteOriginal = false);

    /// <summary>
    /// Перезаписывает информацию в сохраненном изображении по id на предоставленную
    /// </summary>
    Task<NeuroImageResult> UpdateInfo(int idToUpdate, NeuroImageInfo info);

    /// <summary>
    /// Перезаписывает файл в сохраненном изображении по id, заменяя его на копию картинки по предоставленному адресу. При
    /// необходимости оригинальный файл по этому адресу можно удалить.
    /// </summary>
    Task<NeuroImageResult> UpdateFile(int idToUpdate, string copyFrom, bool deleteOriginal = false);

    /// <summary>
    /// Удаляет изображение.
    /// </summary>
    Task Delete(int id);

    /// <summary>
    /// Возвращает изображение.
    /// </summary>
    Task<NeuroImageResult> GetById(int id);

    /// <summary>
    /// Возвращает все изображения пользователя определенного типа.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<NeuroImageResult>> GetAllOfUserOfStatus(string userID, ImageStatus status);











}
