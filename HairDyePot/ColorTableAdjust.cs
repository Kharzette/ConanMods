using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DyeColoursToHairColours
{
	class Program
	{
		struct DyeData
		{
			internal Int64	ID;
			internal float	R, G, B, A;
		};

		const Int64	IDIncrement	=1000;


		static float	ParseColorComponent(string line, string prefix)
		{
			//find prefix
			int	pIdx	=line.IndexOf(prefix);
			if(pIdx == -1)
			{
				return	-1f;
			}

			//find trailing comma
			int	cIdx	=line.IndexOf(',', pIdx);
			if(cIdx == -1)
			{
				//find trailing )
				cIdx	=line.IndexOf(')', pIdx);
			}

			string	colourString	=line.Substring(pIdx + 2, cIdx - (pIdx + 2));

			float	result	=0.0f;

			if(!float.TryParse(colourString, out result))
			{
				Console.WriteLine("Parse error on: " + colourString);
				return	-1f;
			}
			return	result;
		}


		static void Main(string[] args)
		{
			if(args.Length < 1)
			{
				Console.WriteLine("Usage: DyeColoursToHairClours DyeColoursFile.csv");
				return;
			}

			if(!File.Exists(args[0]))
			{
				Console.WriteLine("No such file: " + args[0]);
				return;
			}

			FileStream	fs	=new FileStream(args[0], FileMode.Open, FileAccess.Read);
			if(fs == null)
			{
				Console.WriteLine("File open failed for " + args[0]);
				return;
			}

			StreamReader	sr	=new StreamReader(fs);
			if(sr == null)
			{
				Console.WriteLine("StreamReader failed for " + args[0]);
				return;
			}

			List<DyeData>	ddata	=new List<DyeData>();

			while(!sr.EndOfStream)
			{
				string	line	=sr.ReadLine();

				if(line.StartsWith("---"))
				{
					continue;
				}

				int	commaPos	=line.IndexOf(',');

				string	id	=line.Substring(0, commaPos);

				Int64	dyeID	=0;

				if(!Int64.TryParse(id, out dyeID))
				{
					Console.WriteLine("Parse error on ID: " + id);
					continue;
				}

				DyeData	dd	=new DyeData();

				dd.ID	=dyeID;
				dd.R	=ParseColorComponent(line, "R=");
				dd.G	=ParseColorComponent(line, "G=");
				dd.B	=ParseColorComponent(line, "B=");
				dd.A	=ParseColorComponent(line, "A=");

				ddata.Add(dd);
			}

			sr.Close();
			fs.Close();

			fs	=new FileStream("DyeToHairColors.csv", FileMode.Create, FileAccess.Write);
			if(fs == null)
			{
				Console.WriteLine("File open write failed for " + args[0]);
				return;
			}

			StreamWriter	sw	=new StreamWriter(fs);

			//dyes use colour, hair uses color
			//probably a brit doing dye, murican doing hair
			sw.WriteLine("---,Color,RequiredFeats");

			foreach(DyeData dd in ddata)
			{
				//increment the ID for insertion into other tables
				Int64	cid	=dd.ID + IDIncrement;

				sw.WriteLine(cid
					+ ",\"(R=" + dd.R.ToString("N6")
					+ ",G=" + dd.G.ToString("N6")
					+ ",B=" + dd.B.ToString("N6")
					+ ",A=" + dd.A.ToString("N6") + ")\",\"\"");
			}

			//blank line for some reason
			sw.WriteLine("");

			sw.Close();
			fs.Close();

		}
	}
}
