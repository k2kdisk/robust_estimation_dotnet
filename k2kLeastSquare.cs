using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace k2kLib
{
	public class k2kLeastSquare
	{
		public virtual double[] GetSolution(k2kObservationEquation equation)
		{
			throw new NotImplementedException();
		}
	}

	public class k2kModGramSchmidt : k2kLeastSquare
	{
		public override double[] GetSolution(k2kObservationEquation equation)
		{
			var objective = equation.Objective;
			var explaining = equation.Explaining;
			var Q = explaining.Select(arr => (double[])arr.Clone()).ToArray();
			var R = explaining.Select(arr => new double[arr.Length]).ToArray();
			var Approximation = new double[explaining[0].Length];
			var nRows = objective.Length;
			var nCols = R.Length;


			//Get QR
			int row, col, col2;
			double norm, work;
			for (col = 0; col < nCols; ++col)
			{
				norm = 0;
				for (row = 0; row < nRows; ++row)
					norm += Q[row][col] * Q[row][col];
				norm = Math.Sqrt(norm);

				R[col][col] = norm;
				for (row = 0; row < nRows; ++row)
					Q[row][col] = Q[row][col] / norm;
				for (col2 = col + 1; col2 < nCols; ++col2)
				{
					work = 0;
					for (row = 0; row < nRows; ++row)
						work += Q[row][col] * Q[row][col2];
					R[col][col2] = work;
					for (row = 0; row < nRows; ++row)
						Q[row][col2] -= Q[row][col] * work;
				}
			}


			//Solve QR
			double[] Z = new double[R.Length];
			double[] Y = (double[])objective.Clone();

			for (col = 0; col < nCols; ++col)
			{
				Z[col] = 0;
				for (row = 0; row < nRows; ++row)
					Z[col] += Q[row][col] * Y[row];
				//for (row = 0; row < nRows; ++row) Y[row] -= Z[col] * Y[row];
			}
			for (col = nCols - 1; col >= 0; --col)
			{
				work = 0;
				for (col2 = col + 1; col2 < nCols; ++col2)
					work += R[col][col2] * Approximation[col2];
				Approximation[col] = (Z[col] - work) / R[col][col];
			}


			return Approximation;
		}
	}

	public class k2kObservationEquation : List<k2kObservedData>
	{
		public k2kObservationEquation() : base()
		{
		}

		public k2kObservationEquation(IEnumerable<k2kObservedData> collection) : base(collection)
		{
		}

		public double[] Objective
		{
			get
			{
				double[] ret = new double[Count];
				for (int i = 0; i < Count; ++i)
					ret[i] = this[i].Objective;
				return ret;
			}
		}

		public double[][] Explaining
		{
			get
			{
				double[][] ret = new double[Count][];
				for (int i = 0; i < Count; ++i)
				{
					ret[i] = new double[this[i].Length];
					this[i].Explaining.CopyTo(ret[i], 0);
				}
				return ret;
			}
		}

		public void Add(double Objective, params double[] Explaining)
		{
			Add(new k2kObservedData(Objective, Explaining));
		}

	}

	public class k2kObservedData
	{
		public k2kObservedData(double Objective, params double[] Explaining)
		{
			this.Objective = Objective;
			this.Explaining = Explaining;
		}

		public k2kObservedData(int ExplainingCount)
		{
			this.Explaining = new double[ExplainingCount];
		}

		public double Objective;
		public double[] Explaining;

		public int Length { get { return Explaining.Length; } }
	}





}
