using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Workflow;
using TrafficFineManagement.Entities.Constants;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Tests;

public class FineWorkflowTests
{
    private static CurrentUser UserWithRole(string role) => new("user-1", [role]);

    [Theory]
    [InlineData(FineStatus.PendingManagerApproval, AppRoles.Manager, true)]
    [InlineData(FineStatus.PendingManagerApproval, AppRoles.Finance, false)]
    [InlineData(FineStatus.PendingFinanceApproval, AppRoles.Finance, true)]
    [InlineData(FineStatus.PendingFinanceApproval, AppRoles.Manager, false)]
    [InlineData(FineStatus.New, AppRoles.Manager, false)]
    [InlineData(FineStatus.Completed, AppRoles.Finance, false)]
    [InlineData(FineStatus.Rejected, AppRoles.Manager, false)]
    public void CanDecide_OnlyApproverOfCurrentStepCanDecide(FineStatus status, string role, bool expected)
    {
        Assert.Equal(expected, FineWorkflow.CanDecide(status, UserWithRole(role)));
    }

    [Theory]
    [InlineData(FineStatus.PendingManagerApproval, FineStatus.PendingFinanceApproval)]
    [InlineData(FineStatus.PendingFinanceApproval, FineStatus.Completed)]
    public void GetNextStatus_ReturnsNextStep(FineStatus current, FineStatus expected)
    {
        Assert.Equal(expected, FineWorkflow.GetNextStatus(current));
    }

    [Theory]
    [InlineData(FineStatus.New)]
    [InlineData(FineStatus.Completed)]
    [InlineData(FineStatus.Rejected)]
    public void GetNextStatus_ThrowsForNonApprovalStatus(FineStatus status)
    {
        Assert.Throws<InvalidOperationException>(() => FineWorkflow.GetNextStatus(status));
    }

    [Theory]
    [InlineData(FineStatus.New, true)]
    [InlineData(FineStatus.Rejected, true)]
    [InlineData(FineStatus.PendingManagerApproval, false)]
    [InlineData(FineStatus.PendingFinanceApproval, false)]
    [InlineData(FineStatus.Completed, false)]
    public void CanEdit_OnlyNewOrRejectedFinesAreEditable(FineStatus status, bool expected)
    {
        Assert.Equal(expected, FineWorkflow.CanEdit(status, UserWithRole(AppRoles.Personnel)));
    }

    [Fact]
    public void CanEdit_NonPersonnelCannotEdit()
    {
        Assert.False(FineWorkflow.CanEdit(FineStatus.New, UserWithRole(AppRoles.Manager)));
    }
}