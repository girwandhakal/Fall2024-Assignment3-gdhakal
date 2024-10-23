using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fall2024_Assignment3_gdhakal.Models;

public class MovieActor
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Movie")]
    public int MovieID { get; set; }

    public Movie? Movie { get; set; }

    [ForeignKey("Actor")]
    public int ActorID { get; set; }

    public Actor? Actor { get; set; }
}

