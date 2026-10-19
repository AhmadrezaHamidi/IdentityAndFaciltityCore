using System.Runtime.InteropServices.JavaScript;
using IdentityService.Domain.Models;

namespace IdentityService.Domain.Entity.Basic.BankData;

public class BankDataResponse : BaseModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public int RsCode { get; set; }
    public List<ResultDataItem> ResultData { get; set; } = new();
    public List<ErrorListItem> ErrorList { get; set; } = new();
}

public class ErrorListItem
{
    public int Code { get; set; }
    public string Desc { get; set; }
    public string ParamName { get; set; }
    public string ParamPath { get; set; }
}

public class ResultDataItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string StateId { get; set; }

}