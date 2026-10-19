using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;
using IdentityService.Application.Services.Kyc.Factories;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagementKycLog;

namespace IdentityService.Application.Services.Kyc.Farashenasa;

public class FarashenasaKycRegisterUmService(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IUserManagementRepository userManagementRepository,
    IKycServiceFactory kycServiceFactory) : KycRegisterUmBaseService(userManagementKycLogRepository, 
    userManagementRepository, kycServiceFactory, EKycProvider.Farashenasa), IKycRegisterUmService;