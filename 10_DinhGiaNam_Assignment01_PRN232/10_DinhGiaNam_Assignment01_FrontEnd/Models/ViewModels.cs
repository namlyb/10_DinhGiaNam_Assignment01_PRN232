using System.ComponentModel.DataAnnotations;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Models;

public class LoginVm { [Required, EmailAddress] public string Email { get; set; } = ""; [Required] public string Password { get; set; } = ""; }
public class LoginResponseVm { public short? AccountId { get; set; } public string Name { get; set; } = ""; public string Email { get; set; } = ""; public string Role { get; set; } = ""; }
public class AccountVm { public short AccountId { get; set; } [Required, StringLength(100)] public string AccountName { get; set; } = ""; [Required, EmailAddress] public string AccountEmail { get; set; } = ""; [Range(1,2)] public int AccountRole { get; set; } [Required] public string AccountPassword { get; set; } = ""; }
public class CategoryVm { public short CategoryId { get; set; } [Required, StringLength(100)] public string CategoryName { get; set; } = ""; [Required, StringLength(250)] public string CategoryDesciption { get; set; } = ""; public short? ParentCategoryId { get; set; } public bool IsActive { get; set; } }
public class TagVm { public int TagId { get; set; } public string? TagName { get; set; } public string? Note { get; set; } }
public class NewsVm
{
    public string NewsArticleId { get; set; } = ""; [StringLength(400)] public string? NewsTitle { get; set; }
    [Required, StringLength(150)] public string Headline { get; set; } = ""; public DateTime? CreatedDate { get; set; }
    [StringLength(4000)] public string? NewsContent { get; set; } [StringLength(400)] public string? NewsSource { get; set; }
    [Required] public short? CategoryId { get; set; } public bool NewsStatus { get; set; } public short? CreatedById { get; set; }
    public short? UpdatedById { get; set; } public DateTime? ModifiedDate { get; set; }
    public CategoryVm? Category { get; set; } public AccountVm? CreatedBy { get; set; } public List<TagVm> Tags { get; set; } = new();
    public List<int> TagIds { get; set; } = new();
}
public class NewsPageVm { public List<NewsVm> News { get; set; } = new(); public List<CategoryVm> Categories { get; set; } = new(); public List<TagVm> Tags { get; set; } = new(); public string? Search { get; set; } }
