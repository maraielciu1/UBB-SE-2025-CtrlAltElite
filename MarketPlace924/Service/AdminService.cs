using MarketPlace924.Repository;

namespace MarketPlace924.Service
{
	public class AdminService
	{
		private readonly UserRepository _userRepository;

		public AdminService(UserRepository userRepository)
		{
			_userRepository = userRepository;
		}
	}
}
