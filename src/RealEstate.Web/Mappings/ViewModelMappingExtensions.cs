using RealEstate.Application.DTOs.Admin;
using RealEstate.Application.DTOs.Agents;
using RealEstate.Application.DTOs.Inquiries;
using RealEstate.Application.DTOs.Lookup;
using RealEstate.Application.DTOs.Notifications;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.DTOs.RentalApplications;
using RealEstate.Application.DTOs.Reviews;
using RealEstate.Application.DTOs.Tenants;
using RealEstate.Application.DTOs.Visits;
using RealEstate.Web.ViewModels.Agent;
using RealEstate.Web.ViewModels.Dashboard;
using RealEstate.Web.ViewModels.Property;
using RealEstate.Web.ViewModels.Shared;
using RealEstate.Web.ViewModels.Tenant;

namespace RealEstate.Web.Mappings;

public static class ViewModelMappingExtensions
{
    public static PropertyListItemViewModel ToViewModel(this PropertyListItemDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Slug = dto.Slug,
        City = dto.City,
        District = dto.District,
        Price = dto.Price,
        AreaSqm = dto.AreaSqm,
        Bedrooms = dto.Bedrooms,
        Bathrooms = dto.Bathrooms,
        PropertyType = dto.PropertyType,
        ListingType = dto.ListingType,
        Status = dto.Status,
        CoverImageUrl = dto.CoverImageUrl,
        PublishedAtUtc = dto.PublishedAtUtc
    };

    public static PropertyDetailsViewModel ToViewModel(this PropertyDetailsDto dto) => new()
    {
        Id = dto.Id,
        CityId = dto.CityId,
        DistrictId = dto.DistrictId,
        DistrictName = dto.DistrictName,
        Title = dto.Title,
        Description = dto.Description,
        City = dto.City,
        District = dto.District,
        Price = dto.Price,
        SecurityDeposit = dto.SecurityDeposit,
        AreaSqm = dto.AreaSqm,
        Bedrooms = dto.Bedrooms,
        Bathrooms = dto.Bathrooms,
        Floor = dto.Floor,
        AddressLine = dto.AddressLine,
        PostalCode = dto.PostalCode,
        PropertyType = dto.PropertyType,
        ListingType = dto.ListingType,
        Status = dto.Status,
        AvailableFromUtc = dto.AvailableFromUtc,
        PublishedAtUtc = dto.PublishedAtUtc,
        AgentDisplayName = dto.AgentDisplayName,
        Images = dto.Images.Select(x => new PropertyImageViewModel
        {
            Id = x.Id,
            ImageUrl = x.ImageUrl,
            IsCover = x.IsCover,
            DisplayOrder = x.DisplayOrder
        }).ToArray(),
        Amenities = dto.Amenities,
        InquiryForm = new SendInquiryViewModel { PropertyId = dto.Id, Subject = "Demande d'information" },
        VisitForm = new RequestVisitViewModel { PropertyId = dto.Id, ScheduledAtUtc = DateTime.UtcNow.AddDays(1) },
        ApplicationForm = new SubmitRentalApplicationViewModel { PropertyId = dto.Id, RequestedMoveInDateUtc = DateTime.UtcNow.AddMonths(1) }
    };

    public static CreatePropertyViewModel ToCreateViewModel(this PropertyDetailsDto dto) => new()
    {
        CityId = dto.CityId,
        DistrictId = dto.DistrictId,
        DistrictName = dto.DistrictName,
        Title = dto.Title,
        Description = dto.Description,
        PropertyType = dto.PropertyType,
        ListingType = dto.ListingType,
        Price = dto.Price,
        SecurityDeposit = dto.SecurityDeposit,
        AreaSqm = dto.AreaSqm,
        Bedrooms = dto.Bedrooms,
        Bathrooms = dto.Bathrooms,
        Floor = dto.Floor,
        AddressLine = dto.AddressLine,
        PostalCode = dto.PostalCode,
        AvailableFromUtc = dto.AvailableFromUtc,
        ExistingImages = dto.Images.Select(x => new PropertyImageViewModel
        {
            Id = x.Id,
            ImageUrl = x.ImageUrl,
            IsCover = x.IsCover,
            DisplayOrder = x.DisplayOrder
        }).ToArray()
    };

    public static EditPropertyViewModel ToEditViewModel(this PropertyDetailsDto dto) => new()
    {
        Id = dto.Id,
        CityId = dto.CityId,
        DistrictId = dto.DistrictId,
        DistrictName = dto.DistrictName,
        Title = dto.Title,
        Description = dto.Description,
        PropertyType = dto.PropertyType,
        ListingType = dto.ListingType,
        Price = dto.Price,
        SecurityDeposit = dto.SecurityDeposit,
        AreaSqm = dto.AreaSqm,
        Bedrooms = dto.Bedrooms,
        Bathrooms = dto.Bathrooms,
        Floor = dto.Floor,
        AddressLine = dto.AddressLine,
        PostalCode = dto.PostalCode,
        AvailableFromUtc = dto.AvailableFromUtc
    };

    public static AgentProfileViewModel ToViewModel(this AgentProfileDto dto) => new()
    {
        Id = dto.Id,
        AgencyName = dto.AgencyName,
        LicenseNumber = dto.LicenseNumber,
        Bio = dto.Bio,
        YearsOfExperience = dto.YearsOfExperience,
        IsApproved = dto.IsApproved,
        ApprovalDateUtc = dto.ApprovalDateUtc
    };

    public static EditAgentProfileViewModel ToEditViewModel(this AgentProfileDto dto) => new()
    {
        AgencyName = dto.AgencyName,
        LicenseNumber = dto.LicenseNumber,
        Bio = dto.Bio,
        YearsOfExperience = dto.YearsOfExperience
    };

    public static TenantProfileViewModel ToViewModel(this TenantProfileDto dto) => new()
    {
        Id = dto.Id,
        Occupation = dto.Occupation,
        MonthlyIncome = dto.MonthlyIncome,
        PreferredCityId = dto.PreferredCityId,
        PreferredBudgetMin = dto.PreferredBudgetMin,
        PreferredBudgetMax = dto.PreferredBudgetMax
    };

    public static EditTenantProfileViewModel ToEditViewModel(this TenantProfileDto dto) => new()
    {
        Occupation = dto.Occupation,
        MonthlyIncome = dto.MonthlyIncome,
        PreferredCityId = dto.PreferredCityId,
        PreferredBudgetMin = dto.PreferredBudgetMin,
        PreferredBudgetMax = dto.PreferredBudgetMax
    };

    public static AgentInquiryListItemViewModel ToAgentViewModel(this InquiryDto dto) => new()
    {
        Id = dto.Id,
        PropertyTitle = dto.PropertyTitle,
        TenantDisplayName = dto.TenantDisplayName,
        Subject = dto.Subject,
        Message = dto.Message,
        Status = dto.Status,
        CreatedAtUtc = dto.CreatedAtUtc
    };

    public static TenantInquiryListItemViewModel ToTenantViewModel(this InquiryDto dto) => new()
    {
        Id = dto.Id,
        PropertyTitle = dto.PropertyTitle,
        Subject = dto.Subject,
        Message = dto.Message,
        Status = dto.Status,
        CreatedAtUtc = dto.CreatedAtUtc
    };

    public static AgentVisitListItemViewModel ToAgentViewModel(this VisitAppointmentDto dto) => new()
    {
        Id = dto.Id,
        PropertyTitle = dto.PropertyTitle,
        ScheduledAtUtc = dto.ScheduledAtUtc,
        Status = dto.Status,
        CounterpartyName = dto.CounterpartyName,
        Notes = dto.Notes
    };

    public static TenantVisitListItemViewModel ToTenantViewModel(this VisitAppointmentDto dto) => new()
    {
        Id = dto.Id,
        PropertyTitle = dto.PropertyTitle,
        ScheduledAtUtc = dto.ScheduledAtUtc,
        Status = dto.Status,
        CounterpartyName = dto.CounterpartyName,
        Notes = dto.Notes
    };

    public static AgentApplicationListItemViewModel ToAgentViewModel(this RentalApplicationDto dto) => new()
    {
        Id = dto.Id,
        PropertyTitle = dto.PropertyTitle,
        Status = dto.Status,
        RequestedMoveInDateUtc = dto.RequestedMoveInDateUtc,
        SubmittedAtUtc = dto.SubmittedAtUtc
    };

    public static TenantApplicationListItemViewModel ToTenantViewModel(this RentalApplicationDto dto) => new()
    {
        Id = dto.Id,
        PropertyTitle = dto.PropertyTitle,
        Status = dto.Status,
        RequestedMoveInDateUtc = dto.RequestedMoveInDateUtc,
        SubmittedAtUtc = dto.SubmittedAtUtc
    };

    public static FavoriteListItemViewModel ToViewModel(this FavoriteDto dto) => new()
    {
        FavoriteId = dto.FavoriteId,
        PropertyId = dto.PropertyId,
        PropertyTitle = dto.PropertyTitle,
        Price = dto.Price,
        City = dto.City,
        CoverImageUrl = dto.CoverImageUrl
    };

    public static NotificationListItemViewModel ToViewModel(this NotificationDto dto) => new()
    {
        Id = dto.Id,
        Type = dto.Type,
        Title = dto.Title,
        Message = dto.Message,
        LinkUrl = dto.LinkUrl,
        IsRead = dto.IsRead,
        CreatedAtUtc = dto.CreatedAtUtc
    };

    public static IReadOnlyCollection<LocationOptionViewModel> ToViewModels(this IReadOnlyCollection<LocationOptionDto> locations)
        => locations.Select(x => new LocationOptionViewModel { Id = x.Id, Name = x.Name }).ToArray();

    public static AdminDashboardViewModel ToViewModel(this AdminDashboardDto dto) => new()
    {
        UsersCount = dto.UsersCount,
        AgentsCount = dto.AgentsCount,
        ActiveAgentsCount = dto.ActiveAgentsCount,
        TenantCount = dto.TenantCount,
        PropertiesCount = dto.PropertiesCount,
        RentalsCount = dto.RentalsCount,
        ReviewsCount = dto.ReviewsCount,
        PendingAgents = dto.PendingAgents.Select(x => x.ToViewModel()).ToArray()
    };

    public static AdminUserListItemViewModel ToViewModel(this AdminUserListItemDto dto) => new()
    {
        UserId = dto.UserId,
        DisplayName = dto.DisplayName,
        Email = dto.Email,
        IsActive = dto.IsActive,
        IsAgent = dto.IsAgent,
        IsTenant = dto.IsTenant
    };

    public static AdminAgentListItemViewModel ToViewModel(this AdminAgentListItemDto dto) => new()
    {
        AgentProfileId = dto.AgentProfileId,
        AgencyName = dto.AgencyName,
        LicenseNumber = dto.LicenseNumber,
        IsApproved = dto.IsApproved,
        StatusLabel = dto.StatusLabel,
        PropertiesCount = dto.PropertiesCount,
        ReviewsCount = dto.ReviewsCount
    };

    public static AdminPropertyListItemViewModel ToViewModel(this AdminPropertyListItemDto dto) => new()
    {
        PropertyId = dto.PropertyId,
        Title = dto.Title,
        AgentName = dto.AgentName,
        City = dto.City,
        Status = dto.Status,
        IsDeleted = dto.IsDeleted
    };

    public static AdminReviewListItemViewModel ToViewModel(this AdminReviewListItemDto dto) => new()
    {
        ReviewId = dto.ReviewId,
        PropertyTitle = dto.PropertyTitle,
        AgentName = dto.AgentName,
        Rating = dto.Rating,
        Status = dto.Status,
        Comment = dto.Comment
    };
}
