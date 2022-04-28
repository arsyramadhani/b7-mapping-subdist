using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MappingSubdist.Models
{
    public class InsertSubdistModel
    {
        public string KodeSubdist { get; set; }
        public string NamaSubdist { get; set; }
        public string Region { get; set; }
        public string Cabang { get; set; }
        public string Group { get; set; }
        public string GroupDesc { get; set; }
        public string GroupSPB { get; set; }
        public string PIC { get; set; }
        public string NPWP { get; set; }
        public string Alamat { get; set; }
    }
}