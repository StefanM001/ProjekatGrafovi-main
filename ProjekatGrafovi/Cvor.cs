using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatGrafovi
{
	public class Cvor
	{
		public int Id { get; set; }
		public double X { get; set; }
		public double Y { get; set; }

		public Cvor(int id, double x, double y)
		{
			Id = id;
			X = x;
			Y = y;
		}

		public Cvor()
		{
		}
	}
}
