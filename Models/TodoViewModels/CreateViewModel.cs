using System.ComponentModel.DataAnnotations;

namespace RiskyTodo.Models.TodoViewModels
{
    public class CreateViewModel
    {
        [Required]
        [StringLength(512)]
        public string Name { get; set;}
    }
}