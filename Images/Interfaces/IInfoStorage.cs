using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Images.Models;
using Users.Identity.Classes;

namespace Images.Interfaces;


/// <summary>
/// Интерфейс, хранящий информацию о изображениях. Ключами являются числовые id. Все методы возвращают изображения в модели
/// <see cref="PathedImageResult"/>.
/// Post-методы принимают изображение на хранение в модели <see cref="PathedImage"/>, чтобы вместе с информацией хранить
/// название файла изображения.
/// </summary>
public interface IInfoStorage
{

    Task<PathedImageResult> Get(int id);

    Task Delete(int id);

    Task<IEnumerable<PathedImageResult>> GetAllOfUserOfStatus(string userId, ImageStatus type);

    Task<PathedImageResult> Create(PathedImage info);

    Task<PathedImageResult> Replace(int id, PathedImage image);

    

}
