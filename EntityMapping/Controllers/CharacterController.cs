using EntityMapping.Entity;
using EntityMapping.Helpers;
using EntityMapping.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityMapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly DataContext _context;

        public CharacterController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Character>>> Get(int userId)
        {
            var characters = await _context.Characters
                .Where(c => c.UserId == userId)
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .ToListAsync();

            return characters;
        }
        [HttpGet("getUser")]
        public async Task<ActionResult<List<User>>> GetUser(int userId)
        {
            var users = await _context.Users
                .Where(c => c.Id == userId)
                .ToListAsync();

            return users;
        }
        [HttpGet("getSkill")]
        public async Task<ActionResult<List<Skill>>> GetSkill(int skillId)
        {
            var skills = await _context.Skills
                .Where(c => c.Id == skillId)
                .ToListAsync();

            return skills;
        }

        [HttpPost]
        public async Task<ActionResult<List<Character>>> Create(CreateCharacterDto request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if(user == null)
                return NotFound();

            var newCharacter = new Character
            {
                Name = request.Name,
                RpgClass = request.RpgClass,
                User = user
            };

            _context.Characters.Add(newCharacter);
            await _context.SaveChangesAsync();

            return await Get(newCharacter.UserId);
        }
        [HttpPost("addUser")]
        public async Task<ActionResult<List<User>>> CreateUser(UserDto request)
        {

            var newUser = new User
            {
                Username = request.Username,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return await GetUser(newUser.Id);
        }
        [HttpPost("addSkill")]
        public async Task<ActionResult<List<Skill>>> CreateSkill(SkillDto request)
        {

            var newSkill = new Skill
            {
                Name = request.Name,
                Damage=request.Damage
            };

            _context.Skills.Add(newSkill);
            await _context.SaveChangesAsync();

            return await GetSkill(newSkill.Id);
        }

        [HttpPost("weapon")]
        public async Task<ActionResult<Character>> AddWeapon(AddWeaponDto request)
        {
            var character = await _context.Characters.FindAsync(request.CharacterId);
            if (character == null)
                return NotFound();

            var newWeapon = new Weapon
            {
                Name = request.Name,
                Damage = request.Damage,
                Character = character
            };

            _context.Weapons.Add(newWeapon);
            await _context.SaveChangesAsync();

            return character;
        }

        [HttpPost("skill")]
        public async Task<ActionResult<Character>> AddCharacterSkill(AddCharacterSkillDto request)
        {
            var character = await _context.Characters
                .Where(c => c.Id == request.CharacterId)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync();
            if (character == null)
                return NotFound();

            var skill = await _context.Skills.FindAsync(request.SkillId);
            if (skill == null)
                return NotFound();

            character.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return character;
        }
    }
}
