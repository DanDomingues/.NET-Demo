namespace Demo.Models.ViewModels
{
    public class FormFieldModel
    {
        public string? Label { get; set; }
        public string? Key { get; set; }
        public bool RequiresRoleValidation { get; set; }

        public FormFieldModel(string? key, string? label = null, bool requiresRoleValidation = true)
        {
            RequiresRoleValidation = requiresRoleValidation;
            Key = key;
            Label = label ?? key.Split('.').Last();
        }
    }
}