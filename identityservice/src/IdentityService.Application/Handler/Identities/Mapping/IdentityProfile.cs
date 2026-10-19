using AutoMapper;
using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Mapping;

public class IdentityProfile : Profile
{
    public IdentityProfile()
    {
        CreateMap<LoginRegisterUserInformation, VerifyLoginOtpDto>();
        CreateMap<LoginRegisterUserInformation, RegisterUserDto>();
    }
}