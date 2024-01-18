using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace API.Controllers
{
	[ApiController]
	[Route("/api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly DataContext _dataContext;

		public UsersController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
		{
			var user = await _dataContext.Users.ToListAsync();
			return user;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AppUser>> GetUser(int id)
		{
			return await _dataContext.Users.FindAsync(id);
		}
	}
}
