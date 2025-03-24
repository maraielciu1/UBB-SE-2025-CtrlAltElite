using System;

namespace MarketPlace924.Service
{
	public class CaptchaService
	{
		private static readonly Random _random = new Random();

		public static string GenerateCaptcha()
		{
			var captchaCodeLength = _random.Next(6, 8);
			var captchaCode = string.Empty;

			while (captchaCode.Length != captchaCodeLength)
			{
				var currentCharacterAscii = _random.Next(48, 123);
				var currentCharacter = (char)currentCharacterAscii;

				if (!IsAlphaNumeric(currentCharacter))
					continue;

				captchaCode += currentCharacter;
			}

			return captchaCode;
		}

		private static bool IsAlphaNumeric(char currentCharacter)
		{
			return (currentCharacter >= '0' && currentCharacter <= '9')
				|| (currentCharacter >= 'a' && currentCharacter <= 'z')
				|| (currentCharacter >= 'A' && currentCharacter <= 'Z');
		}

		public static bool IsEnteredCaptchaValid(string generetedCaptcha, string currentCaptcha)
		{
			return generetedCaptcha == currentCaptcha;
		}
	}
}
