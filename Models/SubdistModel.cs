using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MappingSubdist.Models
{
	public class SubdistModel
	{ 
		public string Sp { set; get; }
		public string Option { set; get; } 
		public string KodeCabang { set; get; } 
		public string KodeSubdist { set; get; } 
		public string NamaSubdist { get; set; }
		public string IndukKodeSubdist { get; set; }
		public string GroupSPB { get; set; }
		public string Region { set; get; } 
		public string Username { set; get; } 
		public string Password { set; get; } 
		public string Email { set; get; } 
		public int Phone { set; get; } 
	}
	 
}