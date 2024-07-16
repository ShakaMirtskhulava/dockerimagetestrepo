using FluentValidation;
using GHotel.API.Models.Room;

namespace GHotel.API.Infrastructure.Validators;

public class RoomUpdateModelValidator : AbstractValidator<UpdateRoomModel>
{
    public RoomUpdateModelValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("RoomId is required");

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("Image is required");

        RuleForEach(x => x.Images)
            .Must(image => image.Length < 10 * 1024 * 1024)
            .WithMessage("Image size should be less than 10MB");
    }
}
