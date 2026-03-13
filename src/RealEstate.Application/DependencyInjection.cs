using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Application.Services;

namespace RealEstate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IInquiryService, InquiryService>();
        services.AddScoped<IVisitAppointmentService, VisitAppointmentService>();
        services.AddScoped<IRentalApplicationService, RentalApplicationService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IOwnershipAccessService, OwnershipAccessService>();
        services.AddScoped<IPropertyImageService, PropertyImageService>();

        return services;
    }
}
