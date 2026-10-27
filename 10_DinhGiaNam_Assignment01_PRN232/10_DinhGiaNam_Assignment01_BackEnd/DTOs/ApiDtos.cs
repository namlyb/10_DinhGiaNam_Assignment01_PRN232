using System.ComponentModel.DataAnnotations;

namespace _10_DinhGiaNam_Assignment01_BackEnd.DTOs;

public class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}

public class LoginResponse
{
    public short? AccountId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string Token { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }
}

public class AccountUpsertDto
{
    [Required, StringLength(100)] public string AccountName { get; set; } = "";
    [Required, EmailAddress, StringLength(70)] public string AccountEmail { get; set; } = "";
    [Range(1, 2)] public int AccountRole { get; set; }
    [StringLength(70)] public string AccountPassword { get; set; } = "";
}

public class CategoryUpsertDto
{
    [Required, StringLength(100)] public string CategoryName { get; set; } = "";
    [Required, StringLength(250)] public string CategoryDesciption { get; set; } = "";
    public short? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class NewsArticleUpsertDto
{
    [StringLength(400)] public string? NewsTitle { get; set; }
    [Required, StringLength(150)] public string Headline { get; set; } = "";
    [StringLength(4000)] public string? NewsContent { get; set; }
    [StringLength(400)] public string? NewsSource { get; set; }
    [Required] public short? CategoryId { get; set; }
    public bool NewsStatus { get; set; } = true;
    public List<int> TagIds { get; set; } = new();
}
