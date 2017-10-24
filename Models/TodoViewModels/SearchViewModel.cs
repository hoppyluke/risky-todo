using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RiskyTodo.Models.TodoViewModels
{
    public class SearchViewModel
    {
        [Display(Name="Search my tasks for")]
        [Required]
        public string SearchTerm { get; set; }

        public List<Todo> Results { get; set;}
    }
}