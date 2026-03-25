namespace Demo.Models.ViewModels
{
    public class FormFieldModel(string? value, string? label = null, bool requiresRoleValidation = true)
    {
        public string? Label { get; set; } = label;
        public string? Value { get; set; } = value;
        public bool RequiresRoleValidation { get; set; } = requiresRoleValidation;
    }
}