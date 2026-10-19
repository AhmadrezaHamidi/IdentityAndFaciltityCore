using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Common;

/// <summary>تصمیم تأیید/رد گروهی یا تکی.</summary>
public enum DecisionType : byte { Approve = 1, Reject = 2 }
