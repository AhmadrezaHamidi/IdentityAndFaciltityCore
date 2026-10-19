using IdentityService.Domain.Models;
using AutoMapper;
using IdentityService.Services.ViewModels.Admin;

namespace IdentityService.Services.Helper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<ForgotPasswordOtpModel, ResponseForgotPasswordOtpViewModel>().ReverseMap();
        }
    }
}