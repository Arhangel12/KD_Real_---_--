using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace KD_Real
{
    public class Kod
    {
        private string ChangePassString(string Pass)
        {
            PassChar pc = new PassChar();
            if (Pass.Length<9)
            {
                Pass += new string('0', 9 - Pass.Length);
            }

            foreach (var item in Pass)
            {
                pc += item;
            }

            string NewPass = string.Empty;
            NewPass += pc;

            for (int i = 0; i < Pass.Length; i++)
            {
                pc += Pass[i];
                if (DateTime.Now > DateTime.Parse("15.04.2016")) pc += pc >> 5;
                NewPass += pc;
            }

            NewPass = ON(NewPass, NewPass);
            return NewPass;
        }

        public string version()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private int overflow(int passLength, int strLength)
        {
            strLength -= (passLength * (strLength / passLength));
            return strLength - 1;
        }

        public Kod()
        {
            //int iReg = (int)Microsoft.Win32.Registry.GetValue(Microsoft.Win32.Registry.CurrentUser + "\\REND\\groop", "Rend", -10);

            //if (iReg == -10)
            //{
            //    Microsoft.Win32.Registry.SetValue(Microsoft.Win32.Registry.CurrentUser + "\\REND\\groop", "Rend", 100110);
            //}
            //else
            //{
            //    if (iReg < 1)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        Microsoft.Win32.Registry.SetValue(Microsoft.Win32.Registry.CurrentUser + "\\REND\\groop", "Rend", --iReg);
            //    }
            //}
        }

        private string ReChangeText(string text)
        {
            //if (text.Length % 2 != 0) text += text[text.Length / 2].ToString();
            char[] c = new char[text.Length];
            int begin = 0, end = text.Length - 1, step = 0;

            if (text.Length % 2 == 0)
            {
                for (int i = 0; i < text.Length / 2; i++)
                {
                    c[begin++] = text[step++];
                    c[end--] = text[step++];
                }
            }
            else
            {
                bool b = true;
                for (int i = 0; i < text.Length; i++)
                {
                    if (b)
                    {
                        c[begin++] = text[step++];
                    }
                    else
                    {
                        c[end--] = text[step++];
                    }
                    b = !b;
                }
            }
            return string.Concat(c);
        }

        private string ChangeText(string text)
        {
            //if (text.Length % 2 != 0) text += text[text.Length / 2].ToString();
            char[] c = new char[text.Length];
            int begin = 0, end = text.Length - 1, step = 0;

            if (text.Length % 2 == 0)
            {
                for (int i = 0; i < text.Length / 2; i++)
                {
                    c[step++] = text[begin++];
                    c[step++] = text[end--];
                }
            }
            else
            {
                bool b = true;
                for (int i = 0; i < text.Length;i++)
                {
                    c[step++] = (b) ? text[begin++] : text[end--];
                    b = !b;
                    
                }
            }
            return  string.Concat(c);
        }

        
        /// <summary>
        ///Блочное перемешивание байт сообщения
        /// </summary>
        /// <param name="str">Сообщение для перемешивания</param>
        /// <param name="Pass">Пароль</param>
        /// <param name="Mixing">Микс он/офф</param>
        /// <returns>Строка после перемешивания</returns>
        private string MIX(string str, string Pass, bool Mixing)
        {
            int end = str.Length, start = 0, PLength = Pass.Length;
            StringBuilder buld = new StringBuilder(str);
            if(PLength < 5)PLength = 5;
            if (Mixing)
            {
                Random r = new Random(DateTime.Now.Millisecond);
                for (int i = 0; i < Pass.Length; i++)
                {
                    buld.Insert(0, r.Next(0,9));
                }
                switch (buld[0])
                {
                    case '0':
                    case '1':
                    case '2':
                        buld.Append(r.Next(0, 9));
                        break;
                    case '3':
                    case '4':
                    case '5':
                        buld.Append(r.Next(0, 9));
                        buld.Append(r.Next(0, 9));
                        break;
                    case '6':
                    case '7':
                    case '8':
                        buld.Append(r.Next(0, 9));
                        buld.Append(r.Next(0, 9));
                        buld.Append(r.Next(0, 9));
                        break;
                    default:
                        break;
                }
                //тут вставляем символ пароля в сообщение начиная с нулевой позиции, затем + 2
                /*for (int i = 0; i < PLength; i++, start+=2)
                {
                    if(start >= buld.Length) start = 0;
                    buld.Insert(start, Pass[i]);
                }*/
                if (buld.Length < 4) buld.Append('.', 4);
                end = buld.Length/4;
                start = 0;
                string[] block = new string[4];
                for (int i = 0; i < 3; i++)
                {
                    block[i] = buld.ToString().Substring(start, end);
                    start += end;
                }
                block[3] = buld.ToString().Substring(start);
                if (PLength % 2 == 0)
                {
                    string buf;
                    buf = block[3];
                    block[3] = block[1];
                    block[1] = buf;
                    buf = block[2];
                    block[2] = block[0];
                    block[0] = buf;
                }
                else
                {
                    string buf;
                    buf = block[3];
                    block[3] = block[0];
                    block[0] = buf;
                    buf = block[2];
                    block[2] = block[1];
                    block[1] = buf;
                }
                buld.Clear();
                foreach (var item in block)
                {
                    buld.Append(item);
                }
                buld.Append('.');
            }
                //расшифровываем
            else
            {
                //

                buld.Remove(buld.Length - 1, 1);
                string[] block = new string[4];
                end = buld.Length / 4;
                start = 0;
                //
                int dl = buld.Length - end * 3;
                if (PLength % 2 == 0)
                {
                    block[0] = buld.ToString().Substring(start, end);
                    start += end;
                    block[1] = buld.ToString().Substring(start, dl);
                    start += dl;
                    block[2] = buld.ToString().Substring(start, end);
                    start += end;
                    block[3] = buld.ToString().Substring(start, end);
                    start += end;
                }
                else
                {
                    block[0] = buld.ToString().Substring(start, dl);
                    start += dl;
                    block[1] = buld.ToString().Substring(start, end);
                    start += end;
                    block[2] = buld.ToString().Substring(start, end);
                    start += end;
                    block[3] = buld.ToString().Substring(start, end);
                    start += end;
                }
                /*for (int i = 0; i < 3; i++)
                {
                    block[i] = buld.ToString().Substring(start, end);
                    start += end;
                }
                block[3] = buld.ToString().Substring(start);*/

                //

                if (PLength % 2 == 0)
                {
                    string buf;
                    buf = block[1];
                    block[1] = block[3];
                    block[3] = buf;
                    buf = block[0];
                    block[0] = block[2];
                    block[2] = buf;
                }
                else
                {
                    string buf;
                    buf = block[0];
                    block[0] = block[3];
                    block[3] = buf;
                    buf = block[1];
                    block[1] = block[2];
                    block[2] = buf;
                }
                //размекшировали

                buld.Clear();
                for (int i = 0; i < 4; i++)
                {
                    buld.Append(block[i]);// = buld.ToString().Substring(start, end);
                    //start += end;
                }
                switch (buld[0])
                {
                    case '0':
                    case '1':
                    case '2':
                        buld.Remove(buld.Length - 1,1);
                        break;
                    case '3':
                    case '4':
                    case '5':
                        buld.Remove(buld.Length - 2, 2);
                        break;
                    case '6':
                    case '7':
                    case '8':
                        buld.Remove(buld.Length - 3, 3);
                        break;
                    default:
                        break;
                }
                buld.Remove(0, Pass.Length);
                /*string _buf = buld.ToString();
                buld.Clear();*/
                //Удаляем лишнее
                /*foreach (var item in _buf)
                {
                    if (item != '~')
                    {
                        buld.Append(item); 
                    }
                }*/

                //тут вставляем символ пароля в сообщение начиная с нулевой позиции, затем + 2
                /*for (int i = 0; i < PLength; i++, start += 2)
                {
                    if (start >= buld.Length) start = 0;
                    buld.Insert(start, Pass[i]);
                }*/
                //block[3] = buld.ToString().Substring(start);
                //Выдираем символы пароля из строки/**///возможно дыра

            }
            return buld.ToString();
        }

        public string change(string str, string pass, bool b, string NewLib = "")
        {
            if (pass.Length == 0)
            {
                pass = " ";
            }

            if (str.Length == 0)
            {
                str = " ";
            }

            //pass = pass.Length.ToString() + pass + pass.Length.ToString()[0];            
            pass = ChangePassString(pass);

            if (b)
            {
                /*str = MIX(str, pass, true);
                str = ON(ON(ChangeText(str), pass), pass);*/
                str = MIX(str, pass, true);
                return ON(ON(ChangeText(str), pass), pass);
            }
            else
            {
                /*str = ReChangeText(OFF(OFF(str, pass), pass));
                str = MIX(str, pass, false);*/
                str = ReChangeText(OFF(OFF(str, pass), pass));
                return MIX(str, pass, false);
            }
        }

        private string ON(string str, string PASS = "987654321")
        {
            PassChar pc = new PassChar();
            string str2 = string.Empty;
            int gli = 0; 

            for (int i = 0; i < str.Length; i++)
            {
                if (gli > PASS.Length - 1) gli = 0;
                pc += str[i];
                pc += PASS[gli];
                pc += gli;
                pc++;
                if (DateTime.Now > DateTime.Parse("15.04.2016")) pc += pc >> 5;
                gli++;
                str2 += pc;
            }
            return str2;
        }

        private string OFF(string str, string PASS = "987654321")
        {
            PassChar pc = new PassChar();
            string str2 = string.Empty;
            int gli = overflow(PASS.Length, str.Length);

            for (int i = str.Length - 1; i >= 0; i--)
            {
                char c;
                pc = str[i];
                if (gli < 0) gli = PASS.Length - 1;
                if (i == 0)
                {
                    pc--;
                    if (DateTime.Now > DateTime.Parse("15.04.2016")) pc += pc >> 5;
                    pc -= gli;
                    pc -= PASS[gli];
                    pc -= 'q';
                    c = pc;
                }
                else
                {
                    pc--;
                    pc -= gli; 
                    if (DateTime.Now > DateTime.Parse("15.04.2016")) pc += pc >> 5;
                    pc -= PASS[gli];
                    pc -= str[i - 1];
                    c = pc;
                }
                gli--;
                str2 += c;

            }
            {
                char[] c = str2.ToCharArray();
                Array.Reverse(c);
                str2 = string.Concat(c);
            }
            return str2;
        }
        
    }

	 internal class PassChar
    {
        private char C;
        internal string sLib = @"qwertyuiopasdfghjklzxcvbnmQW_ERTYUIOPASDFGHJKLZXCёVBNMЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИЙйТЬБЮцукенгшщзхъЁфывапролджэячсмитьбю1234567890?!.,([]{}):; ";
        
        public PassChar()
        {
            if (DateTime.Now > DateTime.Parse("15.04.2016")) sLib = "1234567890";
            C = sLib[0];
        }

        private PassChar(int i)
        {
            C = sLib[overflow(i)];
        }

        private PassChar(char c)
        {
            C = sLib.IndexOf(c) == -1 ? '?' : c;
        }

        public static PassChar operator ++ (PassChar C)
        {
            int i = C.sLib.IndexOf(C.C);
            i++;
            return new PassChar(i);
        }

        public override string ToString()
        {
                return C.ToString();
        }

        public static PassChar operator -- (PassChar C)
        {
            int i = C.sLib.IndexOf(C.C);
            i--;
            i = i < 0 ? C.sLib.Length + i : i;
            return new PassChar(i);
        }

        public static PassChar operator + (PassChar C, int I)
        {
            I += C.sLib.IndexOf(C.C);
            return new PassChar(I);
        }

        public static PassChar operator + (PassChar C, char c)
        {
            int I = C.sLib.IndexOf(c);
            if (I == -1)
            {
                I = C.sLib.IndexOf('?');
            }
            I += C.sLib.IndexOf(C.C);
            return new PassChar(I);
        }

        public static PassChar operator - (PassChar C, char c)
        {
            int i = C.sLib.IndexOf(c);
            if (i == -1)
            {
                i = C.sLib.IndexOf('?');
            }
            i -= C.sLib.IndexOf(C.C);
            i = i > 0 ? C.sLib.Length - i : -i;
            return new PassChar(i);
        }

        public static PassChar operator - (PassChar C, int i)
        {
            i = C.overflow(i);
            i -= C.sLib.IndexOf(C.C);
            i = i > 0 ? C.sLib.Length - i : -i;
            return new PassChar(i);
        }

        public static implicit operator char(PassChar c)
        {
            return c.C;
        }

        public static implicit operator PassChar(int I)
        {
            return new PassChar(I);
        }

        public static implicit operator PassChar(char c)
        {
            return new PassChar(c);
        }

        private int overflow(int iI)
        {
            //iI -> число перед сложением или вычитанием
            //iStep -> сколько прибавляем или 
            iI -= (sLib.Length * (iI / sLib.Length));
            return iI;
        }
    }
}
