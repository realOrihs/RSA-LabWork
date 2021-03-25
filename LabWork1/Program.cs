using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabWork1
{
    public class Program
    {
        public class BigInt
        {
            private int sign;
            public int Sign
            {
                set
                {
                    if (Math.Abs(value) != 1)
                    {
                        throw new Exception("Invalid value (available 1 or -1)");
                        //1 = +, -1 = -
                    }
                    else sign = value;
                }
                get
                {
                    return sign;
                }
            }

            public List<int> number = new List<int>();

            public BigInt()
            {
                Sign = 1;
                number.Add(0);
            }

            public BigInt(int sign, List<int> number)
            {
                Sign = sign;
                this.number = number.ToList();
            }

            public BigInt(int digit)
            {
                if (Math.Abs(digit) > 9) throw new Exception("Only Digits available in this constr., use other constr. for numbers");
                else 
                {
                    number.Add(Math.Abs(digit));
                    if (digit != 0)
                        Sign = Math.Sign(digit);
                    else Sign = 1;
                }
            }

            public BigInt(string str)
            {
                int start = 0;

                if (str[0] == '+')
                {
                    Sign = 1;
                    start = 1;
                }
                else if (str[0] == '-')
                {
                    Sign = -1;
                    start = 1;
                }
                else if (char.IsDigit(str[0]))
                    Sign = 1;
                else
                    throw new Exception("Invalid Values - only digits available");

                for (int i = start; i < str.Length; i++)
                    number.Add(int.Parse(string.Empty + str[i]));
            }

            public BigInt(BigInt other)
            {
                this.Sign = other.Sign;
                this.number = other.number.ToList();
            }

            public static BigInt Abs(BigInt num)
            {
                return new BigInt(1, num.number);
            }

            public BigInt GetDigit(int index) => new BigInt(number[index]);

            public static int Mod(int a, int b)
            {
                return a - b * Div(a, b);
            }

            public static int Div(int a, int b)
            {
                if (a >= 0)
                    return a / b;
                else if ((-a) % b != 0)
                    return -((-a) / b + 1);
                else
                    return -((-a) / b);
            }

            public static bool operator ==(BigInt a, BigInt b) 
	        {
                return a.Sign == b.Sign && a.number.SequenceEqual(b.number);
	        }

            public static bool operator !=(BigInt a, BigInt b)
            {
                return a.Sign != b.Sign || !a.number.SequenceEqual(b.number);
            }

            public static bool operator <(BigInt a, BigInt b)
            {
                int sizeA = a.number.Count;
                int sizeB = b.number.Count;
                bool res = false;

                if (a.Sign < b.Sign || (a.Sign == 1 && b.Sign == 1 && sizeA < sizeB) || (a.Sign == -1 && b.Sign == -1 && sizeA > sizeB)) return true;
                if (a.Sign > b.Sign) return false;

                else if (sizeA == sizeB)
                {
                    for (int i = 0; i < sizeA; i++)
                    {
                        if (a.number[i] < b.number[i])
                        {
                            res = true;
                            break;
                        }
                        if (a.number[i] > b.number[i]) break; 
                        
                    }

                    if (a.Sign == -1 && b.Sign == -1) res = !res;
                }
                
                return res;
            }

            public static bool operator >(BigInt a, BigInt b)
            {
                return a != b && !(a < b);
            }

            public static bool operator >=(BigInt a, BigInt b)
            {
                return a > b || a == b;
            }

            public static bool operator <=(BigInt a, BigInt b)
            {
                return a < b || a == b;
            }

            public static BigInt operator -(BigInt a)
            {
                return new BigInt(-a.Sign, a.number.ToList());
            }

            public static BigInt operator +(BigInt a, BigInt b)
            {
                BigInt sum = new BigInt(0);
                BigInt aCopy = new BigInt(a);
                BigInt bCopy = new BigInt(b);
                sum.Sign = 1;
                sum.number.Clear();  // знак +, нет никаких элементов

                //выравниваем числа по  длине
                int sizeA = aCopy.number.Count, sizeB = bCopy.number.Count;
                if (sizeA < sizeB)
                    for (int i = 0; i < sizeB - sizeA; i++)
                        aCopy.number.Insert(0, 0);
                else if (sizeA > sizeB)
                    for (int i = 0; i < -(sizeB - sizeA); i++)
                        bCopy.number.Insert(0, 0);

                sizeA = aCopy.number.Count;

                if (aCopy.Sign * bCopy.Sign == 1)   //  оба числа одного знака
                {
                    int k = 0, w;
                    for (int i = sizeA - 1; i >= 0; i--)
                    {
                        w = Mod(aCopy.number[i] + bCopy.number[i] + k, 10);
                        k = Div(aCopy.number[i] + bCopy.number[i] + k, 10);
                        sum.number.Insert(0, w);
                    }

                    if (k > 0) //  k=0 или k=1
                        sum.number.Insert(0, k);
                    if (aCopy.Sign == -1) sum.Sign = -1;
                }

                else
                {
                    BigInt bigger;
                    BigInt smaller;
                    if (Abs(aCopy) >= Abs(bCopy))
                    {
                        sum.Sign = aCopy.Sign;
                        bigger = Abs(aCopy);
                        smaller = Abs(bCopy);
                    }
                    else
                    {
                        sum.Sign = bCopy.Sign;
                        bigger = Abs(bCopy);
                        smaller = Abs(aCopy);
                    }

                    {
                        int k = 0, w;
                        for (int i = sizeA - 1; i >= 0; i--)
                        {
                            w = Mod(bigger.number[i] - smaller.number[i] + k, 10);
                            k = Div(bigger.number[i] - smaller.number[i] + k, 10);
                            sum.number.Insert(0, w);
                        }

                        
                    }
                }

                sum.RemoveBeginZeros();
                
                return sum;
            }

            public static BigInt operator -(BigInt a, BigInt b)
            {
                BigInt res;
                var aCopy = new BigInt(a);
                var bCopy = new BigInt(b);
                bCopy.Sign = -bCopy.Sign;
                res = aCopy + bCopy;
                
                return res;
            } 

            public static BigInt operator ++(BigInt a)
            {
                return a + new BigInt(1);
            }

            public static BigInt operator --(BigInt a)
            {
                return a - new BigInt(1);
            }

            public static BigInt operator /(BigInt a, BigInt b)
            {
                BigInt res = new BigInt(0);
                BigInt aCopy = new BigInt(a);
                BigInt bCopy = new BigInt(b);
                
                while (aCopy >= bCopy)
                {
                    aCopy -= bCopy;
                    res++;
                }

                if (a.Sign != b.Sign) res = -res;
                
                return res;
            }

            public static BigInt operator %(BigInt a, BigInt b)
            {
                BigInt aCopy = new BigInt(a);
                BigInt bCopy = new BigInt(b);
                return aCopy - bCopy * (aCopy / bCopy);
            }

            private BigInt MultOnDigit(int num)
            {
                BigInt res = new BigInt();

                if (num < 0 || num > 9)
                    throw new Exception("Digit is out of range");
                else if (num == 0)
                    return res;
                else if (num == 1)
                    return this;
                else
                {
                    res = new BigInt(this);
                    for (int i = 1; i < num; i++)
                        res = res + this;
                    return res;
                }
            }

            private BigInt MultOn10Pow(int pow)
            {
                BigInt res = this;
                for (int i = 1; i <= pow; i++)
                    res.number.Add(0);
                return res;
            }

            public static BigInt operator *(BigInt a,BigInt b)
            {
                BigInt temp = new BigInt(a), sum = new BigInt();

                for (int i = 0; i < b.number.Count; i++)
                {
                    temp = temp.MultOnDigit(b.number[i]);
                    temp = temp.MultOn10Pow(b.number.Count - i - 1);
                    sum = sum + temp;
                    temp = new BigInt(a);
                }
                if (sum != new BigInt(0))
                    sum.Sign = a.Sign * b.Sign;
                else sum.Sign = 1;
                return sum;
            }

            public static BigInt Pow(BigInt num, BigInt power)
            {
                var two = new BigInt(2);
                if (power == new BigInt(0))
                    return new BigInt(1);
                if (power == new BigInt(1))
                    return num;
                if (power % two == new BigInt(1))
                    return num * Pow(num, power - new BigInt(1));
                var b = Pow(num, power / two);
                return b * b;
            }

            public static BigInt ModPow(BigInt baseNum, BigInt exponent, BigInt modulus)
            {
                BigInt pow = new BigInt(1);
                if (modulus == new BigInt(1))
                    return new BigInt(0);
                BigInt curPow = baseNum % modulus;
                BigInt res = new BigInt(1);
                while (exponent > new BigInt(0))
                {
                    if (exponent % new BigInt(2) == new BigInt(1))
                    {
                        res = (res * curPow) % modulus;
                    }
                    exponent = exponent / new BigInt(2);
                    curPow = (curPow * curPow) % modulus;
                }
                return res;
            }

            private void RemoveBeginZeros()
            {
                var size = number.Count;
                for (int i = 0; i < size; i++) // удаляем ведущие нули 
                {
                    if (number[i] != 0)
                    {
                        number.RemoveRange(0, i);
                        break;
                    }
                }
                if (number.First() == 0 && size == number.Count) number = new List<int>() { 0 };
            }

            public override string ToString()
            {
                var builder = new StringBuilder();
                builder.Append(sign.ToString()[0] == '-' ? "-" : "");
                foreach(int num in number)
                {
                    builder.Append(num.ToString());
                }
                return builder.ToString();
            }

	}

        public static class RSA 
        {
            public static BigInt CalculatePublicExponent(BigInt module)
            {
                var exponent = new BigInt(3);
                var one = new BigInt(1);

                for (var i = new BigInt(0); i < module; i++)
                {
                    if (ExtEuqlFunc(exponent, module, out var _, out var _) == one)
                        return exponent;
                    exponent += one;
                }
                
                return exponent;
            }

            public static BigInt CalculatePrivateExponent(BigInt publicExp, BigInt module)
                => RevEuqlFuncRes(publicExp, module);
            

            public static BigInt ExtEuqlFunc(BigInt num, BigInt module, out BigInt x, out BigInt y) // Расширенная функция Евклида (Поиск НОД)
            {
                if (num == new BigInt(0))
                {
                    x = new BigInt(0); y = new BigInt(1);
                    return module;
                }
                BigInt x1, y1;
                BigInt d = ExtEuqlFunc(module % num, num, out x1, out y1);
                x = y1 - (module / num) * x1;
                y = x1;
                return d;
            }

            public static BigInt RevEuqlFuncRes(BigInt num, BigInt module) //Нахождение обратного числа по модолю
            {
                BigInt g = ExtEuqlFunc(num, module, out var x, out var y);
                if (g != new BigInt(1))
                    throw new Exception("Solution doesn't exist");
                return (x % module + module) % module;
            }

            public static List<BigInt> Encode()
            {
                Console.Write("Enter first prime number:");
                var p = new BigInt(Console.ReadLine());
                Console.Write("Enter second prime number:");
                var q = new BigInt(Console.ReadLine());
                var module = p * q;
                Console.Write($"Your module:{module}\n");
                var fi = (p - new BigInt(1)) * (q - new BigInt(1));
                Console.Write($"Your fi:{fi}\n");
                var publicExp = CalculatePublicExponent(fi);
                Console.Write($"Your public exp:{publicExp}\n");
                var privateExp = CalculatePrivateExponent(publicExp, fi);
                Console.Write($"Your private exponent:{privateExp}\n");
                Console.Write("Enter your message:");
                var message = Encoding.ASCII
                                 .GetBytes(Console.ReadLine() ?? string.Empty)
                                 .Select(x => (int)x)
                                 .ToArray();

                var encodedMsg = new List<BigInt>();
                Console.Write($"Your encoded message:\n");
                foreach (var code in message)
                {
                    if (new BigInt(code.ToString()) > module) throw new Exception("Module is too small - use bigger prime numbers");
                    var encodedLetter = BigInt.ModPow(new BigInt(code.ToString()), publicExp, module);
                    encodedMsg.Add(encodedLetter);
                    Console.Write(encodedLetter + " ");
                }
                Console.WriteLine();

                return encodedMsg;
            }

            public static string Decode()
            {
                Console.Write("Enter your module:");
                var module = new BigInt(Console.ReadLine());
                Console.Write("Enter your private exponent:");
                var privateExp = new BigInt(Console.ReadLine());
                Console.Write("Enter your encoded message:");
                var encodedMsg = Console.ReadLine().Split(' ');
                var decodedLetters = new List<int>();
                foreach(var code in encodedMsg)
                {
                    var decodedLetter = BigInt.ModPow(new BigInt(code.ToString()), privateExp, module);
                    decodedLetters.Add(int.Parse(decodedLetter.ToString()));
                }
                var decodedMsg = Encoding.ASCII.GetString(decodedLetters.Select(x => (byte)x).ToArray());
                Console.WriteLine($"Decoded message:{decodedMsg}");
                return decodedMsg;
            }
        }

        public static void Main(string[] args)
        {
            RSA.Encode();
            RSA.Decode();
        }
    }
}
