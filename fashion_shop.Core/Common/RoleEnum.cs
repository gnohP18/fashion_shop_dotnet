using System.ComponentModel.DataAnnotations;

namespace fashion_shop.Core.Common;

public enum RoleEnum
{
    [Display(Name = "Admin")]
    Admin,
    [Display(Name = "Manager")]
    Manager,
    [Display(Name = "User")]
    User,
}