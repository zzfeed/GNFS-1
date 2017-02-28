﻿using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;
using GNFSCore.FactorBase;
using GNFSCore.Polynomial;
using GNFSCore.IntegerMath;

namespace GNFSCore
{
	public partial class GNFS
	{
		public BigInteger N { get; private set; }

		public int PrimeBound { get; private set; }
		//public List<int> Primes { get; private set; }

		public Cyclotomic RationalPolynomial { get; private set; }
		public Irreducible AlgebraicPolynomial { get; private set; }

		public IEnumerable<Tuple<int, int>> RFB { get; internal set; } = null;
		public IEnumerable<Tuple<int, int>> AFB { get; internal set; } = null;
		public IEnumerable<Tuple<int, int>> QFB { get; internal set; } = null;

		public GNFS(BigInteger n, BigInteger polynomialBase, int degree)
		{
			N = n;
			//degree = 3; // or 4
			//BigInteger remainder = new BigInteger();
			PrimeBound = 60;//(int)n.NthRoot(degree, ref remainder);

			ConstructPolynomial(polynomialBase, degree);
			ConstructFactorBase();
		}

		private void ConstructPolynomial(BigInteger polynomialBase, int degree)
		{
			RationalPolynomial = new Cyclotomic(N);
			AlgebraicPolynomial = new Irreducible(N, polynomialBase, degree);
		}

		private void ConstructFactorBase()
		{
			RFB = Rational.Factory.BuildRationalFactorBase(this);
			AFB = Algebraic.Factory.GetAlgebraicFactorBase(this);
			QFB = Quadradic.Factory.GetQuadradicFactorBase(this);
		}

		internal static IEnumerable<Tuple<int, int>> PolynomialModP(Irreducible poly, IEnumerable<int> primes, IEnumerable<int> integers)
		{
			IEnumerable<Tuple<int, int>> factors = integers
				.SelectMany(r => primes.Where(p => (poly.EvalMod(r, p).IsZero))
				.Select(p => new Tuple<int, int>(p, r)));
			return factors.OrderBy(tup => tup.Item1);
		}


	}
}