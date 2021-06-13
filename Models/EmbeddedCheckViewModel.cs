﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Models
{
    public class EmbeddedCheckViewModel : CheckViewModel
    {

        public override string ToString()
        {
            return $"{ID},{PrescriptionID},{Flag},{TimeStamp}";
        }
    }
}
