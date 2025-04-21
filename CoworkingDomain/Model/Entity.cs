using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingDomain.Model
{
    public abstract class Entity
    {
        [Key]
        [Column("id")]
        public virtual int Id { get; set; }
    }
}

