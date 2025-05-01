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

public class RoleContants
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string User = "User";
}