using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Loans.Commands.RemoveLoanBatchMember;

/// <summary>US-D4: حذف فرد از ثبت (قبل از هر تصمیم) — درخواست او لغو می‌شود.</summary>
public sealed record RemoveLoanBatchMemberCommand(long BatchId, long RequestId) : FacilityBaseCommand<FacilityBaseMessageResponse<LoanBatchDto>>;
