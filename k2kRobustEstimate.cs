using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace k2kLib
{
    public class k2kRobustEstimate
    {
        public k2kRobustEstimate(k2kLeastSquare LeastSquare, int IterationCount)
        {
            this.LeastSquare = LeastSquare;
            this.IterationCount = IterationCount;
        }

        public k2kLeastSquare LeastSquare;
        public int IterationCount;

        public virtual k2kEstimateResult GetMaxLikelihood(k2kObservationEquation equation)
        {
            throw new NotImplementedException();
        }

        protected virtual double[] GetResidualError(double[] Approximation, k2kObservationEquation equation)
        {
            int i, j;
            var ret = new double[equation.Count];
            double TempExplaining;

            for (i = 0; i < equation.Count; ++i)
            {
                TempExplaining = 0;
                for (j = 0; j < equation[i].Explaining.Length; ++j)
                {
                    TempExplaining += equation[i].Explaining[j] * Approximation[j];
                }
                TempExplaining -= equation[i].Objective;
                ret[i] = TempExplaining * TempExplaining;
            }

            return ret;
        }

        protected virtual void ResidualEvaluation(double[] ResidualErrors, k2kObservationEquation equation)
        {
            throw new NotImplementedException();
        }
    }

    public class k2kEstimateResult
    {
        public k2kEstimateResult(double[] Approximation, double[] ResidualError)
        {
            this.Approximation = Approximation;
            this.ResidualError = ResidualError;
        }

        public double[] Approximation;
        public double[] ResidualError;
    }

    public class k2kMEstimator : k2kRobustEstimate
    {
        public k2kMEstimator(k2kLeastSquare LeastSquare, int IterationCount) : base(LeastSquare, IterationCount) { }

        public override k2kEstimateResult GetMaxLikelihood(k2kObservationEquation equation)
        {
            Inlier = new k2kObservationEquation(equation);
            double[] ResidualError = null;
            double[] Approximation = null;

            for (int i = 0; i < IterationCount; ++i)
            {
                if (Inlier.Count < equation[0].Length)
                    break;

                Approximation = LeastSquare.GetSolution(Inlier);
                ResidualError = GetResidualError(Approximation, equation);
                ResidualEvaluation(ResidualError, equation);
            }

            return new k2kEstimateResult(Approximation, ResidualError);
        }

        public double AllowableErrorCoefficient = 1.5;
        protected k2kObservationEquation Inlier;

        protected override void ResidualEvaluation(double[] ResidualErrors, k2kObservationEquation equation)
        {
            Inlier.Clear();

            int i;
            var Sigma = k2kMath.GetMedian<double>(ResidualErrors) * 1.4826;
            var AllowableError = Sigma * AllowableErrorCoefficient;

            for (i = 0; i < equation.Count; ++i)
            {
                if (ResidualErrors[i] <= AllowableError)
                    Inlier.Add(equation[i]);
            }
        }
    }

    public class k2kRandomizedEstimate : k2kRobustEstimate
    {
        public k2kRandomizedEstimate(k2kLeastSquare LeastSquare, int IterationCount, int SampleCount)
            : base(LeastSquare, IterationCount)
        {
            this.SampleCount = SampleCount;
        }

        public int SampleCount;

        public override k2kEstimateResult GetMaxLikelihood(k2kObservationEquation equation)
        {
            if (SampleCount < equation[0].Length)
                throw new ArgumentException();

            int i;
            var rand = new k2kRandom();

            k2kObservationEquation sample;
            double[] ResidualError = null;
            double[] Approximation, BestApproximation = null;

            InitBestResidualError();

            for (i = 0; i < IterationCount; ++i)
            {
                sample = new k2kObservationEquation(rand.RandomSampling(SampleCount, equation));

                Approximation = LeastSquare.GetSolution(sample);
                ResidualError = GetResidualError(Approximation, equation);
                ResidualEvaluation(ResidualError, equation);

                if (BestResidualError)
                    BestApproximation = Approximation;
            }

            return new k2kEstimateResult(BestApproximation, ResidualError);
        }

        protected bool BestResidualError = false;

        protected override void ResidualEvaluation(double[] ResidualErrors, k2kObservationEquation equation)
        {
            throw new NotImplementedException();
        }

        protected virtual void InitBestResidualError()
        {
            throw new NotImplementedException();
        }
    }

    public class k2kLMedS : k2kRandomizedEstimate
    {
        public k2kLMedS(k2kLeastSquare LeastSquare, int IterationCount, int SampleCount) : base(LeastSquare, IterationCount, SampleCount) { }

        protected double MedResidualError;
        protected double MinMedResidualError;

        protected override void InitBestResidualError()
        {
            MinMedResidualError = double.MaxValue;
        }

        protected override void ResidualEvaluation(double[] ResidualErrors, k2kObservationEquation equation)
        {
            double MedResidualError = k2kMath.GetMedian<double>(ResidualErrors);

            BestResidualError = (MedResidualError < MinMedResidualError);
            if (BestResidualError)
                MinMedResidualError = MedResidualError;
        }
    }

    public class k2kRANSAC : k2kRandomizedEstimate
    {
        public k2kRANSAC(k2kLeastSquare LeastSquare, int IterationCount, int SampleCount) : base(LeastSquare, IterationCount, SampleCount) { }

        public double AllowableResidualError = 1.0;
        protected int AllowableCount;
        protected int MinCount;

        protected override void InitBestResidualError()
        {
            MinCount = int.MaxValue;
        }

        protected override void ResidualEvaluation(double[] ResidualErrors, k2kObservationEquation equation)
        {
            AllowableCount = 0;
            foreach (var err in ResidualErrors)
                if (err <= AllowableResidualError)
                    ++AllowableCount;

            BestResidualError = (AllowableCount < MinCount);
            if (BestResidualError)
                MinCount = AllowableCount;
        }
    }















}
