
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace k2kLib
{



    public class k2kScreen
    {
        protected k2kScreen(int Width, int Height, k2kMainPlane MainPlane)
        {
            this.Width = Width;
            this.Height = Height;
            this.MainPlane = MainPlane;
        }

        public k2kScreen(int Width, int Height, k2kMainPlane MainPlane, k2kPoint3D LeftTop, k2kPoint3D RightBottom, k2kPoint3D YDirection)
            : this(Width, Height, MainPlane)
        {
            var ScreenVec = RightBottom - LeftTop;
            var HeightVec = ScreenVec.GetComponent(YDirection);
            var WidthVec = ScreenVec - HeightVec;
            UnitX = WidthVec / Width;
            UnitY = HeightVec / Height;
            this.LeftTop = LeftTop;
            this.RightBottom = RightBottom;
            this.RightTop = LeftTop + WidthVec;
            this.LeftBottom = LeftTop + HeightVec;
        }
        public k2kScreen(int Width, int Height, k2kMainPlane MainPlane, Point LeftTop, Point RightBottom, Point LeftBottom)
            : this(
                Width, Height, MainPlane,
                MainPlane.AddZ(LeftTop),
                MainPlane.AddZ(RightBottom),
                MainPlane.AddZ(LeftBottom) - MainPlane.AddZ(LeftTop))
        { }
        /*public k2kScreen(int Width, int Height, k2kMainPlane MainPlane, k2kPoint3D Head, k2kPoint3D LeftTopHand, k2kPoint3D RightBottomHand, k2kPoint3D Torso)
            : this(
                Width, Height, MainPlane,
                MainPlane.GetIntersection(Head, LeftTopHand),
                MainPlane.GetIntersection(Head, RightBottomHand),
                //GetYDirection(MainPlane, Head, Torso)
                new k2kPoint3D(0,-1,0)
            )
        { }*/
        public k2kScreen(int Width, int Height, k2kMainPlane MainPlane, k2kPoint3D LeftTopHead, k2kPoint3D LeftTopHand, k2kPoint3D RightBottomHead, k2kPoint3D RightBottomHand)
            : this(
                Width, Height, MainPlane,
                MainPlane.GetIntersection(LeftTopHead, LeftTopHand),
                MainPlane.GetIntersection(RightBottomHead, RightBottomHand),
                new k2kPoint3D(0, -1, 0)
            )
        { }
        public k2kScreen(int Width, int Height, k2kMainPlane MainPlane, k2kPoint3D LeftTop, k2kPoint3D RightBottom)
            : this(
                Width, Height, MainPlane,
                MainPlane.GetIntersection(LeftTop),
                MainPlane.GetIntersection(RightBottom),
                new k2kPoint3D(0, -1, 0)
            )
        { }

        protected static k2kPoint3D GetYDirection(k2kMainPlane MainPlane, k2kPoint3D Head, k2kPoint3D Torso)
        {
            var BackBone = Torso - Head;
            return BackBone - BackBone.GetComponentByUnit(MainPlane.NormalVector);
        }


        public int Width { protected set; get; }
        public int Height { protected set; get; }

        public k2kMainPlane MainPlane { protected set; get; }
        public k2kPoint3D UnitX { protected set; get; }
        public k2kPoint3D UnitY { protected set; get; }

        public k2kPoint3D LeftTop { protected set; get; }
        public k2kPoint3D RightTop { protected set; get; }
        public k2kPoint3D LeftBottom { protected set; get; }
        public k2kPoint3D RightBottom { protected set; get; }

        public k2kPoint3D GetIntersection(k2kPoint3D p1, k2kPoint3D p2)
        {
            return MainPlane.GetIntersection(p1, p2);
        }
        public PointF GetImageCoord0(k2kPoint3D p1, k2kPoint3D p2)
        {
            var position = GetIntersection(p1, p2);
            var vec = position - LeftTop;
            return new PointF((float)vec.Dot(UnitX), (float)vec.Dot(UnitY));
        }
        public PointF GetIntersectionImageCoord(k2kPoint3D p1, k2kPoint3D p2)
        {
            return GetImageCoord(GetIntersection(p1, p2));
        }
        public PointF GetImageCoord(k2kPoint3D RealPosition)
        {
            var vec = RealPosition - LeftTop;
            var ComponentX = vec.GetComponent(UnitX);
            var ComponentY = vec.GetComponent(UnitY);
            return new PointF((float)(ComponentX.Norm / UnitX.Norm), (float)(ComponentY.Norm / UnitY.Norm));
        }
    }

    public class k2kMainPlane
    {
        public k2kMainPlane(k2kPoint3D[] PointCloud, k2kRobustEstimate estimator)
        {
            int i;

            var list = new k2kObservationEquation();
            for (i = 0; i < PointCloud.Length; ++i)
                list.Add(PointCloud[i].Z, PointCloud[i].X, PointCloud[i].Y, 1);

            if (list.Count < 5) throw new ArgumentException();

            var Approximation = estimator.GetMaxLikelihood(list).Approximation;
            if (Approximation == null) throw new ArgumentException();

            a = Approximation[0];
            b = Approximation[1];
            c = Approximation[2];

            NormalVector = new k2kPoint3D(a, b, -1) / k2kMath.GetDistance(a, b, -1);
        }

        public k2kMainPlane(k2kPoint3D[] PointCloud) : this(PointCloud, 100, 4) { }
        public k2kMainPlane(k2kPoint3D[] PointCloud, int IterationCount, int SampleCount)
            : this(PointCloud, new k2kLMedS(new k2kModGramSchmidt(), IterationCount, SampleCount)) { }


        /// <summary>z = ax + by + c, n = (a, b, -1)</summary>
        public double a { protected set; get; }
        /// <summary>z = ax + by + c, n = (a, b, -1)</summary>
        public double b { protected set; get; }
        /// <summary>z = ax + by + c, n = (a, b, -1)</summary>
        public double c { protected set; get; }

        public k2kPoint3D NormalVector { protected set; get; }


        public double GetDistance(k2kPoint3D p) { return (a * p.X + b * p.Y - p.Z + c) / Math.Sqrt(a * a + b * b + 1); }
        public double GetRelativeDistance(k2kPoint3D p) { return a * p.X + b * p.Y - p.Z; }


        public k2kPoint3D GetIntersection(k2kPoint3D p1, k2kPoint3D p2) { return k2kMath3D.GetIntersection(p1, p2, a, b, c); }
        public k2kPoint3D GetIntersection(k2kPoint3D p) { return k2kMath3D.GetIntersection(k2kPoint3D.ZeroPoint, p, a, b, c); }

        public double GetZ(Point p) { return a * p.X + b * p.Y + c; }
        public k2kPoint3D AddZ(Point p) { return new k2kPoint3D(p.X, p.Y, GetZ(p)); }


        static string PlaneFormatString0 = string.Format("Z = {{0{0}X{{1{0}Y{{2{0}", ",8:+0.00;-0.00}");

        public override string ToString() { return ToString(PlaneFormatString0); }
        public string ToString(string PlaneFormatString)
        {
            return string.Format(PlaneFormatString, a, b, c);
        }
    }





}