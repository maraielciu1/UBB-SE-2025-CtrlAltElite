using MarketPlace924.Repository;
using System.Threading.Tasks;

namespace MarketPlace924.Service;

public class AnalyticsService
{
	private readonly UserRepository _userRepository;

	private readonly BuyerRepository _buyerRepository;

	public AnalyticsService(UserRepository userRepository, BuyerRepository buyerRepository)
	{
		_userRepository = userRepository;
		_buyerRepository = buyerRepository;
	}

	public async Task<int> GetTotalUsersCount()
	{
		return await _userRepository.GetTotalCount();
	}

	public async Task<int> GetTotalBuyersCount()
	{
		return await _buyerRepository.GetTotalCount();
	}
}
