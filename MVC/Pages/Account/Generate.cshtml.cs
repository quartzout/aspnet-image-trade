using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp174.Models.Interfaces;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using RazorPages.Models.Classes.UI;
using RazorPages.Models.Implementations;
using static System.Net.Mime.MediaTypeNames;

namespace Webapp174;

[Authorize]
public class Generate : PageModel
{

}