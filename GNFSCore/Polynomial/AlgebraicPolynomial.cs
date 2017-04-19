﻿using System;
using System.Linq;
using System.Numerics;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using GNFSCore.IntegerMath;
using ExtendedNumerics;

namespace GNFSCore.Polynomial
{
	public class AlgebraicPolynomial
	{
		public int Degree { get; private set; }
		public BigInteger N { get; private set; }
		public BigInteger Base { get; private set; }
		public BigInteger[] Terms { get; private set; }
		public BigInteger BaseTotal { get; private set; }
		public BigInteger FormalDerivative { get; private set; }

		public AlgebraicPolynomial(BigInteger n, BigInteger polynomialBase, int degree)
		{
			Base = polynomialBase;
			Degree = degree;
			Terms = Enumerable.Repeat(BigInteger.Zero, degree + 1).ToArray();

			N = n;
			SetPolynomialValue(N);

			BaseTotal = AlgebraicPolynomial.Evaluate(this, Base);
			FormalDerivative = Derivative(this, Base);
		}

		private void SetPolynomialValue(BigInteger value)
		{
			N = value;
			int d = Degree;
			BigInteger toAdd = N;

			// Build out Terms[]
			while (d >= 0)
			{
				BigInteger placeValue = BigInteger.Pow(Base, d);

				if (placeValue == 1)
				{
					Terms[d] = toAdd;
				}
				else if (placeValue < toAdd)
				{
					BigInteger quotient = BigInteger.Divide(toAdd, placeValue);
					if (quotient > Base)
					{
						quotient = Base;
					}

					Terms[d] = quotient;
					BigInteger toSubtract = BigInteger.Multiply(quotient, placeValue);
					toAdd -= toSubtract;
				}

				d--;
			}
		}

		//public double Evaluate(double baseM)
		//{
		//	double result = 0;

		//	int d = this.Degree;
		//	while (d >= 0)
		//	{
		//		double placeValue = Math.Pow(baseM, d);

		//		double addValue = this.Terms[d] * placeValue;

		//		result += addValue;

		//		d--;
		//	}

		//	return result;
		//}

		//public double EvaluateMod(double baseM, double mod)
		//{
		//	double result = 0;

		//	int d = this.Degree;
		//	while (d >= 0)
		//	{
		//		double placeValue = Math.Pow(baseM, d);

		//		double addValue = this.Terms[d] * placeValue;

		//		result += addValue;

		//		result %= mod;

		//		d--;
		//	}

		//	return result;
		//}

		public BigRational Evaluate(BigRational baseM)
		{
			BigRational result = new BigRational(0);

			int d = this.Degree;
			while (d >= 0)
			{
				BigRational placeValue = BigRational.Pow(baseM, d);

				BigRational addValue = BigRational.Multiply(new BigRational(this.Terms[d]), placeValue);

				result = BigRational.Add(result, addValue);

				d--;
			}

			return result;
		}

		public static BigInteger Evaluate(AlgebraicPolynomial polynomial, BigInteger baseM)
		{
			BigInteger result = 0;

			int d = polynomial.Degree;
			while (d >= 0)
			{
				BigInteger placeValue = BigInteger.Pow(baseM, d);

				BigInteger addValue = BigInteger.Multiply((BigInteger)polynomial.Terms[d], placeValue);

				result += addValue;

				d--;
			}

			return result;
		}

		public static BigInteger Derivative(AlgebraicPolynomial polynomial, BigInteger baseM)
		{
			BigInteger result = new BigInteger(0);
			BigInteger m = baseM;
			int d = polynomial.Degree;
			int d1 = d - 1;
			while (d >= 0)
			{
				BigInteger placeValue = 0;

				if (d1 > -1)
				{
					placeValue = BigInteger.Pow(m, d1);
				}

				BigInteger addValue = BigInteger.Multiply(BigInteger.Multiply(d, placeValue), (BigInteger)polynomial.Terms[d]);
				result += addValue;

				d--;
			}

			return result;
		}

		//public double Derivative(double baseM)
		//{
		//	double result = 0;

		//	int d = this.Degree;
		//	while (d >= 0)
		//	{
		//		double placeValue = 0;

		//		if (d - 1 > -1)
		//		{
		//			placeValue = Math.Pow(baseM, d - 1);
		//		}

		//		double addValue = this.Terms[d] * d * placeValue;

		//		result += addValue;

		//		d--;
		//	}

		//	return result;
		//}

		public static IEnumerable<int> GetRootsMod(AlgebraicPolynomial polynomial, BigInteger baseM, IEnumerable<int> modList)
		{
			BigInteger polyResult = AlgebraicPolynomial.Evaluate(polynomial, baseM);
			IEnumerable<int> result = modList.Where(mod => (polyResult % mod) == 0);
			return result;
		}

		public override string ToString()
		{
			return AlgebraicPolynomial.FormatString(this);
		}

		public static string FormatString(AlgebraicPolynomial polynomial)
		{
			List<string> stringTerms = new List<string>();

			int degree = polynomial.Terms.Length - 1;
			while (degree >= 0)
			{
				if (degree > 1)
				{
					if (polynomial.Terms[degree] == 1)
					{
						stringTerms.Add($"{polynomial.Base}^{degree}");
					}
					else
					{
						stringTerms.Add($"{polynomial.Terms[degree]} * {polynomial.Base}^{degree}");
					}
				}
				else if (degree == 1)
				{
					stringTerms.Add($"{polynomial.Terms[degree]} * {polynomial.Base}");
				}
				else if (degree == 0)
				{
					stringTerms.Add($"{polynomial.Terms[degree]}");
				}

				degree--;
			}

			return string.Join(" + ", stringTerms);
		}
	}
}