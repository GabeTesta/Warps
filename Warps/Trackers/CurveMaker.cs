using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warps
{
	public class CurveMaker : ITracker
	{
		public CurveMaker(WarpFrame frame, Sail s)
		{
			m_frame = frame;
			m_sail = s;
			m_edit = new CurveImportEditor(this);
		}

		public void Track(WarpFrame frame)
		{
			m_frame = frame;
			if (m_frame != null)
				m_frame.EditorPanel = m_edit;
		}
		bool m_editMode = false;

		public bool EditMode
		{
			get { return m_editMode; }
			set { m_editMode = value; }
		}

		WarpFrame m_frame;
		Sail m_sail;
		MouldCurve m_curve;
		CurveImportEditor m_edit;

		public MouldCurve Curve
		{
			get { return m_curve; }
			set { m_curve = value; }
		}

		#region ITracker Members

		public void OnCancel(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnBuild(object sender, EventArgs e)
		{
			if (Curve == null || !EditMode)
				return;
			if (m_sail.FindCurve(Curve.Label) == null)
			{
				CurveGroup cg = (m_sail.Layout.Last() as CurveGroup);
				cg.Add(Curve);
				cg.Update();//rebuild
			}
			Curve.Update();//rebuild
			if (m_frame.AutoBuild)
				m_sail.Rebuild(m_curve);

		}

		public void OnImport(object sender, EventArgs e)
		{
			string script = m_edit.Script;
			if (script.StartsWith("GIRTH"))
				CreateGirth(script);
			else if (script.StartsWith("CURVE"))
				CreateCurve(script);
			else
				return;
			m_edit.Curve = Curve;
		}

		private void CreateCurve(string script)
		{
			List<IFitPoint> fits = new List<IFitPoint>();
			int nS = script.IndexOf("tarting");
			nS = script.IndexOf(' ', nS);
			fits.Add(ReadFitPoint(script, ++nS));
			while ((nS = script.IndexOf("hrough", nS)) > 0)
			{
				nS += 6;
				while (script[++nS] == ' ') ;//skip spaces
				fits.Add(ReadFitPoint(script, nS));
			}
			nS = script.IndexOf("topping");
			nS = script.IndexOf(' ', nS);
			fits.Add(ReadFitPoint(script, ++nS));

			if (Curve == null)
				Curve = new MouldCurve(script.Substring(7, 5), m_sail, fits.ToArray());
			else
			{
				Curve.Label = script.Substring(7, 5);
				Curve.Fit(fits.ToArray());
			}
		}

		private void CreateGirth(string script)
		{
			IFitPoint[] fits = new IFitPoint[2];
			int nS = script.IndexOf("tarting");
			nS = script.IndexOf(' ', nS);
			fits[0] = ReadFitPoint(script, ++nS);
			nS = script.IndexOf("topping");
			nS = script.IndexOf(' ', nS);
			fits[1] = ReadFitPoint(script, ++nS);

			if( Curve == null )
				Curve = new MouldCurve(script.Substring(7, 5), m_sail, fits);
			else
			{
				Curve.Label = script.Substring(7, 5);
				Curve.Fit(fits.ToArray());
			}
		}

		IFitPoint ReadFitPoint(string script, int nType)
		{
			int nE, nS = nType+5;
			double u, v;
			string curve;
			MouldCurve cur;
			string type = script.Substring(nType, 5);
			switch (type)
			{
				case "POINT":
					nS = script.IndexOf('[', nS);
					nE = script.IndexOf(';', ++nS);
					curve = script.Substring(nS, nE - nS);
					double.TryParse(curve, out u);
					nS = script.IndexOf(']', ++nE);
					curve = script.Substring(nE, nS - nE);
					double.TryParse(curve, out v);
					return new FixedPoint(u, v);
				case "CURVE":
					nS = script.IndexOf('[', nS);
					nE = script.IndexOf(';', ++nS);
					curve = script.Substring(nS, nE - nS).Trim();
					cur = m_sail.FindCurve(curve);
					nS = script.IndexOf(']', ++nE);
					curve = script.Substring(nE, nS - nE);
					double.TryParse(curve, out v);
					return new CurvePoint(cur, v);
				case "SLIDE":
					nS = script.IndexOf('[', nS);
					nE = script.IndexOf(';', ++nS);
					curve = script.Substring(nS, nE - nS);
					cur = m_sail.FindCurve(curve);
					nS = script.IndexOf(']', ++nE);
					double.TryParse(script.Substring(nE, nS - nE), out v);
					return new SlidePoint(cur, v);
			}
			return null;
		}


		public void OnCancel()
		{
			throw new NotImplementedException();
		}

		public void OnSelect(object sender, EventArgs<IRebuild> e)
		{
			throw new NotImplementedException();
		}

		public void OnClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public bool IsTracking(object obj)
		{
			return obj == m_curve;
		}

		public void OnEditor(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnPreview(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}


		public void OnCopy(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		public void OnPaste(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}
	
		#endregion

	}
}
