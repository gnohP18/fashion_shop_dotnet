using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace fashion_shop.Core.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Lấy tên hiển thị (Display Name) của Enum nếu có [Display], không thì trả về tên Enum.
    /// </summary>
    public static string GetDisplayName<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        return typeof(TEnum)
            .GetMember(enumValue.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName() ?? enumValue.ToString();
    }

    /// <summary>
    /// Lấy giá trị nguyên của enum.
    /// </summary>
    public static int GetIntValue<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        return Convert.ToInt32(enumValue);
    }

    /// <summary>
    /// Lấy tất cả các enum và display name dưới dạng Dictionary.
    /// </summary>
    public static Dictionary<int, string> ToDictionary<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .ToDictionary(
                e => Convert.ToInt32(e),
                e => e.GetDisplayName()
            );
    }

}