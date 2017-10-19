using System;
using System.ComponentModel.DataAnnotations;

namespace RiskyTodo.Models
{
    public class Todo
    {
        public Guid Id { get; set; }
        
        [Required]
        public string OwnerId { get; set; }
        [Required]
        public ApplicationUser Owner { get; set; }

        [Required]
        [StringLength(512)]
        public string Name { get; set; }

        public bool IsComplete { get; set; }
    }
}