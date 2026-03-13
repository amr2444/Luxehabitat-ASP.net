namespace RealEstate.Domain.Enums;

public enum RentalApplicationStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    Accepted = 4,
    Approved = Accepted,
    Rejected = 5,
    Withdrawn = 6
}
