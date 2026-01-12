using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models
{
    public abstract class ModelBase : IModelBase
    {
        [Key]
        public int Id { get; set; }
        public virtual string GetName() => "";
    }

    public interface IModelBase
    {
        public int Id { get; }
        public string GetName() => "";
    }
}
