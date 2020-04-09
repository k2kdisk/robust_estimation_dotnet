
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;

namespace k2kLib
{
	public class k2kMath3D
	{
		public static double[] GetIntersection (double x1, double y1, double z1, double x2, double y2, double z2, double a, double b, double c)
		{
			var dx = x2 - x1;
			var dy = y2 - y1;
			var dz = z2 - z1;
			if (a * dx + b * dy - dz == 0)
				return null;

			var t = (a * x1 + b * y1 + c - z1) / (dz - a * dx - b * dy);
			return new double[] { x1 + t * dx, y1 + t * dy, z1 + t * dz };
		}

		public static double[] GetIntersection (double x1, double y1, double z1, double x2, double y2, double z2, double[] plane)
		{
			return GetIntersection (x1, y1, z1, x2, y2, z2, plane [0], plane [1], plane [2]);
		}

		public static double[] GetIntersection (double[] p, double[] q, double[] plane)
		{
			return GetIntersection (p [0], p [1], p [2], q [0], q [1], q [2], plane [0], plane [1], plane [2]);
		}

		public static k2kPoint3D GetIntersection (k2kPoint3D p, k2kPoint3D q, double[] plane)
		{
			return GetIntersection (p, q, plane [0], plane [1], plane [2]);
		}
		public static k2kPoint3D GetIntersection (k2kPoint3D p, k2kPoint3D q, double a, double b, double c)
		{
			return new k2kPoint3D (GetIntersection (p.X, p.Y, p.Z, q.X, q.Y, q.Z, a, b, c));
		}
	}


	public class k2kPoint3D
	{
		public static k2kPoint3D ZeroPoint;
		static k2kPoint3D ()
		{
			ZeroPoint = new k2kPoint3D (0, 0, 0);
		}

		public double X;
		public double Y;
		public double Z;

		public float Xf { get { return (float)X; } }
		public float Yf { get { return (float)Y; } }
		public float Zf { get { return (float)Z; } }
		public int Xi { get { return (int)X; } }
		public int Yi { get { return (int)Y; } }
		public int Zi { get { return (int)Z; } }

		public k2kPoint3D (double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public k2kPoint3D (double[] coord)
		{
			X = coord [0];
			Y = coord [1];
			Z = coord [2];
		}
		public k2kPoint3D (float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public k2kPoint3D (float[] coord)
		{
			X = coord [0];
			Y = coord [1];
			Z = coord [2];
		}
		public k2kPoint3D (int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public k2kPoint3D (int[] coord)
		{
			X = coord [0];
			Y = coord [1];
			Z = coord [2];
		}

		public static k2kPoint3D operator * (k2kPoint3D p, double k)
		{
			return new k2kPoint3D (p.X * k, p.Y * k, p.Z * k);
		}
		public static k2kPoint3D operator * (double k, k2kPoint3D p)
		{
			return p * k;
		}
		public static k2kPoint3D operator / (k2kPoint3D p, double k)
		{
			return p * (1 / k);
		}
		public static k2kPoint3D operator - (k2kPoint3D p)
		{
			return p * (-1);
		}
		public static k2kPoint3D operator + (k2kPoint3D p1, k2kPoint3D p2)
		{
			return new k2kPoint3D (p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
		}
		public static k2kPoint3D operator - (k2kPoint3D p1, k2kPoint3D p2)
		{
			return p1 + (p2 * (-1));
		}

		public double Dot (k2kPoint3D vec)
		{
			return X * vec.X + Y * vec.Y + Z * vec.Z;
		}

		public double Norm { get { return k2kMath.GetDistance (X, Y, Z); } }
		public k2kPoint3D GetNormalized ()
		{
			return this / this.Norm;
		}

		public k2kPoint3D GetComponent (k2kPoint3D DirectionVector)
		{
			return DirectionVector * Dot (DirectionVector) /
				(
                        DirectionVector.X * DirectionVector.X +
				DirectionVector.Y * DirectionVector.Y +
				DirectionVector.Z * DirectionVector.Z
                    );
		}
		public k2kPoint3D GetComponentByUnit (k2kPoint3D DirectionUnitVector)
		{
			return DirectionUnitVector * Dot (DirectionUnitVector);
		}

		public override string ToString ()
		{
			return string.Format ("(\t{0}\t,\t{1}\t,\t{2}\t)", X, Y, Z);
		}

		public double[] ToArray ()
		{
			return new double[] { X, Y, Z };
		}
		public PointF ToPointF ()
		{
			return new PointF (Xf, Yf);
		}
		public Point ToPoint ()
		{
			return new Point (Xi, Yi);
		}




		public static double[][] GetRotationMatrixZYX (double Omega, double Phi, double Kappa)
		{
			double so = Math.Sin (-Omega);
			double co = Math.Cos (-Omega);
			double sp = Math.Sin (-Phi);
			double cp = Math.Cos (-Phi);
			double sk = Math.Sin (-Kappa);
			double ck = Math.Cos (-Kappa);
			return new double[][]{
                                    new double[]{ck * cp,-sk * cp,sp},
                                    new double[]{ck * so * sp + sk * co,ck * co - sk * so * sp,-so * cp},
                                    new double[]{sk * so - ck * co * sp,sk * co * sp + ck * so,co * cp}
                                };
		}
		public static double[][] GetRotationMatrixXYZ (double Omega, double Phi, double Kappa)
		{
			throw new NotImplementedException ();
			double so = Math.Sin (-Omega);
			double co = Math.Cos (-Omega);
			double sp = Math.Sin (-Phi);
			double cp = Math.Cos (-Phi);
			double sk = Math.Sin (-Kappa);
			double ck = Math.Cos (-Kappa);
			return new double[][]{
                                    new double[]{1,1,1},
                                    new double[]{1,1,1},
                                    new double[]{1,1,1}
                                };
		}

		public k2kPoint3D GetRotate (double[][] RotationMatrix)
		{
			var ret = new double[3];
			var vec = new double[] { this.X, this.Y, this.Z };
			int i, j;

			for (i = 0; i < 3; ++i) {
				ret [i] = 0;
				for (j = 0; j < 3; ++j)
					ret [i] += RotationMatrix [i] [j] * vec [j];
			}
			return new k2kPoint3D (ret);
		}

		public k2kPoint3D GetRotateZYX (double Omega, double Phi, double Kappa)
		{
			return GetRotate (GetRotationMatrixZYX (Omega, Phi, Kappa));
		}
		public k2kPoint3D GetRotateXYZ (double Omega, double Phi, double Kappa)
		{
			return GetRotate (GetRotationMatrixXYZ (Omega, Phi, Kappa));
		}
	}



	public class k2kRotation3D
	{
		public k2kRotation3D () : this(0, 0, 0)
		{
		}
		public k2kRotation3D (double Omega, double Phi, double Kappa)
		{
			this.Omega = Omega;
			this.Phi = Phi;
			this.Kappa = Kappa;

			so = Math.Sin (-Omega);
			co = Math.Cos (-Omega);
			sp = Math.Sin (-Phi);
			cp = Math.Cos (-Phi);
			sk = Math.Sin (-Kappa);
			ck = Math.Cos (-Kappa);

			MatrixZYX = new double[][]{  new double[]{ck * cp,-sk * cp,sp},
                                        new double[]{ck * so * sp + sk * co,ck * co - sk * so * sp,-so * cp},
                                        new double[]{sk * so - ck * co * sp,sk * co * sp + ck * so,co * cp}};
		}

		protected double Omega, Phi, Kappa;
		protected double so, co, sp, cp, sk, ck;
		protected double[][] MatrixZYX, MatrixXYZ;

		public double[][] RotationMatrixZYX { get { return MatrixZYX; } }
		public double[][] RotationMatrixXYZ { get { return MatrixXYZ; } }

		protected k2kPoint3D Rotate (k2kPoint3D vec, double[][] RotationMatrix)
		{
			var ret = new double[3];
			var tmp = new double[] { vec.X, vec.Y, vec.Z };
			int i, j;

			for (i = 0; i < 3; ++i) {
				ret [i] = 0;
				for (j = 0; j < 3; ++j)
					ret [i] += RotationMatrix [i] [j] * tmp [j];
			}
			return new k2kPoint3D (ret);
		}

		public k2kPoint3D RotateZYX (k2kPoint3D vec)
		{
			return Rotate (vec, MatrixZYX);
		}
		public k2kPoint3D RotateXYZ (k2kPoint3D vec)
		{
			return Rotate (vec, MatrixXYZ);
		}
		public k2kPoint3D RotateZYX (double[] vec)
		{
			return Rotate (new k2kPoint3D (vec), MatrixZYX);
		}
		public k2kPoint3D RotateXYZ (double[] vec)
		{
			return Rotate (new k2kPoint3D (vec), MatrixXYZ);
		}
		public k2kPoint3D RotateZYX (double x, double y, double z)
		{
			return Rotate (new k2kPoint3D (x, y, z), MatrixZYX);
		}
		public k2kPoint3D RotateXYZ (double x, double y, double z)
		{
			return Rotate (new k2kPoint3D (x, y, z), MatrixXYZ);
		}
	}

















}
