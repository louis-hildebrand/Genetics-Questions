using System;
using System.Collections.Generic;
using System.Text;

namespace Genetics_Questions
{
	class Fraction
	{
		public int Numer { get; set; }
		public int NumerReduced { get; set; }
		public int Denom { get; set; }
		public int DenomReduced { get; set; }
		public double DecimalValue { get; set; }
		public string StringValue { get; set; }
		public string StringValueReduced { get; set; }

		public Fraction(int n, int d = 1)
		{
			if (d == 0)
				throw new System.ArgumentException("Denominator cannot be zero");

			// Check the sign of the numerator and denominator
			if (n < 0 && d < 0)
			{
				n = -n;
				d = -d;
			}
			else if (d < 0)
			{
				n = -n;
				d = -d;
			}

			Numer = n;
			Denom = d;
			DecimalValue = (double)n / d;
			if (Numer == 0)
			{
				StringValue = "0";
			}
			else if (Denom == 1)
			{
				StringValue = n.ToString();
			}
			else
			{
				StringValue = n.ToString() + "/" + d.ToString();
			}

			// Reduce fraction
			if (n > 0)
			{
				for (int i = 2; i <= Math.Min(n, d); i++)
				{
					while (n % i == 0 && d % i == 0)
					{
						n /= i;
						d /= i;
					}
				}
			}
			else
			{
				n = -n;

				for (int i = 2; i <= Math.Min(n, d); i++)
				{
					while (n % i == 0 && d % i == 0)
					{
						n /= i;
						d /= i;
					}
				}

				n = -n;
			}

			NumerReduced = n;
			DenomReduced = d;
			if (NumerReduced == 0)
			{
				StringValueReduced = "0";
				DenomReduced = 1;
			}
			else if (DenomReduced == 1)
			{
				StringValueReduced = NumerReduced.ToString();
			}
			else
			{
				StringValueReduced = NumerReduced.ToString() + "/" + DenomReduced.ToString();
			}
		}

		// Converts the string str to a fraction (assuming it has been confirmed to be a valid fraction)
		public static Fraction Parse(string str)
		{
			int n, d;   // Numerator and denominator

			// Remove spaces and parentheses
			str = str.Replace(" ", "");
			str = str.Replace("(", "");
			str = str.Replace(")", "");

			if (str.IndexOf('/') == -1 && int.TryParse(str, out n))     // If string doesn't contain a '/' (i.e. integer answer), denominator = 1 and numerator = single number remaining
			{
				return new Fraction(int.Parse(str));
			}
			else
			{
				d = int.Parse(str.Substring(str.IndexOf('/') + 1, str.Length - str.IndexOf('/') - 1));  // Debominator is the number following the '/'
				n = int.Parse(str.Substring(0, str.IndexOf('/')));  // Numerator is the number preceding the '/'

				return new Fraction(n, d);
			}
		}

		// Checks that the string str is a valid fraction
		public static bool TryParse(string str)
		{
			bool slash = false;
			bool numBeforeSlash = false;
			bool numAfterSlash = false;

			// If the answer is an integer, it can be converted to a fraction
			if (int.TryParse(str, out int trash))
			{
				return true;
			}

			// Remove spaces
			str = str.Replace(" ", "");

			// Check that there is a single hyphen (i.e. negative sign) either at the very beginning of the string or directly following an opening bracket
			if (str[0] == '-')
			{
				str = str.Remove(0, 1);
			}
			else if (str[1] == '-' && str[0] == '(')
			{
				str = str.Remove(1, 1);
			}
			// Check that there are no more hyphens remaining
			if (str.IndexOf('-') != -1)
				return false;

			// Remove opening and closing parentheses (only if there is exactly one opening bracket at the very beginning AND one closing bracket at the very end)
			if (str[0] == '(' && str[str.Length - 1] == ')')
			{
				str = str.Remove(0, 1);
				str = str.Remove(str.Length - 1, 1);
			}
			// Check that there are no more parentheses remaining
			if (str.IndexOf('(') != -1 || str.IndexOf(')') != -1)
				return false;

			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == '/')
				{
					if (slash == true)
					{
						return false;
					}
					else
					{
						slash = true;
					}
				}
				else if (int.TryParse(str[i].ToString(), out int garbage))
				{
					if (slash == true)
					{
						numAfterSlash = true;
					}
					else
					{
						numBeforeSlash = true;
					}
				}
				else
				{
					return false;
				}
			}

			if (slash && numBeforeSlash && numAfterSlash)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static Fraction operator +(Fraction frac1, Fraction frac2)
		{
			return new Fraction(frac1.NumerReduced * frac2.DenomReduced + frac2.NumerReduced * frac1.DenomReduced, frac1.DenomReduced * frac2.DenomReduced);
		}

		public static Fraction operator +(int integer, Fraction frac)
		{
			return new Fraction(integer * frac.DenomReduced + frac.NumerReduced, frac.DenomReduced);
		}

		public static Fraction operator +(Fraction frac, int integer)
		{
			return new Fraction(frac.NumerReduced + integer * frac.DenomReduced, frac.DenomReduced);
		}

		public static Fraction operator -(Fraction frac1, Fraction frac2)
		{
			return new Fraction(frac1.NumerReduced * frac2.DenomReduced - frac2.NumerReduced * frac1.DenomReduced, frac1.DenomReduced * frac2.DenomReduced);
		}

		public static Fraction operator -(int integer, Fraction frac)
		{
			return new Fraction(integer * frac.DenomReduced - frac.NumerReduced, frac.DenomReduced);
		}

		public static Fraction operator -(Fraction frac, int integer)
		{
			return new Fraction(frac.NumerReduced - integer * frac.DenomReduced, frac.DenomReduced);
		}

		public static Fraction operator *(Fraction frac1, Fraction frac2)
		{
			return new Fraction(frac1.NumerReduced * frac2.NumerReduced, frac1.DenomReduced * frac2.DenomReduced);
		}

		public static Fraction operator *(int integer, Fraction frac)
		{
			return new Fraction(integer * frac.NumerReduced, frac.DenomReduced);
		}

		public static Fraction operator *(Fraction frac, int integer)
		{
			return new Fraction(integer * frac.NumerReduced, frac.DenomReduced);
		}

		public static Fraction operator /(Fraction frac1, Fraction frac2)
		{
			return new Fraction(frac1.NumerReduced * frac2.DenomReduced, frac1.DenomReduced * frac2.NumerReduced);
		}

		public static Fraction operator /(int integer, Fraction frac)
		{
			return new Fraction(integer * frac.DenomReduced, frac.NumerReduced);
		}

		public static Fraction operator /(Fraction frac, int integer)
		{
			return new Fraction(frac.NumerReduced, frac.DenomReduced * integer);
		}

		public override bool Equals(Object frac1)
		{
			return frac1 is Fraction && this == (Fraction)frac1;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Numer, Denom).GetHashCode();
		}

		public static bool operator ==(Fraction frac1, Fraction frac2)
		{
			return frac1.DecimalValue == frac2.DecimalValue;
		}

		public static bool operator ==(Fraction frac, int integer)
		{
			return (double)integer == frac.DecimalValue;
		}

		public static bool operator ==(int integer, Fraction frac)
		{
			return (double)integer == frac.DecimalValue;
		}

		public static bool operator !=(Fraction frac1, Fraction frac2)
		{
			return frac1.DecimalValue != frac2.DecimalValue;
		}

		public static bool operator !=(Fraction frac, int integer)
		{
			return (double)integer != frac.DecimalValue;
		}

		public static bool operator !=(int integer, Fraction frac)
		{
			return (double)integer != frac.DecimalValue;
		}

		public static bool operator >(Fraction frac1, Fraction frac2)
		{
			return frac1.DecimalValue > frac2.DecimalValue;
		}

		public static bool operator >(Fraction frac, int integer)
		{
			return frac.DecimalValue > (double)integer;
		}

		public static bool operator >(int integer, Fraction frac)
		{
			return (double)integer > frac.DecimalValue;
		}

		public static bool operator >=(Fraction frac1, Fraction frac2)
		{
			return frac1.DecimalValue >= frac2.DecimalValue;
		}

		public static bool operator >=(Fraction frac, int integer)
		{
			return frac.DecimalValue >= (double)integer;
		}

		public static bool operator >=(int integer, Fraction frac)
		{
			return (double)integer >= frac.DecimalValue;
		}

		public static bool operator <(Fraction frac1, Fraction frac2)
		{
			return frac1.DecimalValue < frac2.DecimalValue;
		}

		public static bool operator <(Fraction frac, int integer)
		{
			return frac.DecimalValue < (double)integer;
		}

		public static bool operator <(int integer, Fraction frac)
		{
			return (double)integer < frac.DecimalValue;
		}

		public static bool operator <=(Fraction frac1, Fraction frac2)
		{
			return frac1.DecimalValue <= frac2.DecimalValue;
		}

		public static bool operator <=(Fraction frac, int integer)
		{
			return frac.DecimalValue <= (double)integer;
		}

		public static bool operator <=(int integer, Fraction frac)
		{
			return (double)integer <= frac.DecimalValue;
		}
	}
}
