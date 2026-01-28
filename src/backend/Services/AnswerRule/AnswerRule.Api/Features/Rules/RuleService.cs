using System.Text.RegularExpressions;
using AnswerRule.Api.Definitions;
using AnswerRule.Api.Features.Rules.Model;
using AnswerRule.Data.Entities;
using AnswerRule.Data.Enums;
using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;

namespace AnswerRule.Api.Features.Rules;

public partial class RuleService : IRuleService
{
    private readonly IRuleDataHandler _dataHandler;

    public RuleService(IRuleDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<PagedResult<AnsweringRuleResponse>>> GetListAsync(
        PortalAuthInfo auth, AnsweringRuleListFilter filter)
    {
        var (items, totalCount) = await _dataHandler.GetPagedAsync(filter);

        var result = new PagedResult<AnsweringRuleResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<AnsweringRuleResponse>>.Success(result);
    }

    public async Task<Result<AnsweringRuleDetailResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var rule = await _dataHandler.GetByGidWithTimeSlotsAsync(gid);
        if (rule == null)
        {
            return Result<AnsweringRuleDetailResponse>.Failure(
                new NotFoundError(ErrorCodes.Rule.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Rule.NotFound)));
        }

        return Result<AnsweringRuleDetailResponse>.Success(MapToDetailResponse(rule));
    }

    public async Task<Result<AnsweringRuleDetailResponse>> CreateAsync(
        PortalAuthInfo auth, CreateAnsweringRuleRequest request)
    {
        // Validate
        var validationError = ValidateRequest(request.Name, request.SipAccountGid, request.ActionType,
            request.ActionTarget, request.VoicemailBoxGid, request.VoiceMessageGid,
            request.NotificationEmail, request.TimeSlots);

        if (validationError != null)
        {
            return Result<AnsweringRuleDetailResponse>.Failure(validationError);
        }

        // Create entity
        var rule = new AnsweringRule
        {
            SipAccountGid = request.SipAccountGid,
            Name = request.Name,
            Description = request.Description,
            Priority = request.Priority,
            IsEnabled = request.IsEnabled,
            ActionType = request.ActionType,
            ActionTarget = request.ActionTarget,
            VoicemailBoxGid = request.VoicemailBoxGid,
            VoiceMessageGid = request.VoiceMessageGid,
            SendEmailNotification = request.SendEmailNotification,
            NotificationEmail = request.NotificationEmail,
            CreatedByUserId = auth.UserId,
            TimeSlots = request.TimeSlots.Select(MapToTimeSlotEntity).ToList()
        };

        await _dataHandler.CreateAsync(rule);

        return Result<AnsweringRuleDetailResponse>.Success(MapToDetailResponse(rule));
    }

    public async Task<Result<AnsweringRuleDetailResponse>> UpdateAsync(
        PortalAuthInfo auth, string gid, UpdateAnsweringRuleRequest request)
    {
        var rule = await _dataHandler.GetByGidWithTimeSlotsAsync(gid);
        if (rule == null)
        {
            return Result<AnsweringRuleDetailResponse>.Failure(
                new NotFoundError(ErrorCodes.Rule.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Rule.NotFound)));
        }

        // Validate
        var validationError = ValidateRequest(request.Name, rule.SipAccountGid, request.ActionType,
            request.ActionTarget, request.VoicemailBoxGid, request.VoiceMessageGid,
            request.NotificationEmail, request.TimeSlots);

        if (validationError != null)
        {
            return Result<AnsweringRuleDetailResponse>.Failure(validationError);
        }

        // Update entity
        rule.Name = request.Name;
        rule.Description = request.Description;
        rule.Priority = request.Priority;
        rule.IsEnabled = request.IsEnabled;
        rule.ActionType = request.ActionType;
        rule.ActionTarget = request.ActionTarget;
        rule.VoicemailBoxGid = request.VoicemailBoxGid;
        rule.VoiceMessageGid = request.VoiceMessageGid;
        rule.SendEmailNotification = request.SendEmailNotification;
        rule.NotificationEmail = request.NotificationEmail;
        rule.ModifiedByUserId = auth.UserId;
        rule.ModifiedAt = DateTime.UtcNow;

        // Update time slots - delete old and add new
        await _dataHandler.DeleteTimeSlotsAsync(rule.Id);
        var newSlots = request.TimeSlots.Select(dto =>
        {
            var slot = MapToTimeSlotEntity(dto);
            slot.AnsweringRuleId = rule.Id;
            return slot;
        }).ToList();
        await _dataHandler.AddTimeSlotsAsync(newSlots);

        await _dataHandler.UpdateAsync(rule);

        // Reload with time slots
        var updatedRule = await _dataHandler.GetByGidWithTimeSlotsAsync(gid);
        return Result<AnsweringRuleDetailResponse>.Success(MapToDetailResponse(updatedRule!));
    }

    public async Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid)
    {
        var rule = await _dataHandler.GetByGidAsync(gid);
        if (rule == null)
        {
            return Result<bool>.Failure(
                new NotFoundError(ErrorCodes.Rule.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Rule.NotFound)));
        }

        await _dataHandler.DeleteAsync(rule);
        return Result<bool>.Success(true);
    }

    public async Task<Result<AnsweringRuleDetailResponse>> ToggleAsync(PortalAuthInfo auth, string gid)
    {
        var rule = await _dataHandler.GetByGidWithTimeSlotsAsync(gid);
        if (rule == null)
        {
            return Result<AnsweringRuleDetailResponse>.Failure(
                new NotFoundError(ErrorCodes.Rule.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.Rule.NotFound)));
        }

        rule.IsEnabled = !rule.IsEnabled;
        rule.ModifiedByUserId = auth.UserId;
        rule.ModifiedAt = DateTime.UtcNow;

        await _dataHandler.UpdateAsync(rule);
        return Result<AnsweringRuleDetailResponse>.Success(MapToDetailResponse(rule));
    }

    public async Task<Result<CheckRuleResponse>> CheckActiveRuleAsync(PortalAuthInfo auth, CheckRuleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SipAccountGid))
        {
            return Result<CheckRuleResponse>.Failure(
                new ValidationError(ErrorCodes.Rule.SipAccountRequired,
                    ErrorCodeHelper.GetMessage(ErrorCodes.Rule.SipAccountRequired)));
        }

        var checkTime = request.CheckDateTime ?? DateTime.UtcNow;
        var dayOfWeek = checkTime.DayOfWeek;
        var timeOfDay = TimeOnly.FromDateTime(checkTime);

        var rules = await _dataHandler.GetActiveRulesForSipAccountAsync(request.SipAccountGid);

        // Find matching rule (already ordered by priority, then by created date desc)
        foreach (var rule in rules)
        {
            var matchingSlot = rule.TimeSlots.FirstOrDefault(slot =>
                slot.DayOfWeek == dayOfWeek &&
                (slot.IsAllDay || (timeOfDay >= slot.StartTime && timeOfDay < slot.EndTime)));

            if (matchingSlot != null)
            {
                return Result<CheckRuleResponse>.Success(new CheckRuleResponse
                {
                    HasActiveRule = true,
                    Rule = new ActiveRuleInfo
                    {
                        Gid = rule.Gid,
                        Name = rule.Name,
                        ActionType = rule.ActionType,
                        ActionTarget = rule.ActionTarget,
                        VoicemailBoxGid = rule.VoicemailBoxGid,
                        VoiceMessageGid = rule.VoiceMessageGid,
                        SendEmailNotification = rule.SendEmailNotification,
                        NotificationEmail = rule.NotificationEmail
                    }
                });
            }
        }

        return Result<CheckRuleResponse>.Success(new CheckRuleResponse { HasActiveRule = false });
    }

    private ValidationError? ValidateRequest(
        string name, string sipAccountGid, AnsweringRuleAction actionType,
        string? actionTarget, string? voicemailBoxGid, string? voiceMessageGid,
        string? notificationEmail, List<TimeSlotDto> timeSlots)
    {
        // Name required
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ValidationError(ErrorCodes.Rule.NameRequired,
                ErrorCodeHelper.GetMessage(ErrorCodes.Rule.NameRequired));
        }

        // SIP account required
        if (string.IsNullOrWhiteSpace(sipAccountGid))
        {
            return new ValidationError(ErrorCodes.Rule.SipAccountRequired,
                ErrorCodeHelper.GetMessage(ErrorCodes.Rule.SipAccountRequired));
        }

        // Action-specific validation
        if (actionType == AnsweringRuleAction.Redirect || actionType == AnsweringRuleAction.RedirectToGroup)
        {
            if (string.IsNullOrWhiteSpace(actionTarget))
            {
                return new ValidationError(ErrorCodes.Rule.ActionTargetRequired,
                    ErrorCodeHelper.GetMessage(ErrorCodes.Rule.ActionTargetRequired));
            }
        }

        if (actionType == AnsweringRuleAction.Voicemail)
        {
            if (string.IsNullOrWhiteSpace(voicemailBoxGid))
            {
                return new ValidationError(ErrorCodes.Rule.VoicemailBoxRequired,
                    ErrorCodeHelper.GetMessage(ErrorCodes.Rule.VoicemailBoxRequired));
            }
        }

        if (actionType == AnsweringRuleAction.DisconnectWithVoicemessage)
        {
            if (string.IsNullOrWhiteSpace(voiceMessageGid))
            {
                return new ValidationError(ErrorCodes.Rule.VoiceMessageRequired,
                    ErrorCodeHelper.GetMessage(ErrorCodes.Rule.VoiceMessageRequired));
            }
        }

        // Email format validation
        if (!string.IsNullOrWhiteSpace(notificationEmail) && !EmailRegex().IsMatch(notificationEmail))
        {
            return new ValidationError(ErrorCodes.Rule.InvalidEmailFormat,
                ErrorCodeHelper.GetMessage(ErrorCodes.Rule.InvalidEmailFormat));
        }

        // Time slots required
        if (timeSlots == null || timeSlots.Count == 0)
        {
            return new ValidationError(ErrorCodes.Rule.TimeSlotRequired,
                ErrorCodeHelper.GetMessage(ErrorCodes.Rule.TimeSlotRequired));
        }

        // Validate each time slot
        foreach (var slot in timeSlots)
        {
            var slotError = ValidateTimeSlot(slot);
            if (slotError != null)
            {
                return slotError;
            }
        }

        // Check for overlapping slots
        var overlappingError = CheckOverlappingSlots(timeSlots);
        if (overlappingError != null)
        {
            return overlappingError;
        }

        return null;
    }

    private static ValidationError? ValidateTimeSlot(TimeSlotDto slot)
    {
        if (slot.IsAllDay)
        {
            return null;
        }

        if (!TimeOnly.TryParse(slot.StartTime, out var startTime))
        {
            return new ValidationError(ErrorCodes.TimeSlot.InvalidStartTimeGranularity,
                ErrorCodeHelper.GetMessage(ErrorCodes.TimeSlot.InvalidStartTimeGranularity));
        }

        if (!TimeOnly.TryParse(slot.EndTime, out var endTime))
        {
            return new ValidationError(ErrorCodes.TimeSlot.InvalidEndTimeGranularity,
                ErrorCodeHelper.GetMessage(ErrorCodes.TimeSlot.InvalidEndTimeGranularity));
        }

        // Check 15-minute granularity
        if (startTime.Minute % 15 != 0)
        {
            return new ValidationError(ErrorCodes.TimeSlot.InvalidStartTimeGranularity,
                ErrorCodeHelper.GetMessage(ErrorCodes.TimeSlot.InvalidStartTimeGranularity));
        }

        if (endTime.Minute % 15 != 0)
        {
            return new ValidationError(ErrorCodes.TimeSlot.InvalidEndTimeGranularity,
                ErrorCodeHelper.GetMessage(ErrorCodes.TimeSlot.InvalidEndTimeGranularity));
        }

        // End time must be after start time
        if (endTime <= startTime)
        {
            return new ValidationError(ErrorCodes.TimeSlot.EndTimeBeforeStartTime,
                ErrorCodeHelper.GetMessage(ErrorCodes.TimeSlot.EndTimeBeforeStartTime));
        }

        return null;
    }

    private static ValidationError? CheckOverlappingSlots(List<TimeSlotDto> slots)
    {
        // Group by day of week
        var slotsByDay = slots.GroupBy(s => s.DayOfWeek);

        foreach (var dayGroup in slotsByDay)
        {
            var daySlots = dayGroup.ToList();

            for (int i = 0; i < daySlots.Count; i++)
            {
                for (int j = i + 1; j < daySlots.Count; j++)
                {
                    if (SlotsOverlap(daySlots[i], daySlots[j]))
                    {
                        return new ValidationError(ErrorCodes.TimeSlot.OverlappingSlots,
                            ErrorCodeHelper.GetMessage(ErrorCodes.TimeSlot.OverlappingSlots));
                    }
                }
            }
        }

        return null;
    }

    private static bool SlotsOverlap(TimeSlotDto a, TimeSlotDto b)
    {
        // If either is all day, they overlap
        if (a.IsAllDay || b.IsAllDay)
        {
            return true;
        }

        var aStart = TimeOnly.Parse(a.StartTime);
        var aEnd = TimeOnly.Parse(a.EndTime);
        var bStart = TimeOnly.Parse(b.StartTime);
        var bEnd = TimeOnly.Parse(b.EndTime);

        // Check overlap: a starts before b ends AND a ends after b starts
        return aStart < bEnd && aEnd > bStart;
    }

    private static AnsweringRuleResponse MapToResponse(AnsweringRule rule) => new()
    {
        Gid = rule.Gid,
        Name = rule.Name,
        Description = rule.Description,
        Priority = rule.Priority,
        IsEnabled = rule.IsEnabled,
        ActionType = rule.ActionType,
        ActionTarget = rule.ActionTarget,
        SendEmailNotification = rule.SendEmailNotification,
        TimeSlotsCount = rule.TimeSlots?.Count ?? 0,
        CreatedAt = rule.CreatedAt
    };

    private static AnsweringRuleDetailResponse MapToDetailResponse(AnsweringRule rule) => new()
    {
        Gid = rule.Gid,
        Name = rule.Name,
        Description = rule.Description,
        Priority = rule.Priority,
        IsEnabled = rule.IsEnabled,
        ActionType = rule.ActionType,
        ActionTarget = rule.ActionTarget,
        SendEmailNotification = rule.SendEmailNotification,
        TimeSlotsCount = rule.TimeSlots?.Count ?? 0,
        CreatedAt = rule.CreatedAt,
        SipAccountGid = rule.SipAccountGid,
        VoicemailBoxGid = rule.VoicemailBoxGid,
        VoiceMessageGid = rule.VoiceMessageGid,
        NotificationEmail = rule.NotificationEmail,
        TimeSlots = rule.TimeSlots?.Select(MapToTimeSlotDto).ToList() ?? []
    };

    private static TimeSlotDto MapToTimeSlotDto(AnsweringRuleTimeSlot slot) => new()
    {
        Gid = slot.Gid,
        DayOfWeek = slot.DayOfWeek,
        StartTime = slot.StartTime.ToString("HH:mm"),
        EndTime = slot.EndTime.ToString("HH:mm"),
        IsAllDay = slot.IsAllDay
    };

    private static AnsweringRuleTimeSlot MapToTimeSlotEntity(TimeSlotDto dto) => new()
    {
        Gid = dto.Gid ?? Guid.NewGuid().ToString("N"),
        DayOfWeek = dto.DayOfWeek,
        StartTime = dto.IsAllDay ? new TimeOnly(0, 0) : TimeOnly.Parse(dto.StartTime),
        EndTime = dto.IsAllDay ? new TimeOnly(23, 59) : TimeOnly.Parse(dto.EndTime),
        IsAllDay = dto.IsAllDay
    };

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
