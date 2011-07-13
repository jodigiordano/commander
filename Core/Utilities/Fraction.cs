/*
 * Author: Syed Mehroz Alam
 * Email: smehrozalam@yahoo.com
 * URL: Programming Home "http://www.geocities.com/smehrozalam/" 
 * Date: 6/15/2004
 * Time: 10:54 AM
 *
 */

using System;

namespace EphemereGames.Core.Utilities
{
	/// <summary>
	/// Classes Contained:
	/// 	Fraction
	/// 	FractionException
	/// 	Application	
	/// </summary>
	
	
	
	/// <summary>
	/// Class name: Fraction
	/// Developed by: Syed Mehroz Alam
	/// Email: smehrozalam@yahoo.com
	/// URL: Programming Home "http://www.geocities.com/smehrozalam/"
	/// 
	/// Properties:
	/// 	Numerator: Set/Get value for Numerator
	/// 	Denominator:  Set/Get value for Numerator
	/// 	Value:  Set an integer value for the fraction
	/// 
	/// Constructors:
	/// 	no arguments:	initializes fraction as 0/1
	/// 	(Numerator, Denominator): initializes fraction with the given numerator and denominator values
	/// 	(integer):	initializes fraction with the given integer value
	/// 	(double):	initializes fraction with the given double value
	/// 	(string):	initializes fraction with the given string value
	/// 				the string can be an in the form of and integer, double or fraction.
	/// 				e.g it can be like "123" or "123.321" or "123/456"
	/// 
	/// Public Methods (Description is given with respective methods' definitions)
	/// 	string ConvertToString(Fraction)
	/// 	Fraction ConvertToFraction(string)
	/// 	Fraction ConvertToFraction(double)
	/// 	double ConvertToDouble(Fraction)
	/// 	Fraction Duplicate()
	/// 	Fraction Inverse(integer)
	/// 	Fraction Inverse(Fraction)
	/// 	ReduceFraction(Fraction)
	/// 	Equals(object)
	/// 	GetHashCode()
	/// 
	///	Private Methods (Description is given with respective methods' definitions)
	/// 	Initialize(Numerator, Denominator)
	/// 	Fraction Negate(Fraction)
	/// 	Fraction Add(Fraction1, Fraction2)
	/// 	int[] Factors(integer)
	/// 
	/// Operators Overloaded (overloaded for Fractions, Integers and Doubles)
	/// 	Unary: -
	/// 	Binary: +,-,*,/ 
	/// 	Relational and Logical Operators: ==,!=,<,>,<=,>=
	/// </summary>
	public class Fraction
	{
		/// <summary>
		/// Class attributes/members
		/// </summary>
		int m_iNumerator;
		int m_iDenominator;
		
		/// <summary>
		/// Constructors
		/// </summary>
		public Fraction()
		{
			Initialize(0,1);
		}
	
		public Fraction(int iWholeNumber)
		{
			Initialize(iWholeNumber, 1);
		}
	
		public Fraction(double dDecimalValue)
		{
			Fraction temp=ConvertToFraction(dDecimalValue);
			Initialize(temp.Numerator, temp.Denominator);
		}
		
		public Fraction(string strValue)
		{
			Fraction temp=ConvertToFraction(strValue);
			Initialize(temp.Numerator, temp.Denominator);
		}
		
		public Fraction(int iNumerator, int iDenominator)
		{
			Initialize(iNumerator, iDenominator);
		}
		
		/// <summary>
		/// Internal function for constructors
		/// </summary>
		private void Initialize(int iNumerator, int iDenominator)
		{
			Numerator=iNumerator;
			Denominator=iDenominator;
			ReduceFraction(this);
		}
	
		/// <summary>
		/// Properites
		/// </summary>
		public int Denominator
		{
			get
			{	return m_iDenominator;	}
			set
			{
				if (value!=0)
					m_iDenominator=value;
				else
					throw new FractionException("Denominator cannot be assigned a ZERO Value");
			}
		}
	
		public int Numerator
		{
			get	
			{	return m_iNumerator;	}
			set
			{	m_iNumerator=value;	}
		}
	
		public int Value
		{
			set
			{	m_iNumerator=value;
				m_iDenominator=1;	}
		}
	
		
		/// <summary>
		/// The function takes a Fraction object and returns its value as double
		/// </summary>
		public static double ConvertToDouble(Fraction frac)
		{
			return ( (double)frac.Numerator/frac.Denominator );
		}

		/// <summary>
		/// The function returns the current Fraction object as double
		/// </summary>
		public double ConvertToDouble()
		{
			return ( (double)this.Numerator/this.Denominator );
		}

		/// <summary>
		/// The function takes a Fraction object and returns it as a string
		/// </summary>
		public static string ConvertToString (Fraction frac1)
		{
			string str;
			if ( frac1.Denominator==1 )
				str=frac1.Numerator.ToString();
			else
				str=frac1.Numerator + "/" + frac1.Denominator;
			return str;
		}
	
		/// <summary>
		/// The function returns the current Fraction object as a string
		/// </summary>
		public string ConvertToString()
		{
			return Fraction.ConvertToString(this);
		}

		/// <summary>
		/// The function takes an string as an argument and returns its corresponding reduced fraction
		/// the string can be an in the form of and integer, double or fraction.
		/// e.g it can be like "123" or "123.321" or "123/456"
		/// </summary>
		public static Fraction ConvertToFraction(string strValue)
		{
			int i;
			for (i=0;i<strValue.Length;i++)
				if (strValue[i]=='/')
					break;
			
			if (i==strValue.Length)		// if string is not in the form of a fraction
				// then it is double or integer
				return ( ConvertToFraction( Convert.ToDouble(strValue) ) );
			
			// else string is in the form of Numerator/Denominator
			int iNumerator=Convert.ToInt32(strValue.Substring(0,i));
			int iDenominator=Convert.ToInt32(strValue.Substring(i+1));
			return new Fraction(iNumerator, iDenominator);
		}
		
		
		/// <summary>
		/// The function takes a floating point number as an argument 
		/// and returns its corresponding reduced fraction
		/// </summary>
		public static Fraction ConvertToFraction(double dValue)
		{
			try
			{
				Fraction frac;
				if (dValue>2147483647 && dValue<1.0/2147483647)
					throw new FractionException("Conversion not possible");
				if (dValue%1==0)	// if whole number
				{
					frac=new Fraction( (int) dValue );
				}
				else
				{
					double dTemp=dValue;
					int iMultiple=1;
					string strTemp=dValue.ToString();
					int i=0;
					while ( strTemp[i]!='.' )
						i++;
					int iDigitsAfterDecimal=strTemp.Length-i-1;
					while ( dTemp*10<2147483647 && iMultiple*10<2147483647 && iDigitsAfterDecimal>0  )
					{
						dTemp*=10;
						iMultiple*=10;
						iDigitsAfterDecimal--;
					}
					frac=new Fraction( (int)Math.Round(dTemp) , iMultiple );
				}
				return frac;
			}
			catch(Exception)
			{
				throw new FractionException("Conversion not possible");
			}
		}

		/// <summary>
		/// The function replicates current Fraction object
		/// </summary>
		public Fraction Duplicate()
		{
			Fraction frac=new Fraction();
			frac.Numerator=Numerator;
			frac.Denominator=Denominator;
			return frac;
		}

		/// <summary>
		/// The function returns the inverse of a Fraction object
		/// </summary>
		public static Fraction Inverse(Fraction frac1)
		{
			if (frac1.Numerator==0)
				throw new FractionException("Operation not possible (Denominator cannot be assigned a ZERO Value)");
	
			int iNumerator=frac1.Denominator;
			int iDenominator=frac1.Numerator;
			return ( new Fraction(iNumerator, iDenominator));
		}	
	

		/// <summary>
		/// Operators for the Fraction object
		/// includes -(unary), and binary opertors such as +,-,*,/
		/// also includes relational and logical operators such as ==,!=,<,>,<=,>=
		/// </summary>
		public static Fraction operator -(Fraction frac1)
		{	return ( Negate(frac1) );	}
	                              
		public static Fraction operator +(Fraction frac1, Fraction frac2)
		{	return ( Add(frac1 , frac2) );	}
	
		public static Fraction operator +(int iNo, Fraction frac1)
		{	return ( Add(frac1 , new Fraction(iNo) ) );	}
	
		public static Fraction operator +(Fraction frac1, int iNo)
		{	return ( Add(frac1 , new Fraction(iNo) ) );	}

		public static Fraction operator +(double dbl, Fraction frac1)
		{	return ( Add(frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator +(Fraction frac1, double dbl)
		{	return ( Add(frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator -(Fraction frac1, Fraction frac2)
		{	return ( Add(frac1 , -frac2) );	}
	
		public static Fraction operator -(int iNo, Fraction frac1)
		{	return ( Add(-frac1 , new Fraction(iNo) ) );	}
	
		public static Fraction operator -(Fraction frac1, int iNo)
		{	return ( Add(frac1 , -(new Fraction(iNo)) ) );	}

		public static Fraction operator -(double dbl, Fraction frac1)
		{	return ( Add(-frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator -(Fraction frac1, double dbl)
		{	return ( Add(frac1 , -Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator *(Fraction frac1, Fraction frac2)
		{	return ( Multiply(frac1 , frac2) );	}
	
		public static Fraction operator *(int iNo, Fraction frac1)
		{	return ( Multiply(frac1 , new Fraction(iNo) ) );	}
	
		public static Fraction operator *(Fraction frac1, int iNo)
		{	return ( Multiply(frac1 , new Fraction(iNo) ) );	}
	
		public static Fraction operator *(double dbl, Fraction frac1)
		{	return ( Multiply(frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator *(Fraction frac1, double dbl)
		{	return ( Multiply(frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator /(Fraction frac1, Fraction frac2)
		{	return ( Multiply( frac1 , Inverse(frac2) ) );	}
	
		public static Fraction operator /(int iNo, Fraction frac1)
		{	return ( Multiply( Inverse(frac1) , new Fraction(iNo) ) );	}
	
		public static Fraction operator /(Fraction frac1, int iNo)
		{	return ( Multiply( frac1 , Inverse(new Fraction(iNo)) ) );	}
	
		public static Fraction operator /(double dbl, Fraction frac1)
		{	return ( Multiply( Inverse(frac1) , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator /(Fraction frac1, double dbl)
		{	return ( Multiply( frac1 , Fraction.Inverse( Fraction.ConvertToFraction(dbl) ) ) );	}

		public static bool operator ==(Fraction frac1, Fraction frac2)
		{	return frac1.Equals(frac2);		}

		public static bool operator !=(Fraction frac1, Fraction frac2)
		{	return ( !frac1.Equals(frac2) );	}

		public static bool operator ==(Fraction frac1, int iNo)
		{	return frac1.Equals( new Fraction(iNo));	}

		public static bool operator !=(Fraction frac1, int iNo)
		{	return ( !frac1.Equals( new Fraction(iNo)) );	}
		
		public static bool operator ==(Fraction frac1, double dbl)
		{	return frac1.Equals( new Fraction(dbl));	}

		public static bool operator !=(Fraction frac1, double dbl)
		{	return ( !frac1.Equals( new Fraction(dbl)) );	}
		
		public static bool operator<(Fraction frac1, Fraction frac2)
		{	return frac1.Numerator * frac2.Denominator < frac2.Numerator * frac1.Denominator;	}

		public static bool operator>(Fraction frac1, Fraction frac2)
		{	return frac1.Numerator * frac2.Denominator > frac2.Numerator * frac1.Denominator;	}

		public static bool operator<=(Fraction frac1, Fraction frac2)
		{	return frac1.Numerator * frac2.Denominator <= frac2.Numerator * frac1.Denominator;	}
		
		public static bool operator>=(Fraction frac1, Fraction frac2)
		{	return frac1.Numerator * frac2.Denominator >= frac2.Numerator * frac1.Denominator;	}
		
		/// <summary>
		/// checks whether two fractions are equal
		/// </summary>
		public override bool Equals(object obj)
		{
			Fraction frac=(Fraction)obj;
			return ( Numerator==frac.Numerator && Denominator==frac.Denominator);
		}
		
		/// <summary>
		/// returns a hash code for this fraction
		/// </summary>
   		public override int GetHashCode()
   		{
			return ( Convert.ToInt32((Numerator ^ Denominator) & 0xFFFFFFFF) ) ;
		}

		/// <summary>
		/// internal function for negation
		/// </summary>
		private static Fraction Negate(Fraction frac1)
		{
			int iNumerator=-frac1.Numerator;
			int iDenominator=frac1.Denominator;
			return ( new Fraction(iNumerator, iDenominator) );

		}	

		/// <summary>
		/// internal functions for binary operations
		/// </summary>
		private static Fraction Add(Fraction frac1, Fraction frac2)
		{
			int iNumerator=frac1.Numerator*frac2.Denominator + frac2.Numerator*frac1.Denominator;
			int iDenominator=frac1.Denominator*frac2.Denominator;
			return ( new Fraction(iNumerator, iDenominator) );
		}
	
		private static Fraction Multiply(Fraction frac1, Fraction frac2)
		{
			int iNumerator=frac1.Numerator*frac2.Numerator;
			int iDenominator=frac1.Denominator*frac2.Denominator;
			return ( new Fraction(iNumerator, iDenominator) );
		}

		/// <summary>
		/// The function returns GCD of two numbers (used for reducing a Fraction)
		/// </summary>
		private static int GCD(int iNo1, int iNo2)
		{
			// take absolute values
			if (iNo1 < 0) iNo1 = -iNo1;
			if (iNo2 < 0) iNo2 = -iNo2;
			
			do
			{
				if (iNo1 < iNo2)
				{
					int tmp = iNo1;  // swap the two operands
					iNo1 = iNo2;
					iNo2 = tmp;
				}
				iNo1 = iNo1 % iNo2;
			} while (iNo1 != 0);
			return iNo2;
		}
	
		/// <summary>
		/// The function reduces(simplifies) a Fraction object by dividing both its numerator 
		/// and denominator by their GCD
		/// </summary>
		public static void ReduceFraction(Fraction frac)
		{
			try
			{
				if (frac.Numerator==0)
				{
					frac.Denominator=1;
					return;
				}
				
				int iGCD=GCD(frac.Numerator, frac.Denominator);
				frac.Numerator/=iGCD;
				frac.Denominator/=iGCD;
				
				if ( frac.Denominator<0 )	// if -ve sign in denominator
				{
					//pass -ve sign to numerator
					frac.Numerator*=-1;
					frac.Denominator*=-1;	
				}
			} // end try
			catch(Exception exp)
			{
				throw new FractionException("Cannot reduce Fraction: " + exp.Message);
			}
		}
			
	}	//end class Fraction


	/// <summary>
	/// Exception class for Fraction, derived from System.Exception
	/// </summary>
	public class FractionException : Exception
	{
		public FractionException() : base()
		{}
	
		public FractionException(string Message) : base(Message)
		{}
		
		public FractionException(string Message, Exception InnerException) : base(Message, InnerException)
		{}
	}	//end class FractionException

}	//end namespace Mehroz
