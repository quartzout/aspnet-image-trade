using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Images.Models;

/// <summary>
/// Определяет, какой у изображения статус и какие пользователи могут его видеть
/// </summary>
public enum ImageStatus
{
    /// <summary>
    /// Пользователь выложил сгенерированную модель в галерею, где ее могут увидеть все пользователи, в том числе анонимные.
    /// </summary>
    InGallery,
    /// <summary>
    /// Изображение было сгенерировано пользователем, но не выложено в галерею. Доступ к таким имеет только их автор.
    /// </summary>
    InHeap,
    /// <summary>
    /// Пользователь выставил снегерированную модель или модель из галереи на продажу. 
		/// Ее могут увидеть все пользователи, в том числе анонимные.
    /// </summary>
    OnSale
}


