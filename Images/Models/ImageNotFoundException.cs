using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Images.Models;

public class ImageNotFoundException : Exception
{
    public ImageNotFoundException() { }
    public ImageNotFoundException(string message) : base(message) { }
}
