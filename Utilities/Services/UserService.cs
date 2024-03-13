using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc.Routing;

namespace LearningBlazor.Utilities.Services;
/// <summary>
/// This service handles all user-related methods and functionality.
/// Currently the service stores the user in the SessionStorage of the browser. 
/// </summary>
public class UserService(ProtectedSessionStorage sessionStorage)
{
	private readonly ProtectedSessionStorage _sessionStorage = sessionStorage;
	private const string USERNAME_STORAGE_KEY = "username";
	private bool _isUsernameSubmitted = false;

	public bool UsernameExists => _isUsernameSubmitted;

	public event EventHandler UsernameSet;

	public async Task SetUsernameAsync(string username)
	{
		await _sessionStorage.SetAsync(USERNAME_STORAGE_KEY, username);
		_isUsernameSubmitted = true;

		UsernameSet?.Invoke(this, EventArgs.Empty);
	}

	public async Task<string> GetUsernameAsync()
	{
		if (_isUsernameSubmitted == false)
			throw new Exception("Called GET on username before it was stored in session storage");

		var result = await _sessionStorage.GetAsync<string>(USERNAME_STORAGE_KEY);

		if (result.Success == false || result.Value == null)
			throw new Exception("Failed to retrieve username from Session Storage");

		return result.Value;
	}
}
