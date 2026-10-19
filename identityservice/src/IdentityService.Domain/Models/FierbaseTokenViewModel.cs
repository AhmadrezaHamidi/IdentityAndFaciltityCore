

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Models
{
    public class FierbaseTokenViewModel : BaseModel
    {
        public string MobileNumber { get; set; }
        public string FierbaseToken { get; set; }
    }
}
