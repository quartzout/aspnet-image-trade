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
/// <see cref="PathedNeuroImageResult"/>.
/// Post-методы принимают изображение на хранение в модели <see cref="PathedNeuroImage"/>, чтобы вместе с информацией хранить
/// название файла изображения.
/// </summary>
public interface IInfoStorage
{

    Task<PathedNeuroImageResult> Get(int id);

    Task Delete(int id);

    Task<IEnumerable<PathedNeuroImageResult>> GetAllOfUserOfStatus(string userId, ImageStatus type);

    Task<PathedNeuroImageResult> Create(PathedNeuroImage info);

    Task<PathedNeuroImageResult> Replace(int id, PathedNeuroImage image);

    

}
