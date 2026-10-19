using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entity.UserManagementKycLog.Dto;

public class RegisterUserManagementKycLogDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string NationalCardSeries { get; set; }
    public DateTime BirthDate { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } // UniqueKey From Farashenasa Link

}
  