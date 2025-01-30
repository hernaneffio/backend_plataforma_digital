using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payload.File;

public class CreateFileNewPayload
{
    public string base64File { get; set; }

    public string fileName { get; set; }

    public string base64Firma { get; set; }
}
