using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Application.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorFullName => AuthorName;
}

public class CreateBookDto
{
    [Required(ErrorMessage = "Назва книги обов'язкова.")]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string ISBN { get; set; } = string.Empty;

    [Range(1000, 2030)]
    public int PublicationYear { get; set; }

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Вкажіть Id автора (> 0).")]
    public int AuthorId { get; set; }
}

public class UpdateBookDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string ISBN { get; set; } = string.Empty;

    [Range(1000, 2030)]
    public int PublicationYear { get; set; }

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int AuthorId { get; set; }
}
