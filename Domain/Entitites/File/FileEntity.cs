using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entitites.File;

public class FileEntity
{
    public int id { get; set; }

    public string fileName { get; set; }

    public string fileRuta { get; set; }

    public DateTime fecha { get; set; }

    public bool estado { get; set; }
}
