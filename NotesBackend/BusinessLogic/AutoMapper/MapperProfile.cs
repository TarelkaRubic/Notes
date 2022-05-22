using AutoMapper;
using PS_223020_Server.BusinessLogic.Core.Models;
using PS_223020_Server.DataAccess.Core.Models;

namespace PS_223020_Server.BusinessLogic.AutoMapperProfile
{
    public class BusinessLogicProfile : Profile
    {
        public BusinessLogicProfile()
        {
            CreateMap<UserRto, UserInformationBlo>()
                .ForMember(x => x.Id, x => x.MapFrom(m => m.Id))
                .ForMember(x => x.IsBoy, x => x.MapFrom(m => m.IsBoy))
                .ForMember(x => x.FirstName, x => x.MapFrom(m => m.FirstName))
                .ForMember(x => x.LastName, x => x.MapFrom(m => m.LastName))
                .ForMember(x => x.Patronymic, x => x.MapFrom(m => m.Patronymic))
                .ForMember(x => x.Birthday, x => x.MapFrom(m => m.Birthday))
                .ForMember(x => x.AvatarUrl, x => x.MapFrom(m => m.AvatarUrl));

            CreateMap<UserRto, UserUpdateBlo>()
                .ForMember(x => x.Id, x => x.MapFrom(m => m.Id))
                .ForMember(x => x.IsBoy, x => x.MapFrom(m => m.IsBoy))
                .ForMember(x => x.Password, x => x.MapFrom(m => m.Password))
                .ForMember(x => x.FirstName, x => x.MapFrom(m => m.FirstName))
                .ForMember(x => x.LastName, x => x.MapFrom(m => m.LastName))
                .ForMember(x => x.Patronymic, x => x.MapFrom(m => m.Patronymic))
                .ForMember(x => x.Birthday, x => x.MapFrom(m => m.Birthday))
                .ForMember(x => x.AvatarUrl, x => x.MapFrom(m => m.AvatarUrl));
        }
    }
}