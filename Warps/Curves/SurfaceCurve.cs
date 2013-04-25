using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warps
{
	[Serializable()]
	class SurfaceCurve : MouldCurve
	{
		public SurfaceCurve()
			: this("", null, null) { }
		public SurfaceCurve(string label, Sail sail, IFitPoint[] fits)
			:base(label, sail)
		{
			if( fits != null && fits.Length > 1 )
				Fit(fits);
				//Fit(fits, fits[0][0] != 0 || fits.Last()[0] != 1);
		}

		/// <summary>
		/// fit a curve to the specified points
		/// interpolates interal points if less than 5
		/// optionally recalculated the S parameters
		/// </summary>
		/// <param name="fits">points to fit to, minimum 2</param>
		/// <param name="RePos">true to recalculate S paramaters, false if S parameters is supplied</param>
		public override void Fit(IFitPoint[] fits)
		{
			Fit(this, fits);
		}
		/// <summary>
		/// Will fit the spline using either InterpoFit or SimpleFit depending on the number of fitpoints
		/// </summary>
		/// <param name="c">the curve to fit</param>
		/// <param name="fits">the fit points to fit to</param>
		public static void Fit(MouldCurve c, IFitPoint[] fits)
		{
			if (fits.Length <= 1)//need at least 2 points to fit a curve
				return;

			if (fits.Length >= 5)
				SimpleFit(c, fits);
			else //linear uv interpolation to match minimum 5 points required
				InterpoFit(c, fits);
		}
		public static void InterpoFit(MouldCurve c, IFitPoint[] fits)
		{
			if (fits.Length <= 1 || fits.Length >= 5)//need at least 2 points to fit a curve, can't interpolate more than 4
				return;
			//recalculate s-positions based on xyz-values
			//if (RePos)
			//{
			//	Vect2 uv = new Vect2();
			//	Vect3 x0 = new Vect3();
			//	Vect3 x1 = new Vect3();
			//	fits[0][0] = 0;//initialize s paramter
			//	Surface.xVal(fits[0].UV, ref x0);//initialize starting x point
			//	for (int nFit = 1; nFit < fits.Length; nFit++)
			//	{
			//		Surface.xVal(fits[nFit].UV, ref x1);
			//		fits[nFit][0] = fits[nFit - 1][0] + x1.Distance(x0);
			//		x1.Set(x0);//store previous x point
			//	}
			//	for (int nFit = 0; nFit < fits.Length; nFit++)
			//		fits[nFit][0] /= fits.Last()[0];//convert length to position
			//	fits.Last()[0] = 1;//enforce unit length
			//}

			//linear uv interpolation to match minimum 5 points required


				double[][] uFits = new double[2][];
				double[] sFits = new double[5];
				uFits[0] = new double[5];
				uFits[1] = new double[5];
				double p;
				int i, nint = 0;
				Vect3 x2 = new Vect3(), x1 = new Vect3();
				Vect2 umid = new Vect2();
				Dictionary<int, int> FitstoSFits = new Dictionary<int, int>();
				if (fits.Length == 4)
				{
					//store fitpoints
					sFits[nint] = fits[nint][0] = 0;
					uFits[0][nint] = fits[nint][1];
					uFits[1][nint] = fits[nint][2];
					c.xVal(fits[nint].UV, ref x1);
					FitstoSFits[nint] = nint;
					nint++;
					//store fitpoints
					//sFits[nint] = fits[nint][0];
					uFits[0][nint] = fits[nint][1];
					uFits[1][nint] = fits[nint][2];
					c.xVal(fits[nint].UV, ref x2);
					sFits[nint] = x2.Distance(x1) + sFits[nint - 1];
					FitstoSFits[nint] = nint;
					nint++;
					//insert midpoint
					//sFits[nint] = (fits[nint][0] + fits[nint - 1][0]) / 2.0;
					umid = fits[nint].UV + fits[nint - 1].UV;
					umid /= 2;
					uFits[0][nint] = umid[0];
					uFits[1][nint] = umid[1];
					c.xVal(umid, ref x1);
					sFits[nint] = x2.Distance(x1) + sFits[nint - 1];
					nint++;
					//store fitpoints
					//sFits[nint] = fits[nint-1][0];
					uFits[0][nint] = fits[nint - 1][1];
					uFits[1][nint] = fits[nint - 1][2];
					c.xVal(fits[nint - 1].UV, ref x2);
					sFits[nint] = x2.Distance(x1) + sFits[nint - 1];
					FitstoSFits[nint - 1] = nint;
					nint++;
					//store fitpoints
					//sFits[nint] = fits[nint-1][0];
					uFits[0][nint] = fits[nint - 1][1];
					uFits[1][nint] = fits[nint - 1][2];
					c.xVal(fits[nint - 1].UV, ref x1);
					sFits[nint] = x2.Distance(x1) + sFits[nint - 1];
					FitstoSFits[nint - 1] = nint;
					nint++;
				}
				else
				{
					int num = (5 - fits.Length) / (fits.Length - 1);//insertion points
					num = num < 1 ? 1 : num;
					for (i = 0; i < fits.Length - 1; i++)
					{
						//store fitpoints
						//sFits[nint] = fits[i][0];
						uFits[0][nint] = fits[i][1];
						uFits[1][nint] = fits[i][2];
						FitstoSFits[i] = nint;
						c.xVal(fits[i].UV, ref x1);
						if (i == 0)
						{
							sFits[nint] = 0;
						}
						else
						{
							sFits[nint] = x2.Distance(x1) + sFits[nint - 1];
						}
						x2.Set(x1);//store xprev
						nint++;
						for (int j = 1; j <= num; j++)
						{
							//interpolate insertion points and store
							p = BLAS.interpolant(j, num + 2);
							//sFits[nint] = BLAS.interpolate(p, fits[i + 1][0], fits[i][0]);
							umid[0] = uFits[0][nint] = BLAS.interpolate(p, fits[i + 1][1], fits[i][1]);
							umid[1] = uFits[1][nint] = BLAS.interpolate(p, fits[i + 1][2], fits[i][2]);
							c.xVal(umid, ref x1);
							sFits[nint] = x2.Distance(x1) + sFits[nint - 1];
							x2.Set(x1);//store xprev
							nint++;
						}
					}
					//endpoint
					//sFits[nint] = 1;
					uFits[0][nint] = fits[i][1];
					uFits[1][nint] = fits[i][2];
					c.xVal(fits[i].UV, ref x1);
					sFits[nint] = x2.Distance(x1) + sFits[nint - 1];
					System.Diagnostics.Debug.Assert(nint == 4);
					FitstoSFits[i] = nint;

				}

				for (i = 0; i < sFits.Length; i++)
					sFits[i] /= sFits.Last();
				//sFits[sFits.Length - 1] = 1;

				foreach (int key in FitstoSFits.Keys)
					fits[key][0] = sFits[FitstoSFits[key]];

				c.FitPoints = fits;
				c.ReSpline(sFits, uFits);
		}
		
		/// <summary>
		/// fit a curve to the specified points
		/// requires S parameter specified for each point
		/// requires at least 5 points
		/// </summary>
		/// <param name="fits">points to fit to, minimum 5</param>
		public static void SimpleFit(MouldCurve c, IFitPoint[] fits)
		{
			Vect2 uv = new Vect2();
			Vect3 x0 = new Vect3();
			Vect3 x1 = new Vect3();
	

			double[][] uFits = new double[2][];
			double[] sFits = new double[fits.Length];
			uFits[0] = new double[fits.Length];
			uFits[1] = new double[fits.Length];
			//complile fitpoints to fitting arrays
			sFits[0] = 0;
			fits[0][0] = 0;
			c.xVal(fits[0].UV, ref x0);//initialize starting x point
			for (int nFit = 0; nFit < fits.Length; nFit++)
			{
				//sFits[nFit] = fits[nFit][0];
				uFits[0][nFit] = fits[nFit][1];
				uFits[1][nFit] = fits[nFit][2];

				if (nFit > 0)
				{
					c.xVal(fits[nFit].UV, ref x1);
					fits[nFit][0] = fits[nFit - 1][0] + x1.Distance(x0);
					x0.Set(x1);//store previous x point
				}
			}

			for (int nFit = 0; nFit < fits.Length; nFit++)
				sFits[nFit] = fits[nFit][0] /= fits.Last()[0];//convert length to position
			//sFits[sFits.Length-1] = fits.Last()[0] = 1;//enforce unit length

			c.FitPoints = fits;
			c.ReSpline(sFits, uFits);
		}
	}
}
