using WebApi.Dto.Character;
using WebApi.Dto.Weapon;
using WebApi.Models;

namespace WebApi.Services.WeaponService
{
    public interface IWeaponService
    { 
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
     }
}
