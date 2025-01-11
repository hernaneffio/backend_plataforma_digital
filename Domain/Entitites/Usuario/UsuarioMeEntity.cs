using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entitites.Usuario;

public class UsuarioMeEntity
{
    public int id { get; set; }

    public string nombre { get; set; }

    public string apellido { get; set; }

    public string user { get; set; }

    public bool estado { get; set; }
}
