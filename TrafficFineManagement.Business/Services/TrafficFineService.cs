using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Fines;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.Business.Workflow;
using TrafficFineManagement.DataAccess.Repositories.Interfaces;
using TrafficFineManagement.Entities.Constants;
using TrafficFineManagement.Entities.Enums;
using TrafficFineManagement.Entities.Identity;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.Business.Services;

public class TrafficFineService : ITrafficFineService
{
    private readonly ITrafficFineRepository _fineRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public TrafficFineService(ITrafficFineRepository fineRepository, IVehicleRepository vehicleRepository)
    {
        _fineRepository = fineRepository;
        _vehicleRepository = vehicleRepository;
    }

    // ---------- Okuma ----------

    public async Task<List<FineListItemDto>> GetListAsync(FineStatus? status, string? plate)
    {
        var normalizedPlate = string.IsNullOrWhiteSpace(plate)
            ? null
            : string.Concat(plate.Where(c => !char.IsWhiteSpace(c))).ToUpperInvariant();

        var fines = await _fineRepository.GetListAsync(status, normalizedPlate);

        return fines
            .Select(f => new FineListItemDto(f.Id, f.FineNumber, f.Vehicle.Plate, f.ViolationType,
                f.ViolationDate, f.Amount, f.Status))
            .ToList();
    }

    public async Task<FineDetailDto?> GetDetailAsync(int id, CurrentUser user)
    {
        var fine = await _fineRepository.GetWithDetailsAsync(id);
        if (fine is null) return null;

        return new FineDetailDto
        {
            Id = fine.Id,
            FineNumber = fine.FineNumber,
            Plate = fine.Vehicle.Plate,
            VehicleInfo = $"{fine.Vehicle.Brand} {fine.Vehicle.Model}",
            ViolationType = fine.ViolationType,
            ViolationDate = fine.ViolationDate,
            Location = fine.Location,
            Amount = fine.Amount,
            Description = fine.Description,
            Status = fine.Status,
            CreatedBy = DisplayName(fine.CreatedByUser),
            CreatedAt = fine.CreatedAt,
            History = fine.ApprovalHistories
                .OrderBy(h => h.CreatedAt)
                .Select(h => new FineHistoryItemDto(h.CreatedAt, DisplayName(h.User), h.ActionType,
                    h.FromStatus, h.ToStatus, h.Note))
                .ToList(),
            CanEdit = FineWorkflow.CanEdit(fine.Status, user),
            CanSubmit = FineWorkflow.CanSubmit(fine.Status, user),
            CanDecide = FineWorkflow.CanDecide(fine.Status, user)
        };
    }

    public async Task<FineFormDto?> GetForEditAsync(int id)
    {
        var fine = await _fineRepository.GetByIdAsync(id);
        if (fine is null) return null;

        return new FineFormDto
        {
            VehicleId = fine.VehicleId,
            FineNumber = fine.FineNumber,
            ViolationDate = fine.ViolationDate,
            ViolationType = fine.ViolationType,
            Location = fine.Location,
            Amount = fine.Amount,
            Description = fine.Description
        };
    }

    // ---------- Oluşturma / Güncelleme ----------

    public async Task<ServiceResult<int>> CreateAsync(FineFormDto dto, CurrentUser user)
    {
        if (!user.IsInRole(AppRoles.Personnel))
            return ServiceResult<int>.Fail("Ceza oluşturma yetkiniz bulunmuyor.");

        var validation = await ValidateFormAsync(dto, excludeId: null);
        if (!validation.Success)
            return ServiceResult<int>.Fail(validation.ErrorMessage!);

        var fine = new TrafficFine { CreatedByUserId = user.Id, Status = FineStatus.New };
        ApplyForm(fine, dto);
        AddHistory(fine, user, ApprovalActionType.Created, fromStatus: null, toStatus: FineStatus.New, note: null);

        await _fineRepository.AddAsync(fine);
        await _fineRepository.SaveChangesAsync();

        return ServiceResult<int>.Ok(fine.Id);
    }

    public async Task<ServiceResult> UpdateAsync(int id, FineFormDto dto, CurrentUser user)
    {
        var fine = await _fineRepository.GetWithDetailsAsync(id);
        if (fine is null)
            return ServiceResult.Fail("Ceza bulunamadı.");

        if (!FineWorkflow.CanEdit(fine.Status, user))
            return ServiceResult.Fail("Bu ceza mevcut durumunda düzenlenemez.");

        var validation = await ValidateFormAsync(dto, excludeId: id);
        if (!validation.Success)
            return validation;

        ApplyForm(fine, dto);
        fine.UpdatedAt = DateTime.UtcNow;
        AddHistory(fine, user, ApprovalActionType.Updated, fine.Status, fine.Status, "Ceza bilgileri güncellendi.");

        return await SaveAsync();
    }

    // ---------- Onay süreci ----------

    public async Task<ServiceResult> SubmitAsync(int id, CurrentUser user)
    {
        var fine = await _fineRepository.GetWithDetailsAsync(id);
        if (fine is null)
            return ServiceResult.Fail("Ceza bulunamadı.");

        if (!FineWorkflow.CanSubmit(fine.Status, user))
            return ServiceResult.Fail("Bu ceza mevcut durumunda onaya gönderilemez.");

        return await ChangeStatusAsync(fine, user, ApprovalActionType.Submitted,
            FineStatus.PendingManagerApproval, note: null);
    }

    public async Task<ServiceResult> ApproveAsync(int id, string? note, CurrentUser user)
    {
        var fine = await _fineRepository.GetWithDetailsAsync(id);
        if (fine is null)
            return ServiceResult.Fail("Ceza bulunamadı.");

        if (!FineWorkflow.CanDecide(fine.Status, user))
            return ServiceResult.Fail("Bu cezayı onaylama yetkiniz yok veya ceza sizin onay aşamanızda değil.");

        if (note?.Length > 500)
            return ServiceResult.Fail("Açıklama en fazla 500 karakter olabilir.");

        var nextStatus = FineWorkflow.GetNextStatus(fine.Status);
        return await ChangeStatusAsync(fine, user, ApprovalActionType.Approved, nextStatus, note?.Trim());
    }

    public async Task<ServiceResult> RejectAsync(int id, string? reason, CurrentUser user)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return ServiceResult.Fail("Ret nedeni zorunludur.");

        if (reason.Length > 500)
            return ServiceResult.Fail("Ret nedeni en fazla 500 karakter olabilir.");

        var fine = await _fineRepository.GetWithDetailsAsync(id);
        if (fine is null)
            return ServiceResult.Fail("Ceza bulunamadı.");

        if (!FineWorkflow.CanDecide(fine.Status, user))
            return ServiceResult.Fail("Bu cezayı reddetme yetkiniz yok veya ceza sizin onay aşamanızda değil.");

        return await ChangeStatusAsync(fine, user, ApprovalActionType.Rejected, FineStatus.Rejected, reason.Trim());
    }

    // ---------- Yardımcılar ----------

    // Tüm durum değişiklikleri buradan geçer: durum değişir + geçmişe yazılır, tek SaveChanges (aynı transaction)
    private async Task<ServiceResult> ChangeStatusAsync(TrafficFine fine, CurrentUser user,
        ApprovalActionType action, FineStatus newStatus, string? note)
    {
        var oldStatus = fine.Status;
        fine.Status = newStatus;
        fine.UpdatedAt = DateTime.UtcNow;
        AddHistory(fine, user, action, oldStatus, newStatus, note);

        return await SaveAsync();
    }

    private static void AddHistory(TrafficFine fine, CurrentUser user, ApprovalActionType action,
        FineStatus? fromStatus, FineStatus toStatus, string? note)
    {
        fine.ApprovalHistories.Add(new FineApprovalHistory
        {
            UserId = user.Id,
            ActionType = action,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Note = string.IsNullOrWhiteSpace(note) ? null : note
        });
    }

    private async Task<ServiceResult> SaveAsync()
    {
        try
        {
            await _fineRepository.SaveChangesAsync();
            return ServiceResult.Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            // RowVersion: aynı kayıt eşzamanlı olarak başka biri tarafından değiştirildi
            return ServiceResult.Fail("Kayıt siz işlem yaparken başka bir kullanıcı tarafından değiştirildi. Lütfen sayfayı yenileyip tekrar deneyin.");
        }
    }

    private async Task<ServiceResult> ValidateFormAsync(FineFormDto dto, int? excludeId)
    {
        if (await _vehicleRepository.GetByIdAsync(dto.VehicleId!.Value) is null)
            return ServiceResult.Fail("Seçilen araç bulunamadı.");

        var fineNumber = dto.FineNumber.Trim().ToUpperInvariant();
        if (await _fineRepository.FineNumberExistsAsync(fineNumber, excludeId))
            return ServiceResult.Fail($"{fineNumber} numaralı tutanak zaten kayıtlı.");

        if (dto.ViolationDate > DateTime.Now)
            return ServiceResult.Fail("İhlal tarihi gelecekte olamaz.");

        return ServiceResult.Ok();
    }

    private static void ApplyForm(TrafficFine fine, FineFormDto dto)
    {
        fine.VehicleId = dto.VehicleId!.Value;
        fine.FineNumber = dto.FineNumber.Trim().ToUpperInvariant();
        fine.ViolationDate = dto.ViolationDate!.Value;
        fine.ViolationType = dto.ViolationType.Trim();
        fine.Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim();
        fine.Amount = dto.Amount!.Value;
        fine.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
    }

    private static string DisplayName(AppUser user) =>
        string.IsNullOrWhiteSpace(user.FullName) ? user.Email ?? "-" : user.FullName;
}