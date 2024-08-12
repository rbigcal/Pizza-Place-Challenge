using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Pizza_Place_Challenge.Core.Enumerations;


// IMPORTANT!
// ALL ENUMS MUST BE DECORATED WITH [EnumAsInt]
// IN ORDER TO SAVE THEM AS INTEGER TO THE DATABASE

[EnumAsInt]
public enum PizzaSizes_Enumeration
{
    [Display(Name = "Small")]
    S = 0,

    [Display(Name = "Medium")]
    M,

    [Display(Name = "Large")]
    L,

    [Display(Name = "Extra Large")]
    XL,

    [Display(Name = "Extra Extra Large")]
    XXL,
}

[EnumAsInt]
public enum PizzaCategories_Enumeration
{
    [Display(Name = "Chicken")]
    Chicken = 100,

    [Display(Name = "Classic")]
    Classic = 200,

    [Display(Name = "Supreme")]
    Supreme = 300,

    [Display(Name = "Veggie")]
    Veggie = 400
}