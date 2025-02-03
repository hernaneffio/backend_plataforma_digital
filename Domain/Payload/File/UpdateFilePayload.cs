using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payload.File;

public class UpdateFilePayload
{
    public int id { get; set; }

    public string base64File { get; set; }
}
