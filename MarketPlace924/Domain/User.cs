using System;

namespace MarketPlace924.Domain
{
	public class User
	{

		public int UserId { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public UserRole Role { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime? BannedUntil { get; set; }
		public bool IsBanned { get; set; }
		public int FailedLogins { get; set; }

		public User(int userID = 0, string username = "", string email = "", string phoneNumber = "", string password = "", UserRole role = UserRole.Unassigned, int failedLogIns = 0, DateTime? bannedUntil = null, bool isBanned = false)
		{
			UserId = userID;
			Username = username;
			Email = email;
			Password = password;
			Role = role;
			PhoneNumber = phoneNumber;
			BannedUntil = bannedUntil;
			IsBanned = isBanned;
			FailedLogins = failedLogIns;
		}
	}
}
