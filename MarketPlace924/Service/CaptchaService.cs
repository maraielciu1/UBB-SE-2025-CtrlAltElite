using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
    public class CaptchaService
    {
        private static readonly Random _random = new Random();
        public string GenerateCaptcha()

        {
            int lengthOfCaptchaCode = _random.Next(6, 8);
            string captcha = "";
            int totalSoFar = 0;
            do
            {

                int current_char_being_added = _random.Next(48, 123);
                if ((current_char_being_added >= 48 && current_char_being_added <= 57) || (current_char_being_added >= 65 && current_char_being_added <= 90) || (current_char_being_added >= 97 && current_char_being_added <= 122))
                {
                    captcha = captcha + (char)current_char_being_added;
                    totalSoFar++;
                    if (totalSoFar == lengthOfCaptchaCode)
                        break;

                }
            } while (true);
            return captcha;

        }

        public bool validateEnteredCaptcha(string GeneratedCaptcha, string enterdCaptcha)
        {
            return GeneratedCaptcha == enterdCaptcha;

        }
    }
}
