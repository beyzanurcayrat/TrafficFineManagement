using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Fines;
using TrafficFineManagement.Business.Services;
using TrafficFineManagement.DataAccess.Context;
using TrafficFineManagement.DataAccess.Repositories;
using TrafficFineManagement.Entities.Constants;
using TrafficFineManagement.Entities.Enums;
using TrafficFineManagement.Entities.Identity;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.Tests;

public class TrafficFineServiceTests : IDisposable
{
    private static readonly CurrentUser Personnel = new("u-personel", [AppRoles.Personnel]);
    private static readonly CurrentUser Manager = new("u-yonetici", [AppRoles.Manager]);
    private static readonly CurrentUser Finance = new("u-finans", [AppRoles.Finance]);

    private readonly AppDbContext _context;
    private readonly TrafficFineService _service;
    private readonly int _vehicleId;

    // Her test için yeni, boş bir bellek içi veritabanı
    public TrafficFineServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        _context.Users.AddRange(
            new AppUser { Id = Personnel.Id, UserName = "personel", Email = "personel@test.com", FullName = "Test Personel" },
            new AppUser { Id = Manager.Id, UserName = "yonetici", Email = "yonetici@test.com", FullName = "Test Yönetici" },
            new AppUser { Id = Finance.Id, UserName = "finans", Email = "finans@test.com", FullName = "Test Finans" });

        var vehicle = new Vehicle { Plate = "34ABC123", VehicleType = VehicleType.Car, Brand = "Renault", Model = "Megane" };
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
        _vehicleId = vehicle.Id;

        _service = new TrafficFineService(new TrafficFineRepository(_context), new VehicleRepository(_context));
    }

    public void Dispose() => _context.Dispose();

    private FineFormDto ValidForm(string fineNumber = "TC-001") => new()
    {
        VehicleId = _vehicleId,
        FineNumber = fineNumber,
        ViolationDate = DateTime.Today.AddDays(-1),
        ViolationType = "Hız sınırı aşımı",
        Amount = 1500m
    };

    private async Task<int> CreateFineAsync()
    {
        var result = await _service.CreateAsync(ValidForm(), Personnel);
        Assert.True(result.Success);
        return result.Data;
    }

    [Fact]
    public async Task Create_ByPersonnel_CreatesNewFineWithCreatedHistory()
    {
        var id = await CreateFineAsync();

        var detail = await _service.GetDetailAsync(id, Personnel);

        Assert.NotNull(detail);
        Assert.Equal(FineStatus.New, detail.Status);
        var history = Assert.Single(detail.History);
        Assert.Equal(ApprovalActionType.Created, history.ActionType);
        Assert.Null(history.FromStatus);
    }

    [Fact]
    public async Task Create_ByManager_Fails()
    {
        var result = await _service.CreateAsync(ValidForm(), Manager);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Create_DuplicateFineNumber_FailsCaseInsensitively()
    {
        await CreateFineAsync();

        var result = await _service.CreateAsync(ValidForm("tc-001"), Personnel);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task FullApprovalFlow_EndsCompletedAndRecordsEveryStep()
    {
        var id = await CreateFineAsync();

        Assert.True((await _service.SubmitAsync(id, Personnel)).Success);
        Assert.True((await _service.ApproveAsync(id, "Uygun", Manager)).Success);
        Assert.True((await _service.ApproveAsync(id, null, Finance)).Success);

        var detail = await _service.GetDetailAsync(id, Finance);
        Assert.Equal(FineStatus.Completed, detail!.Status);
        Assert.Equal(4, detail.History.Count);

        var last = detail.History.Last();
        Assert.Equal(ApprovalActionType.Approved, last.ActionType);
        Assert.Equal(FineStatus.PendingFinanceApproval, last.FromStatus);
        Assert.Equal(FineStatus.Completed, last.ToStatus);
        Assert.Equal("Test Finans", last.UserName);
    }

    [Fact]
    public async Task Finance_CannotApprove_WhenFineIsAtManagerStep()
    {
        var id = await CreateFineAsync();
        await _service.SubmitAsync(id, Personnel);

        var result = await _service.ApproveAsync(id, null, Finance);

        Assert.False(result.Success);
        var detail = await _service.GetDetailAsync(id, Finance);
        Assert.Equal(FineStatus.PendingManagerApproval, detail!.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Reject_WithoutReason_Fails(string? reason)
    {
        var id = await CreateFineAsync();
        await _service.SubmitAsync(id, Personnel);

        var result = await _service.RejectAsync(id, reason, Manager);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Reject_StoresReason_AndRejectedFineCanBeResubmitted()
    {
        var id = await CreateFineAsync();
        await _service.SubmitAsync(id, Personnel);

        Assert.True((await _service.RejectAsync(id, "Tutar hatalı", Manager)).Success);

        var rejected = await _service.GetDetailAsync(id, Personnel);
        Assert.Equal(FineStatus.Rejected, rejected!.Status);
        Assert.Equal("Tutar hatalı", rejected.History.Last().Note);
        Assert.True(rejected.CanEdit);

        Assert.True((await _service.SubmitAsync(id, Personnel)).Success);
        var resubmitted = await _service.GetDetailAsync(id, Personnel);
        Assert.Equal(FineStatus.PendingManagerApproval, resubmitted!.Status);
    }

    [Fact]
    public async Task Update_AfterApprovalProcessStarted_Fails()
    {
        var id = await CreateFineAsync();
        await _service.SubmitAsync(id, Personnel);

        var result = await _service.UpdateAsync(id, ValidForm(), Personnel);

        Assert.False(result.Success);
    }
}