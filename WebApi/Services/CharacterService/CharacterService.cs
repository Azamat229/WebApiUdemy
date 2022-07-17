using WebApi.Dto.Character;
using WebApi.Models;
using WebApi.Dto.Character;
using AutoMapper;
using WebApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApi.Services.CharacterServices
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>()
        {
            new Character(),
            new Character() { Id = 1, Name = "Sam" }
        };
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User 
            .FindFirstValue(ClaimTypes.NameIdentifier));
            // мистическим ебаничским образом получает user id from Token
            // вопросы для себя как получить пользователя из бд 
            // как создать или дополнить объект который будет сохранен в бд
            // как находить и сравнивать параметны id, name ....
            // как использовать lambda, почему нельзя просто писать понятные функции, всегда нужно выебываться ? 

    

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            Character character = _mapper.Map<Character>(newCharacter);// для чего нужен mapp?

            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            // запрос в бд найти пользователя с ID. и присвоить его к создающему персонажу
           
            _context.Character.Add(character);
            await _context.SaveChangesAsync();

            response.Data = await _context.Character
                .Where(c => c.User.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return response; 
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                Character character = await _context.Character.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

                if (character != null)
                {
                    _context.Character.Remove(character);
                    await _context.SaveChangesAsync();
                    response.Data = _context.Character
                        .Where(c => c.User.Id == GetUserId())
                        .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
                }
                else
                {
                    response.Success = false;
                    response.Message = "Character not found";
                }
                            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }


        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            var dbChar = await _context.Character
                .Where(c => c.User.Id == GetUserId())
                .ToListAsync();
            response.Data = dbChar.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }


        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var response = new ServiceResponse<GetCharacterDto>();

            var dbChar = await _context.Character
                //.Where(c => c.User.Id == GetUserId())
                .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

            

            response.Data = _mapper.Map<GetCharacterDto>(dbChar);
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdataCharacter(UpdateCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            

            try
            {
                var character = await _context.Character
                    .Include(c=> c.User)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

                if (character.User.Id == GetUserId())
                {
                    character.Name = updatedCharacter.Name;
                    character.HitPoints = updatedCharacter.HitPoints;
                    character.Strength = updatedCharacter.Strength;
                    character.Defense = updatedCharacter.Defense;
                    character.Intelligence = updatedCharacter.Intelligence;
                    character.Class = updatedCharacter.Class;

                    await _context.SaveChangesAsync();

                    response.Data = _mapper.Map<GetCharacterDto>(character);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                }
                
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
                return response;
;
        }

       
    }
}
