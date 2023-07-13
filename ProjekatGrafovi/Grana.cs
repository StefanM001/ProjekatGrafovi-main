using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatGrafovi
{
	public class Grana
	{
		public Cvor prvi { get; set; }
		public Cvor drugi { get; set; }

		public bool KruznaPrva { get; set; }
		public Grana(Cvor prvi, Cvor drugi)
		{
			this.prvi = prvi;
			this.drugi = drugi;
			KruznaPrva = false;
		}
	}
}
